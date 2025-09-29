using System.Security.Cryptography;
using System.Text;

namespace Arpal.SiApi.Utils
{
    /// <summary>
    /// Reverting a SHA-512 hash back to its original value is practically impossible because SHA-512 is a cryptographic 
    /// hash function designed to be irreversible. It generates a fixed-size hash value from the input data, 
    /// and it's designed to be computationally infeasible to reverse this process and obtain the original input from the hash.
    /// </summary>
    public static class HashManager
    {
        public static string ComputeHash(string input)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha512.ComputeHash(inputBytes);

                // Convert the byte array to a hexadecimal string
                string hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                return hashString;
            }
        }
    }
}
