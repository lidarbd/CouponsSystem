﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;

namespace CouponsSystem.Models
{
    public class AdminUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string HashedPassword { get; set; } = string.Empty;

        public AdminUser() { }
        public AdminUser(string username, string password)
        {
            Username = username;
            HashedPassword = HashPassword(password);
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = password + "s0m3S@lt"; // Adding salt to the password
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public bool VerifyPassword(string password)
        {
            string hashedInput = HashPassword(password);
            return HashedPassword == hashedInput;
        }
    }
}
