﻿using System.Collections.Generic;

namespace ApiServer.Model.Model
{
    public class UserRoleCheckedIds
    {
        public long UserId { get; set; }

        public List<long> CheckedIds { get; set; }
    }
}
