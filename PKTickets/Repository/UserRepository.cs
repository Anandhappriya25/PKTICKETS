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
            return db.Users.Where(x => x.IsActive).ToList();
        }

        public User UserById(int id)
        {
            var userExist = db.Users.Where(x => x.IsActive).FirstOrDefault(x => x.UserId == id);
            return userExist;
        }

        public Messages CreateUser(User user)
        {
            var phoneExist = db.Users.FirstOrDefault(x => x.PhoneNumber == user.PhoneNumber);
            var emailIdExist = db.Users.FirstOrDefault(x => x.EmailId == user.EmailId);
            return (phoneExist != null) ? Request.Conflict($"The {user.PhoneNumber}, PhoneNumber is already Registered.")
                : (emailIdExist != null) ? Request.Conflict($"The {user.EmailId}, Email Id is already Registered.")
                : Create(user);
        }

        public Messages DeleteUser(int userId)
        {
            var user = UserById(userId);
            return (user == null) ? Request.Not($"User Id {userId} Not found") 
               : Delete(user);
        }

        public Messages UpdateUser(User userDTO)
        {
            if (userDTO.UserId == 0)
            {
                return Request.Bad("Enter the User Id field");
            }
            var userExist = UserById(userDTO.UserId);
            var phoneExist = db.Users.FirstOrDefault(x => x.PhoneNumber == userDTO.PhoneNumber);
            var emailIdExist = db.Users.FirstOrDefault(x => x.EmailId == userDTO.EmailId);
            return (userExist == null) ? Request.Not($"User Id {userDTO.UserId}  Not found")
                : (phoneExist != null && phoneExist.UserId != userExist.UserId) ? Request.Conflict($"The {userDTO.PhoneNumber}, PhoneNumber is already Registered.")
                : (emailIdExist != null && emailIdExist.UserId != userExist.UserId) ? Request.Conflict($"The {userDTO.EmailId}, Email Id is already Registered.")
                : Update(userExist, userDTO);
        }
        #region
        private Messages messages = new Messages() { Success = true };
        private Messages Create(User user)
        {
            db.Users.Add(user);
            db.SaveChanges();
            messages.Status = Statuses.Created;
            messages.Message = $"{user.UserName}, Your Account is Successfully Registered";
            return messages;
        }
        private Messages Delete(User user)
        {
            user.IsActive = false;
            db.SaveChanges();
            messages.Message = $"The {user.UserName} Account is Successfully removed";
            messages.Status = Statuses.Success;
            return messages;
        }
        private Messages Update(User userExist,User userDTO)
        {
            userExist.UserName = userDTO.UserName;
            userExist.PhoneNumber = userDTO.PhoneNumber;
            userExist.EmailId = userDTO.EmailId;
            userExist.Password = userDTO.Password;
            userExist.Location = userDTO.Location;
            db.SaveChanges();
            messages.Message = $"The {userDTO.UserName} Account is Successfully Updated";
            messages.Status = Statuses.Success;
            return messages;
        }
        #endregion
    }
}
