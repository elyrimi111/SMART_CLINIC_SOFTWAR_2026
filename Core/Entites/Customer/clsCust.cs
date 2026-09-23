
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entites.Customer
{
    public class clsCust
    {
        public  long   CUST_ID {  get; set; }
        public  long   CUST_CODE { get; set; }
        public  string CUST_F_NAME { get; set; } = string.Empty;
        public  string CUST_S_NAME { get; set; } = string.Empty;
        public  string CUST_T_NAME { get; set; } = string.Empty;
        public  string CUST_L_NAME { get; set; } = string.Empty;
        public  string CUST_AGE { get; set; } = string.Empty;
        public  DateOnly CUST_BD {  get; set; }
        public  string CUST_MOBILE1 { get; set; } = string.Empty;
        public  string CUST_MOBILE2 { get; set; } = string.Empty;
        public  string CUST_ADDRESS { get; set; } = string.Empty;
        public  string CUST_SAVE_STATE { get; set; } = string.Empty;
        public  long   CARD_ID { get; set; }
        public  long   CLI_ID { get; set; }
    }
}
