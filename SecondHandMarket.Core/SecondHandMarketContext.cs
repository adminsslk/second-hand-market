using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace SecondHandMarket.Core
{
    public partial class SecondHandMarketContext : DbContext
    {
        public SecondHandMarketContext()
            : base("name=SecondHandMarketContext")
        {
            Database.SetInitializer(new SecondHandMarketDBInitializer());
        }

        public virtual DbSet<FrontPageSection> FrontPageSections { get; set; }
        public virtual DbSet<GlobalSetting> GlobalSettings { get; set; }
        public virtual DbSet<ItemCategory> ItemCategories { get; set; }
        public virtual DbSet<ItemChangeLog> ItemChangeLogs { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<ItemStatus> ItemStatus { get; set; }
        public virtual DbSet<ReceiptPrintOut> ReceiptPrintOuts { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Year> Years { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FrontPageSection>()
                .Property(e => e.Section)
                .IsUnicode(false);

            modelBuilder.Entity<FrontPageSection>()
                .Property(e => e.Html)
                .IsUnicode(false);

            modelBuilder.Entity<GlobalSetting>()
                .Property(e => e.Key)
                .IsUnicode(false);

            modelBuilder.Entity<GlobalSetting>()
                .Property(e => e.Value)
                .IsUnicode(false);

            modelBuilder.Entity<ItemCategory>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<ItemChangeLog>()
                .Property(e => e.UserName)
                .IsUnicode(false);

            modelBuilder.Entity<ItemChangeLog>()
                .Property(e => e.Log)
                .IsUnicode(false);

            modelBuilder.Entity<Item>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<ItemStatus>()
                .Property(e => e.Status)
                .IsUnicode(false);

            modelBuilder.Entity<ItemStatus>()
                .HasMany(e => e.Items)
                .WithOptional(e => e.ItemStatus)
                .HasForeignKey(e => e.StatusId);

            modelBuilder.Entity<ReceiptPrintOut>()
                .Property(e => e.UserName)
                .IsUnicode(false);

            modelBuilder.Entity<UserRole>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<UserRole>()
                .HasMany(e => e.Users)
                .WithOptional(e => e.UserRole)
                .HasForeignKey(e => e.RoleId);

            modelBuilder.Entity<User>()
                .Property(e => e.FirstName)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.LastName)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Bank)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.ClearingNumber)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.AccountNumber)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Items)
                .WithOptional(e => e.Salesman)
                .HasForeignKey(e => e.SalemanId);

            modelBuilder.Entity<Year>()
                .HasMany(e => e.FrontPageSections)
                .WithOptional(e => e.Year1)
                .HasForeignKey(e => e.Year);
        }
    }

    public class SecondHandMarketDBInitializer : CreateDatabaseIfNotExists<SecondHandMarketContext>
    {
        protected override void Seed(SecondHandMarketContext context)
        {
            context.Years.Add(
                    new Year()
                    {
                        Value = DateTime.Now.Year,
                        RevenueShare = 0.15,
                        SalesCost = 30
                    }
                );

            context.UserRoles.Add(
                        new UserRole()
                        {
                            Id = 1,
                            Name = "[okänd]"
                        }
                    );
            context.UserRoles.Add(
                        new UserRole()
                        {
                            Id = 2,
                            Name = "sysadm"
                        }
                    );
            context.UserRoles.Add(
                        new UserRole()
                        {
                            Id = 3,
                            Name = "salesadm"
                        }
                    );
            context.UserRoles.Add(
                        new UserRole()
                        {
                            Id = 4,
                            Name = "salesman"
                        }
                    );
            context.UserRoles.Add(
                        new UserRole()
                        {
                            Id = 5,
                            Name = "wholesale"
                        }
                    );

            context.Users.Add(
                    new User()
                    {
                        FirstName = "Admin",
                        LastName = "Sslk",
                        Email = "admin@sslk.onmicrosoft.com",
                        Password = "sslk",
                        RoleId = 2,
                        IsMember = true
                    }
                );
            context.Users.Add(
                    new User()
                    {
                        FirstName = "Bo",
                        LastName = "Kunnig",
                        Email = "bytesmarknad@sslk.se",
                        Password = "sslk",
                        RoleId = 4,
                        IsMember = true
                    }
                );

            context.GlobalSettings.Add(
                        new GlobalSetting()
                        {
                            Key = "ActiveYear",
                            Value = DateTime.Now.Year.ToString()
                        }
                    );
            context.GlobalSettings.Add(
                        new GlobalSetting()
                        {
                            Key = "DefaultSalesmanId",
                            Value = "2"
                        }
                    );
            context.GlobalSettings.Add(
                        new GlobalSetting()
                        {
                            Key = "RevenueShare",
                            Value = "0.15"
                        }
                    );
            context.GlobalSettings.Add(
                        new GlobalSetting()
                        {
                            Key = "SalesCost",
                            Value = "30"
                        }
                    );

            context.ItemStatus.Add(
                        new ItemStatus()
                        {
                            Id = 1,
                            Status = "Inregisterad"
                        }
                    );
            context.ItemStatus.Add(
                        new ItemStatus()
                        {
                            Id = 2,
                            Status = "Inlämnad"
                        }
                    );
            context.ItemStatus.Add(
                        new ItemStatus()
                        {
                            Id = 3,
                            Status = "Såld"
                        }
                    );
            context.ItemStatus.Add(
                        new ItemStatus()
                        {
                            Id = 4,
                            Status = "Återlämnad"
                        }
                    );
            context.ItemStatus.Add(
                        new ItemStatus()
                        {
                            Id = 5,
                            Status = "Utbetald"
                        }
                    );

            context.ItemCategories.Add(
                        new ItemCategory()
                        {
                            Id = 1,
                            Name = "BENSKYDD"
                        }
                    );
            context.ItemCategories.Add(
                        new ItemCategory()
                        {
                            Id = 2,
                            Name = "SNOWBOARD"
                        }
                    );
            context.ItemCategories.Add(
                        new ItemCategory()
                        {
                            Id = 3,
                            Name = "FARTDRÄKT"
                        }
                    );
            context.ItemCategories.Add(
                        new ItemCategory()
                        {
                            Id = 4,
                            Name = "BYXOR"
                        }
                    );
            context.ItemCategories.Add(
                        new ItemCategory()
                        {
                            Id = 5,
                            Name = "GLASÖGON"
                        }
                    );
            context.ItemCategories.Add(
                        new ItemCategory()
                        {
                            Id = 6,
                            Name = "LÄNGDSKIDOR"
                        }
                    );
            context.ItemCategories.Add(
                        new ItemCategory()
                        {
                            Id = 7,
                            Name = "JACKA"
                        }
                    );
            context.ItemCategories.Add(
                        new ItemCategory()
                        {
                            Id = 8,
                            Name = "HJÄLM"
                        }
                    );
            context.ItemCategories.Add(
                        new ItemCategory()
                        {
                            Id = 9,
                            Name = "KLUBBA"
                        }
                    );
            context.ItemCategories.Add(
                        new ItemCategory()
                        {
                            Id = 10,
                            Name = "SKOR"
                        }
                    );
            context.ItemCategories.Add(
                        new ItemCategory()
                        {
                            Id = 11,
                            Name = "SLALOMSKIDOR"
                        }
                    );
            context.ItemCategories.Add(
                        new ItemCategory()
                        {
                            Id = 12,
                            Name = "SLALOMPJÄXOR"
                        }
                    );
            context.ItemCategories.Add(
                        new ItemCategory()
                        {
                            Id = 13,
                            Name = "LÄNGDPJÄXOR"
                        }
                    );
            context.ItemCategories.Add(
                        new ItemCategory()
                        {
                            Id = 14,
                            Name = "RYGGSKYDD"
                        }
                    );
            context.ItemCategories.Add(
                        new ItemCategory()
                        {
                            Id = 15,
                            Name = "SLALOMSTAVAR"
                        }
                    );
            context.ItemCategories.Add(
                        new ItemCategory()
                        {
                            Id = 16,
                            Name = "LÄNGDSTAVAR"
                        }
                    );
            context.ItemCategories.Add(
                        new ItemCategory()
                        {
                            Id = 17,
                            Name = "SKRIDSKOR"
                        }
                    );
            context.ItemCategories.Add(
                        new ItemCategory()
                        {
                            Id = 18,
                            Name = "SNOWBOARDSKOR"
                        }
                    );
            context.ItemCategories.Add(
                        new ItemCategory()
                        {
                            Id = 19,
                            Name = "TWINTIPS"
                        }
                    );
            context.ItemCategories.Add(
                        new ItemCategory()
                        {
                            Id = 20,
                            Name = "STORSLALOMSKIDOR"
                        }
                    );
            context.ItemCategories.Add(
                        new ItemCategory()
                        {
                            Id = 21,
                            Name = "STÖRTLOPPSSKIDOR"
                        }
                    );
            context.ItemCategories.Add(
                        new ItemCategory()
                        {
                            Id = 22,
                            Name = "SUPER-G SKIDOR"
                        }
                    );

            context.FrontPageSections.Add(
                        new FrontPageSection()
                        {
                            Section = "info",
                            Year = DateTime.Now.Year,
                            Html = SectionInfo()
                        }
                    );
            context.FrontPageSections.Add(
                        new FrontPageSection()
                        {
                            Section = "openinghours",
                            Year = DateTime.Now.Year,
                            Html = SectionOpeningHours()
                        }
                    );
            context.FrontPageSections.Add(
                        new FrontPageSection()
                        {
                            Section = "map",
                            Year = DateTime.Now.Year,
                            Html = SectionMap()
                        }
                    );


            base.Seed(context);
        }

        private string SectionInfo()
        {
            return "<div class=\"container\"><div class=\"row\"><div class=\"col-lg-12 col-md-12\"><h1>V&auml;lkommen till &aring;rets bytesmarknad</h1><hr /><p class=\"lead\"><strong>Helgen 21-23 okt&nbsp;</strong>k&ouml;r Sundsvalls Slalomklubb som vanligt sin &aring;rliga bytemarknad i <strong>Bilbolagets lokaler</strong> p&aring; Bultgatan 4 i Sundsvall. Kom och l&auml;mna in de vinterprylar du vill bli av med och uppgradera er egen utrustning inf&ouml;r kommande vinter.</p></div></div><div class=\"row\">&nbsp;</div><div class=\"row\"><div class=\"col-lg-6 col-md-6\"><h2>Inl&auml;mning</h2><hr /><p class=\"lead\">Under fredageftermiddag och under l&ouml;rdagen kan du l&auml;mna in dina varor. P&aring; plats finns medlemmar fr&aring;n klubben som kan hj&auml;lpa dig med tips och priss&auml;ttning. N&auml;r du l&auml;mnat in dina prylar f&aring;r du ett inl&auml;mningskvitto av oss som du ska vara r&auml;dd om. Du kan sedan <strong>kolla om du s&aring;lt varan p&aring; webben</strong>. Det g&ouml;r du genom att knappa in varunummer eller ditt mobilnummer p&aring; denna sida.</p></div><div class=\"col-lg-6 col-md-6\"><h2>F&ouml;rs&auml;ljning</h2><hr /><p class=\"lead\">P&aring; l&ouml;rdag startar f&ouml;rs&auml;ljningen. P&aring; golvet finns det personal, i gula v&auml;star, som hj&auml;lper er med fr&aring;gor. I kassan tar vi emot <strong>kort- swish- och kontantbetalning</strong>.</p></div></div><div class=\"row\"><div class=\"col-lg-6 col-md-6\"><h2>Utbetalning &amp; &aring;terl&auml;mning</h2><hr /><p class=\"lead\">Fr&aring;n och med <strong>l&ouml;rdag eftermiddag</strong>&nbsp;<strong>kan du f&aring; utbetalt</strong> f&ouml;r dina s&aring;lda varor och &aring;terh&auml;mta de varor du inte lyckats s&auml;lja. Det kommer finnas kassor som hanterar b&aring;de utbetalning och &aring;terl&auml;mning under s&ouml;ndagen. Utbetalningar g&ouml;r kontant. Dina <strong>os&aring;lda varor m&aring;ste h&auml;mtas senast s&ouml;ndag 16:00 </strong>annars bortsk&auml;nkes de till Lidens skola.</p></div><div class=\"col-lg-6 col-md-6\"><h2>Mat &amp; Fika</h2><hr /><p class=\"lead\">Vi s&auml;ljer <strong>hamburgare med festis f&ouml;r 25 kr</strong> utanf&ouml;r Bilbolagets huvudentr&eacute;. Barn i ungdomsgrupperna har bakat och s&auml;ljer <strong>fika inne i bilhallen</strong>. Pengarna g&aring;r till deras aktiviteter. P&aring; s&ouml;ndag bjuder GB alla barn p&aring; glass.&nbsp;</p></div></div></div><p><br /> <br /> <br /> <br /><br /></p><p>&nbsp;</p><p>&nbsp;</p>";
        }

        private string SectionOpeningHours()
        {
            return "<div class=\"container\"><div class=\"row\"><div class=\"col-lg-4 col-md-4\">&nbsp;</div><div class=\"col-lg-4 col-md-4\"><br /><br /><h2>&Ouml;ppettider</h2><hr /><p class=\"lead\">Fredag 21&nbsp;okt ...... kl. 17:00 - 20:00 Inl&auml;mning</p><p class=\"lead\">L&ouml;rdag 22&nbsp;okt ...... kl. 10:00 - 16:00 Inl&auml;mning &amp; f&ouml;rs&auml;ljning</p><p class=\"lead\">S&ouml;ndag 23&nbsp;okt ...... kl. 10:00 - 16:00 F&ouml;rs&auml;ljning</p><hr /><br /><br /><br /></div><div class=\"col-lg-4 col-md-4\">&nbsp;</div></div></div>";
        }

        private string SectionMap()
        {
            return "<p><iframe style=\"border: 0;\" src=\"https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d1849.0928495746962!2d17.27606241643834!3d62.38985808283227!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x46645d8cb0945039%3A0xee0980556b3869c8!2sBultgatan+4%2C+853+50+Sundsvall!5e0!3m2!1ssv!2sse!4v1475663009028\" width=\"100%\" height=\"550\" frameborder=\"0\" allowfullscreen=\"\"></iframe></p>";
        }
    }
}
