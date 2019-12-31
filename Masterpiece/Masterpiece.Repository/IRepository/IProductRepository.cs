using Masterpiece.Domain.Entity;
using Masterpiece.Repository.RepositoryBase;
using System.Collections.Generic;

namespace Masterpiece.Repository.IRepository
{
    public interface IProductRepository:IRepositoryBase<Product>
    {
        int Add(Product entity);

        int Add(List<Product> entities);

        int Update(Product entity);

        Product GetEntity(int keyValue);

        int Delete(Product entity);

        List<Product> GetAll();
    }
}
