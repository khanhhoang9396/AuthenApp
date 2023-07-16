using System;
using System.Collections.Generic;

namespace Gateway
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }
    }


    public class LoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class LoginByRefreshToken
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }




    public class UserAccount 
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string BirthDay { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string Role { get; set; }
    }

    public class RefreshTokenModel
    {
        public int UserId { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expires { get; set; }
    }

    public static class DataHelper
    {
        public static List<UserAccount> UserAccounts = new()
        {
            new UserAccount()
            {
                UserId = 1,
                UserName = "admin",
                Password = "admin@123",
                BirthDay = "01/04/1996",
                Country= "VN",
                Email="email@gmail.com",
                Gender = "male",
                Role = "Manager"
            },
            new UserAccount()
            {
                UserId = 2,
                UserName = "adminKr",
                Password = "admin@123",
                BirthDay = "01/04/2006",
                Country= "KR",
                Email="email@gmail.com",
                Gender = "male",
                Role = "Staff"
            },
        };

        public static List<RefreshTokenModel> RefreshTokens = new();
    }
}
