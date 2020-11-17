using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mvvm;
using Ninject;
using TourAssistant.Client.Core.Stubs;
using TourAssistant.Client.Core.ViewModels;
using Xamarin.Forms;

namespace TourAssistant.Client.Core.Pages.Tests
{
    [TestClass]
    public class BootstrapperFixture
    {
        [TestMethod]
        public void ShouldResolveMainPage()
        {
            var sut = new TestableBootstrapper();
            //when
            var mainPage = sut.Run();
            //then
            Assert.IsNotNull(mainPage);
        }
    }

    public class App : Application
    {
    }

    sealed class TestableBootstrapper : Bootstrapper
    {
        public TestableBootstrapper() 
            : base(new StubAppPlatform())
        {
            
        }

        public override Page ResolveMainPage(IKernel kernel)
        {
            var viewFactory = kernel.Get<IViewFactory>();
            viewFactory.Register<MainMenuViewModel, Page>();
            var mainPage = viewFactory.Resolve<MainMenuViewModel>();
            return mainPage.Item1;
        }
    }
}
