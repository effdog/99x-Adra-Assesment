using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountBalanceViwer.Business.ManagerClasses;
using AccountBalanceViwer.Business.ManagerClasses.Interfaces;
using AccountBalanceViwer.Common;
using AccountBalanceViwer.Common.Dtos;
using AccountBalanceViwer.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccountBalanceViwer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        #region properties
        public readonly IAuthManager _authManager; 
        #endregion

        #region Constructor
        public AuthController(IAuthManager authManager)
        {
            _authManager = authManager;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// To register new users
        /// </summary>
        /// <param name="userForRegisterDto"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDto userForRegisterDto)
        {
            if (await _authManager.Register(userForRegisterDto))
                return StatusCode(201);

            return BadRequest(Constants.UserExists);

        }

        /// <summary>
        /// To register new users
        /// </summary>
        /// <param name="userForRegisterDto"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDto userForRegisterDto)
        {
            ReturnUser user = await _authManager.Login(userForRegisterDto);
            if (user.isValid == false)
                return Unauthorized(user.ErrorMessage);

            TokenReturn returnUser = new TokenReturn{
                username = user.username,
                token = user.Token
            };
            return Ok(returnUser);
        } 
        #endregion
    }
}
