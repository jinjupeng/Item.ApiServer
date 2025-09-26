using ApiServer.Models.Model.DataTree;
using ApiServer.Models.Model.ViewModel;
using System.Collections.Generic;

namespace ApiServer.Models.Model.Nodes
{
    public class SysMenuNode : SysMenu, IDataTree<SysMenuNode, long>
    {
        public List<SysMenuNode> Children { get; set; }

        public string path { get => url; }

        public string name { get => menuName; }

        public long GetId()
        {
            return id;
        }

        public long GetParentId()
        {
            return menuPid;
        }
    }
}
