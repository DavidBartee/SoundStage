namespace SoundStage
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class SoundStageDataModel : DbContext
    {
        public SoundStageDataModel()
            : base("name=SoundStageDataModel")
        {
        }

        public virtual DbSet<BUTTONBIND> BUTTONBINDS { get; set; }
        public virtual DbSet<KEYBIND> KEYBINDS { get; set; }
        public virtual DbSet<SOUND> SOUNDs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<KEYBIND>()
                .Property(e => e.keys)
                .IsUnicode(false);

            modelBuilder.Entity<SOUND>()
                .Property(e => e.filePath)
                .IsUnicode(false);

            modelBuilder.Entity<SOUND>()
                .Property(e => e.name)
                .IsUnicode(false);
        }
    }
}
