using AccountBalanceViwer.Business.ManagerClasses.Interfaces;
using AccountBalanceViwer.Common;
using AccountBalanceViwer.Common.Dtos;
using AccountBalanceViwer.Data.Models;
using AccountBalanceViwer.Data.Repository;
using AccountBalanceViwer.Data.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AccountBalanceViwer.Business.ManagerClasses
{
    public class AuthManager : IAuthManager
    {
        #region Propeties
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AuthManager> _logger;

        #endregion

        #region Constructor
        public AuthManager(IConfiguration config,
            UserManager<User> userManager, SignInManager<User> signInManager, ILogger<AuthManager> logger)
        {
            _config = config;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method for login logic
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<ReturnUser> Login(UserDto userForLoginDto)
        {
            string token = string.Empty;
            ReturnUser returnUser = new ReturnUser();

            try
            {
                var user = await _userManager.FindByNameAsync(userForLoginDto.UserName);

                if (user == null)
                {
                    returnUser.ErrorMessage = "User does not exists";
                    returnUser.isValid = false; 
                }
                else
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user, userForLoginDto.Password, false);

                    if (result.Succeeded)
                    {
                        var appUser = await _userManager.Users.FirstOrDefaultAsync(p => p.NormalizedUserName == userForLoginDto.UserName.ToUpper());
                        token = GenerateJwtToken(appUser);

                        returnUser.username = appUser.UserName;
                        returnUser.Token = token;
                        returnUser.isValid = true;
                    }
                    else
                    {
                        returnUser.ErrorMessage = "Incorrect Credentials";
                        returnUser.isValid = false;
                    }
                }

            }
            catch (Exception ex)
            {
                returnUser.ErrorMessage = "Error in Login";
                returnUser.isValid = false;
                _logger.LogError(ex.Message.ToString());
            }

            return returnUser;

        }

        /// <summary>
        /// To create new users
        /// </summary>
        /// <param name="user">userdto carries username and password</param>
        /// <returns></returns>
        public async Task<bool> Register(UserDto userReg)
        {
            try
            {
                var userToCreate = new User
                {
                    UserName = userReg.UserName
                };

                var result = await _userManager.CreateAsync(userToCreate, userReg.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"User Created Succeffully {userToCreate.UserName}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
            return false;
        }


        #endregion

        #region Private Methods
        /// <summary>
        /// To Generate JWT token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName)
                };

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
        #endregion
    }
}
