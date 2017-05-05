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
            CreateIndex("dbo.Permissions", "checkInListID");
            AddForeignKey("dbo.Permissions", "checkInListID", "dbo.CheckInLists", "checkInListID", cascadeDelete: true);
            DropColumn("dbo.Permissions", "startDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Permissions", "startDate", c => c.DateTime(nullable: false));
            DropForeignKey("dbo.Permissions", "checkInListID", "dbo.CheckInLists");
            DropIndex("dbo.Permissions", new[] { "checkInListID" });
            DropColumn("dbo.Permissions", "checkInListID");
            DropTable("dbo.CheckInLists");
        }
    }
}
