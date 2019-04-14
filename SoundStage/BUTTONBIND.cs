namespace SoundStage
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BUTTONBINDS")]
    public partial class BUTTONBIND
    {
        [Key]
        public int bindID { get; set; }

        public int soundID { get; set; }

        public virtual SOUND SOUND { get; set; }
    }
}
