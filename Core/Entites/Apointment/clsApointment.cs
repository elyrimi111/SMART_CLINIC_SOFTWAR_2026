using System;

namespace Core.Entities.Appointments
{
    public class clsAppointments
    {
        public long APO_ID { get; set; }
        public long? APO_CODE { get; set; }
        public string? APO_NAME { get; set; }
        public DateTime? APO_DATE { get; set; }
        public TimeSpan? APO_TIME { get; set; }
        public string? APO_NOTE { get; set; }
        public long? CLI_ID { get; set; }
        public long? CUST_ID { get; set; }
        public long? DOC_ID { get; set; }

        public string? CUST_NAME { get; set; }
        public string? DOC_NAME { get; set; }

    }
}
