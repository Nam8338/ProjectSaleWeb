using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace SmartZone.Models
{
    public partial class Role
    {
        public Role()
        {
            Accounts = new HashSet<Account>();
        }

        public int RoleId { get; set; }
        [Display(Name ="Tên chức vụ")]
        public string RoleName { get; set; }
        [Required]
        [StringLength(1000, ErrorMessage = "Mô tả chức vụ không được quá 1000 ký tự")]
        [Display(Name = "Mô tả chức vụ")]
        public string Descriptions { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
    }
}
