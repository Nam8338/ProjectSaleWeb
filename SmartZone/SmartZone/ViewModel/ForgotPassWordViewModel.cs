using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartZone.ViewModel
{
    public class ForgotPasswordViewModel
    {
        [Key]
        [MaxLength(100)]
        [Required(ErrorMessage = "Vui lòng nhập tên tài khoản")]
        public string Mail { get; set; }
    }
}
