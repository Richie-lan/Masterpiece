using Masterpiece.Domain.Entity;
using Masterpiece.Repository.IRepository;
using Masterpiece.Repository.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masterpiece.Bll
{
    public class ProductBll:BllBase
    {
        public ProductBll(MasterpieceDbContextFactory contextFactory) : base(contextFactory)
        {
        }

        private IProductRepository productRepository;
        public IProductRepository ProductRepository
        {
            get
            {
                if(productRepository == null)
                {
                    productRepository = new ProductRepository(contextFactory.DBContext);
                }

                return productRepository;
            }
        }

        public IList<Product> GetProducts()
        {
            return ProductRepository.GetAll();
        }
    }
}
