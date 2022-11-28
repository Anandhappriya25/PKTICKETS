using PKTickets.Interfaces;
using PKTickets.Models;
using PKTickets.Models.DTO;
using System.Linq;
using System.Xml.Linq;

namespace PKTickets.Repository
{
    public class TheaterRepository : ITheaterRepository
    {
        private readonly PKTicketsDbContext db;
        public TheaterRepository(PKTicketsDbContext db)
        {
            this.db = db;
        }

        public List<Theater> GetTheaters()
        {
            return db.Theaters.Where(x => x.IsActive == true).ToList();
        }

        public List<Theater> TheaterByLocation(string location)
        {
            var theaters = db.Theaters.Where(x => x.IsActive == true).Where(x => x.Location == location).ToList();
            return theaters;
        }

        public Theater TheaterById(int id)
        {
            var theater = db.Theaters.Where(x => x.IsActive == true).FirstOrDefault(x => x.TheaterId == id);
            return theater;
        }
        public Theater TheaterByName(string name)
        {
            var theater = db.Theaters.Where(x => x.IsActive == true).FirstOrDefault(x => x.TheaterName == name);
            return theater;
        }
        public Messages DeleteTheater(int theaterId)
        {
            Messages messages = new Messages();
            messages.Success = false;
            var theater = TheaterById(theaterId);
            if (theater == null)
            {
                messages.Message = "Theater Id  ("+ theaterId + ") is not found";
            }
            else
            {
                theater.IsActive = false;
                db.SaveChanges();
                messages.Success = true;
                messages.Message = "Theater "+theater.TheaterName+ " is Successfully Removed";
            }
            return messages;
        }

        public Messages CreateTheater(Theater theater)
        {
            Messages messages = new Messages();
            messages.Success = false;
            var theaterExist = db.Theaters.FirstOrDefault(x => x.TheaterName == theater.TheaterName);
            if (theaterExist != null)
            {
                messages.Message = "Theater Name ("+ theater.TheaterName + ") is already Registered.";
            }
            else
            {
                db.Theaters.Add(theater);
                db.SaveChanges();
                messages.Success = true;
                messages.Message = "Theater " + theater.TheaterName + " is Successfully Added";
            }
            return messages;
        }

        public Messages UpdateTheater(Theater theater)
        {
            Messages messages = new Messages();
            messages.Success = false;
            var theaterExist = TheaterById(theater.TheaterId);
            var nameExist = db.Theaters.FirstOrDefault(x => x.TheaterName == theater.TheaterName);
            if (theaterExist == null)
            {
                messages.Message = "Theater Id is not found";
            }
            else if (nameExist != null && nameExist.TheaterId != theaterExist.TheaterId)
            {
                messages.Message = "Theater Name ("+ theater.TheaterName + ")is already registered";
            }
            else
            {
                theaterExist.TheaterName = theater.TheaterName;
                theaterExist.Location = theater.Location;
                db.SaveChanges();
                messages.Success = true;
                messages.Message = "Theater " + theater.TheaterName + "is Successfully Updated";
            }
            return messages;
        }

    }
}
