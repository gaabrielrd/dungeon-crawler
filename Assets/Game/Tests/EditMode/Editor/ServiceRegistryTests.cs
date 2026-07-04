using DungeonCrawler.Core.Services;
using NUnit.Framework;

namespace DungeonCrawler.Tests.EditMode
{
    public sealed class ServiceRegistryTests
    {
        [SetUp]
        public void SetUp()
        {
            ServiceRegistry.Clear();
        }

        [TearDown]
        public void TearDown()
        {
            ServiceRegistry.Clear();
        }

        [Test]
        public void ResolveReturnsRegisteredService()
        {
            var service = new MockService("first");

            ServiceRegistry.Register<IMockService>(service);

            Assert.That(ServiceRegistry.Resolve<IMockService>(), Is.SameAs(service));
        }

        [Test]
        public void RegisterReplacesExistingService()
        {
            ServiceRegistry.Register<IMockService>(new MockService("first"));
            var replacement = new MockService("replacement");

            ServiceRegistry.Register<IMockService>(replacement);

            Assert.That(ServiceRegistry.Resolve<IMockService>(), Is.SameAs(replacement));
            Assert.That(ServiceRegistry.Resolve<IMockService>().Name, Is.EqualTo("replacement"));
        }

        private interface IMockService
        {
            string Name { get; }
        }

        private sealed class MockService : IMockService
        {
            public MockService(string name)
            {
                Name = name;
            }

            public string Name { get; }
        }
    }
}
