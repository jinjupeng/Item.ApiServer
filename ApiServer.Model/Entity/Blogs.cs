using System;

namespace ApiServer.Model.Entity
{
    public partial class Blogs
    {
        public long Id { get; set; }
        public long Author_Id { get; set; }
        public string Title { get; set; }
        public DateTime Create_Time { get; set; }
        public string Detail { get; set; }
        public int? Preview_Number { get; set; }
        public int? State { get; set; }
        public int? Show { get; set; }
        public int? Type { get; set; }
        public string Link { get; set; }
    }
}
