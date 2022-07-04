using AccountBalanceViwer.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AccountBalanceViwer.Data.Repository.Interfaces
{
    public interface IBalanceRepository
    {
        Task<bool> SaveBalanceData(Balances values);

        Task<List<Balances>> GetBalance(int year);

        Task<bool> RecordsExitsts(int year, int month);
    }
}
