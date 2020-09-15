using ElevenNote.Models.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ElevenNote.Services.User
{
    public interface IUserService
    {
        Task<bool> RegisterUser(UserRegister model);
    }
}
