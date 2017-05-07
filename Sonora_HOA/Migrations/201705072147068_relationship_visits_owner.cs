namespace Sonora_HOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class relationship_visits_owner : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Visits", "ownerID", c => c.Int(nullable: false));
            AddColumn("dbo.Visits", "owner_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Visits", "owner_Id");
            AddForeignKey("dbo.Visits", "owner_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Visits", "owner_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Visits", new[] { "owner_Id" });
            DropColumn("dbo.Visits", "owner_Id");
            DropColumn("dbo.Visits", "ownerID");
        }
    }
}
