using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DynamicDI.Test.WebApi.Data
{
    [Service(ServiceLifeCycle.Scoped, [typeof(DataContext)])]
    public class DataContext : DbContext
    {
        private readonly IConfiguration _config;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="config">Файл конфигурации.</param>
        public DataContext(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Коллекция расчетов.
        /// </summary>
        public DbSet<Calc> Calcs { get; set; }

        /// <summary>
        /// Коллекция интервалов.
        /// </summary>
        public DbSet<CalcInterval> CalcIntervals { get; set; }

        /// <summary>
        /// Коллекция статей интервалов.
        /// </summary>
        public DbSet<IntervalArticle> IntervalArticles { get; set; }

        /// <summary>
        /// Коллекция объектов, представляющих образ критической ситуации.
        /// </summary>
        public DbSet<CriticalSituationImage> CriticalSituationImages { get; set; }

        /// <summary>
        /// Коллекция групп объектов ОКС.
        /// </summary>
        public DbSet<CsiGroup> CsiGroups { get; set; }

        /// <summary>
        /// Коллекция векторов ОКС.
        /// </summary>
        public DbSet<CsiVector> CsiVectors { get; set; }

        /// <summary>
        /// Коллекция абзацев ОКС.
        /// </summary>
        public DbSet<CsiItem> CsiItems { get; set; }

        /// <summary>
        /// Коллекция ключевых выражений ОКС.
        /// </summary>
        public DbSet<CsiQuery> CsiQueries { get; set; }

        /// <summary>
        /// Подключение к БД.
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_config["PostgresqlConnection"]);
        }

        /// <summary>
        /// Настройка модели.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IntervalArticle>()
                .HasKey(k => new { k.IntervalId, k.ArticleId, k.Para });

            modelBuilder.Entity<CriticalSituationImage>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("csi_pkey");

                entity.ToTable("csi");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Created)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("created");
                entity.Property(e => e.OwnerId).HasColumnName("ownerid");
                entity.Property(e => e.Title)
                    .HasMaxLength(448)
                    .HasColumnName("title");
                entity.Property(e => e.Updated)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("updated");
                entity.Property(e => e.ObjectId)
                    .HasColumnName("objectid");
                entity.Property(e => e.LastScore)
                    .HasColumnName("lastscore");

                entity.HasMany(d => d.Groups).WithMany(p => p.CsiObjects)
                    .UsingEntity<Dictionary<string, object>>(
                        "Csigrouprelation",
                        r => r.HasOne<CsiGroup>().WithMany()
                            .HasForeignKey("Groupid")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("csigrouprelation_groupid_fkey"),
                        l => l.HasOne<CriticalSituationImage>().WithMany()
                            .HasForeignKey("Csiid")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("csigrouprelation_csiid_fkey"),
                        j =>
                        {
                            j.HasKey("Csiid", "Groupid").HasName("csigrouprelation_pkey");
                            j.ToTable("csigrouprelation");
                            j.IndexerProperty<int>("Csiid").HasColumnName("csiid");
                            j.IndexerProperty<int>("Groupid").HasColumnName("groupid");
                        });
            });

            modelBuilder.Entity<CsiGroup>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("csigroup_pkey");

                entity.ToTable("csigroup");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Color)
                    .HasMaxLength(20)
                    .HasColumnName("color");
                entity.Property(e => e.Created)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("created");
                entity.Property(e => e.OwnerId).HasColumnName("ownerid");
                entity.Property(e => e.Title)
                    .HasMaxLength(448)
                    .HasColumnName("title");
                entity.Property(e => e.Updated)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("updated");
            });

            modelBuilder.Entity<CsiItem>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("csiitem_pkey");

                entity.ToTable("csiitem");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.CsiId).HasColumnName("csiid");
                entity.Property(e => e.Rel).HasColumnName("rel");
                entity.Property(e => e.State).HasColumnName("state");
                entity.Property(e => e.Text).HasColumnName("text");

                entity.HasOne(d => d.Csi).WithMany(p => p.CsiItems)
                    .HasForeignKey(d => d.CsiId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("csiitem_csiid_fkey");
            });

            modelBuilder.Entity<CsiQuery>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("csiquery_pkey");

                entity.ToTable("csiquery");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.CsiId).HasColumnName("csiid");
                entity.Property(e => e.Query).HasColumnName("query");

                entity.HasOne(d => d.Csi).WithMany(p => p.CsiQueries)
                    .HasForeignKey(d => d.CsiId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("csiquery_csiid_fkey");
            });

            modelBuilder.Entity<CsiVector>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("csivector_pkey");

                entity.ToTable("csivector");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.CsiId).HasColumnName("csiid");
                entity.Property(e => e.Value).HasColumnName("val");

                entity.HasOne(d => d.Csi).WithMany(p => p.CsiVectors)
                    .HasForeignKey(d => d.CsiId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("csivector_csiid_fkey");
            });

            base.OnModelCreating(modelBuilder);
        }
    }

    [Table("intervalarticle")]
    public class IntervalArticle
    {
        /// <summary>
        /// Идентификатор интервала расчета.
        /// </summary>
        [Column("intervalid")] public int IntervalId { get; set; }

        /// <summary>
        /// Идентификатор статьи.
        /// </summary>
        [Column("articleid")] public long ArticleId { get; set; }

        /// <summary>
        /// Параграф.
        /// </summary>
        [Column("para")] public int Para { get; set; }

        /// <summary>
        /// Флаг, указывающий, что абзац имеет выделение.
        /// </summary>
        [Column("hasselection")] public bool HasSelection { get; set; }
    }

    [Table("csivector")]
    public class CsiVector
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор ОКС.
        /// </summary>
        public int CsiId { get; set; }

        /// <summary>
        /// Значение.
        /// </summary>
        public float Value { get; set; }

        /// <summary>
        /// ОКС.
        /// </summary>
        public virtual CriticalSituationImage Csi { get; set; } = null!;
    }

    [Table("csiquery")]
    public class CsiQuery
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор ОКС.
        /// </summary>
        public int CsiId { get; set; }

        /// <summary>
        /// Ключевое выражение.
        /// </summary>
        public string Query { get; set; } = null!;

        /// <summary>
        /// ОКС.
        /// </summary>
        public virtual CriticalSituationImage Csi { get; set; } = null!;
    }

    [Table("csiitem")]
    public class CsiItem
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор ОКС.
        /// </summary>
        public int CsiId { get; set; }

        /// <summary>
        /// Релевантность.
        /// </summary>
        public float Rel { get; set; }

        /// <summary>
        /// Состояние.
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// Текст абзаца.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// ОКС.
        /// </summary>
        public virtual CriticalSituationImage Csi { get; set; } = null!;
    }

    [Table("csigroup")]
    public class CsiGroup
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Дата создания.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Дата обновления.
        /// </summary>
        public DateTime? Updated { get; set; }

        /// <summary>
        /// Идентификатор владельца.
        /// </summary>
        public int OwnerId { get; set; }

        /// <summary>
        /// Заголовок.
        /// </summary>
        public string Title { get; set; } = null!;

        /// <summary>
        /// Цвет.
        /// </summary>
        public string Color { get; set; } = null!;

        /// <summary>
        /// Коллекция ОКС, входящих в группу.
        /// </summary>
        public virtual ICollection<CriticalSituationImage> CsiObjects { get; set; } = new List<CriticalSituationImage>();
    }

    [Table("csi")]
    public class CriticalSituationImage
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Дата создания.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Дата обновления.
        /// </summary>
        public DateTime? Updated { get; set; }

        /// <summary>
        /// Идентификатор владельца.
        /// </summary>
        public int OwnerId { get; set; }

        /// <summary>
        /// Заголовок.
        /// </summary>
        public string Title { get; set; } = null!;

        /// <summary>
        /// Идентификатор привязанного ОИ.
        /// </summary>
        public int? ObjectId { get; set; }

        /// <summary>
        /// Значение релевантности последней полученной статьи.
        /// </summary>
        public double LastScore { get; set; }

        /// <summary>
        /// Коллекция групп, в которых участвует ОКС.
        /// </summary>
        public virtual ICollection<CsiGroup> Groups { get; set; } = new List<CsiGroup>();

        /// <summary>
        /// Коллекция векторов, привязанных к ОКС.
        /// </summary>
        public virtual ICollection<CsiVector> CsiVectors { get; set; } = new List<CsiVector>();

        /// <summary>
        /// Коллекция абзацев, привязанных к ОКС.
        /// </summary>
        public virtual ICollection<CsiItem> CsiItems { get; set; } = new List<CsiItem>();

        /// <summary>
        /// Коллекция запросов, привязанных к ОКС.
        /// </summary>
        public virtual ICollection<CsiQuery> CsiQueries { get; set; } = new List<CsiQuery>();
    }

    [Table("calcinterval")]
    public class CalcInterval
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        [Key][Column("id")] public int Id { get; set; }

        /// <summary>
        /// Идентификатор расчета.
        /// </summary>
        [Column("calcid")] public int CalcId { get; set; }

        /// <summary>
        /// Период.
        /// </summary>
        [Column("fromdate")] public DateTime From { get; set; }

        /// <summary>
        /// Период.
        /// </summary>
        [Column("todate")] public DateTime To { get; set; }

        /// <summary>
        /// Дата начала расчета.
        /// </summary>
        [Column("calcstart")] public DateTime? CalcStart { get; set; }

        /// <summary>
        /// Дата окончания расчета.
        /// </summary>
        [Column("calcfinish")] public DateTime? CalcFinish { get; set; }

        /// <summary>
        /// Мера схожести.
        /// </summary>
        [Column("sm")] public float? SM { get; set; }

        /// <summary>
        /// Состояние расчета.
        /// </summary>
        [Column("state")] public int State { get; set; }

        /// <summary>
        /// Описание ошибки.
        /// </summary>
        [Column("error")] public string? Error { get; set; }

        /// <summary>
        /// Прогресс расчета (0 - 100).
        /// </summary>
        [Column("progress")] public float Progress { get; set; }

        /// <summary>
        /// Количество статей.
        /// </summary>
        [Column("articles")] public int Articles { get; set; }

        /// <summary>
        /// Количество параграфов.
        /// </summary>
        [Column("paras")] public int Paras { get; set; }

        /// <summary>
        /// Количество абзацев, имеющие выделенные выражения.
        /// </summary>
        [Column("highlightedparas")] public int HighlightedParas { get; set; }
    }

    [Table("calc")]
    public class Calc
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        [Key][Column("id")] public int Id { get; set; }

        /// <summary>
        /// Дата создания.
        /// </summary>
        [Column("created")] public DateTime Created { get; set; }

        /// <summary>
        /// Дата обновления.
        /// </summary>
        [Column("updated")] public DateTime? Updated { get; set; }

        /// <summary>
        /// Заголовок расчета.
        /// </summary>
        [Column("title")] public string? Title { get; set; }

        /// <summary>
        /// Владелец.
        /// </summary>
        [Column("ownerid")] public int OwnerId { get; set; }

        /// <summary>
        /// Идентификатор ОИ.
        /// </summary>
        [Column("roid")] public int RoId { get; set; }

        /// <summary>
        /// Идентификатор ОИ, выполняющего роль ОКС.
        /// </summary>
        [Column("csiid")] public int CsiId { get; set; }

        /// <summary>
        /// Временная зона.
        /// </summary>
        [Column("timezoneoffset")] public int TimezoneOffset { get; set; }

        /// <summary>
        /// Период расчета.
        /// </summary>
        [Column("fromdate")] public DateTime From { get; set; }

        /// <summary>
        /// Период расчета.
        /// </summary>
        [Column("todate")] public DateTime To { get; set; }

        /// <summary>
        /// Тип интервала.
        /// </summary>
        [Column("intervaltype")] public int IntervalType { get; set; }

        /// <summary>
        /// Размер интервала.
        /// </summary>
        [Column("intervalsize")] public int IntervalSize { get; set; }

        /// <summary>
        /// Количество интервалов.
        /// </summary>
        [Column("intervalcount")] public int IntervalCount { get; set; }
    }
}
