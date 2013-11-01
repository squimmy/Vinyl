using System;

namespace Vinyl.Transformer
{
    public static class StringUtils
    {
        public static string FirstCharToLower(string x)
        {
            return x.Substring(0,1).ToLower() + x.Substring(1);
        }
    }
}

