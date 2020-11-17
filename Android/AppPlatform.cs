using TourAssistant.Client.Core.Platform;

namespace TourAssistant.Client.Android
{
    sealed class AppPlatform : IAppPlatform
    {
        public ILogger GetLogger()
        {
            return new Logger();
        }

        public ITextFileService GetTextFileService()
        {
            return new TextFileService();
        }
    }
}