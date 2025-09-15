using System.ComponentModel.DataAnnotations;

namespace LeadImagesDemo.Models
{
    public enum OwnerType { Customer = 1, Lead = 2 }

    public class Customer
    {
        public int Id { get; set; }
        [MaxLength(120)] public string Name { get; set; } = "";
        [MaxLength(200)] public string? Email { get; set; }
    }

    public class Lead
    {
        public int Id { get; set; }
        [MaxLength(120)] public string Name { get; set; } = "";
        [MaxLength(200)] public string? Phone { get; set; }
    }

    public class ImageRecord
    {
        public int Id { get; set; }
        public OwnerType OwnerType { get; set; }
        public int OwnerId { get; set; }
        public int SequenceNo { get; set; }
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
        public string Base64Data { get; set; } = "";
    }

    public record ImageDto(int Id, int SequenceNo, DateTime CreatedUtc, string Base64Data);
}
