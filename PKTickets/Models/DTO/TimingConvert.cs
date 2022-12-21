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
    public static class Request
    {
        public static Messages messages = new Messages() {  Success = false };
        public static Messages Bad(string text)
        {
            messages.Message = text;
            messages.Status = Statuses.BadRequest;
            return messages;
        }
        public static Messages Not(string text)
        {
            messages.Message = text;
            messages.Status = Statuses.NotFound;
            return messages;
        }
        public static Messages Conflict(string text)
        {
            messages.Message = text;
            messages.Status = Statuses.Conflict;
            return messages;
        }
    }
 }
