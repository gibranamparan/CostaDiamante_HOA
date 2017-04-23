namespace Sonora_HOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cambiosModeloGuest : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Permissions", "guest_Id", "dbo.Guests");
            DropIndex("dbo.Permissions", new[] { "guest_Id" });
            DropIndex("dbo.Guests", new[] { "owner_Id" });
            DropColumn("dbo.Permissions", "guestID");
            DropColumn("dbo.Guests", "Id");
            RenameColumn(table: "dbo.Guests", name: "owner_Id", newName: "Id");
            RenameColumn(table: "dbo.Permissions", name: "guest_Id", newName: "guestID");
            DropPrimaryKey("dbo.Guests");
            AlterColumn("dbo.Permissions", "guestID", c => c.Int(nullable: false));
            AlterColumn("dbo.Guests", "Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.Guests", "guestID", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Guests", "guestID");
            CreateIndex("dbo.Permissions", "guestID");
            CreateIndex("dbo.Guests", "Id");
            AddForeignKey("dbo.Permissions", "guestID", "dbo.Guests", "guestID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Permissions", "guestID", "dbo.Guests");
            DropIndex("dbo.Guests", new[] { "Id" });
            DropIndex("dbo.Permissions", new[] { "guestID" });
            DropPrimaryKey("dbo.Guests");
            AlterColumn("dbo.Guests", "guestID", c => c.Int(nullable: false));
            AlterColumn("dbo.Guests", "Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Permissions", "guestID", c => c.String(maxLength: 128));
            AddPrimaryKey("dbo.Guests", "Id");
            RenameColumn(table: "dbo.Permissions", name: "guestID", newName: "guest_Id");
            RenameColumn(table: "dbo.Guests", name: "Id", newName: "owner_Id");
            AddColumn("dbo.Guests", "Id", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Permissions", "guestID", c => c.Int(nullable: false));
            CreateIndex("dbo.Guests", "owner_Id");
            CreateIndex("dbo.Permissions", "guest_Id");
            AddForeignKey("dbo.Permissions", "guest_Id", "dbo.Guests", "Id");
        }
    }
}
