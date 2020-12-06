using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Model.Model
{
    public class SysMenuNode
    {
        public List<SysMenuNode> Children { get; set; }

        public string path { get; set; }

        public string name { get; set; }
    }
}
