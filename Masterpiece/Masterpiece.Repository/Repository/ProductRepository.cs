using Masterpiece.Code.Cache;
using Masterpiece.Domain.Entity;
using Masterpiece.Repository.DBContext;
using Masterpiece.Repository.IRepository;
using Masterpiece.Repository.RepositoryBase;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masterpiece.Repository.Repository
{
    public class ProductRepository : RepositoryBase<MasterpieceDBContext, Product>, IProductRepository
    {
        public ProductRepository(MasterpieceDBContext dbContext) : base(dbContext)
        {

        }

        public int Add(Product entity)
        {
            var result = InsertEntity(entity);
            CacheHelper.NotifyRefreshCache(CacheDependencyEnum.Product, CacheDependencyActionType.Add, entity.Id, tranId);
            return result;
        }

        public int Add(List<Product> entities)
        {
            foreach (Product product in entities)
            {
                dbContext.Entry<Product>(product).State = EntityState.Added;
            }

            return dbContext.SaveChanges();
        }

        public int Delete(Product entity)
        {
            var result = DeleteEntity(entity);
            CacheHelper.NotifyRefreshCache(CacheDependencyEnum.Product, CacheDependencyActionType.Delete, entity.Id, tranId);
            return result;
        }

        public int Delete(int keyValue)
        {
            var result = DeleteEntity(new Product() { Id = keyValue });
            CacheHelper.NotifyRefreshCache(CacheDependencyEnum.Product, CacheDependencyActionType.Delete, keyValue, tranId);
            return result;
        }

        public Product GetEntity(int keyValue)
        {
            Product product = GetEntity(keyValue);
            if(product != null)
            {
                product.EnableAudit();
            }

            return product;
        }

        public List<Product> GetAll()
        {
            List<Product> products = base.AllEntities();
            products.EnableAudit();
            return products;
        }

        public int Update(Product entity)
        {
            var result = base.UpdateEntity(entity);
            CacheHelper.NotifyRefreshCache(CacheDependencyEnum.Product, CacheDependencyActionType.Update, entity.Id, tranId);
            return result;
        }
    }
}
