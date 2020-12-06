using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Model.Model
{
    public class RoleCheckedIds
    {
        public long RoleId { get; set; }
        public List<long> CheckedIds { get; set; }

    }
}
