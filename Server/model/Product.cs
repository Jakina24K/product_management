using System.ComponentModel.DataAnnotations.Schema;

namespace Server.model
{
    public class Product
    {
        public int Id { get; set; }

        [Column("product_name")]
        public string? ProductName { get; set; }

        [Column("short_name")]
        public string? ShortName { get; set; }

        [Column("sku")]
        public string? Sku { get; set; }

        [Column("category")]
        public string? Category { get; set; }
        
        [Column("price")]
        public int Price { get; set; }

        [Column("delivery_timespan")]
        public string? DeliveryTimeSpan { get; set; }

        [Column("thumbnail_imageurl")]
        public string? ThumbnailImageUrl { get; set; }

    }
}