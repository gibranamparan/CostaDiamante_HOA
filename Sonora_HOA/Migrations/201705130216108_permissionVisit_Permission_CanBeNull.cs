namespace Sonora_HOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class permissionVisit_Permission_CanBeNull : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Permissions_Visits", "permissionsID", "dbo.Permissions");
            DropIndex("dbo.Permissions_Visits", new[] { "permissionsID" });
            AddColumn("dbo.Permissions_Visits", "guestFullName", c => c.String());
            AlterColumn("dbo.Permissions_Visits", "permissionsID", c => c.Int());
            CreateIndex("dbo.Permissions_Visits", "permissionsID");
            AddForeignKey("dbo.Permissions_Visits", "permissionsID", "dbo.Permissions", "permissionsID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Permissions_Visits", "permissionsID", "dbo.Permissions");
            DropIndex("dbo.Permissions_Visits", new[] { "permissionsID" });
            AlterColumn("dbo.Permissions_Visits", "permissionsID", c => c.Int(nullable: false));
            DropColumn("dbo.Permissions_Visits", "guestFullName");
            CreateIndex("dbo.Permissions_Visits", "permissionsID");
            AddForeignKey("dbo.Permissions_Visits", "permissionsID", "dbo.Permissions", "permissionsID", cascadeDelete: true);
        }
    }
}
