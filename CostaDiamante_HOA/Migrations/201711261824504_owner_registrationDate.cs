namespace CostaDiamante_HOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class owner_registrationDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "registrationDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "registrationDate");
        }
    }
}
