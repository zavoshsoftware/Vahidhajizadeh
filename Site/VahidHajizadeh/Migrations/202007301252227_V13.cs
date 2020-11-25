namespace Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V13 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Orders", "CityId", "dbo.Cities");
            DropIndex("dbo.Orders", new[] { "CityId" });
            AddColumn("dbo.Orders", "City", c => c.String());
            DropColumn("dbo.Orders", "CityId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "CityId", c => c.Guid());
            DropColumn("dbo.Orders", "City");
            CreateIndex("dbo.Orders", "CityId");
            AddForeignKey("dbo.Orders", "CityId", "dbo.Cities", "Id");
        }
    }
}
