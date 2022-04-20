using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ProductsAndCategories.Models
{
    public class ProductCategoryAssociation
    {
        [Key]
        public int ProductCategoryAssociationId { get; set; }

        [Display(Name = "Product")]
        public int ProductId { get; set; }
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public Product AssociatedProduct { get; set; }
        public Category AssociatedCategory { get; set; }
    }
}