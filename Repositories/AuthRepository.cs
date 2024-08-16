using CategorySys.DTO;
using CategorySys.Models;

namespace CategorySys.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;

        public AuthRepository(AppDbContext context)
        {
            _context = context;
        }

        public User Login(UserDTO userDTO)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.UserName == userDTO.UserName);
                if (user == null || !BCrypt.Net.BCrypt.Verify(userDTO.Password, user.Password))
                {
                    throw new Exception("Invalid username or password");
                }
                return user;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Register(UserDTO userDTO)
        {
            try
            {
                _context.Users.Add(new User()
                {
                    UserName = userDTO.UserName,
                    Password = BCrypt.Net.BCrypt.HashPassword(userDTO.Password)
                });
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
