namespace CostaDiamante_HOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hoaFee_year_number : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Payments", "quarterNumber", c => c.Int());
            AddColumn("dbo.Payments", "year", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Payments", "year");
            DropColumn("dbo.Payments", "quarterNumber");
        }
    }
}
