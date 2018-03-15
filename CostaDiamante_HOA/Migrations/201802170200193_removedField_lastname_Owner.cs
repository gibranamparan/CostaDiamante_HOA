namespace CostaDiamante_HOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removedField_lastname_Owner : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "lastName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "lastName", c => c.String());
        }
    }
}
