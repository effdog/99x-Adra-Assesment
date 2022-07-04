using AccountBalanceViwer.Data.Data;
using AccountBalanceViwer.Data.Models;
using AccountBalanceViwer.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace AccountBalanceViwer.Data.Repository
{
    public class BalanceRepository : IBalanceRepository
    {
        private readonly DataContext _context;
        private readonly ILogger<BalanceRepository> _logger;

        public BalanceRepository(DataContext context, ILogger<BalanceRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// To save balance data from excel to db
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public async Task<bool> SaveBalanceData(Balances values)
        {
            try
            {
                await _context.Balances.AddAsync(values);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return false;
            }

        }

        /// <summary>
        /// to check the excel uploading data exisits in database
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public async Task<bool> RecordsExitsts(int year, int month)
        {
            var record = await _context.Balances.FirstOrDefaultAsync(f => f.Month == month && f.Year == year);

            if (record != null)
                return true;

            return false;
        }

        /// <summary>
        /// Get all the data for balance table
        /// </summary>
        /// <returns></returns>
        public async Task<List<Balances>> GetBalance(int year)
        {
            List<Balances> newBalanceList = null;
            var query = await _context.Users.ToListAsync();

            var balances = (from balance in _context.Balances
                            orderby balance.Month
                            where balance.Year == year
                            select new Balances
                            {
                                Marketing = balance.Marketing,
                                Canteen = balance.Canteen,
                                CeoCar = balance.CeoCar,
                                ParkingFees = balance.ParkingFees,
                                RnD = balance.RnD,
                                Month = balance.Month,
                                Year = balance.Year
                            }); ;
            newBalanceList = balances.ToList<Balances>();
            return newBalanceList;

        }
        


    }
}
