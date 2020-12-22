namespace ApiServer.Model.Model.ViewModel
{
    public class SysRole
    {
        public long id { get; set; }
        public string roleName { get; set; }
        public string roleDesc { get; set; }
        public string roleCode { get; set; }
        public int sort { get; set; }
        public bool? status { get; set; }
    }
}
