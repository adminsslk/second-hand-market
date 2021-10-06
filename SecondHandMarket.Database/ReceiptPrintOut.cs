namespace SecondHandMarket.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ReceiptPrintOut
    {
        public int Id { get; set; }

        public int Year { get; set; }

        public DateTime TimeStamp { get; set; }

        public int SalesmanId { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; }
    }
}
