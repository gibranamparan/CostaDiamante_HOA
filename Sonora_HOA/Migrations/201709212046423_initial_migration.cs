namespace Sonora_HOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial_migration : DbMigration
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
                        ownerID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.checkInListID)
                .ForeignKey("dbo.AspNetUsers", t => t.ownerID)
                .Index(t => t.ownerID);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        name = c.String(),
                        lastName = c.String(),
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
                        condoID = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        ownerID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.condoID)
                .ForeignKey("dbo.AspNetUsers", t => t.ownerID)
                .Index(t => t.ownerID);
            
            CreateTable(
                "dbo.Visits",
                c => new
                    {
                        visitsID = c.Int(nullable: false, identity: true),
                        date = c.DateTime(nullable: false),
                        arrivalDate = c.DateTime(nullable: false),
                        departureDate = c.DateTime(nullable: false),
                        condoID = c.Int(nullable: false),
                        ownerID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.visitsID)
                .ForeignKey("dbo.Condoes", t => t.condoID, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ownerID)
                .Index(t => t.condoID)
                .Index(t => t.ownerID);
            
            CreateTable(
                "dbo.Permissions_Visits",
                c => new
                    {
                        permissions_visitsID = c.Int(nullable: false, identity: true),
                        permissionsID = c.Int(),
                        guestFullName = c.String(),
                        visitsID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.permissions_visitsID)
                .ForeignKey("dbo.Permissions", t => t.permissionsID)
                .ForeignKey("dbo.Visits", t => t.visitsID, cascadeDelete: true)
                .Index(t => t.permissionsID)
                .Index(t => t.visitsID);
            
            CreateTable(
                "dbo.Permissions",
                c => new
                    {
                        permissionsID = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false),
                        lastName = c.String(nullable: false),
                        isWildCard = c.Boolean(nullable: false),
                        checkInListID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.permissionsID)
                .ForeignKey("dbo.CheckInLists", t => t.checkInListID, cascadeDelete: true)
                .Index(t => t.checkInListID);
            
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
            DropForeignKey("dbo.CheckInLists", "ownerID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Permissions_Visits", "visitsID", "dbo.Visits");
            DropForeignKey("dbo.Permissions_Visits", "permissionsID", "dbo.Permissions");
            DropForeignKey("dbo.Permissions", "checkInListID", "dbo.CheckInLists");
            DropForeignKey("dbo.Visits", "ownerID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Visits", "condoID", "dbo.Condoes");
            DropForeignKey("dbo.Condoes", "ownerID", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.Permissions", new[] { "checkInListID" });
            DropIndex("dbo.Permissions_Visits", new[] { "visitsID" });
            DropIndex("dbo.Permissions_Visits", new[] { "permissionsID" });
            DropIndex("dbo.Visits", new[] { "ownerID" });
            DropIndex("dbo.Visits", new[] { "condoID" });
            DropIndex("dbo.Condoes", new[] { "ownerID" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.CheckInLists", new[] { "ownerID" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.Permissions");
            DropTable("dbo.Permissions_Visits");
            DropTable("dbo.Visits");
            DropTable("dbo.Condoes");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.CheckInLists");
        }
    }
}
