namespace CostaDiamante_HOA
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class visitor_modified : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Visitors", "visit_visitID", "dbo.Visits");
            DropIndex("dbo.Visitors", new[] { "visit_visitID" });
            RenameColumn(table: "dbo.Visitors", name: "visit_visitID", newName: "visitID");
            AlterColumn("dbo.Visitors", "visitID", c => c.Int(nullable: false));
            CreateIndex("dbo.Visitors", "visitID");
            AddForeignKey("dbo.Visitors", "visitID", "dbo.Visits", "visitID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Visitors", "visitID", "dbo.Visits");
            DropIndex("dbo.Visitors", new[] { "visitID" });
            AlterColumn("dbo.Visitors", "visitID", c => c.Int());
            RenameColumn(table: "dbo.Visitors", name: "visitID", newName: "visit_visitID");
            CreateIndex("dbo.Visitors", "visit_visitID");
            AddForeignKey("dbo.Visitors", "visit_visitID", "dbo.Visits", "visitID");
        }
    }
}
