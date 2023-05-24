using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace SmartZone.Models
{
    public partial class Account
    {
        public Account()
        {
            ImportTickets = new HashSet<ImportTicket>();
        }

        public int UserId { get; set; }
        [Required(ErrorMessage = "Tên tài khoản không được để trống!")]
        [StringLength(50, ErrorMessage = "Tên tài khoản không vượt quá 50 kí tự")]
        [Display(Name = "Tên tài khoản")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Mật khẩu không được để trống!")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Mật khẩu không vượt quá 100 kí tự")]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }
        [DataType(DataType.EmailAddress, ErrorMessage = "Sai định dạng Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Họ và tên không được để trống!")]
        [Display(Name = "Họ và Tên")]
        public string FullName { get; set; }
        [Display(Name = "SĐT")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(maximumLength: 12, ErrorMessage = "Số điện thoại quá dài hoặc quá ngắn", MinimumLength = 9)]
        public string Phone { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? LastLogin { get; set; }
        public int? RoleId { get; set; }
        public bool IsActived { get; set; }
        public string Randomkey { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<ImportTicket> ImportTickets { get; set; }
    }
}
