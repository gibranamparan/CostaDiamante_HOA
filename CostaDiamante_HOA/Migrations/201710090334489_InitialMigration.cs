namespace CostaDiamante_HOA
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
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
                "dbo.Payments",
                c => new
                    {
                        paymentsID = c.Int(nullable: false, identity: true),
                        amount = c.Double(nullable: false),
                        date = c.DateTime(nullable: false),
                        typeOfPayment = c.Int(nullable: false),
                        ownerID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.paymentsID)
                .ForeignKey("dbo.AspNetUsers", t => t.ownerID)
                .Index(t => t.ownerID);
            
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
                "dbo.Visitors",
                c => new
                    {
                        visitorID = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        lastName = c.String(),
                        isYounger = c.Boolean(nullable: false),
                        visit_visitsID = c.Int(),
                    })
                .PrimaryKey(t => t.visitorID)
                .ForeignKey("dbo.Visits", t => t.visit_visitsID)
                .Index(t => t.visit_visitsID);
            
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
            DropForeignKey("dbo.Condoes", "ownerID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Visitors", "visit_visitsID", "dbo.Visits");
            DropForeignKey("dbo.Visits", "ownerID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Visits", "condoID", "dbo.Condoes");
            DropForeignKey("dbo.Payments", "ownerID", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Visitors", new[] { "visit_visitsID" });
            DropIndex("dbo.Visits", new[] { "ownerID" });
            DropIndex("dbo.Visits", new[] { "condoID" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.Payments", new[] { "ownerID" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Condoes", new[] { "ownerID" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Visitors");
            DropTable("dbo.Visits");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.Payments");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Condoes");
        }
    }
}
