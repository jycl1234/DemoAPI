using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace DemoAPI.Controllers
{
    [ApiController]
    public class ApiController : Controller
    {

        private static readonly List<Item> items = new List<Item>() {
            new Item { Id = 1, ItemName = "ITEM 1", Cost = 100},
            new Item { Id = 2, ItemName = "ITEM 2", Cost = 200},
            new Item { Id = 3, ItemName = "ITEM 1", Cost = 250},
            new Item { Id = 4, ItemName = "ITEM 3", Cost = 300},
            new Item { Id = 5, ItemName = "ITEM 4", Cost = 50},
            new Item { Id = 6, ItemName = "ITEM 5", Cost = 40},
            new Item { Id = 7, ItemName = "ITEM 2", Cost = 200}
        };

        // base - shows all records

        [HttpGet]
        [Route("[controller]")]
        public List<Item> Get()
        {
            try
            {
                return items;
            }
            catch
            {
                return null;
            }
        }

        // not requested -- shows item by id

        [HttpGet("{id}")]
        [Route("[controller]/{id}")]
        public Item Get(int id)
        {
            try
            {
                return items.Where(x => x.Id == id).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        // task 2 -- shows list of max prices grouped by item

        [HttpGet("{itemname}")]
        [Route("[controller]/getmaxprices")]
        public List<Item> GetMaxPrices()
        {
            try
            {
                return null;
            }
            catch
            {
                return null;
            }
        }

        // task 3 -- shows max price of item by name

        [HttpGet("{itemname}")]
        [Route("[controller]/getmaxprice/{itemname}")]
        public long GetMaxPrice(string itemname)
        {
            try
            {
                List<Item> itemsWithName = items.FindAll(x => x.ItemName == itemname);
                return itemsWithName.OrderByDescending(x => x.Cost).ElementAt(0).Cost;
            }
            catch
            {
                return 0;
            }
        }

        //// POST: API/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(IFormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: API/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: API/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here

        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: API/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: API/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}