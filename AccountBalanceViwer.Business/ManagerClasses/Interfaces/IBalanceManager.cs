using AccountBalanceViwer.Common;
using AccountBalanceViwer.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AccountBalanceViwer.Business.ManagerClasses.Interfaces
{
    public interface IBalanceManager
    {
        Task<FileStatusDto> ExcelUpload(ExcelFileData file);

        Task<List<DataForTable>> GetBalance(int year);
    }
}