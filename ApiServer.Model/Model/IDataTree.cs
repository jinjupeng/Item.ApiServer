using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Model.Model
{
    public interface IDataTree<T, ID>
    {
        //维护树形关系：元素一
        public ID GetId();
        //维护树形关系：元素二
        public ID GetParentId();
        //子节点数组
        public List<T> Children { get; set; }

    }
}
