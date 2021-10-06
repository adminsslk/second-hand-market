namespace SecondHandMarket.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class GlobalSetting
    {
        [Key]
        [StringLength(25)]
        public string Key { get; set; }

        [StringLength(50)]
        public string Value { get; set; }
    }
}
