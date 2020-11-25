namespace Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V06 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "CompanyName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "CompanyName");
        }
    }
}
