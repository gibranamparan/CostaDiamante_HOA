namespace CostaDiamante_HOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newField_username_password_ExcelClass : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Owners_ExcelClass", "username", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Owners_ExcelClass", "username");
        }
    }
}
