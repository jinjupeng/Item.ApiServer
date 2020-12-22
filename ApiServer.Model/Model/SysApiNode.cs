using ApiServer.Model.Model.ViewModel;
using System.Collections.Generic;

namespace ApiServer.Model.Model
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
