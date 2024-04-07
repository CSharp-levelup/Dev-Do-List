using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dev_do_list_cli.Models
{
    public class AuthToken
    {
        public string access_token { get; set; }

        public int expiration { get; set; }

        public string type { get; set; }
    }
}
