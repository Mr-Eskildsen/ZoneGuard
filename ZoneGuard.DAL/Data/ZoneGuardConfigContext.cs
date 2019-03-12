using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using ZoneGuard.DAL.Models.Config;
using ZoneGuard.DAL.Models.Log;
using ZoneGuard.DAL.Models.State;
//using Microsoft.IdentityModel.Protocols;

namespace ZoneGuard.DAL.Data
{
    public class ZoneGuardConfigContext : DbContext
    {

        // Database modelling
        //https://docs.microsoft.com/en-us/ef/core/modeling/relational/

        public ZoneGuardConfigContext (DbContextOptions<ZoneGuardConfigContext> options)
            : base(options)
        {
        }



        // http://www.entityframeworktutorial.net/efcore/create-model-for-existing-database-in-ef-core.aspx

        //TODO:: Denne metode skal specialiseredes for at håndtere forskellige databaser
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {


//            string connectionString = "Server=(localdb)\\mssqllocaldb;Database=AlarmManager;Trusted_Connection=True;MultipleActiveResultSets=true";

            
            /*
            //hostContext.Configuration.GetConnectionString("AlarmManagerDb")
            services.AddDbContext<AlarmManagerContext>((options =>
                        options.UseSqlServer(connectionString)));
                        */

            if (!optionsBuilder.IsConfigured)
            {
             
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                //optionsBuilder.UseSqlServer(@"Server=.\SQLExpress;Database=SchoolDB;Trusted_Connection=True;");


                //String strConnection = ConfigurationManager.ConnectionStrings["AlarmManagerDb"].ConnectionString;
                    //Configuration.GetConnectionString("AlarmManagerDb");
            }
        }


        public DbSet<ZoneGuard.DAL.Models.Config.NodeDAL> Node { get; set; }

        public DbSet<ZoneGuard.DAL.Models.Config.ThingParameterDAL> ThingParameter { get; set; }
        public DbSet<ZoneGuard.DAL.Models.Config.ThingDAL> Thing{ get; set; }


        public DbSet<ZoneGuard.DAL.Models.Config.AlarmZoneDAL> AlarmZone { get; set; }
        public DbSet<ZoneGuard.DAL.Models.Config.AlarmZoneThingDAL> AlarmZoneThing { get; set; }

        public DbSet<ZoneGuard.DAL.Models.Log.SensorStateLogDAL> SensorState { get; set; }
        public DbSet<ZoneGuard.DAL.Models.Log.AlarmStateLogDAL> AlarmState { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            return;
            /*
            modelBuilder.Entity<ThingDAL>().ToTable("config_Thing");
            modelBuilder.Entity<ThingParameterDAL>().ToTable("config_ThingParameter");
            modelBuilder.Entity<AlarmZoneConfig>().ToTable("config_AlarmZone");
            modelBuilder.Entity<AlarmZoneThingConfig>().ToTable("config_AlarmZoneThing");
            modelBuilder.Entity<SensorStateLog>().ToTable("log_SensorState");

            //modelBuilder.Entity<AlarmStateLog>().ToTable("log_AlarmState");


            
            

            //Create Foreign Key 
            modelBuilder.Entity<ThingParameterDAL>()
                    .HasOne(p => p.Thing)
                    .WithMany(b => b.Parameters)
                    .HasForeignKey(p => p.ThingId);


            modelBuilder.Entity<AlarmZoneThingConfig>()
                    .HasOne(p => p.AlarmZone)
                    .WithMany(b => b.Sensors)
                    .HasForeignKey(p => p.AlarmZoneId);

                    */
        }
    }
}
