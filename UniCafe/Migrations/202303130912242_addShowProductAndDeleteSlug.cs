namespace UniCafe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addShowProductAndDeleteSlug : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "Show", c => c.Int(nullable: false));
            DropColumn("dbo.OptionProducts", "Slug");
            DropColumn("dbo.PropertyProducts", "Slug");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PropertyProducts", "Slug", c => c.String());
            AddColumn("dbo.OptionProducts", "Slug", c => c.String());
            DropColumn("dbo.Products", "Show");
        }
    }
}
