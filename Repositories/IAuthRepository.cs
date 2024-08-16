using CategorySys.DTO;
using CategorySys.Models;

namespace CategorySys.Repositories
{
    public interface IAuthRepository
    {
        void Register(UserDTO userDTO);
        User Login(UserDTO userDTO);
    }
}
