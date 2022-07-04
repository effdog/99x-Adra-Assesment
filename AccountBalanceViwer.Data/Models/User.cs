using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountBalanceViwer.Data.Models
{
    [Table("Abv_Users")]
    public class User : IdentityUser<int>
    {
        public DateTime? CreatedDateTime { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
