using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Masterpiece.Repository.DBContext;

namespace Masterpiece.Bll
{
    public class MasterpieceDbContextFactory : IDisposable
    {
        private MasterpieceDBContext dbContext;
        public MasterpieceDBContext DBContext
        {
            set
            {
                dbContext = value;
            }
            get
            {
                if (dbContext == null)
                {
                    dbContext = new MasterpieceDBContext();
                }
                return dbContext;
            }
        }

        //private MasterpieceDBContextPart1 dbContextPart1;
        //public MasterpieceDBContextPart1 DBContextPart1
        //{
        //    set
        //    {
        //        dbContextPart1 = value;
        //    }
        //    get
        //    {
        //        if (dbContextPart1 == null)
        //        {
        //            dbContextPart1 = new MasterpieceDBContextPart1();
        //        }
        //        return dbContextPart1;
        //    }
        //}

        public static MasterpieceDbContextFactory CreateDbContext()
        {
            return new MasterpieceDbContextFactory();
        }

        public static void Init()
        {
            MasterpieceDBContext DBContext1 = new MasterpieceDBContext();
            var objectContext = ((IObjectContextAdapter)DBContext1).ObjectContext;
            var mappingCollection = (StorageMappingItemCollection)objectContext.MetadataWorkspace.GetItemCollection(DataSpace.CSSpace);
            mappingCollection.GenerateViews(new List<EdmSchemaError>());
        }

        public void Dispose()
        {
            if (dbContext != null)
            {
                dbContext.Dispose();
            }
        }
    }
}
