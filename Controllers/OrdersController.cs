using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Joo_Jeana_HW5.DAL;
using Joo_Jeana_HW5.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Joo_Jeana_HW5.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public OrdersController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            List<Order> Orders = new List<Order>();

            if (User.IsInRole("Admin"))
            {
                Orders = await _context.Orders
                            .Include(o => o.OrderDetails)
                            .Include(o => o.User)
                            .ToListAsync();
            }
            else 
            {
                Orders = await _context.Orders
                            .Where(o => o.User.UserName == User.Identity.Name)
                            .Include(o => o.OrderDetails)
                            .ToListAsync();
            }

            return View(Orders);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.OrderID == id);

            if (order == null)
            {
                return NotFound();
            }

            if (User.IsInRole("Customer") && order.User.UserName != User.Identity.Name)
            {
                return View("Error", new string[] { "This is not your order!" });
            }

            return View(order);
        }

        // GET: Orders/Create
        public async Task<IActionResult> Create()
        {
            Order order = new Order();

            if (User.IsInRole("Admin"))
            {
                ViewBag.AllCustomers = await GetAllCustomerUserNamesSelectList();
            }
            
            return View(order);
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderNotes")] Order order, string SelectedCustomer)
        {
            // 1. Tell the controller that these don't need to be filled out yet
            ModelState.Remove("User");
            ModelState.Remove("OrderDetails");
            
            // 2. ADD THIS LINE: It tells the logic to stop complaining 
            // that the dropdown is empty.
            ModelState.Remove("SelectedCustomer");

            if (ModelState.IsValid)
            {
                // Logic for establishing the User on the order
                if (User.IsInRole("Admin") && !string.IsNullOrEmpty(SelectedCustomer))
                {
                    order.User = await _userManager.FindByNameAsync(SelectedCustomer);
                }
                else
                {
                    // If you are a Customer, this finds YOUR info
                    order.User = await _userManager.FindByNameAsync(User.Identity.Name);
                }

                order.OrderDate = DateTime.Now;

                // Logic for Order Number
                if (_context.Orders.Count() == 0)
                {
                    order.OrderNumber = 70001;
                }
                else
                {
                    order.OrderNumber = _context.Orders.Max(o => o.OrderNumber) + 1;
                }

                _context.Add(order);
                await _context.SaveChangesAsync();

                return RedirectToAction("Create", "OrderDetails", new { orderID = order.OrderID });
            }

            // If we still fail, reload dropdowns for Admin
            if (User.IsInRole("Admin"))
            {
                ViewBag.AllCustomers = await GetAllCustomerUserNamesSelectList();
            }
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // We use .Include to load the Details, and .ThenInclude to load the Product names
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product) 
                .FirstOrDefaultAsync(m => m.OrderID == id);

            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderID,OrderNotes")] Order order)
        {
            if (id != order.OrderID)
            {
                return NotFound();
            }

            var dbOrder = await _context.Orders.Include(o => o.User).FirstOrDefaultAsync(o => o.OrderID == id);

            if (ModelState.IsValid)
            {
                try
                {
                    dbOrder.OrderNotes = order.OrderNotes;
                    _context.Update(dbOrder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderID)) { return NotFound(); }
                    else { throw; }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.OrderID == id);
            
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderID == id);
        }

        public async Task<SelectList> GetAllCustomerUserNamesSelectList()
        {
            var customers = await _userManager.GetUsersInRoleAsync("Customer");
            
            return new SelectList(
                customers.OrderBy(c => c.Email),
                "UserName", 
                "Email"     
            );
        }
    }
}