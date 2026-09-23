using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entites.Diagnos
{
    public class clsDiagnos
    {
        public  long DIG_ID { set; get; }
        public  long DIG_CODE { set; get; }
        public  string DIG_NAME { set; get; } = string.Empty;
        public  string DIG_TYPE { set; get; } = string.Empty;
        public string DIG_NOTE { set; get; } = string.Empty; 
        public  long CLI_ID { set; get; }

    }
}
