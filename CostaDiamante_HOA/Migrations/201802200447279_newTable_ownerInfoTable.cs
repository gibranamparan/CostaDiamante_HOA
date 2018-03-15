namespace CostaDiamante_HOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newTable_ownerInfoTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OwnersInfoContacts",
                c => new
                    {
                        ownerContactInfo = c.Int(nullable: false, identity: true),
                        ownerID = c.String(nullable: false, maxLength: 128),
                        ownerName = c.String(),
                        address = c.String(),
                        city = c.String(),
                        state = c.String(),
                        zip = c.String(),
                        email = c.String(),
                        phone = c.String(),
                        mobile = c.String(),
                        mexnum = c.String(),
                        otherPhone = c.String(),
                    })
                .PrimaryKey(t => t.ownerContactInfo)
                .ForeignKey("dbo.AspNetUsers", t => t.ownerID, cascadeDelete: true)
                .Index(t => t.ownerID);
            
            DropColumn("dbo.AspNetUsers", "address");
            DropColumn("dbo.AspNetUsers", "city");
            DropColumn("dbo.AspNetUsers", "state");
            DropColumn("dbo.AspNetUsers", "zip");
            DropColumn("dbo.AspNetUsers", "mobile");
            DropColumn("dbo.AspNetUsers", "mex_phone");
            DropColumn("dbo.AspNetUsers", "other_phone");
            DropColumn("dbo.AspNetUsers", "other_emails");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "other_emails", c => c.String());
            AddColumn("dbo.AspNetUsers", "other_phone", c => c.String());
            AddColumn("dbo.AspNetUsers", "mex_phone", c => c.String());
            AddColumn("dbo.AspNetUsers", "mobile", c => c.String());
            AddColumn("dbo.AspNetUsers", "zip", c => c.String());
            AddColumn("dbo.AspNetUsers", "state", c => c.String());
            AddColumn("dbo.AspNetUsers", "city", c => c.String());
            AddColumn("dbo.AspNetUsers", "address", c => c.String());
            DropForeignKey("dbo.OwnersInfoContacts", "ownerID", "dbo.AspNetUsers");
            DropIndex("dbo.OwnersInfoContacts", new[] { "ownerID" });
            DropTable("dbo.OwnersInfoContacts");
        }
    }
}
