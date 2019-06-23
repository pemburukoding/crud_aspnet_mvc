using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Jamu.Models
{
    [Table("Categories")]
    public class CategoryModel
    {
        public CategoryModel()
        {
            CreatedAt = DateTime.Now;
        }

        [Key]
        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<ProductModel> Product { get; set; }
    }
}