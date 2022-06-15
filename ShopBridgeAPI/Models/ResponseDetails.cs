using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace ShopBridgeAPI.Models
{
    public class ResponseDetails
    {
        public string error_code { get; set; }
        public string error_desc { get; set; }
        public DataSet Data { get; set; }
        public byte[] filedata { get; set; }
    }
}
