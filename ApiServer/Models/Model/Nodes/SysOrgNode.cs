using ApiServer.Models.Model.DataTree;
using ApiServer.Models.Model.ViewModel;
using System.Collections.Generic;

namespace ApiServer.Models.Model.Nodes
{
    public class SysOrgNode : SysOrg, IDataTree<SysOrgNode, long>
    {
        public List<SysOrgNode> Children { get; set; }
        public long GetId()
        {
            return id;
        }

        public long GetParentId()
        {
            return orgPid;
        }


    }
}
