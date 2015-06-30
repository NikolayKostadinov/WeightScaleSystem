namespace WeightScale.WorkstationsChecker.Data
{
    using System;
    using WeightScale.WorkstationsChecker.Contracts;
    using WeightScale.WorkstationsChecker.Model;
    using WeightScale.WorkstationsChecker.Model.Identity;

    //Interface for Unit Of Work implementation
    public interface IUowData: IDisposable
    {
        IDbContext Context { get; }

        IDeletableEntityRepository<WeightScaleWorkStation> WeightScales { get; }

        IDeletableEntityRepository<PingPole> Pings { get; }

        IRepository<ApplicationUser> Users { get; }

        IRepository<ApplicationRole> Roles { get; }

        int SaveChanges();
    }
}
