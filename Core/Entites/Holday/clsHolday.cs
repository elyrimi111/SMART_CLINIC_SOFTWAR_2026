using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entites.Holday
{
    public class clsHolday
    {
        public long HOL_ID { get; set; }
        public long? HOL_CODE { get; set; }
        public DateTime? HOL_DATE { get; set; }
        public TimeSpan? HOL_TIME { get; set; }
        public string HOL_NAME { get; set; } = string.Empty; 
        public string HOL_TEXT { get; set; } = string.Empty;
        public string HOL_NOTE { get; set; } = string.Empty;
        public long? CUST_ID { get; set; }
        public long? CLI_ID { get; set; }
        public long? VIS_ID { get; set; }
    }
}
