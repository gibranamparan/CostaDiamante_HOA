namespace CostaDiamante_HOA
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_modelvisit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Visits", "paymentOmitted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Visits", "paymentOmitted");
        }
    }
}
