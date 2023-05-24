using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace SmartZone.Models
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int CateId { get; set; }
        [Required(ErrorMessage = "Không được để trống tên loại sản phẩm!")]
        [StringLength(100, ErrorMessage = "Tên loại sản phẩm không được quá 100 ký tự")]
        [Display(Name = "Tên loại sản phẩm")]
        public string CategoryName { get; set; }
        [StringLength(1000, ErrorMessage = "Tên loại sản phẩm không được quá 1000 ký tự")]
        [Display(Name = "Mô tả sản phẩm")]
        public string Descriptions { get; set; }
        public int ParentId { get; set; }
        public int? Levels { get; set; }
        public int? Ordering { get; set; }
        public bool IsPublished { get; set; }
        public string ThumbImg { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string Cover { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
