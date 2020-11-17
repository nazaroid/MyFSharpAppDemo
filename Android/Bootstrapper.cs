using Mvvm;
using Ninject;
using SipPhone.Android;
using TourAssistant.Client.Core;
using TourAssistant.Client.Core.Data;
using TourAssistant.Client.Core.Pages;
using TourAssistant.Client.Core.ViewModels;
using Xamarin.Forms;

namespace TourAssistant.Client.Android
{
    public class Bootstrapper : NinjectBootstrapper
    {
        private readonly SipPhoneBootstrapper _sipPhone;

        public Bootstrapper()
            : base(new StandardKernel())
        {
            _sipPhone = new SipPhoneBootstrapper();
        }
        public override void RegisterModels(IKernel kernel)
        {
            base.RegisterModels(kernel);
            var settings = new ServerSettings
            {
                /* 
                    Run DOS 'ipconfig /all' cmd to figure out your local PC ip.

                    On Port 8888 Fiddler should listen incoming traffic. 
                    (http://docs.telerik.com/fiddler/configure-fiddler/tasks/UseFiddlerAsReverseProxy)

                    To configure Fiddler reverse proxy use OnBeforeRequest handler:
                        if (oSession.host.indexOf(":8888") > 0) oSession.host = oSession.host.replace(":8888", ":81");
                */
                //Host = "http://172.16.4.110:81", //JOB Google emu
                Host = "http://172.16.4.85:8888", //JOB Hyper-v emu
                //Host = "http://192.168.1.167:81", //HOME
                AuthPath = "/api/MobileAccount/Authenticate",
                TourPath = "/breeze/MobileDb",
                AreaInfoPath = "/api/MobileAreaInfo",
                ClientCode = "Code1",
                Pwd = "Code1Pwd"
            };
            var core = new CoreComposition(kernel);
            core.Assemble(settings, new AppPlatform());

            _sipPhone.RegisterModels(kernel);
        }

        public override void RegisterViews(IViewFactory factory)
        {
            base.RegisterViews(factory);
            var pages = new PagesComposition(factory);
            pages.Assemble();

            _sipPhone.RegisterViews(factory);
        }

        public override Page ResolveMainPage(IKernel kernel)
        {
            var viewFactory = kernel.Get<IViewFactory>();
            var mainMenu = kernel.Get<MainMenuViewModel>();
            mainMenu.Initialize();
            var mainPage = viewFactory.Resolve(mainMenu);
            var navigationPage = new NavigationPage(mainPage.Item1);

            return navigationPage;
        }

        public override Page Run()
        {
            var mainPage = base.Run();
            _sipPhone.Run();
            return mainPage;
        }
    }
}