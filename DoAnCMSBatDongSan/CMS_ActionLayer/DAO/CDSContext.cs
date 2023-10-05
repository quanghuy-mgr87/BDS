using CMS_Design.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_ActionLayer.DAO
{
    public class CDSContext : DbContext
    {
        #region dbset
        public DbSet<PhieuXemNha> phieuXemNhas { get; set; }
        public DbSet<Product> products { get; set; }
        public DbSet<ProductImg> productsImg { get; set; }
        public DbSet<ProductStatus> productsStatus { get; set; }
        public DbSet<Team> teams { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<TeamStatus > teamsStatus { get; set; }
        public DbSet<UserRole> userRoles { get; set; }
        public DbSet<UserStatus> userStatuses { get; set; }
        public DbSet<NotificationStatus> notificationStatuses { get; set; }
        public DbSet<Notification> notifications { get; set; }
        public DbSet<VideoBaiHoc> videoBaiHocs { get; set; }
        public DbSet<VideoBaiHocStatus> videoBaiHocStatuses { get; set; }

        #endregion
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=bds;Integrated Security=True;encrypt=true;trustservercertificate=true;MultipleActiveResultSets=True;");
        }

    }
}
