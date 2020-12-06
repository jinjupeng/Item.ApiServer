using System;
using System.Collections.Generic;

namespace ApiServer.Model.Entity
{
    public partial class Sys_User_Role
    {
        public long role_id { get; set; }
        public long user_id { get; set; }
    }
}
