
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pomelo.EntityFrameworkCore.Lolita.Tests.Models
{
    public class Item
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [Column("Image_Url")]
        [MaxLength(256)]
        public string Image { get; set; }
    }
}
