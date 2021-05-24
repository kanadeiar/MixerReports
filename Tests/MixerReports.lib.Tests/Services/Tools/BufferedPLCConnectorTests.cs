using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MixerReports.lib.Services.Tools;
using Moq;
using Sharp7;

namespace MixerReports.lib.Tests.Services.Tools
{
    [TestClass]
    public class BufferedPLCConnectorTests
    {
        [TestMethod]
        public void Connector_Ctor_NotNull()
        {
            var stub = Mock.Of<S7Client>();

            var actual = new BufferedPLCConnector(stub, 20, 10);

            Assert.IsNotNull(actual);
            var typ = typeof(BufferedPLCConnector);
            var actClient = (typ.GetField("_client", BindingFlags.Instance | BindingFlags.NonPublic))?.GetValue(actual);
            Assert.AreSame(stub, actClient);
        }
        [TestMethod]
        public void Connector_Ctor_ValidType()
        {
            var stub = Mock.Of<S7Client>();

            var actual = new BufferedPLCConnector(stub, 20, 10);

            Assert.IsInstanceOfType(actual, typeof(BufferedPLCConnector));
        }
        [TestMethod]
        public void Connector_Ctor_PrivateFieldServiceValid()
        {
            var stub = Mock.Of<S7Client>();

            var actual = new BufferedPLCConnector(stub, 20, 10);

            var typ = typeof(BufferedPLCConnector);
            var actClient = (typ.GetField("_client", BindingFlags.Instance | BindingFlags.NonPublic))?.GetValue(actual);
            Assert.AreSame(stub, actClient);
        }
    }
}
