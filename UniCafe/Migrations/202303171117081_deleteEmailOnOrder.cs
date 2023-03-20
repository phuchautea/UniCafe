namespace UniCafe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deleteEmailOnOrder : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Orders", "Email");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "Email", c => c.String());
        }
    }
}
