using System;

namespace Core.Entites.Doctors
{
    public class clsDoctors
    {
        public long DOC_ID { get; set; }
        public long? DOC_CODE { get; set; }
        public string DOC_NAME { get; set; }
        public string DOC_MAJOR { get; set; }
        public string DOC_EXP { get; set; }
        public DateTime? DOC_BD { get; set; }
        public string DOC_MOBILE { get; set; }
        public string DOC_ADDRESS { get; set; }
        public long? CLI_ID { get; set; }
        public long? USER_ID { get; set; }
    }
}
