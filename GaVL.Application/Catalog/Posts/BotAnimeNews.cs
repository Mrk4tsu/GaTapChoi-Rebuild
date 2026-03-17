using GaVL.Application.Systems;
using GaVL.Data;
using GaVL.Data.Entities;
using GaVL.DTO.Settings;
using GaVL.Utilities;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml;

namespace GaVL.Application.Catalog.Posts
{
    public interface IBotAnimeNews
    {
        Task<string> Run(string pass);
    }
    public class BotAnimeNews : IBotAnimeNews
    {
        private readonly AppDbContext _db;
        private readonly IRedisService _redis;
        private readonly IR2Service _r2Service;
        private readonly string KeyPrefix = "posts";
        private readonly BotSettings _bot;

        public BotAnimeNews(AppDbContext db, IRedisService redis, IOptions<BotSettings> bot, IR2Service r2Service)
        {
            _db = db;
            _redis = redis;
            _bot = bot.Value;
            ApiKey = Environment.GetEnvironmentVariable(_bot.Key) ?? _bot.Key;
            GeminiEndpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3-flash-preview:generateContent?key={ApiKey}";
            _r2Service = r2Service;
        }

        private readonly string ApiKey;
        private readonly string GeminiEndpoint;

        // 1. Thay đổi Nguồn RSS sang các trang Anime
        private readonly List<string> RssSources = new()
        {
            "https://www.animenewsnetwork.com/news/rss.xml", // Anime News Network (Nguồn uy tín nhất)
            //"https://cr-news-api-service.prd.crunchyrollsvc.com/v1/en-US/rss", // Crunchyroll News
            "https://myanimelist.net/rss/news.xml" // MyAnimeList News
        };

        // 2. Cập nhật ID Bot Anime của bạn
        private readonly Guid BotUserId = Guid.Parse("019cb1c8-301c-73f4-aa8c-223633236520");

        // 3. Đổi CategoryId (Giả sử 1 là Game, 2 là Anime. Nhớ đổi lại theo thực tế DB của bạn)
        private readonly int DefaultCategoryId = 2;

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
            Console.WriteLine("--- BOT RANDOM ANIME NEWS ---");
            Console.WriteLine("[1] Đang trích xuất tin tức Anime ngẫu nhiên từ RSS...");

            var randomNews = await GetRandomNewsFromRss();
            if (randomNews == null)
            {
                Console.WriteLine("Lỗi: Không tìm thấy tin nào trong 72h qua.");
                return "Chưa có tin tức anime mới hôm nay.";
            }

            string ogImage = randomNews.Image;
            if (string.IsNullOrEmpty(ogImage))
            {
                ogImage = await GetOgImage(randomNews.Link);
            }

            Console.WriteLine($"-> Đã chọn tin: {randomNews.Title}");
            Console.WriteLine($"-> Link gốc: {randomNews.Link}");
            Console.WriteLine("\n[2] Đang gửi cho AI viết bài...");

            string finalImageUrl = ogImage;
            if (!string.IsNullOrEmpty(ogImage))
            {
                Console.WriteLine("[System] Đang Re-upload ảnh lên R2...");
                var r2Link = await ProcessAndUploadToR2(ogImage);

                if (!string.IsNullOrEmpty(r2Link))
                {
                    finalImageUrl = r2Link;
                    Console.WriteLine($"-> Upload thành công: {finalImageUrl}");
                }
                else
                {
                    Console.WriteLine("-> Upload thất bại, sẽ dùng link ảnh gốc.");
                }
            }

            // 4. Thay đổi Prompt để AI nhập vai Biên tập viên Anime/Wibu
            string prompt = $@"
                    Bạn là biên tập viên SEO cho một trang web thông tin Anime/Manga cực kỳ vui tính, lầy lội dành cho Gen Z và cộng đồng Wibu/Otaku Việt Nam.
                    Nhiệm vụ: Dựa vào thông tin tiếng Anh dưới đây, hãy viết một bài đăng Blog (Caption) tiếng Việt.
                    Yêu cầu bài viết:
                    - Tiêu đề giật tít hấp dẫn, gây tò mò (nhưng không fake news).
                    - Nội dung tóm tắt ngắn gọn sự kiện chính (Anime mới ra mắt, spoiler nhẹ, drama tác giả...).
                    - Giọng văn: Hài hước, lầy lội. Sử dụng các từ lóng của cộng đồng anime/manga (waifu, husbando, isekai, wibu, ảo ma, suy, buff bẩn, plot twist, quay xe...). Chèn nhiều emoji hợp lý.
                    - Tuyệt đối không dịch word-by-word (kiểu Google Dịch), hãy viết lại theo ý hiểu (Paraphrase) cho thật tự nhiên.
                    - Lưu ý loại bỏ các icon, emote không cần thiết.
                    Tin tức gốc:
                    Title: {randomNews.Title}
                    Summary: {randomNews.Summary} 
                    Yêu cầu OUTPUT: Chỉ trả về duy nhất 1 chuỗi JSON (không markdown, không code block) theo định dạng sau:
                    {{
                        ""Title"": ""Tiêu đề tiếng Việt hấp dẫn, chứa từ khóa anime"",
                        ""SeoAlias"": ""duong-dan-tinh-theo-tieu-de-khong-dau"",
                        ""Description"": ""Đoạn tóm tắt (Sapo) khoảng 150 ký tự để làm meta description"",
                        ""HtmlContent"": ""Nội dung bài viết chi tiết. Sử dụng các thẻ HTML như <h2>, <p>, <ul>, <li>, <b>. Viết dài khoảng 500 từ. Giọng văn cuốn hút, chuyên nghiệp nhưng vẫn lầy lội."",
                        ""Tags"": [""Tag1"", ""Tag2"", ""Tag3""]
                    }}
                ";

            var generatedJson = await GenerateContentWithGemini(prompt);
            Console.WriteLine("\n--- KẾT QUẢ BÀI VIẾT (Ready to Post) ---\n");

            if (!string.IsNullOrEmpty(generatedJson))
            {
                try
                {
                    var articleDto = JsonSerializer.Deserialize<GeneratedArticleDto>(generatedJson);
                    Console.ForegroundColor = ConsoleColor.Magenta; // Đổi màu console cho điệu đà Anime chút
                    Console.WriteLine(articleDto.Title);
                    Console.WriteLine($"\n[Mô tả]: {articleDto.Description}");
                    Console.WriteLine($"\n{articleDto.HtmlContent}");
                    Console.WriteLine($"\n[Nguồn tin]: {randomNews.Link}");
                    Console.ResetColor();

                    await SavePostToDatabase(articleDto, finalImageUrl);
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

        private async Task<string> ProcessAndUploadToR2(string originalUrl)
        {
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                client.Timeout = TimeSpan.FromSeconds(30);

                var response = await client.GetAsync(originalUrl, HttpCompletionOption.ResponseHeadersRead);
                if (!response.IsSuccessStatusCode) return null;

                var contentType = response.Content.Headers.ContentType?.MediaType ?? "image/jpeg";
                var extension = GetExtensionFromMimeType(contentType);
                var fileName = $"post-images/anime/{DateTime.Now:yyyy/MM/dd}/{Guid.NewGuid()}.{extension}"; // Tạo thư mục con "anime"

                using var networkStream = await response.Content.ReadAsStreamAsync();
                using var memoryStream = new MemoryStream();
                await networkStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                return await _r2Service.UploadStreamGetUrl(memoryStream, fileName, contentType);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Upload Error] Không thể re-up ảnh: {ex.Message}");
                return null;
            }
        }

        private string GetExtensionFromMimeType(string mimeType)
        {
            return mimeType switch
            {
                "image/png" => "png",
                "image/gif" => "gif",
                "image/webp" => "webp",
                "image/jpeg" => "jpg",
                _ => "jpg"
            };
        }

        async Task<NewsItem?> GetRandomNewsFromRss()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

            var allNews = new List<NewsItem>();

            foreach (var url in RssSources)
            {
                try
                {
                    var xmlContent = await client.GetStringAsync(url);
                    using var reader = XmlReader.Create(new StringReader(xmlContent));
                    var feed = SyndicationFeed.Load(reader);

                    foreach (var item in feed.Items)
                    {
                        if (item.PublishDate > DateTimeOffset.Now.AddHours(-72))
                        {
                            var imgFromRss = item.Links.FirstOrDefault(l => l.RelationshipType == "enclosure" && l.MediaType.StartsWith("image"))?.Uri.ToString();
                            allNews.Add(new NewsItem
                            {
                                Title = item.Title.Text,
                                Summary = item.Summary?.Text ?? item.Title.Text,
                                Link = item.Links.FirstOrDefault()?.Uri.ToString() ?? "",
                                Image = imgFromRss
                            });
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }

            if (allNews.Count == 0) return null;

            var random = new Random();
            int index = random.Next(allNews.Count);
            return allNews[index];
        }

        async Task<string> GetOgImage(string url)
        {
            if (string.IsNullOrEmpty(url)) return null;
            try
            {
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(10);
                string encodedUrl = Uri.EscapeDataString(url);
                string apiUrl = $"https://api.microlink.io?url={encodedUrl}";
                var jsonResponse = await client.GetStringAsync(apiUrl);
                var jsonNode = JsonNode.Parse(jsonResponse);
                string imageUrl = jsonNode?["data"]?["image"]?["url"]?.ToString();

                if (!string.IsNullOrEmpty(imageUrl) && Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
                {
                    return imageUrl;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Microlink Error] Không lấy được ảnh cho bài viết: {ex.Message}");
            }
            return null;
        }

        async Task<string> GenerateContentWithGemini(string promptText)
        {
            using var client = new HttpClient();
            var requestBody = new
            {
                contents = new[] { new { parts = new[] { new { text = promptText } } } },
                generationConfig = new { temperature = 0.7 } // Nhiệt độ 0.7 cho độ sáng tạo tốt
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
                Code = "ANIBOT-" + code, // Đổi tiền tố code để dễ phân biệt
                Title = dto.Title,
                SeoAlias = dto.SeoAlias,
                Description = dto.HtmlContent,
                Sumary = dto.Description,
                MainImage = imgUrl,
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
}
