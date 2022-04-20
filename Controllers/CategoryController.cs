using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ProductsAndCategories.Models;


namespace ProductsAndCategories.Controllers
{
    public class CategoryController : Controller
    {
        public CategoryController(ProductsAndCategoriesContext context)
            : base() { _db = context; }

        [HttpGet("categories")]
        public IActionResult Index()
        {
            List<Category> allCategories = _db.Categories.ToList();
            return View("Index", allCategories);
        }

        [HttpPost("categories/create")]
        public IActionResult Create(Category newCategory)
        {
            if (!ModelState.IsValid)
            {
                List<Category> allCategories = _db.Categories.ToList();
                return View("Index", allCategories);
            }

            _db.Categories.Add(newCategory);
            _db.SaveChanges();

            return RedirectToAction("Details", new { categoryId = newCategory.CategoryId });
        }

        [HttpGet("categories/{categoryId}")]
        public IActionResult Details(int categoryId)
        {
            Category categoryWithAssociatedProducts = _db.Categories
                .Include(category => category.CategorysProductAssociations)
                    .ThenInclude(association => association.AssociatedProduct)
                .FirstOrDefault(category => category.CategoryId == categoryId);
            
            ViewBag.NonAssociatedProducts = _db.Products
                .Include(category => category.ProductsCategoryAssociations)
                .Where(product => product.ProductsCategoryAssociations
                    .All(association => association.CategoryId != categoryId))
                .ToList();

            return View("Details", categoryWithAssociatedProducts);
        }

        [HttpPost("/categories/{categoryId}/addProduct")]
        public IActionResult AddProduct(int categoryId, ProductCategoryAssociation formAssociation)
        {
            bool isAlreadyAssociated = _db.ProductCategoryAssociations
                .Any(association => 
                    association.CategoryId == categoryId 
                    && association.ProductId == formAssociation.ProductId
                );
            
            if (isAlreadyAssociated)
            {
                ModelState.AddModelError("ProductId", "Product already associated with that category");
                return View("Details", new { categoryId = categoryId });
            }

            Category dbCategory = _db.Categories
                .Include(category => category.CategorysProductAssociations)
                .FirstOrDefault(category => category.CategoryId == categoryId);
            Product dbProduct = _db.Products
                .Include(product => product.ProductsCategoryAssociations)
                .FirstOrDefault(product => product.ProductId == formAssociation.ProductId);

            ProductCategoryAssociation newAssociation = new ProductCategoryAssociation()
            {
                CategoryId = categoryId,
                ProductId = formAssociation.ProductId,
                AssociatedCategory = dbCategory,
                AssociatedProduct = dbProduct
            };

            dbCategory.CategorysProductAssociations.Add(newAssociation);
            dbProduct.ProductsCategoryAssociations.Add(newAssociation);

            _db.ProductCategoryAssociations.Add(newAssociation);
            _db.SaveChanges();

            return RedirectToAction("Details", new { categoryId = categoryId });
        }
            
        

        private ProductsAndCategoriesContext _db;
    }
}