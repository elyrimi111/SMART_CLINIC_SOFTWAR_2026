using System;

namespace Core.Entites.Med_Check
{
    public class clsMed_Check
    {
        public long MEDCHECK_ID { get; set; }
        public long MEDCHECK_CODE { get; set; }
        public string MEDCHECK_NAME { get; set; } = string.Empty;
        public string MEDCHECK_TYPE { get; set; } = string.Empty;
        public decimal?  MEDCHECK_PRICE { get; set; }  
        public string MEDCHECK_NOTE { get; set; } = string.Empty;
        public long CLI_ID { get; set; }
    }
}
