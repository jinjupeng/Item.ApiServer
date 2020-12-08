using ApiServer.Model.Entity;
using System.Collections.Generic;

namespace ApiServer.Model.Model
{
    public class SysApiNode : Sys_Api, IDataTree<SysApiNode, long>
    {
        public List<SysApiNode> Children { get; set; }
        public long GetParentId() { return api_pid; }

        public long GetId()
        {
            return id;
        }

    }
}
