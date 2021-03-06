﻿using System;
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
            catch (Exception e)
            {
                var response = new { success = false, response = e.Message.ToString() };
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
                Item item = items.Where(x => x.Id == id).FirstOrDefault();
                if (item != null)
                {
                    var response = new { success = true, response = item };
                    return Content(JsonConvert.SerializeObject(response), "application/json");
                } else
                {
                    var response = new { success = false };
                    return Content(JsonConvert.SerializeObject(response), "application/json");
                }
            }
            catch (Exception e)
            {
                var response = new { success = false, response = e.Message.ToString() };
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
            catch (Exception e)
            {
                var response = new { success = false, response = e.Message.ToString() };
                return Content(JsonConvert.SerializeObject(response), "application/json");
            }
        }

        // add new item

        [HttpPost]
        [Route("[controller]/add")]
        public ContentResult AddItem([FromBody] ItemToBeAdded item)
        {
            if (item.ItemName == null || item.Cost < 0)
            {
                var response = new { success = false };
                return Content(JsonConvert.SerializeObject(response), "application/json");
            }
            MySqlConnection conn = new MySqlConnection(Constants.connString);
            try
            {
                string query = "INSERT INTO demoapi.items (ITEM_NAME, COST) VALUES(@name, @cost);";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.Add("@name", MySqlDbType.VarChar, 45).Value = item.ItemName;
                cmd.Parameters.Add("@cost", MySqlDbType.Float).Value = item.Cost;
                conn.Open();
                cmd.ExecuteNonQuery();
                var response = new { success = true, response = item };
                return Content(JsonConvert.SerializeObject(response), "application/json");
            }
            catch (Exception e)
            {
                var response = new { success = false, response = e.Message.ToString() };
                return Content(JsonConvert.SerializeObject(response), "application/json");
            }
            finally
            {
                conn.Dispose();
            }
        }

        // delete item

        [HttpPost]
        [Route("[controller]/delete/{id}")]
        public ContentResult DeleteItem(int id)
        {
            MySqlConnection conn = new MySqlConnection(Constants.connString);
            try
            {
                string query = "DELETE FROM demoapi.items WHERE ID = @id;";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.Add("@id", MySqlDbType.Float).Value = id;
                conn.Open();
                cmd.ExecuteNonQuery();
                var response = new { success = true, response = id };
                return Content(JsonConvert.SerializeObject(response), "application/json");
            }
            catch (Exception e)
            {
                var response = new { success = false, response = e.Message.ToString() };
                return Content(JsonConvert.SerializeObject(response), "application/json");
            }
            finally
            {
                conn.Dispose();
            }
        }

        // update with existing key
        // ideally this should be PUT not POST; not sure i have enough time to amend this

        [HttpPost]
        [Route("[controller]/update")]
        public ContentResult UpdateItem([FromBody] Item item)
        {
            if (item.Id <= 0 || item.ItemName == null || item.Cost < 0)
            {
                var response = new { success = false };
                return Content(JsonConvert.SerializeObject(response), "application/json");
            }
            MySqlConnection conn = new MySqlConnection(Constants.connString);
            try
            {                
                string query = "UPDATE items SET ITEM_NAME = @name, COST = @cost WHERE ID = @id;";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.Add("@name", MySqlDbType.VarChar, 45).Value = item.ItemName;
                cmd.Parameters.Add("@cost", MySqlDbType.Float).Value = item.Cost;
                cmd.Parameters.Add("@id", MySqlDbType.Int64).Value = item.Id;
                conn.Open();
                cmd.ExecuteNonQuery();
                var response = new { success = true, response = item };
                return Content(JsonConvert.SerializeObject(response), "application/json");
            }
            catch (Exception e)
            {
                var response = new { success = false, response = e.Message.ToString() };
                return Content(JsonConvert.SerializeObject(response), "application/json");
            }
            finally
            {
                conn.Dispose();
            }
        }

    }
}