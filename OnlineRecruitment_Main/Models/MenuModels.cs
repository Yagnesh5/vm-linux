using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineRecruitment_Main.Models
{
    public class MenuModels
    {
        public int Id { get; set; }
        public string RoleId { get; set; }
        public int EntityId { get; set; }
        public bool IsAdd { get; set; }
        public bool IsView { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsPrint { get; set; }
        public bool IsExport { get; set; }
        public string Entityname { get; set; }
    }
}