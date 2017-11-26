namespace CostaDiamante_HOA
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class visit_modifiedaddtype : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Visits", "typeOfVisit", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Visits", "typeOfVisit");
        }
    }
}
