using Microsoft.VisualStudio.TestTools.UnitTesting;
using MixerReports.lib.Services;

namespace MixerReports.lib.Tests.Services
{
    [TestClass]
    public class Sharp7EasyMixReaderServiceTests
    {
        [TestMethod]
        public void Service_Ctor_NotNullValid()
        {
            var actual = new Sharp7EasyMixReaderService("1.1.1.1", 20, 10);

            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual, typeof(Sharp7EasyMixReaderService));
        }
    }
}
