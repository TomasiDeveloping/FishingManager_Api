using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Api.Helper.Methods
{
    public static class CreatePassword
    {
        public static string CreateNewPassword()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 10)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        
        public static string CreateHash(string newPassword)
        {
            using var sha256Hash = SHA256.Create();
            // ComputeHash - returns byte array  
            var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(newPassword));  
  
            // Convert byte array to a string   
            var builder = new StringBuilder();  
            foreach (var t in bytes)
            {
                builder.Append(t.ToString("x2"));
            }  
            return builder.ToString();
        }
    }
}