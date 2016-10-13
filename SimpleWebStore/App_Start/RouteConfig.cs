﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SimpleWebStore
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            
            routes.MapRoute(
               name: "ProductsCreate",
               url: "Products/Create",
               defaults: new { controller = "Products", action = "Create" }
           );

            routes.MapRoute(
              name: "ProductsByCategoryByPage",
              url: "Products/{category}/Page{page}",
              defaults: new { controller = "Products", action = "Index" }
          );


            routes.MapRoute(
            name: "ProductsByPage",
            url: "Products/{page}",
            defaults: new { controller = "Products", action = "Index" }
        );

            routes.MapRoute(
              name: "ProductsByCategory",
              url: "Products/{category}",
              defaults: new { controller = "Products", action = "Index" }
          );

           

            routes.MapRoute(
             name: "ProductsIndex",
             url: "Products",
             defaults: new { controller = "Products", action = "Index" }
         );


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Products", action = "Index", id = UrlParameter.Optional }
            );

           
        }
    }
}
