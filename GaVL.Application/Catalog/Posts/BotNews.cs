using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using GaVL.Application.Systems;
using GaVL.Data;
using GaVL.Data.Entities;
using GaVL.DTO.Settings;
using GaVL.Utilities;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using PuppeteerSharp;
using System;
using System.Net;
using System.Net.Http.Json;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml;

namespace GaVL.Application.Catalog.Posts
{
    public interface IBotNews
    {
        Task<string> Run(string pass);
    }
    public class BotNews : IBotNews
    {
        private readonly AppDbContext _db;
        private readonly IRedisService _redis;
        private readonly string KeyPrefix = "posts";
        private readonly BotSettings _bot;

        public BotNews(AppDbContext db, IRedisService redis, IOptions<BotSettings> bot)
        {
            _db = db;
            _redis = redis;
            _bot = bot.Value;
            ApiKey = Environment.GetEnvironmentVariable(_bot.Key) ?? _bot.Key;
            GeminiEndpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3-flash-preview:generateContent?key={ApiKey}";
        }
        private readonly string ApiKey;
        private readonly string GeminiEndpoint;
        private readonly List<string> RssSources = new()
        {
            "https://feeds.ign.com/ign/news",         // IGN News
            "https://www.gamespot.com/feeds/news/",   // GameSpot News
            "https://www.rockpapershotgun.com/feed",  // PC Games
        };
        private readonly Guid BotUserId = Guid.Parse("019bdb9d-07b5-71a4-a310-acc7670b4ea6");
        private readonly int DefaultCategoryId = 1;
        public async Task<string> Run(string password)
        {
            if (string.IsNullOrEmpty(_bot.Password))
            {
                return "Lỗi máy chủ";
            }
            if (password != _bot.Password)
            {
                return "Sai mật khẩu bot";
            }
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("--- BOT RANDOM GAME NEWS ---");
            Console.WriteLine("[1] Đang trích xuất tin tức ngẫu nhiên từ RSS...");
            var randomNews = await GetRandomNewsFromRss();
            if (randomNews == null)
            {
                Console.WriteLine("Lỗi: Không tìm thấy tin nào trong 48h qua.");
                return "Chưa có tin tức mới hôm nay.";
            }
            string ogImage = randomNews.Image;
            if (string.IsNullOrEmpty(ogImage))
            {
                // Nếu RSS không có mới phải đi cào từ web
                ogImage = await GetOgImage(randomNews.Link);
            }
            Console.WriteLine($"-> Đã chọn tin: {randomNews.Title}");
            Console.WriteLine($"-> Link gốc: {randomNews.Link}");
            Console.WriteLine("\n[2] Đang gửi cho AI viết bài...");
            string prompt = $@"
                    Bạn là biên tập viên SEO cho trang web game vui tính, lầy lội dành cho Gen Z Việt Nam.
                    Nhiệm vụ: Dựa vào thông tin tiếng Anh dưới đây, hãy viết một bài đăng Blog (Caption) tiếng Việt.
                    Yêu cầu bài viết:
                    - Tiêu đề giật tít một chút (nhưng không fake news).
                    - Nội dung tóm tắt ngắn gọn sự kiện chính.
                    - Giọng văn: Hài hước, dùng slang game thủ (farm, gank, sấy, toang...), chèn nhiều emoji hợp lý.
                    - Tuyệt đối không dịch word-by-word (kiểu Google Dịch), hãy viết lại theo ý hiểu (Paraphrase).
                    - Lưu ý loại bỏ các icon, emote không cần thiết
                    Tin tức gốc:
                    Title: {randomNews.Title}
                    Summary: {randomNews.Summary} 
                    Yêu cầu OUTPUT: Chỉ trả về duy nhất 1 chuỗi JSON (không markdown, không code block) theo định dạng sau:
                    {{
                        ""Title"": ""Tiêu đề tiếng Việt hấp dẫn, chứa từ khóa"",
                        ""SeoAlias"": ""duong-dan-tinh-theo-tieu-de-khong-dau"",
                        ""Description"": ""Đoạn tóm tắt (Sapo) khoảng 150 ký tự để làm meta description"",
                        ""HtmlContent"": ""Nội dung bài viết chi tiết. Sử dụng các thẻ HTML như <h2>, <p>, <ul>, <li>, <b>. Viết dài khoảng 500 từ. Giọng văn chuyên nghiệp, lôi cuốn."",
                        ""Tags"": [""Tag1"", ""Tag2"", ""Tag3""]
                    }}

                ";

            var generatedJson = await GenerateContentWithGemini(prompt);
            Console.WriteLine("\n--- KẾT QUẢ BÀI VIẾT (Ready to Post) ---\n");
            Console.WriteLine(ogImage);
            if (!string.IsNullOrEmpty(generatedJson))
            {
                try
                {
                    // 3. Parse JSON từ AI sang Object C#
                    var articleDto = JsonSerializer.Deserialize<GeneratedArticleDto>(generatedJson);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(articleDto.Title);
                    Console.WriteLine($"\n[Mô tả]: {articleDto.Description}");
                    Console.WriteLine($"\n{articleDto.HtmlContent}");


                    Console.WriteLine($"\n[Nguồn tin]: {randomNews.Link}"); // Để bạn check lại nếu cần
                    Console.ResetColor();
                    // 4. Lưu vào Database
                    await SavePostToDatabase(articleDto, ogImage);
                    return articleDto.Title;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi xử lý JSON hoặc DB: {ex.Message}");
                    return "Lỗi xử lý JSON hoặc DB " + ex.Message;
                }

            }
            return "Chưa có tin tức mới hôm nay.";
        }

        async Task<NewsItem?> GetRandomNewsFromRss()
        {
            using var client = new HttpClient();
            // Fake User-Agent để tránh bị chặn bởi một số server RSS
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

            var allNews = new List<NewsItem>();

            foreach (var url in RssSources)
            {
                try
                {
                    var xmlContent = await client.GetStringAsync(url);

                    // Dùng XmlReader để đọc RSS
                    using var reader = XmlReader.Create(new StringReader(xmlContent));
                    var feed = SyndicationFeed.Load(reader);

                    foreach (var item in feed.Items)
                    {
                        // Chỉ lấy tin trong vòng 24h qua để đảm bảo tính thời sự
                        if (item.PublishDate > DateTimeOffset.Now.AddHours(-72))
                        {
                            var imgFromRss = item.Links.FirstOrDefault(l => l.RelationshipType == "enclosure" && l.MediaType.StartsWith("image"))?.Uri.ToString();
                            allNews.Add(new NewsItem
                            {
                                Title = item.Title.Text,
                                Summary = item.Summary?.Text ?? item.Title.Text, // Một số feed không có summary thì lấy title
                                Link = item.Links.FirstOrDefault()?.Uri.ToString() ?? "",
                                Image = imgFromRss
                            });
                        }
                    }
                }
                catch
                {
                    // Bỏ qua nếu 1 link RSS bị lỗi, chạy tiếp link khác
                    continue;
                }
            }

            if (allNews.Count == 0) return null;

            // Random chọn 1 tin từ danh sách đã lọc
            var random = new Random();
            int index = random.Next(allNews.Count);
            return allNews[index];
        }
        // Đừng quên using HtmlAgilityPack;
        async Task<string> GetOgImage(string url)
        {
            // Nếu URL rỗng thì trả về null luôn
            if (string.IsNullOrEmpty(url)) return null;

            try
            {
                using var client = new HttpClient();

                // Cài đặt timeout ngắn (10s) để tránh treo bot nếu API chậm
                client.Timeout = TimeSpan.FromSeconds(10);

                // 1. Mã hóa URL gốc để gửi qua API (tránh lỗi ký tự đặc biệt)
                string encodedUrl = Uri.EscapeDataString(url);

                // 2. Gọi API Microlink (Bản Free không cần API Key)
                string apiUrl = $"https://api.microlink.io?url={encodedUrl}";

                // 3. Nhận kết quả JSON
                var jsonResponse = await client.GetStringAsync(apiUrl);

                // 4. Parse JSON để lấy link ảnh
                var jsonNode = JsonNode.Parse(jsonResponse);

                // Cấu trúc JSON trả về: { "status": "success", "data": { "image": { "url": "..." } } }
                // Dùng dấu ? (null conditional) để không bị lỗi nếu không có ảnh
                string imageUrl = jsonNode?["data"]?["image"]?["url"]?.ToString();

                // Kiểm tra xem link có hợp lệ không
                if (!string.IsNullOrEmpty(imageUrl) && Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
                {
                    return imageUrl;
                }
            }
            catch (Exception ex)
            {
                // Log lỗi nhẹ nhàng để biết, không throw exception làm chết app
                Console.WriteLine($"[Microlink Error] Không lấy được ảnh cho bài viết: {ex.Message}");
            }

            // Trả về null nếu có lỗi hoặc không tìm thấy ảnh
            return null;
        }
        async Task<string> GenerateContentWithGemini(string promptText)
        {
            using var client = new HttpClient();
            var requestBody = new
            {
                contents = new[] { new { parts = new[] { new { text = promptText } } } },
                generationConfig = new { temperature = 0.6 } // Tăng creative lên 0.8 cho bài viết bay bổng
            };

            var jsonContent = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await client.PostAsJsonAsync(GeminiEndpoint, requestBody);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var jsonNode = JsonNode.Parse(jsonResponse);
            string rawText = jsonNode?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();
            if (string.IsNullOrEmpty(rawText)) return null;
            rawText = rawText.Replace("```json", "").Replace("```", "").Trim();
            return rawText;
        }
        async Task SavePostToDatabase(GeneratedArticleDto dto, string imgUrl)
        {
            var postTags = new List<PostTag>();
            var code = StringHelper.GenerateRandomCode();
            var newPost = new Post
            {
                UserId = BotUserId,
                CategoryId = DefaultCategoryId,
                Code = "BOT-" + code, // Mã bài viết
                Title = dto.Title,
                SeoAlias = dto.SeoAlias,
                Description = dto.HtmlContent,

                // LƯU Ý: Vì Entity của bạn không có trường Content, tôi đang lưu vào Sumary.
                // Hãy đổi sang trường Content nếu bạn update DB.
                Sumary = dto.Description,

                MainImage = imgUrl, // Link ảnh lấy từ RSS gốc
                CreateAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow,
                IsDeleted = false,
                PostTags = postTags
            };
            _db.Posts.Add(newPost);
            await _db.SaveChangesAsync();

            await RemoveCache();
        }
        private async Task RemoveCache()
        {
            var pattern = $"{KeyPrefix}:*";
            await _redis.DeleteKeysByPatternAsync(pattern);
        }
    }
    public class NewsItem
    {
        public string Title { get; set; } = "";
        public string Summary { get; set; } = "";
        public string Link { get; set; } = "";
        public string Image { get; set; } = "";
    }
    public class GeneratedArticleDto
    {
        public string Title { get; set; }
        public string SeoAlias { get; set; }
        public string Description { get; set; }
        public string HtmlContent { get; set; } // Đây là nội dung chính
        public string ImageUrl { get; set; }
        public List<string> Tags { get; set; }
    }
}
