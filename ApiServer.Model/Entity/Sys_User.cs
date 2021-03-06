﻿using System;

namespace ApiServer.Model.Entity
{
    public partial class Sys_User
    {
        public long id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public long org_id { get; set; }
        public bool? enabled { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public DateTime create_time { get; set; }
    }
}
