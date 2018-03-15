namespace CostaDiamante_HOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newFields_user : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "address", c => c.String());
            AddColumn("dbo.AspNetUsers", "city", c => c.String());
            AddColumn("dbo.AspNetUsers", "state", c => c.String());
            AddColumn("dbo.AspNetUsers", "zip", c => c.String());
            AddColumn("dbo.AspNetUsers", "mobile", c => c.String());
            AddColumn("dbo.AspNetUsers", "mex_phone", c => c.String());
            AddColumn("dbo.AspNetUsers", "other_phone", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "other_phone");
            DropColumn("dbo.AspNetUsers", "mex_phone");
            DropColumn("dbo.AspNetUsers", "mobile");
            DropColumn("dbo.AspNetUsers", "zip");
            DropColumn("dbo.AspNetUsers", "state");
            DropColumn("dbo.AspNetUsers", "city");
            DropColumn("dbo.AspNetUsers", "address");
        }
    }
}
