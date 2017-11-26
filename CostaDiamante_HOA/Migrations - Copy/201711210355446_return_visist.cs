namespace CostaDiamante_HOA
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class return_visist : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Visits", "paymentOmitted");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Visits", "paymentOmitted", c => c.Boolean(nullable: false));
        }
    }
}
