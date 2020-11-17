using Mvvm;
using TourAssistant.Client.Core.ViewModels;

namespace TourAssistant.Client.Core.Pages
{
    public sealed class PagesComposition
    {
        private readonly IViewFactory _factory;

        public PagesComposition(IViewFactory factory)
        {
            _factory = factory;
        }
        public void Assemble()
        {
            _factory.Register<MainMenuViewModel, MainMenuView>();
            _factory.Register<YourTourMenuViewModel, YourTourMenuView>();
            _factory.Register<AreaInfoMenuViewModel, AreaInfoMenuView>();
            _factory.Register<ServicesMenuViewModel, ServicesMenuView>();

            _factory.Register<FlightInfoViewModel, FlightInfoView>();
            _factory.Register<PersonalInfoViewModel, PersonalInfoView>();

            _factory.Register<CountryInfoMenuViewModel, AreaItemInfoMenuView>();
            _factory.Register<KurortInfoMenuViewModel, AreaItemInfoMenuView>();
            _factory.Register<HotelInfoMenuViewModel, AreaItemInfoMenuView>();

            _factory.Register<AreaMapViewModel, AreaMapView>();
            _factory.Register<AreaDescriptionViewModel, AreaDescriptionView>();

        }
    }
}