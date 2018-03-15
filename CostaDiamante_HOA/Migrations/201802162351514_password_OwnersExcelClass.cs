namespace CostaDiamante_HOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class password_OwnersExcelClass : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Owners_ExcelClass", "password", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Owners_ExcelClass", "password");
        }
    }
}
