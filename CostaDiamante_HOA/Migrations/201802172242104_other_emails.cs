namespace CostaDiamante_HOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class other_emails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "other_emails", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "other_emails");
        }
    }
}
