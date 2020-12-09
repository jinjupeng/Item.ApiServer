using ApiServer.Model.Entity;
using System.Collections.Generic;

namespace ApiServer.Model.Model
{
    public class SysOrgNode : Sys_Org, IDataTree<SysOrgNode, long>
    {
        public List<SysOrgNode> Children { get; set; }
        public long GetId()
        {
            return id;
        }

        public long GetParentId()
        {
            return org_pid;
        }


    }
}
