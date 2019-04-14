namespace SoundStage
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("KEYBINDS")]
    public partial class KEYBIND
    {
        [Key]
        public int bindID { get; set; }

        [Required]
        [StringLength(50)]
        public string keys { get; set; }

        public int soundID { get; set; }

        public virtual SOUND SOUND { get; set; }
    }
}
