using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Model.Model
{
    public class UserRoleCheckedIds
    {
        public long UserId { get; set; }

        public List<long> CheckedIds { get; set; }
    }
}
