using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Common.DB
{
    //提供数据库的DbConnect类
    public static class DbConnectionProvider
    {
        /// <summary>
        /// 当前的连接类型
        /// </summary>
        public static readonly BaseDbConfig Connection = new BaseDbConfig
        {
            ConnectionString = ConfigTool.Configuration["Setting:Conn"],
            DbType = (DatabaseType)int.Parse(Common.ConfigTool.Configuration["Setting:ConnType"].ToString())
        };


        /// <summary>
        /// DbConnection
        /// </summary>
        public static dynamic Cnn
        {
            get
            {
                switch (Connection.DbType)
                {
                    case DatabaseType.MySql:
                        return new MySqlConnection(Connection.ConnectionString);
                    case DatabaseType.SqlServer:
                        return new SqlConnection(Connection.ConnectionString);
                    case DatabaseType.Oracle:
                        return new OracleConnection(Connection.ConnectionString);
                    default:
                        return new SqlConnection(Connection.ConnectionString);
                }
            }
        }
    }
}
