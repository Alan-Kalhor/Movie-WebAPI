using System;

namespace MovieAPI.Helper
{
    public static class AppSettings
    {
        public static double CacheExpirationHours
        {
            get
            {
                double temp = 1;
                double.TryParse(System.Configuration.ConfigurationManager.AppSettings["CacheExpirationHours"], out temp);
                return temp;
            }
        }
        public static string MovieList_CacheKey
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["MovieList_CacheKey"];
            }
        }
        public static string Movie_CacheKey
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["Movie_CacheKey"];
            }
        }
    }
}