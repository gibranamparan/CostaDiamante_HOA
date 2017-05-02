namespace Sonora_HOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class condoID_condo : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Permissions", "number", "dbo.Condoes");
            RenameColumn(table: "dbo.Permissions", name: "number", newName: "condoID");
            RenameIndex(table: "dbo.Permissions", name: "IX_number", newName: "IX_condoID");
            DropPrimaryKey("dbo.Condoes");
            DropColumn("dbo.Condoes", "number");
            AddColumn("dbo.Condoes", "condoID", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.Condoes", "num", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Condoes", "condoID");
            AddForeignKey("dbo.Permissions", "condoID", "dbo.Condoes", "condoID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            AddColumn("dbo.Condoes", "number", c => c.Int(nullable: false));
            DropForeignKey("dbo.Permissions", "condoID", "dbo.Condoes");
            DropPrimaryKey("dbo.Condoes");
            DropColumn("dbo.Condoes", "num");
            DropColumn("dbo.Condoes", "condoID");
            AddPrimaryKey("dbo.Condoes", "number");
            RenameIndex(table: "dbo.Permissions", name: "IX_condoID", newName: "IX_number");
            RenameColumn(table: "dbo.Permissions", name: "condoID", newName: "number");
            AddForeignKey("dbo.Permissions", "number", "dbo.Condoes", "number", cascadeDelete: true);
        }
    }
}
