﻿using System.Text;

namespace ECommerceMVC.Helpers
{
    public class MyUtil
    {
        public static string UploadHinh(IFormFile Hinh, string folder)
        {
            try
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folder, Hinh.FileName);
                using (var myfile = new FileStream(fullPath, FileMode.CreateNew))
                {
                    Hinh.CopyTo(myfile);
                }
                return Hinh.FileName;
            }catch(Exception ex)
            {
                return string.Empty;
            }
        }
        public static string GenerateRandomKey(int length=5)
        {
            var pattern = @"qwrtyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM!@#$%^&*()";
            var sb=new StringBuilder();
            var random=new Random();
            for(int i = 0; i < length; i++)
            {
                sb.Append(pattern[random.Next(0, pattern.Length)]);
            }
            return sb.ToString();
        }
    }
}
