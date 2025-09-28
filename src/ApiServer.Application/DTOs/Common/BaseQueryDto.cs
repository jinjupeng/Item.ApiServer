using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiServer.Application.DTOs.Common
{
    public class BaseQueryDto
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string? SearchTerm { get; set; }

        public string? SortBy { get; set; }

        public bool SortDescending { get; set; } = false;

        public Dictionary<string, object> Filters { get; set; } = new();
    }
}
