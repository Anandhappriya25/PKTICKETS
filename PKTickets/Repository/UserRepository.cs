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
            return (phoneExist != null) ? PhoneConflict(user)
                : (emailIdExist != null) ? EmailConflict(user)
                : Create(user);
        }

        public Messages DeleteUser(int userId)
        {
            var user = UserById(userId);
            return (user == null) ? UserNotFound(userId)
               : Delete(user);
        }

        public Messages UpdateUser(User userDTO)
        {
            if (userDTO.UserId == 0)
            {
                return BadRequest.MSG("Enter the User Id field");
            }
            var userExist = UserById(userDTO.UserId);
            var phoneExist = db.Users.FirstOrDefault(x => x.PhoneNumber == userDTO.PhoneNumber);
            var emailIdExist = db.Users.FirstOrDefault(x => x.EmailId == userDTO.EmailId);
            return (userExist == null) ? UserNotFound(userDTO.UserId)
                : (phoneExist != null && phoneExist.UserId != userExist.UserId) ? PhoneConflict(userDTO)
                : (emailIdExist != null && emailIdExist.UserId != userExist.UserId) ? EmailConflict(userDTO)
                : Update(userExist, userDTO);
        }
        #region
        private Messages messages = new Messages() { Status = Statuses.Conflict, Success = false };
       
        private Messages PhoneConflict(User user)
        {
            messages.Message = $"The {user.PhoneNumber}, PhoneNumber is already Registered.";
            return messages;
        }

        private Messages EmailConflict(User user)
        {
            messages.Message = $"The {user.EmailId}, Email Id is already Registered.";
            return messages;
        }
        private Messages Create(User user)
        {
            db.Users.Add(user);
            db.SaveChanges();
            messages.Success = true;
            messages.Status = Statuses.Created;
            messages.Message = $"{user.UserName}, Your Account is Successfully Registered";
            return messages;
        }
        private Messages UserNotFound(int id)
        {
            messages.Message = $"User Id {id}  Not found";
            messages.Status = Statuses.NotFound;
            return messages;
        }
        private Messages Delete(User user)
        {
            user.IsActive = false;
            db.SaveChanges();
            messages.Success = true;
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
            messages.Success = true;
            messages.Message = $"The {userDTO.UserName} Account is Successfully Updated";
            messages.Status = Statuses.Success;
            return messages;
        }
        #endregion
    }
}
