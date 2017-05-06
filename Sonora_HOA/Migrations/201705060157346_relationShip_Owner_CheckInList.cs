namespace Sonora_HOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class relationShip_Owner_CheckInList : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CheckInLists", "ownerID", c => c.String(maxLength: 128));
            CreateIndex("dbo.CheckInLists", "ownerID");
            AddForeignKey("dbo.CheckInLists", "ownerID", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CheckInLists", "ownerID", "dbo.AspNetUsers");
            DropIndex("dbo.CheckInLists", new[] { "ownerID" });
            DropColumn("dbo.CheckInLists", "ownerID");
        }
    }
}
