namespace Sonora_HOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class corrected_ownerIDType_Visits : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Visits", new[] { "owner_Id" });
            DropColumn("dbo.Visits", "ownerID");
            RenameColumn(table: "dbo.Visits", name: "owner_Id", newName: "ownerID");
            AlterColumn("dbo.Visits", "ownerID", c => c.String(maxLength: 128));
            CreateIndex("dbo.Visits", "ownerID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Visits", new[] { "ownerID" });
            AlterColumn("dbo.Visits", "ownerID", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.Visits", name: "ownerID", newName: "owner_Id");
            AddColumn("dbo.Visits", "ownerID", c => c.Int(nullable: false));
            CreateIndex("dbo.Visits", "owner_Id");
        }
    }
}
