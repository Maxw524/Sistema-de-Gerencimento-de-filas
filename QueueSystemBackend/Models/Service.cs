using QueueSystemBackend.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QueueSystemBackend.Models
{
    public class Service
    {
        [Key]
        public int Id { get; set; }
      [Required]
    public string Name { get; set; } = string.Empty;
        
       public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}