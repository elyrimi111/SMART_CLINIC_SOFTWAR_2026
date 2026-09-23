
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entites.Orders
{
    public class clsOrders
    {


        public long ORDER_ID { get; set; }
        public long? ORDER_CODE { get; set; }
        public DateTime? ORDER_DATE { get; set; }
        public TimeSpan? ORDER_TIME { get; set; }
        public string? ORDER_NOTE { get; set; }
        public long? CUST_ID { get; set; }
        public long? CLI_ID { get; set; }

        public string? CUST_NAME { get; set; } = string.Empty;

    }
}
