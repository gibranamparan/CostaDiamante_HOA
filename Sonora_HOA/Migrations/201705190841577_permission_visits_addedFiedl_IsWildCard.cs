namespace Sonora_HOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class permission_visits_addedFiedl_IsWildCard : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Permissions_Visits", "isWildCard", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Permissions_Visits", "isWildCard");
        }
    }
}
