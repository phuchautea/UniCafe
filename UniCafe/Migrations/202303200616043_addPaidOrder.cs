namespace UniCafe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPaidOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "Paid", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "Paid");
        }
    }
}
