using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entites.User
{
    public class clsUser
    {
        public long USER_ID { set; get; }
        public long USER_CODE { set; get; }
        public string USER_NAME { set; get; } = string.Empty;
        public string USER_PASSWORD { set; get; } = string.Empty;
        public string USER_TYPE { set; get; } = string.Empty;
        public long CLI_ID { set; get; }

        // إرجاع الاسم للواجهة بنفس المسمى الخاص بك
        public override string ToString()
        {
            return USER_NAME;
        }
    }
}
