using GaVL.DTO.Settings;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace GaVL.Application.Systems
{
    public interface IMailService
    {
        Task<bool> SendMail(string toEmail, string subject, string body);
        Task<bool> SendMail(string toMail, string subject, string templateId, JObject variables);
        Task<bool> SendMail(string toMail, string subject, string templateId, Dictionary<string, object> variables);
    }
    public class MailService : IMailService
    {
        private readonly MailJetSetting _smtpSettings;
        private readonly ILogger<MailService> _logger;
        public MailService(IOptions<MailJetSetting> smtpSettings, ILogger<MailService> logger)
        {
            _smtpSettings = smtpSettings.Value;
            _logger = logger;
            _logger.LogInformation("MailSetting: ApiKey={ApiKey}, SecretKey={SecretKey}, SenderEmail={SenderEmail}, SenderName={SenderName}",
                    _smtpSettings.ApiKey, _smtpSettings.SecretKey, _smtpSettings.SenderEmail, _smtpSettings.SenderName);
        }
        public async Task<bool> SendMail(string toMail, string subject, string templateId, JObject variables)
        {
            MailjetClient client = new MailjetClient(_smtpSettings.ApiKey, _smtpSettings.SecretKey);
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }.Property(Send.Vars, variables)
            .Property(Send.FromEmail, _smtpSettings.SenderEmail)
            .Property(Send.FromName, _smtpSettings.SenderName)
            .Property(Send.Subject, subject)
            .Property(Send.MjTemplateID, templateId)
            .Property(Send.MjTemplateLanguage, true)
            .Property(Send.To, toMail);
            MailjetResponse response = await client.PostAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(string.Format("Total: {0}, Count: {1}", response.GetTotal(), response.GetCount()));
                Console.WriteLine(response.GetData());
                return true;
            }
            else
            {
                Console.WriteLine(string.Format("StatusCode: {0}", response.StatusCode));
                Console.WriteLine(string.Format("ErrorInfo: {0}", response.GetErrorInfo()));
                Console.WriteLine(response.GetData());
                Console.WriteLine(string.Format("ErrorMessage: {0}", response.GetErrorMessage()));
                return false;
            }
        }

        public async Task<bool> SendMail(string toMail, string subject, string templateId, Dictionary<string, object> variables)
        {
            MailjetClient client = new MailjetClient(_smtpSettings.ApiKey, _smtpSettings.SecretKey);
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
            .Property(Send.FromEmail, _smtpSettings.SenderEmail)
            .Property(Send.FromName, _smtpSettings.SenderName)
            .Property(Send.Subject, subject)
            .Property(Send.MjTemplateID, templateId)
            .Property(Send.MjTemplateLanguage, true)
            .Property(Send.Vars, JObject.FromObject(variables))
            .Property(Send.To, toMail);
            MailjetResponse response = await client.PostAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(string.Format("Total: {0}, Count: {1}", response.GetTotal(), response.GetCount()));
                Console.WriteLine(response.GetData());
                return true;
            }
            else
            {
                Console.WriteLine(string.Format("StatusCode: {0}", response.StatusCode));

                Console.WriteLine(string.Format("ErrorInfo: {0}", response.GetErrorInfo()));

                Console.WriteLine(response.GetData());
                Console.WriteLine(string.Format("ErrorMessage: {0}", response.GetErrorMessage()));
                return false;
            }
        }

        public async Task<bool> SendMail(string toEmail, string subject, string body)
        {
            MailjetClient client = new MailjetClient(_smtpSettings.ApiKey, _smtpSettings.SecretKey);
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }.Property(Send.FromEmail, _smtpSettings.SenderEmail)
            .Property(Send.FromName, _smtpSettings.SenderName)
            .Property(Send.Subject, subject)
            .Property(Send.TextPart, body)
            .Property(Send.To, toEmail);
            MailjetResponse response = await client.PostAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(string.Format("Total: {0}, Count: {1}", response.GetTotal(), response.GetCount()));
                Console.WriteLine(response.GetData());
                return true;
            }
            else
            {
                Console.WriteLine(string.Format("StatusCode: {0}", response.StatusCode));

                Console.WriteLine(string.Format("ErrorInfo: {0}", response.GetErrorInfo()));

                Console.WriteLine(response.GetData());
                Console.WriteLine(string.Format("ErrorMessage: {0}", response.GetErrorMessage()));
                return false;
            }
        }
    }
}
