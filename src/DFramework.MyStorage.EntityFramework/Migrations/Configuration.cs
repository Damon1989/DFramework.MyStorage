using System;
using System.Data.Entity.Migrations;
using DFramework.MyStorage.EntityFramework;

namespace DFramework.MyStorage.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<MyStorageDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "StorageDbContextConnectionString";
        }

        protected override void Seed(MyStorageDbContext context)
        {
            // This method will be called every time after migrating to the latest version.
            // You can add any seed data here...
            var node = new Node
            {
                Id = Guid.NewGuid().ToString("n"),
                Name = "server 01",
                UrlHost = "http://localhost:8080/mystorage/",
                Config = @"{'RootPath':'storage\\',  'PhysicalHost':'C:\\' }",
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