namespace SecondHandMarket.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Item
    {
        public int Id { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public int? Year { get; set; }

        public int? Price { get; set; }

        public int? SalemanId { get; set; }

        public int? StatusId { get; set; }

        public int? SellersShare { get; set; }

        public int NumberOfLabels { get; set; }

        public bool IsExchangeItem { get; set; }

        public virtual ItemStatus ItemStatus { get; set; }

        public virtual User Salesman { get; set; }
    }
}
