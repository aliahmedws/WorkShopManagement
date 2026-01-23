using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using WorkShopManagement.Bays;
using WorkShopManagement.CarBayItems;
using WorkShopManagement.CarBays;
using WorkShopManagement.CarModels;
using WorkShopManagement.Cars;
using WorkShopManagement.CheckInReports;
using WorkShopManagement.CheckLists;
using WorkShopManagement.EntityAttachments;
using WorkShopManagement.EntityAttachments.FileAttachments;
using WorkShopManagement.Issues;
using WorkShopManagement.ListItems;
using WorkShopManagement.LogisticsDetails;
using WorkShopManagement.LogisticsDetails.ArrivalEstimates;
using WorkShopManagement.ModelCategories;
using WorkShopManagement.QualityGates;
using WorkShopManagement.RadioOptions;
using WorkShopManagement.Recalls;
using WorkShopManagement.VinInfos;

namespace WorkShopManagement.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class WorkShopManagementDbContext :
    AbpDbContext<WorkShopManagementDbContext>,
    ITenantManagementDbContext,
    IIdentityDbContext,
    IDataProtectionKeyContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */


    #region Entities from the modules

    /* Notice: We only implemented IIdentityProDbContext and ISaasDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityProDbContext and ISaasDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }

    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion
    public DbSet<ModelCategory> ModelCategories { get; set; }
    public DbSet<CarModel> CarModels { get; set; }
    public DbSet<Bay> Bays { get; set; }
    public DbSet<CheckList> CheckLists { get; set; }
    public DbSet<ListItem> ListItems { get; set; }
    public DbSet<RadioOption> RadioOptions { get; set; }
    public DbSet<QualityGate> QualityGates { get; set; }
    public DbSet<EntityAttachment> EntityAttachments { get; set; }
    public DbSet<VinInfo> VinInfos { get; set; }
    public DbSet<CarBay> CarBays { get; set; }
    public DbSet<CarBayItem> CarBayItems { get; set; }
    public DbSet<CheckInReport> CheckInReports { get; set; }

    public DbSet<Car> Cars { get; set; }
    public DbSet<CarOwner> CarOwners { get; set; }
    public DbSet<Recall> Recalls { get; set; }
    public DbSet<Issue> Issues { get; set; }
    public DbSet<ArrivalEstimate> ArrivalEstimates { get; set; }
    public DbSet<LogisticsDetail> LogisticsDetails { get; set; }

    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

    public WorkShopManagementDbContext(DbContextOptions<WorkShopManagementDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureTenantManagement();

        /* Configure your own tables/entities inside here */
        builder.Entity<EntityAttachment>(b =>
        {
            b.ToTable(WorkShopManagementConsts.DbTablePrefix + "EntityAttachments", WorkShopManagementConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.EntityId).IsRequired();
            b.Property(x => x.EntityType).IsRequired();
            b.OwnsOne(x => x.Attachment, fa =>
            {
                fa.Property(f => f.Name).IsRequired().HasMaxLength(FileAttachmentConsts.MaxNameLength);
                fa.Property(f => f.BlobName).IsRequired().HasMaxLength(FileAttachmentConsts.MaxPathLength);
                fa.Property(f => f.Path).IsRequired().HasMaxLength(FileAttachmentConsts.MaxPathLength);
            });
        });

        builder.Entity<ModelCategory>(b =>
        {
            b.ToTable(WorkShopManagementConsts.DbTablePrefix + "ModelCategories", WorkShopManagementConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired();

            b.HasMany(x => x.CarModels)
               .WithOne(x => x.ModelCategory)
               .HasForeignKey(x => x.ModelCategoryId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

            b.OwnsOne(x => x.FileAttachments, fa =>
            {
                fa.Property(f => f.Name).IsRequired().HasMaxLength(FileAttachmentConsts.MaxNameLength);
                fa.Property(f => f.Path).IsRequired().HasMaxLength(FileAttachmentConsts.MaxPathLength);
            });

            b.HasIndex(x => x.Name);
        });

        builder.Entity<CarModel>(b =>
        {
            b.ToTable(WorkShopManagementConsts.DbTablePrefix + "CarModels", WorkShopManagementConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired();

            b.OwnsOne(x => x.FileAttachments, fa =>
            {
                fa.Property(f => f.Name).IsRequired().HasMaxLength(FileAttachmentConsts.MaxNameLength);
                fa.Property(f => f.Path).IsRequired().HasMaxLength(FileAttachmentConsts.MaxPathLength);
            });

            b.HasIndex(x => x.Name);
        });

        builder.Entity<Bay>(b =>
        {
            b.ToTable(WorkShopManagementConsts.DbTablePrefix + "Bays", WorkShopManagementConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired();
            b.HasIndex(x => x.Name);
        });

        builder.Entity<QualityGate>(b =>
        {
            b.ToTable(WorkShopManagementConsts.DbTablePrefix + "QualityGates", WorkShopManagementConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasOne(x => x.CarBays)
               .WithMany(x => x.QualityGates)
                   .HasForeignKey(x => x.CarBayId)
                       .OnDelete(DeleteBehavior.Restrict);

            b.Property(x => x.Status).IsRequired();
            b.Property(x => x.GateName).IsRequired();
        });

        builder.Entity<CheckList>(b =>
        {
            b.ToTable(WorkShopManagementConsts.DbTablePrefix + "CheckLists", WorkShopManagementConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired();
            b.Property(x => x.Position).IsRequired();
            b.Property(x => x.EnableCheckInReport).IsRequired(false);
            b.Property(x => x.EnableIssueItems).IsRequired(false);
            b.Property(x => x.EnableTags).IsRequired(false);

            b.HasOne(x => x.CarModels)
                .WithMany(x => x.CheckLists)
                    .HasForeignKey(x => x.CarModelId)
                        .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => x.CarModelId);
            b.HasIndex(x => x.Name);
        });

        builder.Entity<RadioOption>(b =>
        {
            b.ToTable(WorkShopManagementConsts.DbTablePrefix + "RadioOptions", WorkShopManagementConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.ListItemId).IsRequired();

            b.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(RadioOptionConsts.MaxNameLength);

            b.HasOne(x => x.ListItems)
                .WithMany(x => x.RadioOptions)
                .HasForeignKey(x => x.ListItemId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => x.ListItemId);
        });

        builder.Entity<ListItem>(b =>
        {
            b.ToTable(WorkShopManagementConsts.DbTablePrefix + "ListItems", WorkShopManagementConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Position).IsRequired();

            b.Property(x => x.Name).IsRequired().HasMaxLength(256);

            b.Property(x => x.CommentPlaceholder).IsRequired(false).HasMaxLength(512);

            b.Property(x => x.CommentType).IsRequired(false);

            b.Property(x => x.IsAttachmentRequired).IsRequired(false);

            b.Property(x => x.IsSeparator).IsRequired(false);


            b.HasOne(x => x.CheckLists)
                .WithMany(x => x.ListItems)
                .HasForeignKey(x => x.CheckListId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => x.CheckListId);
            b.HasIndex(x => new { x.CheckListId, x.Position });
        });

        builder.Entity<Car>(b =>
        {
            b.ToTable(WorkShopManagementConsts.DbTablePrefix + "Cars", WorkShopManagementConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.OwnerId).IsRequired();
            b.Property(x => x.ModelId).IsRequired();
            b.Property(x => x.Color).IsRequired().HasMaxLength(CarConsts.MaxColorLength);
            b.Property(x => x.ModelYear).IsRequired();
            b.Property(x => x.Stage).IsRequired();
            b.Property(x => x.Cnc).HasMaxLength(CarConsts.MaxCncLength);
            b.Property(x => x.CncFirewall).HasMaxLength(CarConsts.MaxCncFirewallLength);
            b.Property(x => x.CncColumn).HasMaxLength(CarConsts.MaxCncColumnLength);
            b.Property(x => x.Notes).HasMaxLength(CarConsts.MaxNotesLength);
            b.Property(x => x.MissingParts).HasMaxLength(CarConsts.MaxMissingPartsLength);

            b.Property(x => x.Vin)
                .IsRequired()
                .HasMaxLength(CarConsts.VinLength)
                .IsUnicode(false);

            b.HasIndex(x => x.Vin).IsUnique().HasFilter("[IsDeleted] = 0");

            b.HasOne(x => x.Owner)
                .WithMany()
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Model)
                .WithMany()
                .HasForeignKey(x => x.ModelId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.LogisticsDetail)
             .WithOne()                                     // or .WithOne(x => x.Car) if you add nav on LogisticsDetail
             .HasForeignKey<LogisticsDetail>(x => x.CarId)
             .OnDelete(DeleteBehavior.Cascade);

             });

        builder.Entity<CarOwner>(b =>
        {
            b.ToTable(WorkShopManagementConsts.DbTablePrefix + "CarOwners", WorkShopManagementConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(CarOwnerConsts.MaxNameLength);
            b.Property(x => x.Email).HasMaxLength(CarOwnerConsts.MaxEmailLength);
            b.Property(x => x.ContactId).HasMaxLength(CarOwnerConsts.MaxContactIdLength);

            b.HasIndex(x => x.Name);
        });

        builder.Entity<CheckInReport>(b =>
        {
            b.ToTable(WorkShopManagementConsts.DbTablePrefix + "CheckInReports", WorkShopManagementConsts.DbSchema);
            b.ConfigureByConvention();
 
            b.Property(x => x.Emission).HasMaxLength(CheckInReportConsts.MaxLength);
            b.Property(x => x.EngineNumber).HasMaxLength(CheckInReportConsts.MaxLength);
            b.Property(x => x.FrontMotorNumber).HasMaxLength(CheckInReportConsts.MaxLength);
            b.Property(x => x.RearMotorNumber).HasMaxLength(CheckInReportConsts.MaxLength);
            b.Property(x => x.TyreLabel).HasMaxLength(CheckInReportConsts.MaxLength);
            b.Property(x => x.ReportStatus).HasMaxLength(CheckInReportConsts.MaxLength);

            b.Property(x => x.CarId).IsRequired();
            b.HasOne(x => x.Car).WithOne().HasForeignKey<CheckInReport>(x => x.CarId).OnDelete(DeleteBehavior.Restrict);

        });
        builder.Entity<VinInfo>(b =>
        {
            b.ToTable(WorkShopManagementConsts.DbTablePrefix + "VinInfos", WorkShopManagementConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.VinNo).IsRequired();
            b.HasIndex(x => x.VinNo).IsUnique();
        });

        builder.Entity<Recall>(b =>
        {
            b.ToTable(WorkShopManagementConsts.DbTablePrefix + "Recalls", WorkShopManagementConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Title).IsRequired().HasMaxLength(RecallConsts.MaxTitleLength);
            b.Property(x => x.RiskDescription).HasMaxLength(RecallConsts.MaxRiskDescriptionLength);
            b.Property(x => x.Notes).HasMaxLength(RecallConsts.MaxNotesLength);
            b.Property(x => x.ManufactureId).HasMaxLength(RecallConsts.MaxManufactureIdLength);
            b.Property(x => x.Make).HasMaxLength(RecallConsts.MaxMakeLength);

            b.Property(x => x.Status).IsRequired();
            b.Property(x => x.Type).IsRequired();
            b.HasOne(x => x.Car)
                .WithMany(x => x.Recalls)
                .HasForeignKey(x => x.CarId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<CarBay>(b =>
        {
            b.ToTable(WorkShopManagementConsts.DbTablePrefix + "CarBays", WorkShopManagementConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.CarId).IsRequired();
            b.Property(x => x.BayId).IsRequired();
            b.Property(x => x.Priority).IsRequired();

            b.Property(x => x.BuildMaterialNumber);
            b.Property(x => x.PdiStatus);
            b.Property(x => x.ConfirmedDeliverDateNotes);
            b.Property(x => x.TransportDestination);
            b.Property(x => x.StorageLocation);
            b.Property(x => x.Row);
            b.Property(x => x.Columns);
            b.Property(x => x.PulseNumber);
            b.Property(x => x.ClockInTime);
            b.Property(x => x.ClockOutTime);
            b.Property(x => x.ClockInStatus);

            b.HasOne(x => x.Car)
                .WithMany(x => x.CarBays)
                .HasForeignKey(x => x.CarId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Bay)
                .WithMany(x => x.CarBays)
                .HasForeignKey(x => x.BayId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => x.CarId);
            b.HasIndex(x => x.BayId);
        });

        builder.Entity<CarBayItem>(b =>
        {
            b.ToTable(WorkShopManagementConsts.DbTablePrefix + "CarBayItems", WorkShopManagementConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.CarBayId).IsRequired();
            b.Property(x => x.CheckListItemId).IsRequired();

            b.Property(x => x.CheckRadioOption).IsRequired(false).HasMaxLength(CarBayItemConsts.MaxCheckRadioOptionLength);
            b.Property(x => x.Comments).IsRequired(false).HasMaxLength(CarBayItemConsts.MaxCommentsLength);


            b.HasOne(x => x.CarBay)
                .WithMany(x => x.CarBayItems)
                .HasForeignKey(x => x.CarBayId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.ListItem)
                .WithMany(x => x.CarBayItems)
                .HasForeignKey(x => x.CheckListItemId)
                .OnDelete(DeleteBehavior.Restrict);


            b.HasIndex(x => x.CarBayId);
            b.HasIndex(x => x.CheckListItemId);
        });


        builder.Entity<Issue>(b =>
        {
            b.ToTable(WorkShopManagementConsts.DbTablePrefix + "Issues", WorkShopManagementConsts.DbSchema, tb =>
            {
                tb.HasCheckConstraint("CK_Issues_SrNo_Range", "[SrNo] >= 1 AND [SrNo] <= 1000");
                tb.HasCheckConstraint("CK_Issues_XPercent_Range", "[XPercent] >= 0 AND [XPercent] <= 100");
                tb.HasCheckConstraint("CK_Issues_YPercent_Range", "[YPercent] >= 0 AND [YPercent] <= 100");
            });

            b.ConfigureByConvention();

            b.Property(x => x.SrNo).IsRequired();
            b.Property(x => x.Description).IsRequired().HasMaxLength(IssueConsts.MaxDescriptionLength);

            b.Property(x => x.RectificationAction).HasMaxLength(IssueConsts.MaxRectificationActionLength);
            b.Property(x => x.RectificationNotes).HasMaxLength(IssueConsts.MaxRectificationNotesLength);
            b.Property(x => x.QualityControlAction).HasMaxLength(IssueConsts.MaxQualityControlActionLength);
            b.Property(x => x.QualityControlNotes).HasMaxLength(IssueConsts.MaxQualityControlNotesLength);
            b.Property(x => x.RepairerAction).HasMaxLength(IssueConsts.MaxRepairerActionLength);
            b.Property(x => x.RepairerNotes).HasMaxLength(IssueConsts.MaxRepairerNotesLength);

            b.Property(x => x.XPercent).IsRequired().HasPrecision(6, 3);
            b.Property(x => x.YPercent).IsRequired().HasPrecision(6, 3);

            b.Property(x => x.Type).IsRequired();
            b.Property(x => x.Status).IsRequired();
            b.Property(x => x.OriginStage).IsRequired();
            b.Property(x => x.DeteriorationType).IsRequired();

            b.HasIndex(x => new { x.CarId, x.SrNo }).IsUnique().HasFilter("[IsDeleted] = 0");

            b.HasOne(x => x.Car)
                .WithMany()
                .HasForeignKey(x => x.CarId)
                .OnDelete(DeleteBehavior.Restrict);
        });


        builder.Entity<LogisticsDetail>(b =>
        {
            b.ToTable(WorkShopManagementConsts.DbTablePrefix + "LogisticsDetails", WorkShopManagementConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.CarId).IsRequired();

            // 1:1 with Car 
            b.HasIndex(x => x.CarId).IsUnique();

            b.Property(x => x.BookingNumber).HasMaxLength(LogisticsDetailConsts.MaxBookingNumberLength);
            b.Property(x => x.ClearingAgent).HasMaxLength(LogisticsDetailConsts.MaxClearingAgentLength);
            b.Property(x => x.ClearanceRemarks).HasMaxLength(LogisticsDetailConsts.MaxClearanceRemarksLength);
            b.Property(x => x.RvsaNumber).HasMaxLength(LogisticsDetailConsts.MaxRvsaNumberLength);

            b.Property(x => x.DeliverTo).HasMaxLength(LogisticsDetailConsts.MaxDeliverToLength);
            b.Property(x => x.DeliverNotes).HasMaxLength(LogisticsDetailConsts.MaxDeliverNotesLength);
            b.Property(x => x.TransportDestination).HasMaxLength(LogisticsDetailConsts.MaxTransportDestinationLength);

            //b.HasOne<Car>()
            //    .WithOne()                                      // you can change this if you add Car.LogisticsDetail navigation
            //    .HasForeignKey<LogisticsDetail>(x => x.CarId)
            //    .OnDelete(DeleteBehavior.Cascade);

            // Estimates relationship
            b.HasMany(x => x.ArrivalEstimates)
                .WithOne()
                .HasForeignKey(x => x.LogisticsDetailId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ArrivalEstimate>(b =>
        {
            b.ToTable(WorkShopManagementConsts.DbTablePrefix + "ArrivalEstimates", WorkShopManagementConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.LogisticsDetailId).IsRequired();

            b.Property(x => x.EtaPort).IsRequired();
            b.Property(x => x.EtaScd).IsRequired();

            b.Property(x => x.Notes).HasMaxLength(ArrivalEstimateConsts.MaxNotesLength);

            b.HasIndex(x => x.LogisticsDetailId);
        });

        builder.Entity<DataProtectionKey>(b =>
        {
            b.ToTable(WorkShopManagementConsts.DbTablePrefix + "DataProtectionKeys", WorkShopManagementConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasKey(x => x.Id);
        });
    }
}