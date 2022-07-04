using AccountBalanceViwer.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AccountBalanceViwer.Business.ManagerClasses.Interfaces
{
    public interface IAuthManager
    {
        Task<bool> Register(UserDto user);

        Task<ReturnUser> Login(UserDto user);

    }
}
