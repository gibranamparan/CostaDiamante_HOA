namespace CostaDiamante_HOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class numberOfVisitors_Visit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Visits", "cantidadVisitantes", c => c.Int(nullable: false));
            AddColumn("dbo.Visits", "numBrazaletes", c => c.Int(nullable: false));
            DropColumn("dbo.Visitors", "isYounger");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Visitors", "isYounger", c => c.Boolean(nullable: false));
            DropColumn("dbo.Visits", "numBrazaletes");
            DropColumn("dbo.Visits", "cantidadVisitantes");
        }
    }
}
