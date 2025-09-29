using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Arpal.SiApi.Utils
{
    public class Password
    {
        public static string IsPasswordValid(string password)
        {
            // Define validation criteria
            int minLength = 8;
            int minUpperCase = 1;
            int minLowerCase = 1;
            int minDigit = 1;
            int minSpecialChar = 1;
            string specialChars = @"!@#$%^&*()_=+";

            // Check length
            if (password.Length < minLength)
                return $"La password deve essere almeno di {minLength}";

            // Check uppercase letters
            if (password.Length - Regex.Replace(password, "[A-Z]", "").Length < minUpperCase)
                return $"La password deve contenere almeno una lettere maiuscola";

            // Check lowercase letters
            if (password.Length - Regex.Replace(password, "[a-z]", "").Length < minLowerCase)
                return $"La password deve contenere almeno una lettere minuscola";

            // Check digits
            if (password.Length - Regex.Replace(password, "[0-9]", "").Length < minDigit)
                return $"La password deve contenere almeno un numero";

            // Check special characters
            if (password.Length - Regex.Replace(password, "[" + Regex.Escape(specialChars) + "]", "").Length < minSpecialChar)
                return $"La password deve contenere almeno uno tra i seguenti caratteri tra gli apici \" {specialChars} \"";

            return "";
        }
    }
}
