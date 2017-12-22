namespace CostaDiamante_HOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_visit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Visits", "withTheOwner", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Visits", "withTheOwner");
        }
    }
}
