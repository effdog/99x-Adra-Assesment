using System;
using System.Collections.Generic;
using System.Text;

namespace AccountBalanceViwer.Common.Dtos
{
    public  class ReturnUser
    {
        public string username { get; set; }
        public string Token { get; set; }
        public string  ErrorMessage { get; set; }
        public bool isValid { get; set; }
    }

    public class TokenReturn
    {
        public string username { get; set; }
        public string token { get; set; }
    }

}
