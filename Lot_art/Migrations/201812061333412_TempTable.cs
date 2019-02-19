namespace Lot_art.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TempTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TempTables",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ForeignId = c.Int(nullable: false),
                        Value = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TempTables");
        }
    }
}
