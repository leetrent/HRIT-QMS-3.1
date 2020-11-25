using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace QmsCore.Engine
{
    public static class Security
    {
//https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.hashalgorithm.computehash?view=netcore-2.2#System_Security_Cryptography_HashAlgorithm_ComputeHash_System_Byte___
        public static string GetHashedValue(string input)
        {
            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            using(SHA512 hasher =SHA512.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = hasher.ComputeHash(Encoding.UTF8.GetBytes(input));


                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
            }//end using

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Verify a hash against a string.
        public static bool VerifyHashedValue(string input, string hash)
        {
            bool retval = false;
            // Hash the input.
            string hashOfInput = GetHashedValue(input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                retval = true;
            }

            return retval;
        }        



    }//end class
}//end namespace