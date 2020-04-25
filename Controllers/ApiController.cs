using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace DemoAPI.Controllers
{
    [ApiController]
    public class ApiController : Controller
    {

        // placeholder - this is only for reference for the first iteration and expected shape of the table

        //private static readonly List<Item> items = new List<Item>() {
        //    new Item { Id = 1, ItemName = "ITEM 1", Cost = 100},
        //    new Item { Id = 2, ItemName = "ITEM 2", Cost = 200},
        //    new Item { Id = 3, ItemName = "ITEM 1", Cost = 250},
        //    new Item { Id = 4, ItemName = "ITEM 3", Cost = 300},
        //    new Item { Id = 5, ItemName = "ITEM 4", Cost = 50},
        //    new Item { Id = 6, ItemName = "ITEM 5", Cost = 40},
        //    new Item { Id = 7, ItemName = "ITEM 2", Cost = 200}
        //};


        // this whole block is obviously highly insecure, especially when committing to a public repo. i've added a constants class and put it on .gitignore, but ideally
        // this would be handled better in a real environment, either through a separate class library or what not
        private static List<Item> FetchData()
        {
            List<Item> items = new List<Item>();
            MySqlConnection conn = new MySqlConnection(Constants.connString);
            string cmd = "SELECT * FROM demoapi.items";

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter(cmd, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    Item item = new Item { Id = Convert.ToInt64(row["ID"]), ItemName = row["ITEM_NAME"].ToString(), Cost = Convert.ToInt64(row["COST"]) };
                    items.Add(item);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.ToString());
            }
            finally
            {
                conn.Dispose();
            }
            return items;
        }

        // i'm using attribute routing here for a few reasons:
        // 1. simplicity. this is a demo/poc, so getting something up and running was priority
        // 2. conciseness. there is no ambiguity about the expected paths
        // in a full project with more endpoints, these would be located in webapiconfig instead

        // base - shows all records

        [HttpGet]
        [Route("[controller]")]
        public ContentResult Get()
        {
            List<Item> items = FetchData();
            try
            {
                var response = new { success = true, response = items };
                return Content(JsonConvert.SerializeObject(response), "application/json");
            }
            catch
            {
                var response = new { success = false };
                return Content(JsonConvert.SerializeObject(response), "application/json");
            }
        }

        // not requested -- shows item by id

        [HttpGet("{id}")]
        [Route("[controller]/getitem/{id}")]
        public ContentResult GetItem(int id)
        {
            List<Item> items = FetchData();
            try
            {
                var response = new { success = true, response = items.Where(x => x.Id == id).FirstOrDefault() };
                return Content(JsonConvert.SerializeObject(response), "application/json");
            }
            catch
            {
                var response = new { success = false };
                return Content(JsonConvert.SerializeObject(response), "application/json");
            }
        }

        // task 2 -- shows list of max prices grouped by item

        [HttpGet("{itemname}")]
        [Route("[controller]/getmaxprices")]
        public ContentResult GetMaxPrices()
        {
            List<Item> items = FetchData();
            try
            {
                List<string> keys = items.Select(x => x.ItemName).Distinct().ToList() as List<string>;
                List<Item> sortedList = items.OrderBy(x => x.ItemName).ThenByDescending(x => x.Cost).ToList() as List<Item>;
                List<Item> itemMaxPrices = new List<Item>();
                foreach (string key in keys)
                {
                    itemMaxPrices.Add(sortedList.Where(x => x.ItemName == key).FirstOrDefault());
                }
                var response = new { success = true, response = itemMaxPrices };
                return Content(JsonConvert.SerializeObject(response), "application/json");
            }
            catch
            {
                var response = new { success = false };
                return Content(JsonConvert.SerializeObject(response), "application/json");
            }
        }

        // task 3 -- shows max price of item by name

        [HttpGet("{itemname}")]
        [Route("[controller]/getmaxprice/{itemname}")]
        public ContentResult GetMaxPrice(string itemname)
        {
            List<Item> items = FetchData();
            try
            {
                List<Item> itemsWithName = items.FindAll(x => x.ItemName == itemname);
                var response = new { success = true, response = itemsWithName.OrderByDescending(x => x.Cost).ElementAt(0).Cost.ToString() };
                return Content(JsonConvert.SerializeObject(response), "application/json");
            }
            catch
            {
                var response = new { success = false };
                return Content(JsonConvert.SerializeObject(response), "application/json");
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