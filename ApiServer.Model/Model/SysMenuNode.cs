using ApiServer.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Model.Model
{
    public class SysMenuNode : Sys_Menu, IDataTree<SysMenuNode, long>
    {
        public List<SysMenuNode> Children { get; set; }

        public string path { get => url; }

        public string name { get => menu_name; }

        public long GetId()
        {
            return id;
        }

        public long GetParentId()
        {
            return menu_pid;
        }
    }
}
