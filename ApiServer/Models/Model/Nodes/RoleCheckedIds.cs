using System.Collections.Generic;

namespace ApiServer.Models.Model.Nodes
{
    public class RoleCheckedIds
    {
        public long RoleId { get; set; }
        public List<long> CheckedIds { get; set; }

    }
}
