using PKTickets.Models;
using PKTickets.Models.DTO;

namespace PKTickets.Interfaces
{
    public interface IUserRepository
    {

        public List<User> GetAllUsers();
        public User UserById(int id);
        public User UserByPhone(string phone);
        public User UserByEmail(string email);
        public Messages CreateUser(User user);
        public Messages UpdateUser(User userDTO);
        public Messages DeleteUser(int userId);

    }
}
