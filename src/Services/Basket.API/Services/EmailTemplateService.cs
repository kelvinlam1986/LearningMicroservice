﻿using Basket.API.Services.Interfaces;
using Shared.Configurations;
using System.Text;

namespace Basket.API.Services
{
    public class EmailTemplateService
    {
        

        private static readonly string _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string _templateFolder = Path.Combine(_baseDirectory, "EmailTemplates");

        protected readonly BackgroundJobSettings BackgroundJobSettings;

        protected EmailTemplateService(BackgroundJobSettings backgroundJobSettings)
        {
            BackgroundJobSettings = backgroundJobSettings;
        }

        protected string ReadEmailTemplateContent(string emailTemplateName, string format = "html")
        {
            var filePath = Path.Combine(_templateFolder, emailTemplateName + "." + format);
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var sr = new StreamReader(fs, Encoding.Default);
            var emailText = sr.ReadToEnd();
            sr.Close();
            return emailText;
        }
    }
}
