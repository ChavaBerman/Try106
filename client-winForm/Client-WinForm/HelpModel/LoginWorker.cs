﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace Client_WinForm
{
    public class LoginWorker
    {
        [Required]
        [MinLength(64),MaxLength(64)]
        public string Password { get; set; }

        [Required]
        [MinLength(2), MaxLength(15)]
        public string WorkerName { get; set; }

        public static String sha256_hash(String value)
        {
            StringBuilder Sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }
    }
}
