namespace UniCafe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateDecimalPrice : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.OptionProducts", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.OptionProducts", "Price", c => c.String());
        }
    }
}
