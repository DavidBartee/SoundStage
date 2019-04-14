namespace SoundStage
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SOUND")]
    public partial class SOUND
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SOUND()
        {
            BUTTONBINDS = new HashSet<BUTTONBIND>();
            KEYBINDS = new HashSet<KEYBIND>();
        }

        public int soundID { get; set; }

        [Required]
        [StringLength(300)]
        public string filePath { get; set; }

        public bool isBackup { get; set; }

        [StringLength(100)]
        public string name { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BUTTONBIND> BUTTONBINDS { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KEYBIND> KEYBINDS { get; set; }
    }
}
