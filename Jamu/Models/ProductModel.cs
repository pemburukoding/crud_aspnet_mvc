using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Jamu.Models
{
    [Table("Products")]
    public class ProductModel
    {

        public ProductModel()
        {
            CreatedAt = DateTime.Now;
        }

        [Key]
        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        [DefaultValue(0)]
        public double Price { get; set; }

        public string Image { get; set; }

        [DisplayName("Brand")]
        public long? BrandId { get; set; }

        public virtual BrandModel Brand { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> Expired { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<CategoryModel> Category { get; set; }
    }
}