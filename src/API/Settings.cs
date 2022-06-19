using System;

namespace API
{
    public static class Settings
    {
        public static string JwtSecret = "5840b8aefc090889942d1e420f9228b8dadcb69193491b4a9b1f5b9d032f89a3";
        public static string PasswordSecret = "12ca314d3077f81f69537726b20c8909f79627a57744cf6a6021ed3864cf1b1e";
        public static DateTime TokenLifeTime = DateTime.UtcNow.AddMinutes(1);
    }
}
