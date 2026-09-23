using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entites.Clinc
{
    public class clsClinc
    {
        public long CLI_ID { get; set; }
        public long CLI_CODE { get; set; }
        public string CLI_NAME { get; set; } = string.Empty;
        public string CLI_LOC { get; set; } = string.Empty;
        public string CLI_NOTE { get; set; }
    }
}

