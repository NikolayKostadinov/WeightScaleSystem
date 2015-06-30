namespace WeightScale.WorkstationsChecker.Data
{
    using System;
    using System.Collections.Generic;
    using WeightScale.WorkstationsChecker.Contracts;
    using WeightScale.WorkstationsChecker.Data.Repositories.Base;
    using WeightScale.WorkstationsChecker.Model;
    using WeightScale.WorkstationsChecker.Model.Identity;

    public class UowData : IUowData
    {
        private readonly IDbContext context;

        private readonly Dictionary<Type, object> repositories = new Dictionary<Type, object>();

        public UowData(IDbContext context)
        {
            this.context = context;
        }

        public IDbContext Context
        {
            get
            {
                return this.context;
            }
        }


        public IRepository<ApplicationUser> Users
        {
            get
            {
                return this.GetRepository<ApplicationUser>();
            }
        }

        public IRepository<ApplicationRole> Roles
        {
            get
            {
                return this.GetRepository<ApplicationRole>();
            }
        }

        public IDeletableEntityRepository<WeightScaleWorkStation> WeightScales
        {
            get
            {
                return this.GetDeletableEntityRepository<WeightScaleWorkStation>();
            }
        }

        public IDeletableEntityRepository<PingPole> Pings
        {
            get
            {
                return this.GetDeletableEntityRepository<PingPole>();
            }
        }
       
        public int SaveChanges()
        {
            return this.context.SaveChanges();
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.context != null)
                {
                    this.context.Dispose();
                }
            }
        }

        private IRepository<T> GetRepository<T>() where T : class, IEntity
        {
            if (!this.repositories.ContainsKey(typeof(T)))
            {
                var type = typeof(GenericRepository<T>);
                this.repositories.Add(typeof(T), Activator.CreateInstance(type, this.context));
            }

            return (IRepository<T>)this.repositories[typeof(T)];
        }

        private IDeletableEntityRepository<T> GetDeletableEntityRepository<T>() where T : class, IDeletableEntity, IEntity
        {
            if (!this.repositories.ContainsKey(typeof(T)))
            {
                var type = typeof(DeletableEntityRepository<T>);
                this.repositories.Add(typeof(T), Activator.CreateInstance(type, this.context));
            }

            return (IDeletableEntityRepository<T>)this.repositories[typeof(T)];
        }

    }
}