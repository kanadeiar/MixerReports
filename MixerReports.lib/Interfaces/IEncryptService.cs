namespace MixerReports.lib.Interfaces
{
    public interface IEncryptService
    {
        string Encrypt(string text, string password);
        string Decrypt(string text, string password);
        byte[] Encrypt(byte[] data, string password);
        byte[] Decrypt(byte[] data, string password);
    }
}
