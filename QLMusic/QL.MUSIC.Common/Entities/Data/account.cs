using System;
using System.Collections.Generic;
using System.Text;

namespace QL.MUSIC.Common.Entities
{
    public class account : UserInfo
    {
        public int id { get; set; }
        public string accountName { get; set; }
        public string password { get; set; }

    }
}
