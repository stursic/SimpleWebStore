using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SimpleWebStore.Models
{
    public class BrowseProductsViewModel
    {

        public PagedList.IPagedList<Products> Products { get; set; }

        public IEnumerable<Categories> Categories { get; set; }

        public string ActiveCategory { get; set; }

        public string Search { get; set; }

    }
}
