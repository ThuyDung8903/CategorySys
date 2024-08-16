using CategorySys.DTO;
using CategorySys.Models;

namespace CategorySys.Services
{
    public interface IAuthService
    {
        void Register(UserDTO userDTO);
        string Login(UserDTO userDTO);
        string GenerateToken(User user);
    }
}
