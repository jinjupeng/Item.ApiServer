using System;

namespace ApiServer.Models.Entity
{
    public partial class sys_config
    {
        public long id { get; set; }
        public string param_name { get; set; }
        public string param_key { get; set; }
        public string param_value { get; set; }
        public string param_desc { get; set; }
        public DateTime create_time { get; set; }
    }
}
