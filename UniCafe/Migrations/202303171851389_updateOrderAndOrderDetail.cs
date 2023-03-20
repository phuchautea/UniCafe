namespace UniCafe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateOrderAndOrderDetail : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        ProductName = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Quantity = c.Int(nullable: false),
                        PropertyProduct = c.String(),
                        OptionProduct = c.String(),
                        Order_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.Order_Id)
                .Index(t => t.Order_Id);
            
            AddColumn("dbo.Orders", "Code", c => c.String());
            AddColumn("dbo.Orders", "Status", c => c.String());
            AddColumn("dbo.Orders", "CreatedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.Orders", "UpdatedAt", c => c.DateTime(nullable: false));
            DropColumn("dbo.Orders", "OrderCode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "OrderCode", c => c.String());
            DropForeignKey("dbo.OrderDetails", "Order_Id", "dbo.Orders");
            DropIndex("dbo.OrderDetails", new[] { "Order_Id" });
            DropColumn("dbo.Orders", "UpdatedAt");
            DropColumn("dbo.Orders", "CreatedAt");
            DropColumn("dbo.Orders", "Status");
            DropColumn("dbo.Orders", "Code");
            DropTable("dbo.OrderDetails");
        }
    }
}
