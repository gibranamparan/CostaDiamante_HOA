namespace CostaDiamante_HOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class payments_discountAndReference : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Payments", "isDiscount", c => c.Boolean(nullable: false));
            AddColumn("dbo.Payments", "reference", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Payments", "reference");
            DropColumn("dbo.Payments", "isDiscount");
        }
    }
}
