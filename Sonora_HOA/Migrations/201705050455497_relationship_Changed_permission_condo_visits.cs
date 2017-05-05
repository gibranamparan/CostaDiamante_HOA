namespace Sonora_HOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class relationship_Changed_permission_condo_visits : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Permissions", "condoID", "dbo.Condoes");
            DropIndex("dbo.Permissions", new[] { "condoID" });
            AddColumn("dbo.Visits", "condoID", c => c.Int(nullable: false));
            CreateIndex("dbo.Visits", "condoID");
            AddForeignKey("dbo.Visits", "condoID", "dbo.Condoes", "condoID", cascadeDelete: true);
            DropColumn("dbo.Permissions", "condoID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Permissions", "condoID", c => c.Int(nullable: false));
            DropForeignKey("dbo.Visits", "condoID", "dbo.Condoes");
            DropIndex("dbo.Visits", new[] { "condoID" });
            DropColumn("dbo.Visits", "condoID");
            CreateIndex("dbo.Permissions", "condoID");
            AddForeignKey("dbo.Permissions", "condoID", "dbo.Condoes", "condoID", cascadeDelete: true);
        }
    }
}
