using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartZone.ViewModel
{
    public class RegisterViewModel
    {
        [Key]
        public int CustommerId { get; set; }

        [Display(Name ="Tên tài khoản")]
        [Remote(action:"ValidateUserName",controller:"Accounts")]
        public string Username { get; set; }

        [Display(Name = "Mật khẩu")]
        [MinLength(6,ErrorMessage ="Mật khẩu cần tối thiểu 6 ký tự")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [MinLength(6, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 5 ký tự")]
        [Display(Name = "Nhập lại mật khẩu")]
        [Compare("Password", ErrorMessage = "Vui lòng nhập mật khẩu giống nhau")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Họ và Tên")]
        [Required(ErrorMessage = "Vui lòng nhập Họ và Tên")]
        public string FullName { get; set; }

        [Display(Name ="Ngày sinh")]
        public DateTime? BirthDay { get; set; }

        public string Avatar { get; set; }

        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [DataType(DataType.EmailAddress, ErrorMessage = "Sai định dạng Email")]
        [Remote(action: "ValidateEmail", controller: "Accounts")]
        public string Mail { get; set; }

        [Display(Name = "SĐT")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(maximumLength: 12, ErrorMessage = "Số điện thoại quá dài hoặc quá ngắn", MinimumLength = 9)]
        [Remote(action: "ValidatePhone", controller: "Accounts")]
        public string Phone { get; set; }

        [Display(Name = "Tỉnh/Thành Phố")]
        public string Province { get; set; }

        [Display(Name = "Quận/Huyện")]
        public string District { get; set; }

        [Display(Name = "Phường/Xã")]
        public string Ward { get; set; }
    }
}
