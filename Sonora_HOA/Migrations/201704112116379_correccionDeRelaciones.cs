namespace Sonora_HOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class correccionDeRelaciones : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Permissions", "guestid", "dbo.Guests");
            DropIndex("dbo.Condoes", new[] { "owner_Id" });
            DropIndex("dbo.Permissions", new[] { "guestid" });
            DropColumn("dbo.Condoes", "id");
            RenameColumn(table: "dbo.Condoes", name: "owner_Id", newName: "Id");
            DropPrimaryKey("dbo.Guests");
            CreateTable(
                "dbo.Permissions_Visits",
                c => new
                    {
                        permissionsID = c.Int(nullable: false),
                        visitsID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.permissionsID, t.visitsID })
                .ForeignKey("dbo.Permissions", t => t.permissionsID, cascadeDelete: true)
                .ForeignKey("dbo.Visits", t => t.visitsID, cascadeDelete: true)
                .Index(t => t.permissionsID)
                .Index(t => t.visitsID);
            
            CreateTable(
                "dbo.Visits",
                c => new
                    {
                        visitsID = c.Int(nullable: false, identity: true),
                        date = c.DateTime(nullable: false),
                        arrivalDate = c.DateTime(nullable: false),
                        departureDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.visitsID);
            
            AddColumn("dbo.Permissions", "guest_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.Condoes", "Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.Guests", "Id", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.Guests", "Id");
            CreateIndex("dbo.Condoes", "Id");
            CreateIndex("dbo.Permissions", "guest_Id");
            AddForeignKey("dbo.Permissions", "guest_Id", "dbo.Guests", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Permissions", "guest_Id", "dbo.Guests");
            DropForeignKey("dbo.Permissions_Visits", "visitsID", "dbo.Visits");
            DropForeignKey("dbo.Permissions_Visits", "permissionsID", "dbo.Permissions");
            DropIndex("dbo.Permissions_Visits", new[] { "visitsID" });
            DropIndex("dbo.Permissions_Visits", new[] { "permissionsID" });
            DropIndex("dbo.Permissions", new[] { "guest_Id" });
            DropIndex("dbo.Condoes", new[] { "Id" });
            DropPrimaryKey("dbo.Guests");
            AlterColumn("dbo.Guests", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Condoes", "Id", c => c.Int(nullable: false));
            DropColumn("dbo.Permissions", "guest_Id");
            DropTable("dbo.Visits");
            DropTable("dbo.Permissions_Visits");
            AddPrimaryKey("dbo.Guests", "id");
            RenameColumn(table: "dbo.Condoes", name: "Id", newName: "owner_Id");
            AddColumn("dbo.Condoes", "id", c => c.Int(nullable: false));
            CreateIndex("dbo.Permissions", "guestid");
            CreateIndex("dbo.Condoes", "owner_Id");
            AddForeignKey("dbo.Permissions", "guestid", "dbo.Guests", "id", cascadeDelete: true);
        }
    }
}
