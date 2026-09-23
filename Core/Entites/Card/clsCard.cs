using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entites.Card
{
    public class clsCard
    {
        public long CARD_ID {  get; set; }
        public long CARD_CODE { get; set; }
        public string CARD_NAME { get; set; } = string.Empty;
        public string CARD_DATE { get; set; } = string.Empty;
        public string CARD_STATE { get; set; } = string.Empty;
        public string CARD_PER { get; set; } = string.Empty;
        public string CARD_NOTE { get; set; } = string.Empty;
        public long COM_ID { get; set; }
        public long CLI_ID { get; set; }

    }
}
