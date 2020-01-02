using System;
using Masterpiece.Bll;
using Masterpiece.Domain.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Masterpiece.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            MasterpieceDbContextFactory.Init();
            ProductBll bll = new ProductBll(new MasterpieceDbContextFactory());
            Product product = new Product()
            {
                Name = "guoqi",
                Age = 18,
                CreateTime = DateTime.Now
            };
            var result = bll.Add(product);
            Assert.Equals(1, result);
        }
    }
}
