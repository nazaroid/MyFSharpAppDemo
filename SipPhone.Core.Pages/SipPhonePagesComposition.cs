using Mvvm;
using SipPhone.Core.ViewModels;

namespace SipPhone.Core.Pages
{
    public sealed class SipPhonePagesComposition
    {
        private readonly IViewFactory _factory;

        public SipPhonePagesComposition(IViewFactory factory)
        {
            _factory = factory;
        }

        public void Assemble()
        {
            _factory.Register<SipPhoneViewModel, SipPhoneView>();
            _factory.Register<OutcomingCallViewModel, OutcomingCallView>();
            _factory.Register<IncomingCallViewModel, IncomingCallView>();
        }
    }
}
