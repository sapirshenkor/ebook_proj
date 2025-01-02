using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace EBook_Proj.Models
{
    public class Books
    {
        [Key]
        public int BookID { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(255)]
        [DisplayName("Book Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Author is required")]
        [StringLength(100)]
        public string Author { get; set; }

        [StringLength(100)]
        public string Publisher { get; set; }

        [Column(TypeName = "Date")]
        [DisplayName("Publication Date")]
        [DataType(DataType.Date)]
        public DateTime PublicationDate { get; set; }

        [StringLength(50)]
        public string Genre { get; set; }

        [DisplayName("Age Limit")]
        [Range(0, 100)]
        public int AgeLimit { get; set; }

        [Column(TypeName = "Text")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [DisplayName("Buying Price")]
        [Range(0, 9999.99)]
        [DataType(DataType.Currency)]
        public decimal BuyingPrice { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [DisplayName("Borrow Price")]
        [Range(0, 9999.99)]
        [DataType(DataType.Currency)]
        public decimal BorrowPrice { get; set; }

        [DisplayName("Borrow Count")]
        [Range(0, 10)]
        public int BorrowCount { get; set; } = 3;

        [DisplayName("Page Count")]
        [Range(1, 10000)]
        public int PageCount { get; set; }

        [StringLength(255)]
        [DisplayName("Book Cover")]
        [DataType(DataType.ImageUrl)]
        public string BookCover { get; set; }
        
        public int Discount { get; set; }
    }
}