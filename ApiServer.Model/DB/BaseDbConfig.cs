namespace ApiServer.Model.DB
{
    public class BaseDbConfig
    {
        public string ConnectionString { get; set; }
        public DatabaseType DbType { get; set; }
    }

    public enum DatabaseType
    {
        SqlServer = 0,
        MySql = 1,
        Oracle = 2
    }
}
