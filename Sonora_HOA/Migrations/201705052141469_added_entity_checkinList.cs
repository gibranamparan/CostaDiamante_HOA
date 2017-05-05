namespace Sonora_HOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_entity_checkinList : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CheckInLists",
                c => new
                    {
                        checkInListID = c.Int(nullable: false, identity: true),
                        startDate = c.DateTime(nullable: false),
                        endDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.checkInListID);
            
            AddColumn("dbo.Permissions", "checkInListID", c => c.Int(nullable: false));
            AddColumn("dbo.Visits", "Permissions_permissionsID", c => c.Int());
            AddColumn("dbo.Visits", "CheckInList_checkInListID", c => c.Int());
            CreateIndex("dbo.Permissions", "checkInListID");
            CreateIndex("dbo.Visits", "Permissions_permissionsID");
            CreateIndex("dbo.Visits", "CheckInList_checkInListID");
            AddForeignKey("dbo.Permissions", "checkInListID", "dbo.CheckInLists", "checkInListID", cascadeDelete: true);
            AddForeignKey("dbo.Visits", "Permissions_permissionsID", "dbo.Permissions", "permissionsID");
            AddForeignKey("dbo.Visits", "CheckInList_checkInListID", "dbo.CheckInLists", "checkInListID");
            DropColumn("dbo.Permissions", "startDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Permissions", "startDate", c => c.DateTime(nullable: false));
            DropForeignKey("dbo.Visits", "CheckInList_checkInListID", "dbo.CheckInLists");
            DropForeignKey("dbo.Visits", "Permissions_permissionsID", "dbo.Permissions");
            DropForeignKey("dbo.Permissions", "checkInListID", "dbo.CheckInLists");
            DropIndex("dbo.Visits", new[] { "CheckInList_checkInListID" });
            DropIndex("dbo.Visits", new[] { "Permissions_permissionsID" });
            DropIndex("dbo.Permissions", new[] { "checkInListID" });
            DropColumn("dbo.Visits", "CheckInList_checkInListID");
            DropColumn("dbo.Visits", "Permissions_permissionsID");
            DropColumn("dbo.Permissions", "checkInListID");
            DropTable("dbo.CheckInLists");
        }
    }
}
