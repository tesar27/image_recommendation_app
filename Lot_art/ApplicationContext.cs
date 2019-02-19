using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lot_art
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("DefaultConnection")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }
        public DbSet<Image> Images { get; set; }
        public DbSet<Tag> Tags { get; set; }
        //public DbSet<TempTable> TempTables { get; set; }
        public DbSet<Score> Scores { get; set; }
    }

    public class Image
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public IList<Score> Scores { get; set; }
    }

    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<Score> Scores { get; set; }
    }

    public class Score
    {
        [Key]
        [Column(Order = 1)]
        [Index("IX_Score", IsUnique = true, Order = 1)]
        public int TagId { get; set; }
        [Key]
        [Column(Order = 2)]
        [Index("IX_Score", IsUnique = true, Order = 3)]
        public int ImageId { get; set; }
        [Index("IX_Score", IsUnique = true, Order = 2)]
        public double Value { get; set; }
        public Image Image { get; set; }
        public Tag Tag { get; set; }
    }

    //public class TempTable
    //{
    //    public Guid Id { get; set; }
    //    public int ForeignId { get; set; }
    //    public double Value { get; set; }

    //}
}
