using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SimpleWebStore.Models
{
    public class CreateProductViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name{ get; set; }

        [Required]
        [Display(Name = "Category")]
        public string Category { get; set; }

        [Required]
        [Display(Name = "Price")]
        public double Price { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Image")]
        public string Image { get; set; }
    }
}
