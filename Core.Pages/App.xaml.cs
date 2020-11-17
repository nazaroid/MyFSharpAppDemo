using Mvvm;
using TourAssistant.Client.Core.Platform;
using Xamarin.Forms;

namespace TourAssistant.Client.Core.Pages
{
    public partial class App : Application
    {
        public App(IBootstrapper bootstrapper)
        {
            InitializeComponent();

            MainPage = bootstrapper.Run();
        }
    }
}
