using System;
using System.Data.Common;
using System.Data.Entity;
using Abp.EntityFramework;

namespace DFramework.MyStorage.EntityFramework
{
    public class MyStorageDbContext : AbpDbContext
    {
        //TODO: Define an IDbSet for each Entity...

        //Example:
        //public virtual IDbSet<User> Users { get; set; }

        /* NOTE: 
         *   Setting "Default" to base class helps us when working migration commands on Package Manager Console.
         *   But it may cause problems when working Migrate.exe of EF. If you will apply migrations on command line, do not
         *   pass connection string name to base classes. ABP works either way.
         */
        public MyStorageDbContext()
            : base("StorageDbContextConnectionString")
        {
            //Database.SetInitializer<MyStorageDbContext>(new MyStorageInitializer());
        }

        /* NOTE:
         *   This constructor is used by ABP to pass connection string defined in MyStorageDataModule.PreInitialize.
         *   Notice that, actually you will not directly create an instance of MyStorageDbContext since ABP automatically handles it.
         */
        public MyStorageDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            //Database.SetInitializer<MyStorageDbContext>(new MyStorageInitializer());
        }

        //This constructor is used in tests
        public MyStorageDbContext(DbConnection existingConnection)
         : base(existingConnection, false)
        {

        }

        public MyStorageDbContext(DbConnection existingConnection, bool contextOwnsConnection)
         : base(existingConnection, contextOwnsConnection)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Node>().ToTable("d_Node");
            modelBuilder.Entity<File>().ToTable("d_File");
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Node> Nodes { get; set; }
        public DbSet<File> Files { get; set; }
    }

    public class MyStorageInitializer : DropCreateDatabaseAlways<MyStorageDbContext>
    {
        protected override void Seed(MyStorageDbContext context)
        {
            base.Seed(context);

            var node = new Node
            {
                Id = Guid.NewGuid().ToString("n"),
                Name = "server 01",
                UrlHost = "http://localhost:8080/mystorage/",
                Config = @"{'RootPath':'storage\\',  'PhysicalHost':'D:\\' }",
                Capacity = 650000000000,
                FileSize = 0,
                FileCount = 0,
                //FullType = "SMBNodeWare.NodeWare, SMBNodeWare",
                Status = NodeStatus.InUsing
            };
            context.Nodes.Add(node);
            context.SaveChanges();
        }
    }
}
