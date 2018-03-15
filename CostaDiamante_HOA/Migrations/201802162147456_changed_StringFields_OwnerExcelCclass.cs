namespace CostaDiamante_HOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changed_StringFields_OwnerExcelCclass : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Owners_ExcelClass", "NAME", c => c.String());
            AlterColumn("dbo.Owners_ExcelClass", "SECTION", c => c.String());
            AlterColumn("dbo.Owners_ExcelClass", "LOT", c => c.String());
            AlterColumn("dbo.Owners_ExcelClass", "ADDRESS", c => c.String());
            AlterColumn("dbo.Owners_ExcelClass", "CITY", c => c.String());
            AlterColumn("dbo.Owners_ExcelClass", "STATE", c => c.String());
            AlterColumn("dbo.Owners_ExcelClass", "ZIP", c => c.String());
            AlterColumn("dbo.Owners_ExcelClass", "EMAIL", c => c.String());
            AlterColumn("dbo.Owners_ExcelClass", "PHONE", c => c.String());
            AlterColumn("dbo.Owners_ExcelClass", "MOBILE", c => c.String());
            AlterColumn("dbo.Owners_ExcelClass", "MEXNUM", c => c.String());
            AlterColumn("dbo.Owners_ExcelClass", "OTHER_PHONE", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Owners_ExcelClass", "OTHER_PHONE", c => c.Int(nullable: false));
            AlterColumn("dbo.Owners_ExcelClass", "MEXNUM", c => c.Int(nullable: false));
            AlterColumn("dbo.Owners_ExcelClass", "MOBILE", c => c.Int(nullable: false));
            AlterColumn("dbo.Owners_ExcelClass", "PHONE", c => c.Int(nullable: false));
            AlterColumn("dbo.Owners_ExcelClass", "EMAIL", c => c.Int(nullable: false));
            AlterColumn("dbo.Owners_ExcelClass", "ZIP", c => c.Int(nullable: false));
            AlterColumn("dbo.Owners_ExcelClass", "STATE", c => c.Int(nullable: false));
            AlterColumn("dbo.Owners_ExcelClass", "CITY", c => c.Int(nullable: false));
            AlterColumn("dbo.Owners_ExcelClass", "ADDRESS", c => c.Int(nullable: false));
            AlterColumn("dbo.Owners_ExcelClass", "LOT", c => c.Int(nullable: false));
            AlterColumn("dbo.Owners_ExcelClass", "SECTION", c => c.Int(nullable: false));
            AlterColumn("dbo.Owners_ExcelClass", "NAME", c => c.Int(nullable: false));
        }
    }
}
