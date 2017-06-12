namespace Sonora_HOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class reuiqered_CheckInList_Permissions : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Permissions", "checkInListID", "dbo.CheckInLists");
            DropIndex("dbo.Permissions", new[] { "checkInListID" });
            AlterColumn("dbo.Permissions", "checkInListID", c => c.Int(nullable: false));
            CreateIndex("dbo.Permissions", "checkInListID");
            AddForeignKey("dbo.Permissions", "checkInListID", "dbo.CheckInLists", "checkInListID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Permissions", "checkInListID", "dbo.CheckInLists");
            DropIndex("dbo.Permissions", new[] { "checkInListID" });
            AlterColumn("dbo.Permissions", "checkInListID", c => c.Int());
            CreateIndex("dbo.Permissions", "checkInListID");
            AddForeignKey("dbo.Permissions", "checkInListID", "dbo.CheckInLists", "checkInListID");
        }
    }
}