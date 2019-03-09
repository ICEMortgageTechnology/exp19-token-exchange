using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TokenExchange.Models
{
    /// <summary>
    /// The class used for input to a TokenRequest
    /// </summary>
    public class TokenRequest
    {
        public string AuthCode { get; set; }
    }
}