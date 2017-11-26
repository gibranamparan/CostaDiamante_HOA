namespace CostaDiamante_HOA
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class payments_impactrect_visit : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Payments", "Visit_visitID", "dbo.Visits");
            DropIndex("dbo.Payments", new[] { "Visit_visitID" });
            DropColumn("dbo.Payments", "visitID");
            RenameColumn(table: "dbo.Payments", name: "Visit_visitID", newName: "visitID");
            AddForeignKey("dbo.Payments", "visitID", "dbo.Visits", "visitID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Payments", "visitID", "dbo.Visits");
            RenameColumn(table: "dbo.Payments", name: "visitID", newName: "Visit_visitID");
            AddColumn("dbo.Payments", "visitID", c => c.Int());
            CreateIndex("dbo.Payments", "Visit_visitID");
            AddForeignKey("dbo.Payments", "Visit_visitID", "dbo.Visits", "visitID");
        }
    }
}
