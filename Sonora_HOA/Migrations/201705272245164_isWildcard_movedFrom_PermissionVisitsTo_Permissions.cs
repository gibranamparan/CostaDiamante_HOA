namespace Sonora_HOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class isWildcard_movedFrom_PermissionVisitsTo_Permissions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Permissions", "isWildCard", c => c.Boolean(nullable: false));
            DropColumn("dbo.Permissions_Visits", "isWildCard");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Permissions_Visits", "isWildCard", c => c.Boolean(nullable: false));
            DropColumn("dbo.Permissions", "isWildCard");
        }
    }
}
