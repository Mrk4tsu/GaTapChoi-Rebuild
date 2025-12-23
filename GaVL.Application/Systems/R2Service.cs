using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using GaVL.DTO.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace GaVL.Application.Systems
{
    public interface IR2Service
    {
        Task UploadFileGetKey(IFormFile file, string key);
        Task<string> UploadFileGetUrl(IFormFile file, string key);
        Task DeleteFileAsync(string key, string keyPref);
        Task DeleteFilesAsync(List<string> keys);
    }
    public class R2Service : IR2Service
    {
        private readonly R2Options _options;
        private readonly IAmazonS3 _s3Client;
        public R2Service(IOptions<R2Options> options)
        {
            _options = options.Value;
            var credentials = new BasicAWSCredentials(_options.AccessKey, _options.SecretKey);
            var config = new AmazonS3Config
            {
                ServiceURL = _options.EndpointUrl,
                ForcePathStyle = true
            };
            _s3Client = new AmazonS3Client(credentials, config);
        }

        public async Task DeleteFileAsync(string key, string keyPrex)
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _options.BucketName,
                Key = $"{keyPrex}/{key}"
            };

            var response = await _s3Client.DeleteObjectAsync(request);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.NoContent)
            {
                throw new Exception($"Delete failed: {response.HttpStatusCode}");
            }
        }

        public async Task DeleteFilesAsync(List<string> keys)
        {
            if (keys == null || keys.Count == 0) return;
            if (keys.Count > 1000) throw new ArgumentException("Maximum 1000 keys per request");

            var request = new DeleteObjectsRequest
            {
                BucketName = _options.BucketName,
                Objects = keys.Select(k => new KeyVersion { Key = k }).ToList()
            };

            var response = await _s3Client.DeleteObjectsAsync(request);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Bulk delete failed: {response.HttpStatusCode}");
            }

            // Kiểm tra có lỗi chi tiết nào không
            if (response.DeleteErrors.Any())
            {
                var errors = string.Join("; ", response.DeleteErrors.Select(e => $"{e.Key}: {e.Message}"));
                throw new Exception($"Some objects failed to delete: {errors}");
            }
        }

        public async Task UploadFileGetKey(IFormFile file, string key)
        {
            using var stream = file.OpenReadStream();
            var putRequest = new PutObjectRequest
            {
                BucketName = _options.BucketName,
                Key = key, // Ví dụ: "uploads/myfile.jpg"
                InputStream = stream,
                ContentType = file.ContentType,
                DisablePayloadSigning = true,
                DisableDefaultChecksumValidation = true
            };

            var response = await _s3Client.PutObjectAsync(putRequest);
            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Upload failed: {response.HttpStatusCode}");
            }
        }

        public async Task<string> UploadFileGetUrl(IFormFile file, string key)
        {
            using var stream = file.OpenReadStream();
            var putRequest = new PutObjectRequest
            {
                BucketName = _options.BucketName,
                Key = key,
                InputStream = stream,
                ContentType = file.ContentType,
                DisablePayloadSigning = true,
                DisableDefaultChecksumValidation = true
            };
            var response = await _s3Client.PutObjectAsync(putRequest);
            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Upload failed: {response.HttpStatusCode}");
            }
            return $"{_options.PublicEndpoint}/{key}";
        }
    }
}
