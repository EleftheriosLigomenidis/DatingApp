using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Helpers
{
    public static class Extentions
    {
        public static void AddApplicationError(this HttpResponse response, string message )
        {
            response.Headers.Add("Application-Error", message); // new header called app erro wit essage message
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");

                
        }

        public static int CalculateAge(this DateTime Birthday)
        {
            var age = DateTime.Today.Year - Birthday.Year;

            // return a datetime variable tha adds the age years
            if(Birthday.AddYears(age) > DateTime.Today)
            {
                age--;
            }
            return age;
        }
    }
}
