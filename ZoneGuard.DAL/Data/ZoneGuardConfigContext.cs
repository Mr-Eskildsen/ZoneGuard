using System;
using System.Collections.Generic;
using System.Data;
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

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            
            /*
            //hostContext.Configuration.GetConnectionString("AlarmManagerDb")
            services.AddDbContext<AlarmManagerContext>((options =>
                        options.UseSqlServer(connectionString)));
                        */

            if (!optionsBuilder.IsConfigured)
            {
                //TODO:: Load from protected section
                //TODO:: https://docs.microsoft.com/da-dk/dotnet/framework/data/adonet/connection-strings-and-configuration-files#encrypting-configuration-file-sections-using-protected-configuration

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

        public DbSet<ZoneGuard.DAL.Models.Log.SensorStateRawLogDAL> RawSensorStateLog { get; set; }

        public DbSet<ZoneGuard.DAL.Models.Log.SensorStateLogDAL> SensorStateLog { get; set; }
        public DbSet<ZoneGuard.DAL.Models.Log.SensorEvent2AlarmEventDAL> SensorAlarmEvent { get; set; }
        public DbSet<ZoneGuard.DAL.Models.Log.AlarmEventLogDAL> AlarmState { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /*            int test = 0;
                        if (!TableExists())
                        {
                            test = 1;
                        }
                        else
                        {
                            test = 2;
                        }*/


            /**************************************************
             * 
             * Configuration
             * ***********************************************/
            /* modelBuilder.Entity<Blog>()
               .Property(b => b.Url)
               .IsRequired();
           */
           /*
            modelBuilder.Entity<NodeDAL>()
                .ToTable("config_Node");
            modelBuilder.Entity<NodeDAL>()
                .Property(p => p.Name).HasMaxLength(50);

            //.Property(a => a.Id). .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
            //.ValueGeneratedOnAdd();


            modelBuilder.Entity<ThingParameterDAL>()
                .ToTable("config_ThingParameter");

            modelBuilder.Entity<ThingParameterDAL>()
                .Property(p => p.Name).
                .HasRequired(t => t.Agent).WithMany().HasForeignKey(t => t.AgentId)


            modelBuilder.Entity<ThingDAL>()
                .ToTable("config_Thing");
            modelBuilder.Entity<ThingDAL>()
                .Property(p => p.Name).HasMaxLength(50);
            modelBuilder.Entity<ThingDAL>()
                .Property(p => p.Description).HasMaxLength(250);


            modelBuilder.Entity<AlarmZoneThingDAL>()
                .ToTable("config_AlarmZoneThing");
            modelBuilder.Entity<AlarmZoneThingDAL>()
                .Property(p => p.Id).ValueGeneratedOnAdd();


            modelBuilder.Entity<AlarmZoneDAL>()
                .ToTable("config_AlarmZone");
            modelBuilder.Entity<AlarmZoneDAL>()
                .Property(p => p.Id).ValueGeneratedOnAdd();

            */
            //modelBuilder.Entity<AlarmStateDAL>().ToTable("config_AlarmZone");

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


        public bool TableExists(string tableName)
        {
            return TableExists("dbo", tableName);
        }

        public bool TableExists(string schema, string tableName)
        {
            var connection = Database.GetDbConnection();

            if (connection.State.Equals(ConnectionState.Closed))
                connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
            SELECT 1 FROM INFORMATION_SCHEMA.TABLES 
            WHERE TABLE_SCHEMA = @Schema
            AND TABLE_NAME = @TableName";

                var schemaParam = command.CreateParameter();
                schemaParam.ParameterName = "@Schema";
                schemaParam.Value = schema;
                command.Parameters.Add(schemaParam);

                var tableNameParam = command.CreateParameter();
                tableNameParam.ParameterName = "@TableName";
                tableNameParam.Value = tableName;
                command.Parameters.Add(tableNameParam);

                return command.ExecuteScalar() != null;
            }
        }

    }
}
