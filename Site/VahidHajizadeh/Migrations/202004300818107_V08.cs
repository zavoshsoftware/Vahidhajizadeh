namespace Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V08 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SiteBlogs", "Body", c => c.String(storeType: "ntext"));
            DropColumn("dbo.SiteBlogs", "OldUrlParam");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SiteBlogs", "OldUrlParam", c => c.String());
            AlterColumn("dbo.SiteBlogs", "Body", c => c.String());
        }
    }
}
