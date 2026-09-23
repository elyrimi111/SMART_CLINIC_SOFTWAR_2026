using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entites.Medcin
{
    public class clsMedcin
    {
        public  long MED_ID { get; set; }
        public  long MED_CODE { get; set; }
        public  string MED_NAME { get; set; } = string.Empty;
        public string MED_S_NAME { get; set; } = string.Empty;
        public string MED_SOURES { get; set; } = string.Empty; 
        public  decimal MED_PRICE { get; set; }
        public  long CLI_ID { get; set; }
    }
}
