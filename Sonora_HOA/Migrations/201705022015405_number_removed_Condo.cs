namespace Sonora_HOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class number_removed_Condo : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Condoes", "num");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Condoes", "num", c => c.Int(nullable: false));
        }
    }
}
