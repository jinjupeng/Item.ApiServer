using System.Collections.Generic;

namespace ApiServer.Model.Model
{
    public class RoleCheckedIds
    {
        public long RoleId { get; set; }
        public List<long> CheckedIds { get; set; }

    }
}
