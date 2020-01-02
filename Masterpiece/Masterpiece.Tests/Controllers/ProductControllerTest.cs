using System;
using Masterpiece.Bll;
using Masterpiece.Domain.Entity;
using Masterpiece.Web.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Masterpiece.Tests.Controllers
{
    [TestClass]
    public class ProductControllerTest
    {
        [TestMethod]
        public void Index()
        {
            ProductController controller = new ProductController();
            Product product = new Product()
            {
                Name = "guoqi",
                Age = 18,
                CreateTime = DateTime.Now
            };
            var result = controller.Index(product);
            Assert.IsNotNull(result);
        }
    }
}
