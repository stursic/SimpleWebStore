using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SimpleWebStore.Models
{
    public class BrowseProductsViewModel
    {

        public IEnumerable<Products> Products { get; set; }

        public IEnumerable<Categories> Categories { get; set; }

    }
}
