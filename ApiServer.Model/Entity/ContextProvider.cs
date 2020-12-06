using ApiServer.Common;
using ApiServer.Model.DB;
using Microsoft.EntityFrameworkCore;

namespace ApiServer.Model.Entity
{
    public class ContextProvider
    {
        /// <summary>
        /// 获取Context
        /// </summary>
        /// <returns></returns>
        public DbContext GetContext()
        {

            //从配置文件中读取当前的数据库类型
            string connTypeStr = ConfigTool.Configuration["Setting:ConnType"];
            //若为SqlServer，返回SqlServer的Context
            if (connTypeStr == ((int)DatabaseType.SqlServer).ToString())
            {
                return new ContextSqlServer();
            }
            //若为Mysql，返回Mysql的Context
            else if (connTypeStr == ((int)DatabaseType.MySql).ToString())
            {
                return new ContextMySql();
            }
            //若为Oracle，返回Oracle的Context
            else if (connTypeStr == ((int)DatabaseType.Oracle).ToString())
            {
                return new ContextOracle();
            }

            //其他情况，返回Mysql的Context
            else
            {
                return new ContextMySql();
            }

        }
    }
}
