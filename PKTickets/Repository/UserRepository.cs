using PKTickets.Interfaces;
using PKTickets.Models;
using PKTickets.Models.DTO;

namespace PKTickets.Repository
{
    public class UserRepository : IUserRepository
    {

        private readonly PKTicketsDbContext db;
        public UserRepository(PKTicketsDbContext db)
        {
            this.db = db;
        }

        public List<User> GetAllUsers()
        {
            return db.Users.Where(x => x.IsActive == true).ToList();
        }

        public User UserById(int id)
        {
            var userExist = db.Users.Where(x => x.IsActive == true).FirstOrDefault(x => x.UserId == id);
            return userExist;
        }

 

        public Messages CreateUser(User user)
        {
            Messages messages = new Messages();
            messages.Success = false;
            var phoneExist = db.Users.FirstOrDefault(x => x.PhoneNumber == user.PhoneNumber);
            var emailIdExist = db.Users.FirstOrDefault(x => x.EmailId == user.EmailId);
            if (phoneExist != null)
            {
                messages.Message = "The ("+ user.PhoneNumber + ") , PhoneNumber is already Registered.";
                return messages;
            }
            else if (emailIdExist != null)
            {
                messages.Message = "The (" + user.EmailId + ") , EmailId is already Registered.";
                return messages;
            }
            else
            {
                db.Users.Add(user);
                db.SaveChanges();
                messages.Success = true;
                messages.Message = user.UserName + ", Your Account is Successfully Registered";
                return messages;
            }
        }

        public Messages DeleteUser(int userId)
        {
            Messages messages = new Messages();
            messages.Success = false;
            var user = UserById(userId);
            if (user == null)
            {
                messages.Message = "User Id ("+userId +") is not found";
                return messages;
            }
            else
            {
                user.IsActive = false;
                db.SaveChanges();
                messages.Success = true;
                messages.Message = "The "+user.UserName + " Account is Successfully removed";
                return messages;
            }
        }
        

        public Messages UpdateUser(User userDTO)
        {
            Messages messages = new Messages();
            messages.Success = false;
            var userExist = UserById(userDTO.UserId);
            var phoneExist = db.Users.FirstOrDefault(x => x.PhoneNumber == userDTO.PhoneNumber);
            var emailIdExist = db.Users.FirstOrDefault(x => x.EmailId == userDTO.EmailId);
            if (userExist == null)
            {
                messages.Message = "User Id is not found";
                return messages;
            }
            else if (phoneExist != null && phoneExist.UserId != userExist.UserId)
            {
                messages.Message = "The ("+ userDTO.PhoneNumber + "), PhoneNumber is already Registered.";
                return messages;
            }
            else if (emailIdExist != null && emailIdExist.UserId != userExist.UserId)
            {
                messages.Message = "The (" + userDTO.EmailId + "), EmailId is already Registered.";
                return messages;
            }
            else
            {
                userExist.UserName = userDTO.UserName;
                userExist.PhoneNumber = userDTO.PhoneNumber;
                userExist.EmailId = userDTO.EmailId;
                userExist.Password = userDTO.Password;
                userExist.Location = userDTO.Location;
                db.SaveChanges();
                messages.Success = true;
                messages.Message = "The " + userDTO.UserName + " Account is Successfully Updated";
                return messages;
            }
        }
       

    }
}
