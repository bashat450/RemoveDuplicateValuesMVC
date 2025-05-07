using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuplicateMVC.Models
{
    public class ItemViewModel
    {
        public List<Item> OriginalItems { get; set; }
        public List<Item> UpdatedItems { get; set; }
        public bool ShowUpdated { get; set; } = false;
    }
}