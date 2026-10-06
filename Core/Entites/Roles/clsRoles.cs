using System;

namespace Core.Entities.Roles
{
    public class clsRole
    {
        public long ROL_ID { get; set; }
        public string ROL_KEY { get; set; } = string.Empty;
        public string ROL_NAME { get; set; } = string.Empty;
        public string ROL_DESCRIPTION { get; set; } = string.Empty;
        public bool STATUS { get; set; } = true;
    }
}
