using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.Entites.Vist
{
    public class clsVist
    {
      public  long VIS_ID { set; get;  }
      public  long   VIS_CODE { set; get; }
      public  string  VIS_NAME { set; get; } = string.Empty;
      public  DateOnly  VIS_DATE { set; get; }  
      public  string  VIS_TYPE { set; get; } = string.Empty;
      public TimeOnly VIS_TIME { set; get; }  
      public  long?   CUST_ID { set; get; }
      public  long?   CLI_ID { set; get; }
      public  long   APO_ID { set; get; }
      public  long   DOC_ID { set; get; }
      public  long   SERLIST_ID { set; get; }
      public decimal VIS_PRICE { set; get; }
      public decimal VIS_DISCOUNT { set; get; }
      public decimal VIS_TOTAL { set; get; }
      public string VIS_PAY_TYPE { set; get; } = string.Empty;

    }
}
