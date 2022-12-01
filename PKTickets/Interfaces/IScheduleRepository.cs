using PKTickets.Models;
using PKTickets.Models.DTO;

namespace PKTickets.Interfaces
{
    public interface IScheduleRepository
    {
        public List<Schedule> SchedulesList();
        public Schedule ScheduleById(int id);
        public List<Schedule> SchedulesByMovieId(int id);
        public Messages DeleteSchedule(int id);
        public Messages CreateSchedule(Schedule schedule);
        public Messages UpdateSchedule(Schedule schedule);
        public List<Schedule> AvailableSchedulesList();
        public SchedulesListDTO SchedulesListByScreenId(int id);
       
        public Movie MovieById(int id);
        public Screen ScreenById(int id);
        public Theater TheaterById(int id);
        public MovieDTO DetailsByMovieId(int id);
        public List<Theater> TheaterByMovieId(int id);
    }
}
