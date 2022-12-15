using System.Globalization;
using static System.Net.WebRequestMethods;

namespace PKTickets.Models.DTO
{
    public static class TimingConvert
    {
        public static int ConvertToInt(string time)
        { 
            DateTime s=DateTime.Parse(time,System.Globalization.CultureInfo.InvariantCulture);
            int hour=Convert.ToInt32(s.ToString("HH"));
            int min = Convert.ToInt32(s.ToString("mm"));
            int hours = hour * 60;
            int timing = hours + min;
            return timing;
        }
        public static string ConvertToString(int time)
        {
            int hour = time / 60;
            int min = time % 60;
            string t = hour+":"+min;
            DateTime ss = DateTime.Parse(t, System.Globalization.CultureInfo.CurrentCulture);
            string tt = ss.ToString("hh:mm tt");
            return tt;
        }
        public static string LocalHost(string name)
        {
            string value = "https://localhost:7221/api/" + name + "/";
            return value;
        }
    }
    public static class BadRequest
    {
        public static Messages MSG(string text)
        {
            Messages messages = new Messages();
            messages.Message = text;
            messages.Success=false;
            messages.Status = Statuses.BadRequest;
            return messages;
        }
            
        }
 }
