using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace SmartZone.Models
{
    public partial class Product
    {
        public Product()
        {
            AttributePrices = new HashSet<AttributePrice>();
            ImportDetails = new HashSet<ImportDetail>();
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int ProductId { get; set; }
        [Display(Name ="Tên sản phẩm")]
        [StringLength(100, ErrorMessage = "Tên loại sản phẩm không được quá 100 ký tự")]
        public string ProductName { get; set; }
        public string ShortDesc { get; set; }
        [StringLength(1000, ErrorMessage = "Mô tả sản phẩm không được quá 1000 ký tự")]
        [Display(Name = "Mô tả sản phẩm")]
        public string Descriptions { get; set; }
        public int? CateId { get; set; }
        [Display(Name ="Giá gốc")]
        [Required(ErrorMessage ="Không để trống giá")]
        [Range(0, 100000000)]
        public decimal? Price { get; set; }
        [Display(Name ="Giá")]
        public decimal? SalesPrice { get; set; }
        [Display(Name ="Chiết khấu")]
        public int? Discount { get; set; }
        public string ThumbImg { get; set; }
        [Display(Name ="Video")]
        public string Video { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public bool IsBestsellers { get; set; }
        public bool Homeflag { get; set; }
        public bool IsActived { get; set; }
        public string Tag { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public int? UnitInStock { get; set; }
        public int? BrandId { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual Category Cate { get; set; }
        public virtual ICollection<AttributePrice> AttributePrices { get; set; }
        public virtual ICollection<ImportDetail> ImportDetails { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
