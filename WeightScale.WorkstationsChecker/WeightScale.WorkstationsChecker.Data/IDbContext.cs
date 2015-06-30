namespace WeightScale.WorkstationsChecker.Data
{
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using WeightScale.WorkstationsChecker.Model;
  
    public interface IDbContext
    {
        IDbSet<WeightScaleWorkStation> WeightScales { get; set; }

        IDbSet<PingPole> Pings { get; set; }

        DbContext DbContext { get; set; }

        int SaveChanges();

        void Dispose();

        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        IDbSet<T> Set<T>() where T : class;
    }
}
