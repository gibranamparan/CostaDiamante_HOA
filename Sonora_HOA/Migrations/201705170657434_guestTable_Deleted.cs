namespace Sonora_HOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class guestTable_Deleted : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Guests", "ownerID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Permissions", "guestID", "dbo.Guests");
            DropIndex("dbo.Permissions", new[] { "guestID" });
            DropIndex("dbo.Guests", new[] { "ownerID" });
            AddColumn("dbo.Permissions", "name", c => c.String(nullable: false));
            AddColumn("dbo.Permissions", "lastName", c => c.String(nullable: false));
            DropColumn("dbo.Permissions", "guestID");
            DropTable("dbo.Guests");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Guests",
                c => new
                    {
                        guestID = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false),
                        lastName = c.String(nullable: false),
                        ownerID = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.guestID);
            
            AddColumn("dbo.Permissions", "guestID", c => c.Int(nullable: false));
            DropColumn("dbo.Permissions", "lastName");
            DropColumn("dbo.Permissions", "name");
            CreateIndex("dbo.Guests", "ownerID");
            CreateIndex("dbo.Permissions", "guestID");
            AddForeignKey("dbo.Permissions", "guestID", "dbo.Guests", "guestID", cascadeDelete: true);
            AddForeignKey("dbo.Guests", "ownerID", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
