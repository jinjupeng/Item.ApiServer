using ApiServer.Models.Model.DataTree;
using ApiServer.Models.Model.ViewModel;
using System.Collections.Generic;

namespace ApiServer.Models.Model.Nodes
{
    public class SysApiNode : SysApi, IDataTree<SysApiNode, long>
    {
        public List<SysApiNode> Children { get; set; }
        public long GetParentId() { return apiPid; }

        public long GetId()
        {
            return id;
        }

    }
}
