namespace Lot_art.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Images",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Url = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Scores",
                c => new
                    {
                        TagId = c.Int(nullable: false),
                        ImageId = c.Int(nullable: false),
                        Value = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.TagId, t.ImageId })
                .ForeignKey("dbo.Images", t => t.ImageId, cascadeDelete: true)
                .ForeignKey("dbo.Tags", t => t.TagId, cascadeDelete: true)
                .Index(t => new { t.TagId, t.Value, t.ImageId }, unique: true, name: "IX_Score");
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Scores", "TagId", "dbo.Tags");
            DropForeignKey("dbo.Scores", "ImageId", "dbo.Images");
            DropIndex("dbo.Scores", "IX_Score");
            DropTable("dbo.Tags");
            DropTable("dbo.Scores");
            DropTable("dbo.Images");
        }
    }
}
