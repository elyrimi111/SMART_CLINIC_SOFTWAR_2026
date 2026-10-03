using Core.CurrentSession;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entites.Med_Report
{
    public class clsMed_Report
    {
        public long MREP_ID { get; set; }
        public long MREP_CODE { get; set; }
        public DateTime? MREP_DATE { get; set; }
        public string MREP_NAME { get; set; } = string.Empty;
        public TimeSpan? MREP_TIME { get; set; }
        public string MREP_TEXT { get; set; } = string.Empty;
        public string MREP_NOTE { get; set; } = string.Empty; 
        public long CUST_ID { get; set; }
        public long CLI_ID { get; set; } = clsCurrentSectioncs.CurrentClinc.CLI_ID; 
        public long VIS_ID { get; set; }
    }
}
