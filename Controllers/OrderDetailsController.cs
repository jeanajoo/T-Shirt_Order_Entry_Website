using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Joo_Jeana_HW5.DAL;
using Joo_Jeana_HW5.Models;

namespace Joo_Jeana_HW5.Controllers
{
    public class OrderDetailsController : Controller
    {
        private readonly AppDbContext _context;

        public OrderDetailsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: OrderDetails
        public IActionResult Index(int? orderID)
        {
            if (orderID == null)
            {
                return View("Error", new string[] { "Please specify an order to view!" });
            }

            // Get only the order details for this order, and include the related product
            List<OrderDetail> orderDetails = _context.OrderDetails
                .Include(od => od.Product)
                .Where(od => od.Order.OrderID == orderID)
                .ToList();

            // Store order ID in ViewBag for use in the view
            ViewBag.OrderID = orderID;

            return View(orderDetails);
        }

        // GET: OrderDetails/Create
        public IActionResult Create(int orderID)
        {
            // Create a new instance of the OrderDetail class
            OrderDetail od = new OrderDetail();

            // Find the order associated with the OrderID that was passed in
            Order dbOrder = _context.Orders.Find(orderID);

            // Set the new order detail's order equal to the order you just found
            od.Order = dbOrder;

            // Populate the ViewBag with a list of the existing products
            ViewBag.AllProducts = GetAllProducts();

            // Pass the newly created order detail object to the view
            return View(od);
        }

        // POST: OrderDetails/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderDetail orderDetail, int orderID, int SelectedProduct)
        {
            // Load the related Order and Product
            Order dbOrder = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrderID == orderID);

            Product dbProduct = await _context.Products.FindAsync(SelectedProduct);

            // Link them to the order detail
            orderDetail.Order = dbOrder;
            orderDetail.Product = dbProduct;

            // Manual validation check for required fields if not handled by ModelBinder
            if (dbProduct == null || dbOrder == null)
            {
                return View("Error", new string[] { "Order or Product not found." });
            }

            // Set the price at the time of purchase and calculate extended price
            orderDetail.ProductPrice = dbProduct.Price;
            orderDetail.ExtendedPrice = dbProduct.Price * orderDetail.Quantity;

            // If model state is valid, save to database
            if (ModelState.IsValid)
            {
                _context.OrderDetails.Add(orderDetail);
                await _context.SaveChangesAsync();

                // Redirect back to the Order Details page in the Orders Controller
                return RedirectToAction("Details", "Orders", new { id = dbOrder.OrderID });
            }

            // If we got here, something went wrong; repopulate dropdown
            ViewBag.AllProducts = GetAllProducts();
            return View(orderDetail);
        }

        // GET: OrderDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.Product)
                .FirstOrDefaultAsync(od => od.OrderDetailID == id);

            if (orderDetail == null)
            {
                return NotFound();
            }
            return View(orderDetail);
        }

        // POST: OrderDetails/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrderDetail orderDetail)
        {
            if (id != orderDetail.OrderDetailID)
            {
                return View("Error", new string[] { "ID mismatch error." });
            }

            OrderDetail dbOD = await _context.OrderDetails
                    .Include(od => od.Product)
                    .Include(od => od.Order)
                    .FirstOrDefaultAsync(od => od.OrderDetailID == id);

            if (ModelState.IsValid)
            {
                try
                {
                    dbOD.Quantity = orderDetail.Quantity;
                    dbOD.ProductPrice = dbOD.Product.Price;
                    dbOD.ExtendedPrice = dbOD.Quantity * dbOD.ProductPrice;

                    _context.Update(dbOD);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return View("Error", new string[] { "Update failed.", ex.Message });
                }

                return RedirectToAction("Details", "Orders", new { id = dbOD.Order.OrderID });
            }
            return View(orderDetail);
        }

        // GET: OrderDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.Product)
                .FirstOrDefaultAsync(m => m.OrderDetailID == id);

            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderDetail = await _context.OrderDetails
                .Include(od => od.Order)
                .FirstOrDefaultAsync(od => od.OrderDetailID == id);

            int orderId = orderDetail.Order.OrderID;

            _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Orders", new { id = orderId });
        }

        private SelectList GetAllProducts()
        {
            List<Product> products = _context.Products.OrderBy(p => p.Name).ToList();
            return new SelectList(products, "ProductID", "Name");
        }

        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetails.Any(e => e.OrderDetailID == id);
        }
    }
}