namespace SecondHandMarket.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ItemChangeLog
    {
        public long Id { get; set; }

        public DateTime TimeStamp { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        public int ItemId { get; set; }

        [Required]
        [StringLength(250)]
        public string Log { get; set; }

        public int Year { get; set; }
    }
}
