using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entites.User
{
    public class clsUser
    {
        public long USER_ID { get; set; }
        public long? USER_CODE { get; set; }
        public string USER_NAME { get; set; } = string.Empty;
        public string USER_PASSWORD { get; set; } = string.Empty;
        public string? FIRST_NAME { get; set; }
        public string? SECOND_NAME { get; set; }
        public string? LAST_NAME { get; set; }

        public long? CLI_ID { get; set; }
        public long? ROL_ID { get; set; }

        public bool STATUS { get; set; } = true;

        public string FULL_NAME => $"{FIRST_NAME} {SECOND_NAME} {LAST_NAME}".Trim();

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(FULL_NAME) ? USER_NAME : FULL_NAME;
        }
    }
}
