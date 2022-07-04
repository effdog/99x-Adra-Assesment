using System;
using System.Collections.Generic;
using System.Text;

namespace AccountBalanceViwer.Common.Dtos
{
    public class ExcelFileData
    {
        public string FileContent { get; set; }
        public string FileName { get; set; }
    }

    public class FileStatusDto
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class CellDataDto
    {
        public string Year { get; set; }
        public string Month { get; set; }
        public string RnD { get; set; }
        public string Canteen { get; set; }
        public string CeoCar { get; set; }
        public string Marketing { get; set; }
        public string ParkingFees { get; set; }
    }

}
