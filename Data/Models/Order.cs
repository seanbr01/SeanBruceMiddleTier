using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SeanBruceMiddleTier.Data.Models
{
    public class Order
    {
        public Order() { }

        [Key]
        [Required]
        public int Id { get; set; }

        [Column(TypeName ="date")]
        public DateTime OrderDate { get; set; }

        public string Region { get; set; }

        [ForeignKey("People")]
        public int PeopleId { get; set; }

        public virtual People Rep { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(7,2)")]
        public decimal UnitCost { get; set; }

        [Column(TypeName = "decimal(7,2)")]
        public decimal Total { get; set; }

        [ForeignKey("Item")]
        public int ItemId { get; set; }

        public virtual Item Item { get; set; }
    }
}
