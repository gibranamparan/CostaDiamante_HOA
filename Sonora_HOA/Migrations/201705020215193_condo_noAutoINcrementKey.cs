namespace Sonora_HOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class condo_noAutoINcrementKey : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Permissions", "number", "dbo.Condoes");
            DropPrimaryKey("dbo.Condoes");
            AlterColumn("dbo.Condoes", "number", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Condoes", "number");
            AddForeignKey("dbo.Permissions", "number", "dbo.Condoes", "number", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Permissions", "number", "dbo.Condoes");
            DropPrimaryKey("dbo.Condoes");
            AlterColumn("dbo.Condoes", "number", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Condoes", "number");
            AddForeignKey("dbo.Permissions", "number", "dbo.Condoes", "number", cascadeDelete: true);
        }
    }
}
