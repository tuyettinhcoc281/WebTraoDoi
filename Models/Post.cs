using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExchangeWebsite.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public required decimal? Price { get; set; }
        [Required]
        [StringLength(100)]
        public string City { get; set; }

        [StringLength(20)]
        public string? ZipCode { get; set; }

        [StringLength(100)]
        public string? Make { get; set; }

        [StringLength(100)]
        public string? ModelNumber { get; set; }
        [Required]
        [StringLength(50)]
        public string Condition { get; set; }

        public bool CryptocurrencyAccepted { get; set; }
        public bool DeliveryAvailable { get; set; }
        [Required]
        [StringLength(100)]
        public string ContactEmail { get; set; }
        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        public bool ShowAddress { get; set; }
        [Required]
        [StringLength(20)]
        public string Language { get; set; }

        public DateTime PostedAt { get; set; } = DateTime.UtcNow;

        // Foreign key
        [Required]
        public int? CategoryId { get; set; }
        public virtual Category? Category { get; set; }
       
        public virtual ICollection<PostImage> PostImages { get; set; } = new List<PostImage>();

        // Foreign key
        public string? UserId { get; set; }

        public virtual User? User { get; set; }
    }
}