using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Teach_MGT_Orders.OrdersAPI.MVC
{

    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        // 1 to Many - Steven Sandersons
        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        [JsonIgnore] // To avoid circular calls. Customer -> Order -> Customer -> Order
        public virtual Customer Customer { get; set; }

    }
}
