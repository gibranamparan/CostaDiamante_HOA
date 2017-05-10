namespace Sonora_HOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cascadeDelete_Owner_Guest : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Guests", "ownerID", "dbo.AspNetUsers");
            DropIndex("dbo.Guests", new[] { "ownerID" });
            AlterColumn("dbo.Guests", "ownerID", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Guests", "ownerID");
            AddForeignKey("dbo.Guests", "ownerID", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Guests", "ownerID", "dbo.AspNetUsers");
            DropIndex("dbo.Guests", new[] { "ownerID" });
            AlterColumn("dbo.Guests", "ownerID", c => c.String(maxLength: 128));
            CreateIndex("dbo.Guests", "ownerID");
            AddForeignKey("dbo.Guests", "ownerID", "dbo.AspNetUsers", "Id");
        }
    }
}
