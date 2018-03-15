namespace CostaDiamante_HOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newField_Email_OwnerExcelCclass : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Owners_ExcelClass", "EMAIL", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Owners_ExcelClass", "EMAIL");
        }
    }
}
