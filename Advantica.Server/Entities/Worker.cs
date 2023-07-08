using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Advantica.Server.Entities
{
    public class Worker
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;

        public string? MiddleName { get; set; }

        [Required]
        public DateTime Birthday { get; set; }

        [Required]
        public bool HasChildren { get; set; }

        [Required]
        public int SexId { get; set; }

        [Required]
        public virtual Sex Sex { get; set; }
    }
}
