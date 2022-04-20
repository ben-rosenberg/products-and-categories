using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ProductsAndCategories.Models;

namespace ProductsAndCategories.Controllers
{
    public class ProductController : Controller
    {
        public ProductController(ProductsAndCategoriesContext context)
            : base() { _db = context; }

        [HttpGet("products")]
        public IActionResult Index()
        {
            List<Product> allProducts = _db.Products.ToList();
            return View("Index", allProducts);
        }

        [HttpPost("products/create")]
        public IActionResult Create(Product newProduct)
        {
            if (!ModelState.IsValid)
            {
                List<Product> allProducts = _db.Products.ToList();
                return View("Index", allProducts);
            }

            _db.Products.Add(newProduct);
            _db.SaveChanges();

            return RedirectToAction("Details", new { productId = newProduct.ProductId });
        }

        [HttpGet("products/{productId}")]
        public IActionResult Details(int productId)
        {
            Product productWithAssociatedCategories = _db.Products
                .Include(product => product.ProductsCategoryAssociations)
                    .ThenInclude(association => association.AssociatedCategory)
                .FirstOrDefault(product => product.ProductId == productId);

            ViewBag.NonAssociatedCategories = _db.Categories
                .Include(category => category.CategorysProductAssociations)
                .Where(category => category.CategorysProductAssociations
                    .All(association => association.ProductId != productId))
                .ToList();

            return View("Details", productWithAssociatedCategories);
        }

        [HttpPost("products/{productId}/add_category")]
        public IActionResult AddCategory(int productId, ProductCategoryAssociation formAssociation)
        {
            Product dbProduct = _db.Products
                .Include(product => product.ProductsCategoryAssociations)
                .FirstOrDefault(product => product.ProductId == productId);
            Category dbCategory = _db.Categories
                .Include(category => category.CategorysProductAssociations)
                .FirstOrDefault(category => category.CategoryId == formAssociation.CategoryId);

            ProductCategoryAssociation newAssociation = new ProductCategoryAssociation()
            {
                ProductId = productId,
                CategoryId = formAssociation.CategoryId,
                AssociatedProduct = dbProduct,
                AssociatedCategory = dbCategory
            };

            _db.ProductCategoryAssociations.Add(newAssociation);

            dbProduct.ProductsCategoryAssociations.Add(newAssociation);
            dbCategory.CategorysProductAssociations.Add(newAssociation);

            _db.SaveChanges();
            return RedirectToAction("Details", new { productId = productId });
        }

        private ProductsAndCategoriesContext _db;
    }
}