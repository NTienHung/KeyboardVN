﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using KeyboardVN.Models;
using Microsoft.EntityFrameworkCore;

namespace KeyboardVN.Areas.Admin.Controllers
{
    public class ManageOrderController : Controller
    {
        private readonly KeyboardVNContext keyboardVN = new();
        // GET: ManageOrderController
        [Area("Admin")]
        public ActionResult ViewOrder()
        {
            var orderList = keyboardVN.Orders.ToList();
            return View(orderList);
        }

        [Area("Admin")]
        // GET: ManageOrderController/Details/5
        public async Task<IActionResult> ViewDetails(int? id)
        {
            if (id == null ||keyboardVN.OrderDetails == null)
            {
                return RedirectToAction(nameof(Index));
            }
            var orderDetail = await keyboardVN.Orders.Include(x => x.OrderDetails).ThenInclude(d => d.Product).AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            if (orderDetail == null)
            {
                return RedirectToAction(nameof(Index));

            }
            return View(orderDetail);
        }

        // GET: ManageOrderController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ManageOrderController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ManageOrderController/Edit/5
        [Area("Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null || keyboardVN.Orders == null)
            {
                return View("ViewOrder");
                Console.WriteLine("null id");
            }
            var editOrder = keyboardVN.Orders.Include(o => o.User).FirstOrDefault(a => a.Id == id);
            if(editOrder == null)
            {
                return View("ViewOrder");
                Console.WriteLine("null order");

            }
            return View(editOrder);
        }

        // POST: ManageOrderController/Edit/5
        [HttpPost]
        [Area("Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOrder(int id, [Bind("Id,UserId,Receiver,ShipStreet,ShipCity,ShipProvince,ShipCountry,ShipEmail,ShipPhone,Status,CreatedTime")] Order order)
        {
            Console.WriteLine("run update");

            List<string> status = new List<string> { "Processing", "Accepted", "Received", "Canceled" };
            ViewBag.status = status;
            if (id != order.Id)
            {
                //return View("ViewOrder");
                Console.WriteLine("not found");

            }

            if (ModelState.IsValid)
            {
                try
                {
                    Console.WriteLine("Update success");
                    keyboardVN.Update(order);
                    await keyboardVN.SaveChangesAsync();
                }
                catch
                {
                    if (!OrderExists(order.Id))
                    {
                        Console.WriteLine("not found");
                        return RedirectToAction(nameof(ViewOrder));
                    }
                    else
                    {
                        Console.WriteLine("not update yet");
                        throw;
                    }
                }
                return RedirectToAction(nameof(ViewOrder));
            }
            else
            {
                Console.WriteLine("not valid model");
                return RedirectToAction(nameof(ViewOrder));

            }
            return RedirectToAction(nameof(ViewOrder));

        }

        // GET: ManageOrderController/Delete/5
        [Area("Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null || keyboardVN.Orders == null)
            {
                return RedirectToAction(nameof(ViewOrder));
            }
            if (keyboardVN.Orders == null)
            {
                Console.WriteLine("its null");
                return RedirectToAction(nameof(ViewOrder));
            }
            var deleteOrder = keyboardVN.Orders.FirstOrDefault(c => c.Id == id);
            if (deleteOrder != null)
            {
                Console.WriteLine("remove done");
                keyboardVN.Orders.Remove(deleteOrder);
            }
            keyboardVN.SaveChanges();
            return RedirectToAction(nameof(ViewOrder));
        }

        // POST: ManageOrderController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        private bool OrderExists(int Id)
        {
            return (keyboardVN.Orders?.Any(o => o.Id == Id)).GetValueOrDefault();
        }
    }
}
