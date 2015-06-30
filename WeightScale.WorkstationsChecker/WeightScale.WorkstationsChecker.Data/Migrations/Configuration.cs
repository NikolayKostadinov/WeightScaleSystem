namespace WeightScale.WorkstationsChecker.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using WeightScale.WorkstationsChecker.Data.Identity;
    using WeightScale.WorkstationsChecker.Model;
    using WeightScale.WorkstationsChecker.Model.Identity;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            this.AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(WeightScale.WorkstationsChecker.Data.ApplicationDbContext context)
        {

            context.WeightScales.AddOrUpdate(
              p => p.Address,
              new WeightScaleWorkStation { Address = "10.94.15.17", Name = "Waste", ScreenPosition = 1, CreatedOn = DateTime.Now },
              new WeightScaleWorkStation { Address = "10.94.1.64", Name = "Cisco before Waste", ScreenPosition = 2, CreatedOn = DateTime.Now },
              new WeightScaleWorkStation { Address = "10.94.34.68", Name = "Petroleum Bitumen Viscous", ScreenPosition = 3, CreatedOn = DateTime.Now },
              new WeightScaleWorkStation { Address = "10.94.1.88", Name = "Cisco Before Petroleum Bitumen Viscous", ScreenPosition = 4, CreatedOn = DateTime.Now },
              new WeightScaleWorkStation { Address = "10.94.15.12", Name = "Weigt scale auto - LPG", ScreenPosition = 5, CreatedOn = DateTime.Now },
              new WeightScaleWorkStation { Address = "10.94.15.42", Name = "Weigt scales railway", ScreenPosition = 6, CreatedOn = DateTime.Now },
              new WeightScaleWorkStation { Address = "10.94.1.86", Name = "Cisco before LPG", ScreenPosition = 7, CreatedOn = DateTime.Now }
            );

            if (context.Users.Where(x => x.UserName == "Administrator").FirstOrDefault() == null)
            {
                var user = new ApplicationUser() {Id=1, Email = "Administrator@bmsystems.local", UserName = "Administrator" };
                var manager = new UserManager<ApplicationUser, int>(new UserStoreIntPk(ApplicationDbContext.Create()));
                manager.Create(user, "K@lvad0s");
            }

        }
    }
}
