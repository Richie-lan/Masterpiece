using Masterpiece.Domain.Entity;
using Masterpiece.Domain.Enum;
using Masterpiece.Domain.MasterException;
using Masterpiece.Domain.MasterResource;
using Masterpiece.Repository.IRepository;
using Masterpiece.Repository.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        public void Trans()
        {
            using (DbContextTransaction transaction = contextFactory.DBContext.Database.BeginTransaction())
            {
                try
                {
                    BeginTran();
                    if (true)
                    {
                        
                    }
                    else
                    {
                        throw new MasterAdvancedException<List<Product>>(ErrorCodeEnum.Wrong, MasterErrorMsg.ConstWrong);
                    }
                    transaction.Commit();
                    CommitTran();
                }
                catch (Exception ex)
                {
                    RollbackTran();
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
    }
}
