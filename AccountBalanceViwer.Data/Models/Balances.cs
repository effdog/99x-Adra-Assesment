using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AccountBalanceViwer.Data.Models
{
    [Table("Abv_AccountBalances")]
    public class Balances 
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal RnD { get; set; }
        public decimal Canteen { get; set; }
        public decimal CeoCar { get; set; }
        public decimal Marketing { get; set; }
        public decimal ParkingFees { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
    }
}
