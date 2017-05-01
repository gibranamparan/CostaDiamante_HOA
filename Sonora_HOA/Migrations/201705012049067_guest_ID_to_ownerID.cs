namespace Sonora_HOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class guest_ID_to_ownerID : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Guests", name: "Id", newName: "ownerID");
            RenameIndex(table: "dbo.Guests", name: "IX_Id", newName: "IX_ownerID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Guests", name: "IX_ownerID", newName: "IX_Id");
            RenameColumn(table: "dbo.Guests", name: "ownerID", newName: "Id");
        }
    }
}
