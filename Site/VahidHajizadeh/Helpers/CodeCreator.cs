using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace Helpers
{
    public static class CodeCreator
    {
        private static DatabaseContext db = new DatabaseContext();

        public static int ReturnProductCode()
        {
            Product product = db.Products.OrderByDescending(current => current.Code).FirstOrDefault();

            if (product != null)
            {
                return product.Code + 1;
            }

            return 100;
        }
    }
}