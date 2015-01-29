using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ScanerApp.Models
{
    public class GeneralDbContext : DbContext
    {
        public GeneralDbContext() : base("DefaultConnection") { }

        public DbSet<Word> Words { get; set; }
        public DbSet<Doc> Docs { get; set; }
        public DbSet<WordByDoc> WordByDocs { get; set; }
        public DbSet<Position> Positions { get; set; }

    }

    public partial class Word
    {
        public Word()
        {
            this.WordByDocs = new HashSet<WordByDoc>();
        }

        public int WordId { get; set; }
        public string Title { get; set; }

        public virtual ICollection<WordByDoc> WordByDocs { get; set; }
    }

    public class WordByDoc
    {
        public WordByDoc()
        {
            this.Positions = new HashSet<Position>();
        }

        public int WordByDocId { get; set; }

        public virtual Word Word { get; set; }
        public virtual Doc Doc { get; set; }
        public virtual ICollection<Position> Positions { get; set; }
    }

    public class Doc
    {
        public Doc()
        {
            this.WordByDocs = new HashSet<WordByDoc>();
        }

        public int DocId { get; set; }
        public string Name { get; set; }
        public string FullPath { get; set; }

        public virtual ICollection<WordByDoc> WordByDocs { get; set; }
    }

    public class Position
    {
        public int PositionId { get; set; }
        public int Line { get; set; }
        public int Step { get; set; }

        public virtual WordByDoc WordByDoc { get; set; }
    }

}