using System.Collections.Generic;

namespace ApiServer.Models.Model.Nodes
{
    public class UserRoleCheckedIds
    {
        public long UserId { get; set; }

        public List<long> CheckedIds { get; set; }
    }
}
