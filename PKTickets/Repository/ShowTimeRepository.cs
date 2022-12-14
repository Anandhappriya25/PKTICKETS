using PKTickets.Interfaces;
using PKTickets.Models;
using PKTickets.Models.DTO;
using System.Linq;
using System.Xml.Linq;

namespace PKTickets.Repository
{
    public class ShowTimeRepository : IShowTimeRepository
    {
        private readonly PKTicketsDbContext db;
        public ShowTimeRepository(PKTicketsDbContext db)
        {
            this.db = db;
        }

        public List<ShowTimeDTO> GetAllShowTimes()
        {
            var showTimes = db.ShowTimes.ToList();
            List<ShowTimeDTO> showTimeDTO = new List<ShowTimeDTO>();
            foreach (ShowTime showTime in showTimes)
            {
                var time = TimingConvert.ConvertToString(showTime.ShowTiming);
                showTimeDTO.Add(new ShowTimeDTO { ShowTimeId = showTime.ShowTimeId, ShowTiming = time });
            }
            return showTimeDTO;
        }

        public ShowTimeDTO ShowTimeasStringById(int id)
        {
            var showTime = db.ShowTimes.FirstOrDefault(x => x.ShowTimeId == id);
            if (showTime == null)
            {
                return null;
            }
            else
            {
                ShowTimeDTO showTimeDTO = new ShowTimeDTO();
                showTimeDTO.ShowTimeId = showTime.ShowTimeId;
                var time = TimingConvert.ConvertToString(showTime.ShowTiming);
                showTimeDTO.ShowTiming = time;
                return showTimeDTO;
            }
        }
        public ShowTime TimeById(int id)
        {
            var showTime = db.ShowTimes.FirstOrDefault(x => x.ShowTimeId == id);
            return showTime;
        }
        public ShowTime DetailsByTiming(int time)
        {
            var showTime = db.ShowTimes.FirstOrDefault(x => x.ShowTiming == time);
            return showTime;
        }
        

        public Messages CreateShowTime(ShowTimeDTO showTimeDTO)
        {
            Messages messages = new Messages();
            messages.Success = false;
            var time = TimingConvert.ConvertToInt(showTimeDTO.ShowTiming);
            var showTimeExist = DetailsByTiming(time);
            if (showTimeExist != null)
            {
                messages.Message = "ShowTiming is already Registered.";
                messages.Status = Statuses.Conflict;
            }
            else
            {
                ShowTime showTime = new ShowTime();
                showTime.ShowTiming = time;
                db.ShowTimes.Add(showTime);
                db.SaveChanges();
                messages.Success = true;
                messages.Status = Statuses.Success;
                messages.Message = "ShowTiming is Successfully added";
            }
            return messages;
        }

        public Messages UpdateShowTime(ShowTimeDTO showTimeDTO)
        {
            Messages messages = new Messages();
            messages.Success = false;
            if (showTimeDTO.ShowTimeId == 0)
            {
                messages.Message = "Enter the ShowTime Id field";
                messages.Status = Statuses.BadRequest;
                return messages;
            }
            var showTimeExist = TimeById(showTimeDTO.ShowTimeId);
            var time = TimingConvert.ConvertToInt(showTimeDTO.ShowTiming);
            var nameExist = DetailsByTiming(time);
            if (showTimeExist == null)
            {
                messages.Message = "ShowTime Id is not found";
                messages.Status = Statuses.NotFound;
            }
            else if (nameExist != null)
            {
                messages.Message = "ShowTiming is already registered";
                messages.Status = Statuses.Conflict;
            }
            else
            {
                showTimeExist.ShowTiming = time;
                db.SaveChanges();
                messages.Success = true;
                messages.Message = $"ShowTime of Id {showTimeDTO.ShowTimeId} is Successfully Updated";
                messages.Status = Statuses.Success;
            }
            return messages;
        }

    }
}
