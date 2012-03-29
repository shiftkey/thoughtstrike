using System;
using System.ComponentModel.DataAnnotations;

namespace Ideastrike.Models
{
    public class Activity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public virtual Idea Idea { get; set; }

        public virtual User User { get; set; }

        public DateTime Time { get; set; }

    }
}