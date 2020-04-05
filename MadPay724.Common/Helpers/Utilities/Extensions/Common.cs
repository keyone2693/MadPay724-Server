using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Ganss.XSS;
using MadPay724.Common.Helpers.Utilities.Pagination;
using MadPay724.Data.Dtos.Common;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MadPay724.Common.Helpers.Utilities.Extensions
{
    public static class Common
    {
        public static void AddAppError(this HttpResponse response, string message)
        {
            response.Headers.Add("App-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "App-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }
        public static void AddPagination(this HttpResponse response,
            int currentPage, int itemsPerPage,
            int totalItems, int totalPages)
        {
            var paginationHeader = new PaginationHeader(currentPage, itemsPerPage,
             totalItems, totalPages);
            var camelCaseFormater = new JsonSerializerSettings();
            camelCaseFormater.ContractResolver = new CamelCasePropertyNamesContractResolver();
            response.Headers.Add("Pagination", JsonConvert.SerializeObject(paginationHeader, camelCaseFormater));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");

        }
        public static short ToBank(this string BankName)
        {
            switch (BankName)
            {
                case "Saman":
                    return 1;
                case "Mellat":
                    return 2;
                case "Parsian":
                    return 3;
                case "Pasargad":
                    return 4;
                case "Iran Kish":
                    return 5;
                case "Melli":
                    return 6;
                case "Asan Pardakht":
                    return 7;
                case "ZarinPal":
                    return 8;
                case "Virtual":
                    return 9;
                default:
                    return 9;
            }
        }
        public static string ToPrice(this object dec)
        {
            if (dec == null)
                dec = 0;
            string Src = dec.ToString();
            Src = Src.Replace(".0000", "");
            if (!Src.Contains("."))
            {
                Src = Src + ".00";
            }
            string[] price = Src.Split('.');
            if (price[1].Length >= 2)
            {
                price[1] = price[1].Substring(0, 2);
                price[1] = price[1].Replace("00", "");
            }

            string Temp = null;
            int i = 0;
            if ((price[0].Length % 3) >= 1)
            {
                Temp = price[0].Substring(0, (price[0].Length % 3));
                for (i = 0; i <= (price[0].Length / 3) - 1; i++)
                {
                    Temp += "," + price[0].Substring((price[0].Length % 3) + (i * 3), 3);
                }
            }
            else
            {
                for (i = 0; i <= (price[0].Length / 3) - 1; i++)
                {
                    Temp += price[0].Substring((price[0].Length % 3) + (i * 3), 3) + ",";
                }
                Temp = Temp.Substring(0, Temp.Length - 1);
                // Temp = price(0)
                //If price(1).Length > 0 Then
                //    Return price(0) + "." + price(1)
                //End If
            }
            if (price[1].Length > 0)
            {
                return Temp + "." + price[1];
            }
            else
            {
                return Temp;
            }
        }

        public static string RemoveHtmlXss(this string html)
        {
            if (string.IsNullOrEmpty(html))
                return "";

            var _htmlSanitizer = new HtmlSanitizer();
            return _htmlSanitizer.Sanitize(html, null);
        }

        #region encrypt_decrypt

        public static string Encrypt(this string clearText)
        {
            const string encryptionKey = "639512!c5-59eb-4c62-93f3-4825@4-d7757b0-ac5bb";
            var clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (var encryptor = Aes.Create())
            {
                var pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
        public static string Decrypt(this string cipherText)
        {
            const string encryptionKey = "639512!c5-59eb-4c62-93f3-4825@4-d7757b0-ac5bb";
            cipherText = cipherText.Replace(" ", "+");
            var cipherBytes = Convert.FromBase64String(cipherText);
            using (var encryptor = Aes.Create())
            {
                var pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }


        #endregion

        public static string ToMobile(this string number)
        {
            if (string.IsNullOrEmpty(number))
            {
                return null;
            }
            var str = number.Trim();
            if (str.StartsWith("+98"))
            {
                str = str.Replace("+98", "0");
            }
            else if (str.StartsWith("98"))
            {
                str = str.Replace("98", "0");
            }
            if (!str.StartsWith("0"))
            {
                str ="0" + str;
            }
            if(str.Length == 11)
            {
                return str;
            }
            return null;
        }
        public static string ToOrderBy(
            this string sortHe,
            string sortDir)
        {
            if (string.IsNullOrEmpty(sortHe) || string.IsNullOrWhiteSpace(sortHe))
                return "";
            else
            {
                return sortHe.FirstCharToUpper() + "," + sortDir;
            }
        }
        public static string FirstCharToUpper(this string input)
        {
            return input.First().ToString().ToUpper() + input.Substring(1);
        }
        public static bool IsUrl1(this string str)
        {
            return Uri.TryCreate(str, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
        public static bool IsUrl2(this string str)
        {
            return Regex.IsMatch(str, @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$");
        }
        public static bool IsMobile(this string str)
        {
            return Regex.IsMatch(str, @"^(((\+|00)98)|0)?9[123]\d{8}$");
        }
        public static int ToAge(this DateTime dateTime)
        {
            var age = DateTime.Today.Year - dateTime.Year;
            if (dateTime.AddYears(age) > DateTime.Today)
            {
                age--;
            }

            return age;
        }


        public static string En2Fa(this string str)
        {
            return str.Replace("0", "۰")
                .Replace("1", "۱")
                .Replace("2", "۲")
                .Replace("3", "۳")
                .Replace("4", "۴")
                .Replace("5", "۵")
                .Replace("6", "۶")
                .Replace("7", "۷")
                .Replace("8", "۸")
                .Replace("9", "۹");
        }

        public static string Fa2En(this string str)
        {
            return str.Replace("۰", "0")
                .Replace("۱", "1")
                .Replace("۲", "2")
                .Replace("۳", "3")
                .Replace("۴", "4")
                .Replace("۵", "5")
                .Replace("۶", "6")
                .Replace("۷", "7")
                .Replace("۸", "8")
                .Replace("۹", "9")
                //iphone numeric
                .Replace("٠", "0")
                .Replace("١", "1")
                .Replace("٢", "2")
                .Replace("٣", "3")
                .Replace("٤", "4")
                .Replace("٥", "5")
                .Replace("٦", "6")
                .Replace("٧", "7")
                .Replace("٨", "8")
                .Replace("٩", "9");
        }

        public static string FixPersianChars(this string str)
        {
            return str.Replace("ﮎ", "ک")
                .Replace("ﮏ", "ک")
                .Replace("ﮐ", "ک")
                .Replace("ﮑ", "ک")
                .Replace("ك", "ک")
                .Replace("ي", "ی")
                //.Replace(" ", " ")
                //.Replace("‌", " ")
                .Replace("ھ", "ه")
                .Replace("ئ", "ی");
        }

        public static string CleanString(this string str)
        {
            return str.Trim().FixPersianChars().Fa2En();
        }

        public static string NullIfEmpty(this string str)
        {
            return str?.Length == 0 ? null : str;
        }

        public static bool HasValue(this string value, bool ignoreWhiteSpace = true)
        {
            return ignoreWhiteSpace ? !string.IsNullOrWhiteSpace(value) : !string.IsNullOrEmpty(value);
        }

        public static string[] Clear(this string[] array)
        {
            if (array == null)
                return array;
            Array.Clear(array, 0, array.Length);
            return array;
        }
    }
}
