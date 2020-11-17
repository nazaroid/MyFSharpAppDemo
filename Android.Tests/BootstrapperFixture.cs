using System;
using Mvvm;
using NUnit.Framework;
using TourAssistant.Client.Android;


namespace Android.Tests
{
    [TestFixture]
    public class BootstrapperFixture
    {

        [SetUp]
        public void Setup() { }


        [TearDown]
        public void Tear() { }

        [Test]
        public void ShouldResolveMainPage()
        {
            Console.WriteLine("ShouldResolveMainPage");

            var sut = new Bootstrapper();
            //when
            var mainPage = sut.Run();
            //then
            Assert.IsNotNull(mainPage);
            Console.WriteLine("OK");
        }
    }
}