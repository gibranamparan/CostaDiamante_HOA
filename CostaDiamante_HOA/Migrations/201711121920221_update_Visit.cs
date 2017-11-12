namespace CostaDiamante_HOA
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_Visit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Visits", "totalCost", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Visits", "totalCost");
        }
    }
}
