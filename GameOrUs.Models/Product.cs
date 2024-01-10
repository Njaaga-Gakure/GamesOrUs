using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOrUs.Models
{

    public class Product
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Genre { get; set; } = string.Empty;

        public string ImageURL { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public int Stock { get; set;  }

        public bool Featured { get; set; }
    }
}
