using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Configuration;
using System.IO;

using System.Reflection;
using FoodOrder.Models;



namespace FoodOrder.Shared
{
    public class Utility
    {
        public static string IsValidString(object obj)
        {
            if (obj is DBNull || obj == null)
                return "";
            else
                return obj.ToString();
        }
        public static char IsValidChar(object obj)
        {
            if (obj is DBNull || obj == null)
                return Convert.ToChar("");
            else
                return Convert.ToChar(obj.ToString());
        }
        public static DateTime? IsValidDateTime(object obj)
        {
            if (obj is DBNull || obj == null)
                return null;
            else
                return Convert.ToDateTime(obj.ToString());
        }
        public static long IsValidLong(object obj)
        {
            if (obj is DBNull || obj == null)
                return 0;
            else
                return Convert.ToInt64(obj.ToString());
        }
        public static double IsValidDouble(object obj)
        {
            if (obj is DBNull || obj == null)
                return 0;
            else
                return Convert.ToDouble(obj.ToString());
        }
        public static int IsValidInt(object obj)
        {
            if (obj is DBNull || obj == null)
                return 0;
            else
                return Convert.ToInt32(obj.ToString());
        }

        public static string GetTimeFormat(TimeSpan ts)
        {
            string format = "";
            //if(ts.Days != 0)
            //{
            //    if (ts.Days == 1 || ts.Days == -1)
            //        format += ts.Days + "<sub>day</sub> ";
            //    else
            //        format += ts.Days + "<sub>days</sub> ";
            //}
            if (ts.Hours != 0 || ts.Days != 0)
            {
                //if (ts.Hours == 1 || ts.Hours == -1)
                format += Math.Truncate(ts.TotalHours) + "<sub>hr</sub> ";

                //else
                //    format += ts.Hours + "<sub>hrs</sub> ";
            }
            if (ts.Minutes != 0)
            {
                //if (ts.Minutes == 1 || ts.Minutes == -1)
                format += ts.Minutes + "<sub>min</sub> ";
                //else
                //    format += ts.Minutes + "<sub>mins</sub> ";
            }

            if (format == "")
                format = "0<sub>min</sub>";

            return format;

        }

        public static string GetDescription(object enumValue, string defDesc)
        {
            FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString());

            if (null != fi)
            {
                object[] attrs = fi.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }

            return defDesc;
        }

        public static string ReplaceLastOccurrenceOfString(string Source, string Find, string Replace)
        {
            int place = Source.LastIndexOf(Find);

            if (place == -1)
                return Source;

            string result = Source.Remove(place, Find.Length).Insert(place, Replace);
            return result;
        }

        public static byte[] ReadFully(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static string GetDateTimeFormat(string dateStr)
        {
            if (dateStr == null || dateStr == "")
            {
                return "NULL";
            }
            else
            {
                return "to_date('" + dateStr + "', 'dd/mm/yyyy hh24:mi:ss')";
            }
        }

        public static string FirstCharToUpper(string input)
        {
            if (String.IsNullOrEmpty(input))
                return input;
            return input.First().ToString().ToUpper() + input.Substring(1).ToLower();
        }
        public static string GetProperCase(string input)
        {
            if (String.IsNullOrEmpty(input))
                return input;

            string[] words = input.Split(',', ' ');

            if (words.Length == 1)
            {
                input = FirstCharToUpper(input);
            }

            else if (words.Length > 1)
            {
                for (int index = 0; index < words.Length; index++)
                {
                    words[index] = FirstCharToUpper(words[index]);
                }
                input = String.Join(" ", words);
            }

            return input;
        }

        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}