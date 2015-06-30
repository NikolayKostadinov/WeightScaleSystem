namespace WeightScale.WorkstationsChecker.Data
{
    using System;
    using System.Data.Entity;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.AspNet.Identity.EntityFramework;
    using WeightScale.WorkstationsChecker.Contracts;
    using WeightScale.WorkstationsChecker.Data.Migrations;
    using WeightScale.WorkstationsChecker.Model;
    using WeightScale.WorkstationsChecker.Model.Identity;

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int,
        UserLoginIntPk, UserRoleIntPk, UserClaimIntPk>, IDbContext
    {
       

        /// <summary>
        /// Gets or sets the db context.
        /// </summary>
        /// <value>The db context.</value>
        DbContext IDbContext.DbContext { get; set; }

        /// <summary>
        /// Gets the db context.
        /// </summary>
        /// <value>The db context.</value>
        public DbContext DbContext
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Gets or sets the weight scales.
        /// </summary>
        /// <value>The weight scales.</value>
        public IDbSet<WeightScaleWorkStation> WeightScales { get; set; }

        /// <summary>
        /// Gets or sets the pings.
        /// </summary>
        /// <value>The pings.</value>
        public IDbSet<PingPole> Pings { get; set; }

        /// <summary>
        /// Sets this instance.
        /// </summary>
        /// <typeparam name="T">The type of the T.</typeparam>
        /// <returns></returns>
        public IDbSet<T> Set<T>() where T : class
        {
            return base.Set<T>();
        }

        static ApplicationDbContext()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());
        }

        public ApplicationDbContext()
            : base("DefaultConnection")
        {
            this.Database.Log = x => Debug.WriteLine(x);
        }

        public ApplicationDbContext(string connectionString)
            : base(connectionString)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Configurations.Add(new MenuItemMap());
            //modelBuilder.Configurations.Add(new LocalizationMap());
            //modelBuilder.Configurations.Add(new DistrictMap());
            //modelBuilder.Configurations.Add(new MunicipalityMap());
            //modelBuilder.Configurations.Add(new CityMap());
            //modelBuilder.Configurations.Add(new AreaMap());
            base.OnModelCreating(modelBuilder);
        }


        public override int SaveChanges()
        {
            this.ApplyAuditInfoRules();
            this.ApplyDeletableEntityRules();
            return base.SaveChanges();
        }

        private void ApplyAuditInfoRules()
        {
            // Approach via @julielerman: http://bit.ly/123661P
            foreach (var entry in
                this.ChangeTracker.Entries()
                    .Where(
                        e =>
                        e.Entity is IAuditInfo && ((e.State == EntityState.Added) || (e.State == EntityState.Modified))))
            {
                var entity = (IAuditInfo)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    if (!entity.PreserveCreatedOn)
                    {
                        entity.CreatedOn = DateTime.Now;
                    }
                }
                else
                {
                    entity.ModifiedOn = DateTime.Now;
                }
            }
        }

        private void ApplyDeletableEntityRules()
        {
            // Approach via @julielerman: http://bit.ly/123661P
            foreach (
                var entry in
                    this.ChangeTracker.Entries()
                        .Where(e => e.Entity is IDeletableEntity && (e.State == EntityState.Deleted)))
            {
                var entity = (IDeletableEntity)entry.Entity;

                entity.DeletedOn = DateTime.Now;
                entity.IsDeleted = true;
                entry.State = EntityState.Modified;
            }
        }
    }
}
