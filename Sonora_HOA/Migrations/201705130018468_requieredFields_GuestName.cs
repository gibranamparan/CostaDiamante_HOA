namespace Sonora_HOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class requieredFields_GuestName : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Guests", "name", c => c.String(nullable: false));
            AlterColumn("dbo.Guests", "lastName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Guests", "lastName", c => c.String());
            AlterColumn("dbo.Guests", "name", c => c.String());
        }
    }
}
