using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Joo_Jeana_HW5.DAL;
using Joo_Jeana_HW5.Models;

namespace Joo_Jeana_HW5.Controllers
{
    public class ProductsController : Controller
    {

        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Suppliers)
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }



        // GET: Products/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            // Populate the ViewBag with the MultiSelectList of all suppliers
            ViewBag.AllSuppliers = GetAllSuppliers();
            return View();
        }

        // Helper method to get all suppliers for the dropdown
        private MultiSelectList GetAllSuppliers()
        {
            // Get all suppliers from the database and order them by name
            List<Supplier> suppliers = _context.Suppliers.OrderBy(s => s.SupplierName).ToList();

            // Create a MultiSelectList from the suppliers
            MultiSelectList supplierList = new MultiSelectList(suppliers, "SupplierID", "SupplierName");

            return supplierList;
        }


        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Product product, int[] SelectedSuppliers)
        {
            ModelState.Remove("Suppliers");

            if (!ModelState.IsValid)
            {
                ViewBag.AllSuppliers = GetAllSuppliers();
                return View(product);
            }

            product.Suppliers = new List<Supplier>();

            _context.Add(product);
            await _context.SaveChangesAsync();

            if (SelectedSuppliers != null)
            {
                foreach (int supplierID in SelectedSuppliers)
                {
                    Supplier supplier = await _context.Suppliers.FindAsync(supplierID);
                    if (supplier != null)
                    {
                        product.Suppliers.Add(supplier);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Find the product and include its suppliers
            var product = await _context.Products
                .Include(p => p.Suppliers)
                .FirstOrDefaultAsync(p => p.ProductID == id);

            if (product == null)
            {
                return NotFound();
            }

            // Get all suppliers for the dropdown
            ViewBag.AllSuppliers = GetAllSuppliers();

            // Create a list of selected supplier IDs to pre-select in the listbox
            ViewBag.SelectedSuppliers = product.Suppliers.Select(s => s.SupplierID).ToList();

            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ProductID,Name,Description,Price,ProductType")] Product product, List<int> SelectedSuppliers)
        {
            if (id != product.ProductID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get the existing product with its suppliers
                    var existingProduct = await _context.Products
                        .Include(p => p.Suppliers)
                        .FirstOrDefaultAsync(p => p.ProductID == id);

                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    // Update the scalar properties
                    existingProduct.Name = product.Name;
                    existingProduct.Description = product.Description;
                    existingProduct.Price = product.Price;
                    existingProduct.ProductType = product.ProductType;

                    // Clear existing supplier relationships
                    existingProduct.Suppliers.Clear();

                    // Add the selected suppliers
                    if (SelectedSuppliers != null)
                    {
                        foreach (var supplierId in SelectedSuppliers)
                        {
                            var supplier = await _context.Suppliers.FindAsync(supplierId);
                            if (supplier != null)
                            {
                                existingProduct.Suppliers.Add(supplier);
                            }
                        }
                    }

                    // Save changes
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            ViewBag.AllSuppliers = GetAllSuppliers();
            ViewBag.SelectedSuppliers = SelectedSuppliers;
            return View(product);
        }


        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductID == id);
        }
    }
}
