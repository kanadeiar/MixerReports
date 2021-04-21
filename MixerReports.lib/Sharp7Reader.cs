using System.Data;
using Sharp7;

namespace MixerReports.lib
{
    public class Sharp7Reader
    {
        public static S7Client _S7Client = new S7Client();
        public static string _Address;

        public Sharp7Reader(string address)
        {
            _Address = address;
        }

        public bool TestConnection(out int error)
        {
            var result = _S7Client.ConnectTo(_Address, 0, 2);
            _S7Client.Disconnect();
            if (result != 0)
            {
                error = result;
                return false;
            }
            error = 0;
            return true;
        }
    }
}
