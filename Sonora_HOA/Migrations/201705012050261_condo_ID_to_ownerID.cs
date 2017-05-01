namespace Sonora_HOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class condo_ID_to_ownerID : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Condoes", name: "Id", newName: "ownerID");
            RenameIndex(table: "dbo.Condoes", name: "IX_Id", newName: "IX_ownerID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Condoes", name: "IX_ownerID", newName: "IX_Id");
            RenameColumn(table: "dbo.Condoes", name: "ownerID", newName: "Id");
        }
    }
}
