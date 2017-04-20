namespace Sonora_HOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                        name = c.String(),
                        lastName = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Condoes",
                c => new
                    {
                        number = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.number)
                .ForeignKey("dbo.AspNetUsers", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.Permissions",
                c => new
                    {
                        permissionsID = c.Int(nullable: false, identity: true),
                        startDate = c.DateTime(nullable: false),
                        number = c.Int(nullable: false),
                        guestID = c.Int(nullable: false),
                        guest_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.permissionsID)
                .ForeignKey("dbo.Condoes", t => t.number, cascadeDelete: true)
                .ForeignKey("dbo.Guests", t => t.guest_Id)
                .Index(t => t.number)
                .Index(t => t.guest_Id);
            
            CreateTable(
                "dbo.Guests",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        guestID = c.Int(nullable: false),
                        name = c.String(),
                        lastName = c.String(),
                        owner_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.owner_Id)
                .Index(t => t.owner_Id);
            
            CreateTable(
                "dbo.Permissions_Visits",
                c => new
                    {
                        permissions_visitsID = c.Int(nullable: false, identity: true),
                        permissionsID = c.Int(nullable: false),
                        visitsID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.permissions_visitsID)
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
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Permissions_Visits", "visitsID", "dbo.Visits");
            DropForeignKey("dbo.Permissions_Visits", "permissionsID", "dbo.Permissions");
            DropForeignKey("dbo.Permissions", "guest_Id", "dbo.Guests");
            DropForeignKey("dbo.Guests", "owner_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Permissions", "number", "dbo.Condoes");
            DropForeignKey("dbo.Condoes", "Id", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.Permissions_Visits", new[] { "visitsID" });
            DropIndex("dbo.Permissions_Visits", new[] { "permissionsID" });
            DropIndex("dbo.Guests", new[] { "owner_Id" });
            DropIndex("dbo.Permissions", new[] { "guest_Id" });
            DropIndex("dbo.Permissions", new[] { "number" });
            DropIndex("dbo.Condoes", new[] { "Id" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.Visits");
            DropTable("dbo.Permissions_Visits");
            DropTable("dbo.Guests");
            DropTable("dbo.Permissions");
            DropTable("dbo.Condoes");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
        }
    }
}
