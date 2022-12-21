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
            var time = TimingConvert.ConvertToInt(showTimeDTO.ShowTiming);
            var showTimeExist = DetailsByTiming(time);
            return (showTimeExist != null) ? Request.Conflict("ShowTiming is already Registered.")
               : Create(time);
        }

        public Messages UpdateShowTime(ShowTimeDTO showTimeDTO)
        {
            if (showTimeDTO.ShowTimeId == 0)
            {
                return Request.Bad("Enter the ShowTime Id field");
            }
            var showTimeExist = TimeById(showTimeDTO.ShowTimeId);
            var time = TimingConvert.ConvertToInt(showTimeDTO.ShowTiming);
            var nameExist = DetailsByTiming(time);
            return (showTimeExist == null) ? Request.Not("ShowTime Id is not found")
                : (nameExist != null) ? Request.Conflict("ShowTiming is already Registered.")
              : Update(showTimeExist ,time);
        }
        #region
        private Messages messages = new Messages() { Success = true };
        private Messages Create(int time)
        {
            ShowTime showTime = new ShowTime();
            showTime.ShowTiming = time;
            db.ShowTimes.Add(showTime);
            db.SaveChanges();
            messages.Status = Statuses.Created;
            messages.Message = "ShowTiming is Successfully added";
            return messages;
        }
        private Messages Update(ShowTime showTimeExist, int time)
        {
            showTimeExist.ShowTiming = time;
            db.SaveChanges();
            messages.Message = $"ShowTime of Id {showTimeExist.ShowTimeId} is Successfully Updated";
            messages.Status = Statuses.Success;
            return messages;
        }

        #endregion
    }
}
