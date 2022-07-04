using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountBalanceViwer.Business.ManagerClasses.Interfaces;
using AccountBalanceViwer.Common;
using AccountBalanceViwer.Common.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccountBalanceViwer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BalancesController : ControllerBase
    {
        private readonly IBalanceManager _balanceManager;

        public BalancesController(IBalanceManager balanceManager)
        {
            _balanceManager = balanceManager;
        }

        /// <summary>
        /// To get the data from excel sheet and save the data to DB
        /// </summary>
        /// <param name="userForRegisterDto"></param>
        /// <returns></returns>
        [HttpPost("uploadexcel")]
        public async Task<IActionResult> ExcelUpload(ExcelFileData fileData)
        {
            if(fileData.FileContent == null  && fileData.FileName == null)
                return BadRequest(Constants.EmptyFile);

            if (fileData.FileName != Constants.FileName)
                return BadRequest(Constants.IncorrectFile);

            FileStatusDto result = await _balanceManager.ExcelUpload(fileData);
            if (result.IsValid)
                return Ok();
            
            return BadRequest(result.ErrorMessage);
        }

        /// <summary>
        /// To get the data from excel sheet and save the data to DB
        /// </summary>
        /// <param name="userForRegisterDto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("getbalances")]
        public async Task<IActionResult> GetBalance(int year)
        {
            
            List<DataForTable> data = await _balanceManager.GetBalance(year);
            return Ok(data);

        }
    }
}
