using AccountBalanceViwer.Business.ManagerClasses.Interfaces;
using AccountBalanceViwer.Common;
using AccountBalanceViwer.Common.Dtos;
using AccountBalanceViwer.Data.Data;
using AccountBalanceViwer.Data.Models;
using AccountBalanceViwer.Data.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBalanceViwer.Business.ManagerClasses
{
    public class BalanceManager : IBalanceManager
    {
        #region Properties
        private readonly IBalanceRepository _balanceRepo;
        private readonly ILogger<BalanceManager> _logger;
        private readonly IConfiguration _config;
        #endregion

        #region Constructor
        public BalanceManager(IBalanceRepository balanceRepo, ILogger<BalanceManager> logger, IConfiguration config)
        {
            _balanceRepo = balanceRepo;
            _logger = logger;
            _config = config;
        }

        #endregion

        /// <summary>
        /// To get the data from the excel template and save to database
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public async Task<FileStatusDto> ExcelUpload(ExcelFileData Data)
        {
            byte[] fileByte;
            string fileLocation = string.Empty;
            string ContainerConnectionString = string.Empty;
            string fileNameForBlob = string.Empty;
            FileStatusDto excelFileStatus = new FileStatusDto();

            //Decode base64 string
            try
            {
                String[] substrings = Data.FileContent.Split(',');
                string imgData = substrings[1];
                fileByte = Convert.FromBase64String(imgData);


                //Read excel using a file stream
                using (Stream fileStream = new MemoryStream(fileByte))
                {
                    using (var excelPackage = new ExcelPackage(fileStream))
                    {
                        var workSheet = excelPackage.Workbook.Worksheets[1];
                        int rowCount = workSheet.Dimension.Rows;
                        int colCount = workSheet.Dimension.Columns;

                        //Validate Excel file TODO rows and columns

                        CellDataDto cellInfo = new CellDataDto();

                        //Starting from row two since the 1st row is headers
                        for (int row = 2; row <= rowCount; row++)
                        {
                            
                            var year = workSheet.Cells[row, 1].Value;

                            //check if the year cell is has a value or a valid year format
                            if ( year == null || string.IsNullOrEmpty(year.ToString().Trim()))
                            {
                                excelFileStatus.IsValid = false;
                                excelFileStatus.ErrorMessage = "Invalid Year";
                                return excelFileStatus;                          
                            }
                            else if (year.ToString().Length != 4)
                            {
                                excelFileStatus.IsValid = false;
                                excelFileStatus.ErrorMessage = "Invalid Year";
                                return excelFileStatus;
                            }
                            else
                            {
                                cellInfo.Year = year.ToString();
                            }

                            cellInfo.Month = workSheet.Cells[row, 2].Value.ToString();
                            cellInfo.RnD = workSheet.Cells[row, 3].Value.ToString();
                            cellInfo.Canteen = workSheet.Cells[row, 4].Value.ToString();
                            cellInfo.CeoCar = workSheet.Cells[row, 5].Value.ToString();
                            cellInfo.Marketing = workSheet.Cells[row, 6].Value.ToString();
                            cellInfo.ParkingFees = workSheet.Cells[row, 7].Value.ToString();

                        };

                        //create object for DB 
                        Balances valuesForDb = new Balances();
                        valuesForDb.Year = Convert.ToInt32(cellInfo.Year);
                        valuesForDb.Month = Convert.ToInt32(cellInfo.Month);
                        valuesForDb.RnD = Convert.ToDecimal(cellInfo.RnD);
                        valuesForDb.Canteen = Convert.ToDecimal(cellInfo.Canteen);
                        valuesForDb.CeoCar = Convert.ToDecimal(cellInfo.CeoCar);
                        valuesForDb.Marketing = Convert.ToDecimal(cellInfo.Marketing);
                        valuesForDb.ParkingFees = Convert.ToDecimal(cellInfo.Marketing);

                        //check if records exists for the given year and month
                        if (await _balanceRepo.RecordsExitsts(valuesForDb.Year, valuesForDb.Month))
                        {
                            excelFileStatus.IsValid = false;
                            excelFileStatus.ErrorMessage = "Records already exist for the given month";

                            return excelFileStatus;
                        }

                        //save data to DB    
                        if (await _balanceRepo.SaveBalanceData(valuesForDb))
                        {
                            _logger.LogInformation("successfully updated database");
                            excelFileStatus.IsValid = true;

                            //Upload the excel file the azure blob
                            fileLocation = _config.GetSection("AppSettings:ExcelFolderPath").Value;
                            ContainerConnectionString = _config.GetSection("AppSettings:FileStorageConnection").Value;

                            string[] stringArray = Data.FileName.Split('.');
                            string fileExtension = stringArray[stringArray.Count() - 1];

                            fileNameForBlob = "Template_" + valuesForDb.Year + "_" + valuesForDb.Month + "." + fileExtension;
                            AccountBalanceViwer.Common.HelperMethods.UploadToBlob(fileByte, fileNameForBlob, fileLocation, ContainerConnectionString);
                        }
                        else
                        {
                            excelFileStatus.IsValid = false;
                            excelFileStatus.ErrorMessage = "Error saving data to database";
                            _logger.LogError("Error saving data to database");
                        }
                        return excelFileStatus;
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                excelFileStatus.IsValid = false;
                excelFileStatus.ErrorMessage = "Error occured uploading excel";
                return excelFileStatus;
            }

        }

        /// <summary>
        /// Get all data from balances tabel
        /// </summary>
        /// <returns></returns>
        public async Task<List<DataForTable>> GetBalance(int year)
        {
            List<DataForTable> newDataList = new List<DataForTable>();
            List<Balances> balance = await _balanceRepo.GetBalance(year);

            balance.ForEach(bal => newDataList.Add(
                new DataForTable()
                {
                    Canteen = bal.Canteen.ToString(),
                    CeoCar = bal.CeoCar.ToString(),
                    Marketing = bal.Marketing.ToString(),
                    ParkingFees = bal.ParkingFees.ToString(),
                    RnD = bal.RnD.ToString(),
                    Month = HelperMethods.GetMonthName(bal.Month),
                    Year = bal.Year.ToString()
                }));


            return newDataList;

        }


    }
}
