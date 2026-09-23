using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entites.Services
{
    public class clsService
    {
        public long SER_ID { set; get; } 
        public string SER_CODE { set; get;  } = string.Empty;
        public string SER_NAME   { set; get;  } = string.Empty;
        public string SER_TYPE   { set; get;  } = string.Empty;
        public string SER_PRICE  { set; get;  } = string.Empty;
        public string SER_NOTE { set; get; } = string.Empty;
        public long CLI_ID { set; get; }
    }
}
