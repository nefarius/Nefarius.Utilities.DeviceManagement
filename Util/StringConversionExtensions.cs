using System.Text;

namespace Nefarius.Utilities.DeviceManagement.Util
{
    internal static class StringConversionExtensions
    {
        public static string ToUTF8String(this byte[] buffer)
        {
            var value = Encoding.UTF8.GetString(buffer);
            return value.Remove(value.IndexOf((char)0));
        }

        public static string ToUTF16String(this byte[] buffer)
        {
            var value = Encoding.Unicode.GetString(buffer);
            return value.Remove(value.IndexOf((char)0));
        }
    }
}