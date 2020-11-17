using Mvvm;
using Ninject;
using SipPhone.Core;
using SipPhone.Core.Pages;
using SipPhone.Core.ViewModels;

namespace SipPhone.Android
{
    public class SipPhoneBootstrapper
    {
        private SipPhoneCoreComposition _c;

        public void RegisterModels(IKernel kernel)
        {
            _c = new SipPhoneCoreComposition(kernel);
            _c.Assemble(new DroidSipPhonePlatform());
        }

        public void RegisterViews(IViewFactory factory)
        {
            var sipPhonePages = new SipPhonePagesComposition(factory);
            sipPhonePages.Assemble();
        }

        public void Run()
        {
            var backgroundService = _c.Resolve<BackgroundServiceBroker>();
            backgroundService.BeginListenIncomingCalls();
            backgroundService.BeginListenRegistrationStatus();
            backgroundService.SendStartServiceCmd();
        }
    }
}