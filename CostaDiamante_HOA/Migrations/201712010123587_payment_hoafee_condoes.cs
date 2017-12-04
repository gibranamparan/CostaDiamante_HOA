namespace CostaDiamante_HOA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class payment_hoafee_condoes : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Payments", "ownerID", "dbo.AspNetUsers");
            DropIndex("dbo.Payments", new[] { "ownerID" });
            AddColumn("dbo.Payments", "condoID", c => c.Int());
            CreateIndex("dbo.Payments", "condoID");
            AddForeignKey("dbo.Payments", "condoID", "dbo.Condoes", "condoID");
            DropColumn("dbo.Payments", "ownerID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Payments", "ownerID", c => c.String(maxLength: 128));
            DropForeignKey("dbo.Payments", "condoID", "dbo.Condoes");
            DropIndex("dbo.Payments", new[] { "condoID" });
            DropColumn("dbo.Payments", "condoID");
            CreateIndex("dbo.Payments", "ownerID");
            AddForeignKey("dbo.Payments", "ownerID", "dbo.AspNetUsers", "Id");
        }
    }
}
