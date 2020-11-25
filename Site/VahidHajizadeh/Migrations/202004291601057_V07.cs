namespace Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V07 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.SiteBlogs", "Order");
            DropColumn("dbo.SiteBlogs", "Summery");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SiteBlogs", "Summery", c => c.String());
            AddColumn("dbo.SiteBlogs", "Order", c => c.Int(nullable: false));
        }
    }
}
