using System;
using System.IO;
using TourAssistant.Client.Core.Platform;

namespace TourAssistant.Client.Android
{
    sealed class TextFileService : ITextFileService
    {
        public void Save(string name, string text)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, name);
            File.WriteAllText(filePath, text);
        }

        public string Load(string name)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, name);
            return File.ReadAllText(filePath);
        }
    }
}