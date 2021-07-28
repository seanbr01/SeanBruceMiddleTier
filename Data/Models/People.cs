using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SeanBruceMiddleTier.Data.Models
{
    public class People
    {
        public People() { }

        [Key]
        [Required]
        public int Id { get; set; }
        
        [Column(name: "RepLastName")]
        public string LastName { get; set; }
    }
}
