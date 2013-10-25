using System;

namespace Vinyl.Transformer
{
    public static class StringUtils
    {
        public static string FirstCharToUpper(string x)
        {
            return x.Substring(0,1).ToUpper() + x.Substring(1);
        }
    }
}

