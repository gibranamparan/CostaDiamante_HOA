namespace CostaDiamante_HOA
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class setPaymentsToVisit : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Payments", "ownerID", "dbo.AspNetUsers");
            DropIndex("dbo.Payments", new[] { "ownerID" });
            AddColumn("dbo.Payments", "visitsID", c => c.Int(nullable: false));
            AddColumn("dbo.Payments", "owner_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.Payments", "ownerID", c => c.Int(nullable: false));
            CreateIndex("dbo.Payments", "visitsID");
            CreateIndex("dbo.Payments", "owner_Id");
            AddForeignKey("dbo.Payments", "visitsID", "dbo.Visits", "visitsID", cascadeDelete: true);
            AddForeignKey("dbo.Payments", "owner_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Payments", "owner_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Payments", "visitsID", "dbo.Visits");
            DropIndex("dbo.Payments", new[] { "owner_Id" });
            DropIndex("dbo.Payments", new[] { "visitsID" });
            AlterColumn("dbo.Payments", "ownerID", c => c.String(maxLength: 128));
            DropColumn("dbo.Payments", "owner_Id");
            DropColumn("dbo.Payments", "visitsID");
            CreateIndex("dbo.Payments", "ownerID");
            AddForeignKey("dbo.Payments", "ownerID", "dbo.AspNetUsers", "Id");
        }
    }
}
