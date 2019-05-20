namespace DFramework.MyStorage.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class databaseinit : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.d_File",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Path = c.String(),
                        Size = c.Long(nullable: false),
                        ReferenceCount = c.Int(nullable: false),
                        NodeId = c.String(maxLength: 128),
                        CreationTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.d_Node", t => t.NodeId)
                .Index(t => t.NodeId);
            
            CreateTable(
                "dbo.d_Node",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        UrlHost = c.String(),
                        Config = c.String(),
                        Capacity = c.Long(nullable: false),
                        FileSize = c.Long(nullable: false),
                        FileCount = c.Long(nullable: false),
                        FullType = c.String(),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.d_File", "NodeId", "dbo.d_Node");
            DropIndex("dbo.d_File", new[] { "NodeId" });
            DropTable("dbo.d_Node");
            DropTable("dbo.d_File");
        }
    }
}
