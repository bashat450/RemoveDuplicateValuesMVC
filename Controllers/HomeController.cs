using System.Web.Mvc;
using DuplicateMVC.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;

namespace DuplicateMVC.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var items = GetAllItems();

            var viewModel = new ItemViewModel
            {
                OriginalItems = items,
                ShowUpdated = false
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Index(string removeDuplicates)
        {
            var originalItems = GetAllItems();

            // Remove duplicates
            string connectionString = ConfigurationManager.ConnectionStrings["dbDub"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                    WITH DuplicateItems AS (
                        SELECT 
                            Id,
                            ROW_NUMBER() OVER (PARTITION BY Name ORDER BY Id) AS rn
                        FROM Item
                    )
                    DELETE FROM DuplicateItems WHERE rn > 1;
                ";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            var updatedItems = GetAllItems();

            var viewModel = new ItemViewModel
            {
                OriginalItems = originalItems,
                UpdatedItems = updatedItems,
                ShowUpdated = true
            };

            return View(viewModel);
        }

        private List<Item> GetAllItems()
        {
            List<Item> items = new List<Item>();
            string connectionString = ConfigurationManager.ConnectionStrings["dbDub"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Id, Name FROM Item";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new Item
                        {
                            Id = (int)reader["Id"],
                            Name = reader["Name"].ToString()
                        });
                    }
                }
            }

            return items;
        }
    }
}
