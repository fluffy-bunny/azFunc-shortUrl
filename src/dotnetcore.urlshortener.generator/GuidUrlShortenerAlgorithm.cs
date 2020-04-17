using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using dotnetcore.urlshortener.contracts;
using dotnetcore.urlshortener.generator.Extensions;


namespace dotnetcore.urlshortener.generator
{


    public class GuidUrlShortenerAlgorithm : IUrlShortenerAlgorithm
    {
        public static string ToBase62(BigInteger number)
        {
            var alphabet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var n = number;
            BigInteger basis = 62;
            var ret = "";
            while (n > 0)
            {
                BigInteger temp;
                BigInteger.DivRem(n, basis, out temp);
                //ulong temp = n % basis;
                ret = alphabet[(int)temp] + ret;
                n = BigInteger.Divide(n, basis);
                //n = (n / basis);

            }
            return ret;
        }

        public static string ToBase62(ulong number)
        {
            var alphabet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var n = number;
            ulong basis = 62;
            var ret = "";
            while (n > 0)
            {
                ulong temp = n % basis;
                ret = alphabet[(int)temp] + ret;
                n = (n / basis);

            }
            return ret;
        }

        public string GenerateUniqueId()
        {
            Guid guid = Guid.NewGuid();

            var id = guid.ToBase62();
            return id;
        }
    }
}
