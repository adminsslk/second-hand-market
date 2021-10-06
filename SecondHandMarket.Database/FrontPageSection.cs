namespace SecondHandMarket.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class FrontPageSection
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Section { get; set; }

        public int? Year { get; set; }

        public string Html { get; set; }

        public virtual Year Year1 { get; set; }
    }
}
