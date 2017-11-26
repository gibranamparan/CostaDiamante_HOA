namespace CostaDiamante_HOA
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class payments_herencia : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Payments", "visitID", "dbo.Visits");
            DropIndex("dbo.Payments", new[] { "visitID" });
            AddColumn("dbo.Payments", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Payments", "Visit_visitID", c => c.Int());
            AlterColumn("dbo.Payments", "visitID", c => c.Int());
            CreateIndex("dbo.Payments", "visitID");
            CreateIndex("dbo.Payments", "Visit_visitID");
            AddForeignKey("dbo.Payments", "visitID", "dbo.Visits", "visitID", cascadeDelete: true);
            AddForeignKey("dbo.Payments", "Visit_visitID", "dbo.Visits", "visitID");
            DropColumn("dbo.Payments", "typeOfPayment");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Payments", "typeOfPayment", c => c.Int(nullable: false));
            DropForeignKey("dbo.Payments", "Visit_visitID", "dbo.Visits");
            DropForeignKey("dbo.Payments", "visitID", "dbo.Visits");
            DropIndex("dbo.Payments", new[] { "Visit_visitID" });
            DropIndex("dbo.Payments", new[] { "visitID" });
            AlterColumn("dbo.Payments", "visitID", c => c.Int(nullable: false));
            DropColumn("dbo.Payments", "Visit_visitID");
            DropColumn("dbo.Payments", "Discriminator");
            CreateIndex("dbo.Payments", "visitID");
            AddForeignKey("dbo.Payments", "visitID", "dbo.Visits", "visitID", cascadeDelete: true);
        }
    }
}
