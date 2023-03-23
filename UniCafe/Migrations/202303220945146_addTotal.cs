namespace UniCafe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTotal : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderDetails", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Orders", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "Total");
            DropColumn("dbo.OrderDetails", "Total");
        }
    }
}
