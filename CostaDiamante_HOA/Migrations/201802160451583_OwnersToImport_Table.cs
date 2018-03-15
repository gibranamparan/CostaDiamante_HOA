namespace CostaDiamante_HOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OwnersToImport_Table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Owners_ExcelClass",
                c => new
                    {
                        regID = c.Int(nullable: false, identity: true),
                        NAME = c.Int(nullable: false),
                        SECTION = c.Int(nullable: false),
                        LOT = c.Int(nullable: false),
                        ADDRESS = c.Int(nullable: false),
                        CITY = c.Int(nullable: false),
                        STATE = c.Int(nullable: false),
                        ZIP = c.Int(nullable: false),
                        PHONE = c.Int(nullable: false),
                        MOBILE = c.Int(nullable: false),
                        MEXNUM = c.Int(nullable: false),
                        OTHER_PHONE = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.regID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Owners_ExcelClass");
        }
    }
}
