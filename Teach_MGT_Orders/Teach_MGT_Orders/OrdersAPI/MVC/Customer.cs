using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Teach_MGT_Orders.OrdersAPI.MVC
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        // 1 to Many - Steven Sandersons
        public virtual List<Order> Orders { get; set; }
    }
}
