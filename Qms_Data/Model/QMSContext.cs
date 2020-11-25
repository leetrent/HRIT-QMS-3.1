using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace QmsCore.Model
{
    public partial class QMSContext : DbContext
    {
        public QMSContext()
        {
        }

        public QMSContext(DbContextOptions<QMSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<NtfEmaillog> NtfEmaillog { get; set; }
        public virtual DbSet<NtfNotification> NtfNotification { get; set; }
        public virtual DbSet<NtfNotificationevent> NtfNotificationevent { get; set; }
        public virtual DbSet<NtfNotificationeventtype> NtfNotificationeventtype { get; set; }
        public virtual DbSet<NtfNotificationuserpreference> NtfNotificationuserpreference { get; set; }
        public virtual DbSet<QmsCorrectiveactionErrortype> QmsCorrectiveactionErrortype { get; set; }
        public virtual DbSet<QmsCorrectiveactionrequest> QmsCorrectiveactionrequest { get; set; }
        public virtual DbSet<QmsCorrectiveactiontype> QmsCorrectiveactiontype { get; set; }
        public virtual DbSet<QmsDataerror> QmsDataerror { get; set; }
        public virtual DbSet<QmsDataerrortype> QmsDataerrortype { get; set; }
        public virtual DbSet<QmsDataItem> QmsDataItem { get; set; }
        public virtual DbSet<QmsEmployee> QmsEmployee { get; set; }
        public virtual DbSet<QmsErrorroutingtype> QmsErrorroutingtype { get; set; }
        public virtual DbSet<QmsErrortype> QmsErrortype { get; set; }
        public virtual DbSet<QmsKnowledgebase> QmsKnowledgebase { get; set; }
        public virtual DbSet<QmsMasterErrorList> QmsMasterErrorList { get; set; }
        public virtual DbSet<QmsNatureofaction> QmsNatureofaction { get; set; }
        public virtual DbSet<QmsOrgStatusTrans> QmsOrgStatusTrans { get; set; }
        public virtual DbSet<QmsOrgtype> QmsOrgtype { get; set; }
        public virtual DbSet<QmsPersonnelOfficeIdentifier> QmsPersonnelOfficeIdentifier { get; set; }
        public virtual DbSet<QmsStatus> QmsStatus { get; set; }
        public virtual DbSet<QmsStatusTrans> QmsStatusTrans { get; set; }
        public virtual DbSet<QmsWorkitemcomment> QmsWorkitemcomment { get; set; }
        public virtual DbSet<QmsWorkitemfile> QmsWorkitemfile { get; set; }
        public virtual DbSet<QmsWorkitemhistory> QmsWorkitemhistory { get; set; }
        public virtual DbSet<QmsWorkitemtype> QmsWorkitemtype { get; set; }
        public virtual DbSet<QmsWorkitemviewlog> QmsWorkitemviewlog { get; set; }
        public virtual DbSet<SecOrg> SecOrg { get; set; }
        public virtual DbSet<SecPermission> SecPermission { get; set; }
        public virtual DbSet<SecRole> SecRole { get; set; }
        public virtual DbSet<SecRolePermission> SecRolePermission { get; set; }
        public virtual DbSet<SecSecurityitemtype> SecSecurityitemtype { get; set; }
        public virtual DbSet<SecSecuritylog> SecSecuritylog { get; set; }
        public virtual DbSet<SecSecuritylogtype> SecSecuritylogtype { get; set; }
        public virtual DbSet<SecUser> SecUser { get; set; }
        public virtual DbSet<SecUserlogin> SecUserlogin { get; set; }
        public virtual DbSet<SecUserRole> SecUserRole { get; set; }
        public virtual DbSet<SysMenuitem> SysMenuitem { get; set; }
        public virtual DbSet<SysModule> SysModule { get; set; }
        public virtual DbSet<SysModuleRole> SysModuleRole { get; set; }
        public virtual DbSet<SysSetting> SysSetting { get; set; }
        public virtual DbSet<SysSettingtype> SysSettingtype { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySQL(Config.Settings.ReconDB);
//                optionsBuilder.UseMySql(Config.Settings.ReconDB);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NtfEmaillog>(entity =>
            {
                entity.HasKey(e => e.EmailLogId);

                entity.ToTable("ntf_emaillog", "aca");

                entity.HasIndex(e => e.SentDate)
                    .HasName("ntf_emaillog_uc")
                    .IsUnique();

                entity.Property(e => e.EmailLogId).HasColumnType("int(10)");

                entity.Property(e => e.SentAmount)
                    .HasColumnType("int(10)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.SentDate)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<NtfNotification>(entity =>
            {
                entity.HasKey(e => e.NotificationId);

                entity.ToTable("ntf_notification", "aca");

                entity.HasIndex(e => e.NotificationEventId)
                    .HasName("ntf_Notification_notificationEvent_fk");

                entity.HasIndex(e => e.UserId)
                    .HasName("ntf_Notification_secuser_fk");

                entity.HasIndex(e => e.WorkItemTypeCode)
                    .HasName("qms_notification_workItemType_fk");

                entity.Property(e => e.NotificationId)
                    .HasColumnName("Notification_Id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.HasBeenRead)
                    .HasColumnType("tinyint(1)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasMaxLength(2500)
                    .IsUnicode(false);

                entity.Property(e => e.NotificationEventId)
                    .HasColumnName("NotificationEvent_Id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.SendAsEmail)
                    .HasColumnType("tinyint(1)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.WorkItemTypeCode)
                    .HasColumnName("WorkItemType_Code")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.WorkitemId)
                    .HasColumnName("workitem_id")
                    .HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.NotificationEvent)
                    .WithMany(p => p.NtfNotification)
                    .HasForeignKey(d => d.NotificationEventId)
                    .HasConstraintName("ntf_Notification_notificationEvent_fk");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.NtfNotification)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("ntf_Notification_secuser_fk");

                entity.HasOne(d => d.WorkItemTypeCodeNavigation)
                    .WithMany(p => p.NtfNotification)
                    .HasPrincipalKey(p => p.WorkItemTypeCode)
                    .HasForeignKey(d => d.WorkItemTypeCode)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("qms_notification_workItemType_fk");
            });

            modelBuilder.Entity<NtfNotificationevent>(entity =>
            {
                entity.HasKey(e => e.NotificationEventId);

                entity.ToTable("ntf_notificationevent", "aca");

                entity.HasIndex(e => e.NotificationEventTypeId)
                    .HasName("ntf_NotificationEvent_notificationEventType_fk");

                entity.Property(e => e.NotificationEventId)
                    .HasColumnName("NotificationEvent_Id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.MessageTemplate)
                    .HasMaxLength(2000)
                    .IsUnicode(false)
                    .HasDefaultValueSql("Corrective Acton ID: {0}<br/>Updated on: {1}<br/>Employee: {2}-{3}");

                entity.Property(e => e.NotificationEventCode)
                    .IsRequired()
                    .HasColumnName("NotificationEvent_Code")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.NotificationEventLabel)
                    .IsRequired()
                    .HasColumnName("NotificationEvent_Label")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NotificationEventTypeId)
                    .HasColumnName("NotificationEventType_Id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.TitleTemplate)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasDefaultValueSql("HRQMS - Corrective Action {0} ({1})");

                entity.HasOne(d => d.NotificationEventType)
                    .WithMany(p => p.NtfNotificationevent)
                    .HasForeignKey(d => d.NotificationEventTypeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("ntf_NotificationEvent_notificationEventType_fk");
            });

            modelBuilder.Entity<NtfNotificationeventtype>(entity =>
            {
                entity.HasKey(e => e.NotificationEventTypeId);

                entity.ToTable("ntf_notificationeventtype", "aca");

                entity.Property(e => e.NotificationEventTypeId)
                    .HasColumnName("NotificationEventType_Id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.NotificationEventTypeCode)
                    .IsRequired()
                    .HasColumnName("NotificationEventType_Code")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.NotificationEventTypeLabel)
                    .IsRequired()
                    .HasColumnName("NotificationEventType_Label")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<NtfNotificationuserpreference>(entity =>
            {
                entity.HasKey(e => e.NotificationUserPreferenceId);

                entity.ToTable("ntf_notificationuserpreference", "aca");

                entity.HasIndex(e => e.NotificationEventId)
                    .HasName("ntf_NotificationUserPreference_notificationEvent_fk");

                entity.HasIndex(e => e.UserId)
                    .HasName("ntf_NotificationUserPreference_secuser_fk");

                entity.Property(e => e.NotificationUserPreferenceId)
                    .HasColumnName("NotificationUserPreference_Id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.CanBeTurnedOffByUser)
                    .HasColumnType("tinyint(1)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.MessageDeliveryIsOn)
                    .HasColumnType("tinyint(1)")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.NotificationEventId)
                    .HasColumnName("NotificationEvent_Id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.NotificationEvent)
                    .WithMany(p => p.NtfNotificationuserpreference)
                    .HasForeignKey(d => d.NotificationEventId)
                    .HasConstraintName("ntf_NotificationUserPreference_notificationEvent_fk");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.NtfNotificationuserpreference)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("ntf_NotificationUserPreference_secuser_fk");
            });

            modelBuilder.Entity<QmsCorrectiveactionErrortype>(entity =>
            {
                entity.ToTable("qms_correctiveaction_errortype", "aca");

                entity.HasIndex(e => e.CorrectiveActionId)
                    .HasName("qms_correctiveaction_car_fk");

                entity.HasIndex(e => e.ErrorTypeId)
                    .HasName("qms_correctiveaction_error_fk");

                entity.Property(e => e.Id).HasColumnType("int(10)");

                entity.Property(e => e.CorrectiveActionId).HasColumnType("int(10)");

                entity.Property(e => e.ErrorTypeId).HasColumnType("int(10)");

                entity.HasOne(d => d.CorrectiveAction)
                    .WithMany(p => p.QmsCorrectiveactionErrortype)
                    .HasForeignKey(d => d.CorrectiveActionId)
                    .HasConstraintName("qms_correctiveaction_car_fk");

                entity.HasOne(d => d.ErrorType)
                    .WithMany(p => p.QmsCorrectiveactionErrortype)
                    .HasForeignKey(d => d.ErrorTypeId)
                    .HasConstraintName("qms_correctiveaction_error_fk");
            });

            modelBuilder.Entity<QmsCorrectiveactionrequest>(entity =>
            {
                entity.ToTable("qms_correctiveactionrequest", "aca");

                entity.HasIndex(e => e.ActionRequestTypeId)
                    .HasName("qms_correctiveaction_requestype_fk");

                entity.HasIndex(e => e.AssignedByUserId)
                    .HasName("qms_correctiveactionrequest_assigner_fk");

                entity.HasIndex(e => e.AssignedToOrgId)
                    .HasName("qms_correctiveactionrequest_org_fk");

                entity.HasIndex(e => e.AssignedToUserId)
                    .HasName("qms_correctiveactionrequest_assignee_fk");

                entity.HasIndex(e => e.CreatedAtOrgId)
                    .HasName("qms_correctiveactionrequest_createdatorg_fk");

                entity.HasIndex(e => e.CreatedByUserId)
                    .HasName("qms_correctiveactionrequest_createdbyuser_fk");

                entity.HasIndex(e => e.EmplId)
                    .HasName("qms_correctiveactionrequest_employee_fk");

                entity.HasIndex(e => e.NatureOfAction)
                    .HasName("qms_correctiveaction_noa_fk");

                entity.HasIndex(e => e.StatusId)
                    .HasName("qms_correctiveactionrequest_status_fk");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.ActionRequestTypeId).HasColumnType("int(10)");

                entity.Property(e => e.AssignedByUserId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.AssignedToOrgId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.AssignedToUserId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.CreatedAtOrgId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.CreatedByUserId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.Details)
                    .HasMaxLength(2000)
                    .IsUnicode(false);

                entity.Property(e => e.EffectiveDateOfPar)
                    .HasColumnName("EffectiveDateOfPAR")
                    .HasColumnType("date");

                entity.Property(e => e.EmplId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.IsPaymentMismatch)
                    .HasColumnType("char(1)")
                    .HasDefaultValueSql("N");

                entity.Property(e => e.NatureOfAction)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.PareffectiveDate)
                    .HasColumnName("PAREffectiveDate")
                    .HasColumnType("date");

                entity.Property(e => e.RowVersion)
                    .HasColumnType("tinyint(2)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.StatusId)
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValueSql("1");

                entity.HasOne(d => d.ActionRequestType)
                    .WithMany(p => p.QmsCorrectiveactionrequest)
                    .HasForeignKey(d => d.ActionRequestTypeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("qms_correctiveaction_requestype_fk");

                entity.HasOne(d => d.AssignedByUser)
                    .WithMany(p => p.QmsCorrectiveactionrequestAssignedByUser)
                    .HasForeignKey(d => d.AssignedByUserId)
                    .HasConstraintName("qms_correctiveactionrequest_assigner_fk");

                entity.HasOne(d => d.AssignedToOrg)
                    .WithMany(p => p.QmsCorrectiveactionrequestAssignedToOrg)
                    .HasForeignKey(d => d.AssignedToOrgId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("qms_correctiveactionrequest_org_fk");

                entity.HasOne(d => d.AssignedToUser)
                    .WithMany(p => p.QmsCorrectiveactionrequestAssignedToUser)
                    .HasForeignKey(d => d.AssignedToUserId)
                    .HasConstraintName("qms_correctiveactionrequest_assignee_fk");

                entity.HasOne(d => d.CreatedAtOrg)
                    .WithMany(p => p.QmsCorrectiveactionrequestCreatedAtOrg)
                    .HasForeignKey(d => d.CreatedAtOrgId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("qms_correctiveactionrequest_createdatorg_fk");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.QmsCorrectiveactionrequestCreatedByUser)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .HasConstraintName("qms_correctiveactionrequest_createdbyuser_fk");

                entity.HasOne(d => d.Empl)
                    .WithMany(p => p.QmsCorrectiveactionrequest)
                    .HasForeignKey(d => d.EmplId)
                    .HasConstraintName("qms_correctiveactionrequest_employee_fk");

                entity.HasOne(d => d.NatureOfActionNavigation)
                    .WithMany(p => p.QmsCorrectiveactionrequest)
                    .HasForeignKey(d => d.NatureOfAction)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("qms_correctiveaction_noa_fk");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.QmsCorrectiveactionrequest)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("qms_correctiveactionrequest_status_fk");
            });

            modelBuilder.Entity<QmsCorrectiveactiontype>(entity =>
            {
                entity.ToTable("qms_correctiveactiontype", "aca");

                entity.Property(e => e.Id).HasColumnType("int(10)");

                entity.Property(e => e.Code)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

                entity.Property(e => e.Label)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<QmsDataerror>(entity =>
            {
                entity.HasKey(e => e.DataErrorId);

                entity.ToTable("qms_dataerror", "aca");

                entity.HasIndex(e => e.AssignedByUserId)
                    .HasName("qms_DataError_AssignedByUser_fk");

                entity.HasIndex(e => e.AssignedToOrgId)
                    .HasName("qms_DataError_AssignedToOrganization_fk");

                entity.HasIndex(e => e.AssignedToUserId)
                    .HasName("qms_DataError_AssignedToUser_fk");

                entity.HasIndex(e => e.CorrectiveActionId)
                    .HasName("qms_DataError_car_fk");

                entity.HasIndex(e => e.CreatedByOrgId)
                    .HasName("qms_DataError_CreatedByOrganization_fk");

                entity.HasIndex(e => e.CreatedByUserId)
                    .HasName("qms_DataError_CreatedByUser_fk");

                entity.HasIndex(e => e.DataErrorKey)
                    .HasName("qms_DataErrorKey_ix");

                entity.HasIndex(e => e.Emplid)
                    .HasName("qms_DataError_employee_fk");

                entity.HasIndex(e => e.ErrorListId)
                    .HasName("qms_DataError_errorlist_fk");

                entity.HasIndex(e => e.StatusId)
                    .HasName("qms_DataError_status_fk");

                entity.Property(e => e.DataErrorId)
                    .HasColumnName("DataError_Id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.AssignedAt).HasColumnName("Assigned_at");

                entity.Property(e => e.AssignedByUserId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.AssignedToOrgId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.AssignedToUserId)
                    .HasColumnName("AssignedToUserID")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.CorrectiveActionId)
                    .HasColumnName("correctiveActionId")
                    .HasColumnType("int(10)");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.CreatedByOrgId)
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.CreatedByUserId)
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.DataErrorKey)
                    .IsRequired()
                    .HasColumnName("DataError_Key")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

                entity.Property(e => e.Details)
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                entity.Property(e => e.Emplid)
                    .IsRequired()
                    .HasColumnName("emplid")
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.ErrorListId)
                    .HasColumnName("error_list_id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.QmsErrorCode)
                    .IsRequired()
                    .HasColumnName("qms_error_code")
                    .HasMaxLength(11)
                    .IsUnicode(false);

                entity.Property(e => e.QmsErrorMessageText)
                    .HasColumnName("qms_error_message_text")
                    .IsUnicode(false);

                entity.Property(e => e.ResolvedAt).HasColumnName("resolved_at");

                entity.Property(e => e.RowVersion)
                    .HasColumnType("tinyint(2)")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.StatusId)
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.SubmittedAt).HasColumnName("submitted_at");

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.HasOne(d => d.AssignedByUser)
                    .WithMany(p => p.QmsDataerrorAssignedByUser)
                    .HasForeignKey(d => d.AssignedByUserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("qms_DataError_AssignedByUser_fk");

                entity.HasOne(d => d.AssignedToOrg)
                    .WithMany(p => p.QmsDataerrorAssignedToOrg)
                    .HasForeignKey(d => d.AssignedToOrgId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("qms_DataError_AssignedToOrganization_fk");

                entity.HasOne(d => d.AssignedToUser)
                    .WithMany(p => p.QmsDataerrorAssignedToUser)
                    .HasForeignKey(d => d.AssignedToUserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("qms_DataError_AssignedToUser_fk");

                entity.HasOne(d => d.CorrectiveAction)
                    .WithMany(p => p.QmsDataerror)
                    .HasForeignKey(d => d.CorrectiveActionId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("qms_DataError_car_fk");

                entity.HasOne(d => d.CreatedByOrg)
                    .WithMany(p => p.QmsDataerrorCreatedByOrg)
                    .HasForeignKey(d => d.CreatedByOrgId)
                    .HasConstraintName("qms_DataError_CreatedByOrganization_fk");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.QmsDataerrorCreatedByUser)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("qms_DataError_CreatedByUser_fk");

                entity.HasOne(d => d.Empl)
                    .WithMany(p => p.QmsDataerror)
                    .HasForeignKey(d => d.Emplid)
                    .HasConstraintName("qms_DataError_employee_fk");

                entity.HasOne(d => d.ErrorList)
                    .WithMany(p => p.QmsDataerror)
                    .HasForeignKey(d => d.ErrorListId)
                    .HasConstraintName("qms_DataError_errorlist_fk");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.QmsDataerror)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("qms_DataError_status_fk");
            });

            modelBuilder.Entity<QmsDataerrortype>(entity =>
            {
                entity.HasKey(e => e.DataRoutingTypeId);

                entity.ToTable("qms_dataerrortype", "aca");

                entity.Property(e => e.DataRoutingTypeId)
                    .HasColumnName("dataRoutingType_Id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.DataRoutingTypeCode)
                    .IsRequired()
                    .HasColumnName("dataRoutingType_Code")
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.DataRoutingTypeLabel)
                    .IsRequired()
                    .HasColumnName("dataRoutingType_Label")
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<QmsDataItem>(entity =>
            {
                entity.HasKey(e => e.DataItemId);

                entity.ToTable("qms_data_item", "aca");

                entity.Property(e => e.DataItemId)
                    .HasColumnName("data_item_Id")
                    .HasColumnType("int(10)")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.DataItemCategory)
                    .IsRequired()
                    .HasColumnName("data_item_category")
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.DataItemName)
                    .IsRequired()
                    .HasColumnName("data_item_name")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

                entity.Property(e => e.SystemName)
                    .IsRequired()
                    .HasColumnName("system_name")
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<QmsEmployee>(entity =>
            {
                entity.HasKey(e => e.EmplId);

                entity.ToTable("qms_employee", "aca");

                entity.HasIndex(e => e.ManagerId)
                    .HasName("idx_qms_employee_ManagerId");

                entity.HasIndex(e => e.UserKey)
                    .HasName("UserKey_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.EmplId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.AgencySubElement)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.DepartmentId)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.EmailAddress)
                    .HasMaxLength(70)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Grade)
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ManagerId)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.MiddleName)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.PayPlan)
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.PersonnelOfficeIdentifier)
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.UserKey)
                    .IsRequired()
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasDefaultValueSql("--");
            });

            modelBuilder.Entity<QmsErrorroutingtype>(entity =>
            {
                entity.HasKey(e => e.ErrorRoutingTypeId);

                entity.ToTable("qms_errorroutingtype", "aca");

                entity.Property(e => e.ErrorRoutingTypeId)
                    .HasColumnName("errorRoutingType_Id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

                entity.Property(e => e.ErrorRoutingTypeCode)
                    .IsRequired()
                    .HasColumnName("errorRoutingType_Code")
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.ErrorRoutingTypeLabel)
                    .IsRequired()
                    .HasColumnName("errorRoutingType_Label")
                    .HasMaxLength(35)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<QmsErrortype>(entity =>
            {
                entity.ToTable("qms_errortype", "aca");

                entity.Property(e => e.Id).HasColumnType("int(10)");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.DisplayOrder).HasColumnType("tinyint(4)");

                entity.Property(e => e.RoutesToBr)
                    .IsRequired()
                    .HasColumnName("RoutesToBR")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("N");
            });

            modelBuilder.Entity<QmsKnowledgebase>(entity =>
            {
                entity.HasKey(e => e.ItemId);

                entity.ToTable("qms_knowledgebase", "aca");

                entity.Property(e => e.ItemId)
                    .HasColumnName("ItemID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Comment)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.CorrectionComplexity)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ErrorCode)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ErrorDescription)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ErrorType)
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.Hrsupport)
                    .HasColumnName("HRSupport")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Impact)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Risk)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ShortDescription)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SupportingDocLink)
                    .HasMaxLength(75)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<QmsMasterErrorList>(entity =>
            {
                entity.HasKey(e => e.ErrorListId);

                entity.ToTable("qms_master_error_list", "aca");

                entity.HasIndex(e => e.DataItemId)
                    .HasName("qms_error_loader_master_DataItem_fk");

                entity.HasIndex(e => e.DataRoutingTypeId)
                    .HasName("qms_error_loader_master_DataErrorType_fk");

                entity.HasIndex(e => e.ErrorRoutingTypeId)
                    .HasName("qms_error_loader_master_ErrorRoutingType_fk");

                entity.Property(e => e.ErrorListId)
                    .HasColumnName("error_list_id")
                    .HasColumnType("int(10)")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.DataItemId)
                    .HasColumnName("data_item_id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.DataRoutingTypeId)
                    .HasColumnName("dataRoutingTypeId")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

                entity.Property(e => e.ErrorMessageText)
                    .IsRequired()
                    .HasColumnName("error_message_text")
                    .IsUnicode(false);

                entity.Property(e => e.ErrorRoutingTypeId)
                    .HasColumnName("errorRoutingTypeId")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.HrdwDataLoadEnabled)
                    .IsRequired()
                    .HasColumnName("hrdw_data_load_enabled")
                    .HasColumnType("char(1)");

                entity.Property(e => e.QmsDataLoadEnabled)
                    .IsRequired()
                    .HasColumnName("qms_data_load_enabled")
                    .HasColumnType("char(1)")
                    .HasDefaultValueSql("Y");

                entity.Property(e => e.QmsErrorCode)
                    .IsRequired()
                    .HasColumnName("qms_error_code")
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.HasOne(d => d.DataItem)
                    .WithMany(p => p.QmsMasterErrorList)
                    .HasForeignKey(d => d.DataItemId)
                    .HasConstraintName("qms_error_loader_master_DataItem_fk");

                entity.HasOne(d => d.DataRoutingType)
                    .WithMany(p => p.QmsMasterErrorList)
                    .HasForeignKey(d => d.DataRoutingTypeId)
                    .HasConstraintName("qms_error_loader_master_DataErrorType_fk");

                entity.HasOne(d => d.ErrorRoutingType)
                    .WithMany(p => p.QmsMasterErrorList)
                    .HasForeignKey(d => d.ErrorRoutingTypeId)
                    .HasConstraintName("qms_error_loader_master_ErrorRoutingType_fk");
            });

            modelBuilder.Entity<QmsNatureofaction>(entity =>
            {
                entity.HasKey(e => e.Noacode);

                entity.ToTable("qms_natureofaction", "aca");

                entity.Property(e => e.Noacode)
                    .HasColumnName("NOACode")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.RoutesToBr)
                    .HasColumnName("RoutesToBR")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("N");

                entity.Property(e => e.ShortDescription)
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.Property(e => e.ValidFrom).HasColumnType("date");

                entity.Property(e => e.ValidTo).HasColumnType("date");
            });

            modelBuilder.Entity<QmsOrgStatusTrans>(entity =>
            {
                entity.HasKey(e => e.OrgStatusTransId);

                entity.ToTable("qms_org_status_trans", "aca");

                entity.HasIndex(e => e.ToOrgtypeId)
                    .HasName("qms_org_status_trans_fk2");

                entity.HasIndex(e => e.WorkItemTypeCode)
                    .HasName("qms_org_status_trans_workItemType_fk");

                entity.HasIndex(e => new { e.StatusTransId, e.FromOrgId, e.ToOrgtypeId, e.WorkItemTypeCode })
                    .HasName("qms_org_status_trans_status_wtk_idx")
                    .IsUnique();

                entity.Property(e => e.OrgStatusTransId)
                    .HasColumnName("org_status_trans_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

                entity.Property(e => e.FromOrgId)
                    .HasColumnName("from_org_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.StatusTransId)
                    .HasColumnName("status_trans_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.ToOrgtypeId)
                    .HasColumnName("to_orgtype_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.Property(e => e.WorkItemTypeCode)
                    .IsRequired()
                    .HasColumnName("WorkItemType_Code")
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("CorrectiveActionRequest");

                entity.HasOne(d => d.StatusTrans)
                    .WithMany(p => p.QmsOrgStatusTrans)
                    .HasForeignKey(d => d.StatusTransId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("qms_org_status_trans_fk1");

                entity.HasOne(d => d.ToOrgtype)
                    .WithMany(p => p.QmsOrgStatusTrans)
                    .HasForeignKey(d => d.ToOrgtypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("qms_org_status_trans_fk2");

                entity.HasOne(d => d.WorkItemTypeCodeNavigation)
                    .WithMany(p => p.QmsOrgStatusTrans)
                    .HasPrincipalKey(p => p.WorkItemTypeCode)
                    .HasForeignKey(d => d.WorkItemTypeCode)
                    .HasConstraintName("qms_org_status_trans_workItemType_fk");
            });

            modelBuilder.Entity<QmsOrgtype>(entity =>
            {
                entity.HasKey(e => e.OrgtypeId);

                entity.ToTable("qms_orgtype", "aca");

                entity.HasIndex(e => e.OrgtypeCode)
                    .HasName("qms_orgtype_uk2")
                    .IsUnique();

                entity.HasIndex(e => e.OrgtypeLabel)
                    .HasName("qms_orgtype_uk1")
                    .IsUnique();

                entity.Property(e => e.OrgtypeId)
                    .HasColumnName("orgtype_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

                entity.Property(e => e.OrgtypeCode)
                    .IsRequired()
                    .HasColumnName("orgtype_code")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.OrgtypeLabel)
                    .IsRequired()
                    .HasColumnName("orgtype_label")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<QmsPersonnelOfficeIdentifier>(entity =>
            {
                entity.HasKey(e => e.PoiId);

                entity.ToTable("qms_personnel_office_identifier", "aca");

                entity.HasIndex(e => e.OrgId)
                    .HasName("qms_poi_org_fk");

                entity.Property(e => e.PoiId)
                    .HasColumnName("poi_Id")
                    .HasColumnType("int(4)")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

                entity.Property(e => e.OrgId)
                    .HasColumnName("OrgID")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.PoiCode)
                    .IsRequired()
                    .HasColumnName("poi_code")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.PoiLabel)
                    .IsRequired()
                    .HasColumnName("poi_label")
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.HasOne(d => d.Org)
                    .WithMany(p => p.QmsPersonnelOfficeIdentifier)
                    .HasForeignKey(d => d.OrgId)
                    .HasConstraintName("qms_poi_org_fk");
            });

            modelBuilder.Entity<QmsStatus>(entity =>
            {
                entity.HasKey(e => e.StatusId);

                entity.ToTable("qms_status", "aca");

                entity.HasIndex(e => e.StatusCode)
                    .HasName("qms_status_uk1")
                    .IsUnique();

                entity.HasIndex(e => e.StatusLabel)
                    .HasName("qms_role_uk2")
                    .IsUnique();

                entity.Property(e => e.StatusId)
                    .HasColumnName("status_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

                entity.Property(e => e.DisplayOrder)
                    .HasColumnName("display_order")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.StatusCode)
                    .IsRequired()
                    .HasColumnName("status_code")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.StatusLabel)
                    .IsRequired()
                    .HasColumnName("status_label")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<QmsStatusTrans>(entity =>
            {
                entity.HasKey(e => e.StatusTransId);

                entity.ToTable("qms_status_trans", "aca");

                entity.HasIndex(e => e.ToStatusId)
                    .HasName("qms_status_trans_fk2");

                entity.HasIndex(e => new { e.FromStatusId, e.ToStatusId })
                    .HasName("qms_status_trans_uk1")
                    .IsUnique();

                entity.Property(e => e.StatusTransId)
                    .HasColumnName("status_trans_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

                entity.Property(e => e.FromStatusId)
                    .HasColumnName("from_status_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.StatusTransCode)
                    .IsRequired()
                    .HasColumnName("status_trans_code")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.StatusTransLabel)
                    .IsRequired()
                    .HasColumnName("status_trans_label")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ToStatusId)
                    .HasColumnName("to_status_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.HasOne(d => d.FromStatus)
                    .WithMany(p => p.QmsStatusTransFromStatus)
                    .HasForeignKey(d => d.FromStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("qms_status_trans_fk1");

                entity.HasOne(d => d.ToStatus)
                    .WithMany(p => p.QmsStatusTransToStatus)
                    .HasForeignKey(d => d.ToStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("qms_status_trans_fk2");
            });

            modelBuilder.Entity<QmsWorkitemcomment>(entity =>
            {
                entity.ToTable("qms_workitemcomment", "aca");

                entity.HasIndex(e => e.AuthorId)
                    .HasName("qms_reconworkitemcomment_user_fk");

                entity.HasIndex(e => e.WorkItemTypeCode)
                    .HasName("qms_workitemcomment_workItemType_fk");

                entity.HasIndex(e => new { e.WorkItemId, e.WorkItemTypeCode })
                    .HasName("qms_workitemtypeandid_ix");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.AuthorId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.CreatedAt).HasColumnName("createdAt");

                entity.Property(e => e.DeletedAt).HasColumnName("deletedAt");

                entity.Property(e => e.Message)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnName("updatedAt");

                entity.Property(e => e.WorkItemId).HasColumnType("int(11)");

                entity.Property(e => e.WorkItemTypeCode)
                    .HasColumnName("WorkItemType_Code")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.QmsWorkitemcomment)
                    .HasForeignKey(d => d.AuthorId)
                    .HasConstraintName("qms_workitemauthor_fk");

                entity.HasOne(d => d.WorkItemTypeCodeNavigation)
                    .WithMany(p => p.QmsWorkitemcomment)
                    .HasPrincipalKey(p => p.WorkItemTypeCode)
                    .HasForeignKey(d => d.WorkItemTypeCode)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("qms_workitemcomment_workItemType_fk");
            });

            modelBuilder.Entity<QmsWorkitemfile>(entity =>
            {
                entity.ToTable("qms_workitemfile", "aca");

                entity.HasIndex(e => e.UploadedByUserId)
                    .HasName("qms_workitemfile_uploader_fk");

                entity.HasIndex(e => e.WorkItemTypeCode)
                    .HasName("qms_workitemfile_workItemType_fk");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Createdat).HasColumnName("createdat");

                entity.Property(e => e.Deletedat).HasColumnName("deletedat");

                entity.Property(e => e.Filepath)
                    .IsRequired()
                    .HasColumnName("filepath")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Filetype)
                    .IsRequired()
                    .HasColumnName("filetype")
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.UploadedByUserId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.WorkItemId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.WorkItemTypeCode)
                    .HasColumnName("WorkItemType_Code")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.UploadedByUser)
                    .WithMany(p => p.QmsWorkitemfile)
                    .HasForeignKey(d => d.UploadedByUserId)
                    .HasConstraintName("qms_workitemfile_uploader_fk");

                entity.HasOne(d => d.WorkItemTypeCodeNavigation)
                    .WithMany(p => p.QmsWorkitemfile)
                    .HasPrincipalKey(p => p.WorkItemTypeCode)
                    .HasForeignKey(d => d.WorkItemTypeCode)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("qms_workitemfile_workItemType_fk");
            });

            modelBuilder.Entity<QmsWorkitemhistory>(entity =>
            {
                entity.ToTable("qms_workitemhistory", "aca");

                entity.HasIndex(e => e.ActionTakenByUserId)
                    .HasName("qms_WorkItemHistory_secuser_fk");

                entity.HasIndex(e => e.PreviousAssignedByUserId)
                    .HasName("qms_WorkItemHistory_assigner_fk");

                entity.HasIndex(e => e.PreviousAssignedToOrgId)
                    .HasName("qms_WorkItemHistory_org_fk");

                entity.HasIndex(e => e.PreviousAssignedtoUserId)
                    .HasName("qms_WorkItemHistory_assignee_fk");

                entity.HasIndex(e => e.PreviousStatusId)
                    .HasName("qms_WorkItemHistory_status_fk_idx");

                entity.HasIndex(e => e.WorkItemTypeCode)
                    .HasName("qms_workitemhistory_workItemType_fk");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.ActionDescription)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.ActionTakenByUserId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.PreviousAssignedByUserId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.PreviousAssignedToOrgId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.PreviousAssignedtoUserId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.PreviousStatusId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.WorkItemId).HasColumnType("int(11)");

                entity.Property(e => e.WorkItemTypeCode)
                    .HasColumnName("WorkItemType_Code")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.ActionTakenByUser)
                    .WithMany(p => p.QmsWorkitemhistoryActionTakenByUser)
                    .HasForeignKey(d => d.ActionTakenByUserId)
                    .HasConstraintName("qms_WorkItemHistory_secuser_fk");

                entity.HasOne(d => d.PreviousAssignedByUser)
                    .WithMany(p => p.QmsWorkitemhistoryPreviousAssignedByUser)
                    .HasForeignKey(d => d.PreviousAssignedByUserId)
                    .HasConstraintName("qms_WorkItemHistory_assigner_fk");

                entity.HasOne(d => d.PreviousAssignedToOrg)
                    .WithMany(p => p.QmsWorkitemhistory)
                    .HasForeignKey(d => d.PreviousAssignedToOrgId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("qms_WorkItemHistory_org_fk");

                entity.HasOne(d => d.PreviousAssignedtoUser)
                    .WithMany(p => p.QmsWorkitemhistoryPreviousAssignedtoUser)
                    .HasForeignKey(d => d.PreviousAssignedtoUserId)
                    .HasConstraintName("qms_WorkItemHistory_assignee_fk");

                entity.HasOne(d => d.PreviousStatus)
                    .WithMany(p => p.QmsWorkitemhistory)
                    .HasForeignKey(d => d.PreviousStatusId)
                    .HasConstraintName("qms_WorkItemHistory_status_fk");

                entity.HasOne(d => d.WorkItemTypeCodeNavigation)
                    .WithMany(p => p.QmsWorkitemhistory)
                    .HasPrincipalKey(p => p.WorkItemTypeCode)
                    .HasForeignKey(d => d.WorkItemTypeCode)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("qms_workitemhistory_workItemType_fk");
            });

            modelBuilder.Entity<QmsWorkitemtype>(entity =>
            {
                entity.HasKey(e => e.WorkItemTypeId);

                entity.ToTable("qms_workitemtype", "aca");

                entity.HasIndex(e => e.WorkItemTypeCode)
                    .HasName("WorkItemType_Code")
                    .IsUnique();

                entity.Property(e => e.WorkItemTypeId)
                    .HasColumnName("WorkItemType_Id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.ControllerName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.MethodName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.WorkItemTypeCode)
                    .IsRequired()
                    .HasColumnName("WorkItemType_Code")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.WorkItemTypeLabel)
                    .IsRequired()
                    .HasColumnName("WorkItemType_Label")
                    .HasMaxLength(75)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<QmsWorkitemviewlog>(entity =>
            {
                entity.ToTable("qms_workitemviewlog", "aca");

                entity.HasIndex(e => e.WorkItemTypeCode)
                    .HasName("qms_workitemviewlog_workItemType_fk");

                entity.Property(e => e.Id).HasColumnType("int(10)");

                entity.Property(e => e.Createdat).HasColumnName("createdat");

                entity.Property(e => e.Userid)
                    .HasColumnName("userid")
                    .HasColumnType("int(10)");

                entity.Property(e => e.WorkItemTypeCode)
                    .HasColumnName("WorkItemType_Code")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Workitemid)
                    .HasColumnName("workitemid")
                    .HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.WorkItemTypeCodeNavigation)
                    .WithMany(p => p.QmsWorkitemviewlog)
                    .HasPrincipalKey(p => p.WorkItemTypeCode)
                    .HasForeignKey(d => d.WorkItemTypeCode)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("qms_workitemviewlog_workItemType_fk");
            });

            modelBuilder.Entity<SecOrg>(entity =>
            {
                entity.HasKey(e => e.OrgId);

                entity.ToTable("sec_org", "aca");

                entity.HasIndex(e => e.OrgCode)
                    .HasName("sec_org_uk1")
                    .IsUnique();

                entity.HasIndex(e => e.OrgLabel)
                    .HasName("sec_org_uk2")
                    .IsUnique();

                entity.HasIndex(e => e.ParentOrgId)
                    .HasName("sec_org_parentorg_fk");

                entity.Property(e => e.OrgId)
                    .HasColumnName("org_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

                entity.Property(e => e.OrgCode)
                    .IsRequired()
                    .HasColumnName("org_code")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.OrgLabel)
                    .IsRequired()
                    .HasColumnName("org_label")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.OrgtypeId)
                    .HasColumnName("orgtype_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.ParentOrgId)
                    .HasColumnName("parent_org_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.HasOne(d => d.ParentOrg)
                    .WithMany(p => p.InverseParentOrg)
                    .HasForeignKey(d => d.ParentOrgId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("sec_org_parentorg_fk");
            });

            modelBuilder.Entity<SecPermission>(entity =>
            {
                entity.HasKey(e => e.PermissionId);

                entity.ToTable("sec_permission", "aca");

                entity.HasIndex(e => e.PermissionCode)
                    .HasName("sec_permission_uk1")
                    .IsUnique();

                entity.HasIndex(e => e.PermissionLabel)
                    .HasName("sec_permission_uk2")
                    .IsUnique();

                entity.Property(e => e.PermissionId)
                    .HasColumnName("permission_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

                entity.Property(e => e.PermissionCode)
                    .IsRequired()
                    .HasColumnName("permission_code")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PermissionLabel)
                    .IsRequired()
                    .HasColumnName("permission_label")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<SecRole>(entity =>
            {
                entity.HasKey(e => e.RoleId);

                entity.ToTable("sec_role", "aca");

                entity.HasIndex(e => e.RoleCode)
                    .HasName("sec_role_uk1")
                    .IsUnique();

                entity.HasIndex(e => e.RoleLabel)
                    .HasName("sec_role_uk2")
                    .IsUnique();

                entity.Property(e => e.RoleId)
                    .HasColumnName("role_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

                entity.Property(e => e.RoleCode)
                    .IsRequired()
                    .HasColumnName("role_code")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.RoleLabel)
                    .IsRequired()
                    .HasColumnName("role_label")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<SecRolePermission>(entity =>
            {
                entity.HasKey(e => e.RolePermissionId);

                entity.ToTable("sec_role_permission", "aca");

                entity.HasIndex(e => e.PermissionId)
                    .HasName("sec_role_permission_fk2");

                entity.HasIndex(e => new { e.RoleId, e.PermissionId })
                    .HasName("sec_role_permission_uk1")
                    .IsUnique();

                entity.Property(e => e.RolePermissionId)
                    .HasColumnName("role_permission_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

                entity.Property(e => e.PermissionId)
                    .HasColumnName("permission_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.RoleId)
                    .HasColumnName("role_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.HasOne(d => d.Permission)
                    .WithMany(p => p.SecRolePermission)
                    .HasForeignKey(d => d.PermissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("sec_role_permission_fk2");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.SecRolePermission)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("sec_role_permission_fk1");
            });

            modelBuilder.Entity<SecSecurityitemtype>(entity =>
            {
                entity.HasKey(e => e.SecurityItemTypeId);

                entity.ToTable("sec_securityitemtype", "aca");

                entity.Property(e => e.SecurityItemTypeId)
                    .HasColumnName("SecurityItemType_ID")
                    .HasColumnType("int(10)");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

                entity.Property(e => e.SecurityItemTypeCode)
                    .IsRequired()
                    .HasColumnName("SecurityItemType_Code")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.SecurityItemTypeLabel)
                    .IsRequired()
                    .HasColumnName("SecurityItemType_Label")
                    .HasMaxLength(80)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SecSecuritylog>(entity =>
            {
                entity.HasKey(e => e.SecurityLogId);

                entity.ToTable("sec_securitylog", "aca");

                entity.HasIndex(e => e.ActionTakenByUserId)
                    .HasName("sec_SecurityLog_User_fk");

                entity.HasIndex(e => e.SecurityLogTypeId)
                    .HasName("sec_SecurityLog_SecurityLogType_fk");

                entity.Property(e => e.SecurityLogId)
                    .HasColumnName("SecurityLog_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ActionTakenByUserId).HasColumnType("int(11) unsigned");

                entity.Property(e => e.ActiontakenOnItemId).HasColumnType("int(10)");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.SecurityLogTypeId)
                    .HasColumnName("SecurityLogType_ID")
                    .HasColumnType("int(10)");

                entity.HasOne(d => d.ActionTakenByUser)
                    .WithMany(p => p.SecSecuritylog)
                    .HasForeignKey(d => d.ActionTakenByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("sec_SecurityLog_User_fk");

                entity.HasOne(d => d.SecurityLogType)
                    .WithMany(p => p.SecSecuritylog)
                    .HasForeignKey(d => d.SecurityLogTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("sec_SecurityLog_SecurityLogType_fk");
            });

            modelBuilder.Entity<SecSecuritylogtype>(entity =>
            {
                entity.HasKey(e => e.SecurityLogTypeId);

                entity.ToTable("sec_securitylogtype", "aca");

                entity.HasIndex(e => e.SecurityItemTypeId)
                    .HasName("qms_SecurityLogType_SecurityItemType_fk");

                entity.Property(e => e.SecurityLogTypeId)
                    .HasColumnName("SecurityLogType_ID")
                    .HasColumnType("int(10)");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

                entity.Property(e => e.SecurityItemTypeId)
                    .HasColumnName("SecurityItemType_ID")
                    .HasColumnType("int(10)");

                entity.Property(e => e.SecurityLogTemplate)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.SecurityLogTypeCode)
                    .IsRequired()
                    .HasColumnName("SecurityLogType_Code")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.SecurityLogTypeLabel)
                    .IsRequired()
                    .HasColumnName("SecurityLogType_Label")
                    .HasMaxLength(80)
                    .IsUnicode(false);

                entity.HasOne(d => d.SecurityItemType)
                    .WithMany(p => p.SecSecuritylogtype)
                    .HasForeignKey(d => d.SecurityItemTypeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("qms_SecurityLogType_SecurityItemType_fk");
            });

            modelBuilder.Entity<SecUser>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.ToTable("sec_user", "aca");

                entity.HasIndex(e => e.EmailAddress)
                    .HasName("sec_user_uk1")
                    .IsUnique();

                entity.HasIndex(e => e.ManagerId)
                    .HasName("sec_user_fk1");

                entity.HasIndex(e => e.OrgId)
                    .HasName("sec_user_org_fk_idx");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

                entity.Property(e => e.DisplayName)
                    .IsRequired()
                    .HasColumnName("display_name")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.EmailAddress)
                    .IsRequired()
                    .HasColumnName("email_address")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ManagerId)
                    .HasColumnName("manager_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.OrgId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.HasOne(d => d.Manager)
                    .WithMany(p => p.InverseManager)
                    .HasForeignKey(d => d.ManagerId)
                    .HasConstraintName("sec_user_fk1");

                entity.HasOne(d => d.Org)
                    .WithMany(p => p.SecUser)
                    .HasForeignKey(d => d.OrgId)
                    .HasConstraintName("sec_user_org_fk");
            });

            modelBuilder.Entity<SecUserlogin>(entity =>
            {
                entity.ToTable("sec_userlogin", "aca");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.Property(e => e.Emailaddress)
                    .IsRequired()
                    .HasColumnName("emailaddress")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.LoginEventType)
                    .IsRequired()
                    .HasColumnName("login_event_type")
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SecUserRole>(entity =>
            {
                entity.HasKey(e => e.UserOrgRoleId);

                entity.ToTable("sec_user_role", "aca");

                entity.HasIndex(e => e.RoleId)
                    .HasName("sec_user_org_role_fk3");

                entity.HasIndex(e => new { e.UserId, e.RoleId })
                    .HasName("sec_user_org_role_uk1")
                    .IsUnique();

                entity.Property(e => e.UserOrgRoleId)
                    .HasColumnName("user_org_role_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

                entity.Property(e => e.RoleId)
                    .HasColumnName("role_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.SecUserRole)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("sec_user_org_role_fk3");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.SecUserRole)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("sec_user_org_role_fk1");
            });

            modelBuilder.Entity<SysMenuitem>(entity =>
            {
                entity.HasKey(e => e.MenuitemId);

                entity.ToTable("sys_menuitem", "aca");

                entity.HasIndex(e => e.ModuleId)
                    .HasName("qms_menuitem_module_fk");

                entity.HasIndex(e => e.PermissionId)
                    .HasName("permission_id")
                    .IsUnique();

                entity.Property(e => e.MenuitemId)
                    .HasColumnName("menuitem_id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.ActionName)
                    .IsRequired()
                    .HasColumnName("action_name")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ControllerName)
                    .IsRequired()
                    .HasColumnName("controller_name")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

                entity.Property(e => e.DisplayOrder)
                    .HasColumnName("display_order")
                    .HasColumnType("tinyint(3)");

                entity.Property(e => e.IconOff)
                    .HasColumnName("icon_off")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.IconOn)
                    .HasColumnName("icon_on")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.MenuitemCode)
                    .IsRequired()
                    .HasColumnName("menuitem_code")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.MenuitemLabel)
                    .IsRequired()
                    .HasColumnName("menuitem_label")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ModuleId)
                    .HasColumnName("module_id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.PermissionId)
                    .HasColumnName("permission_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.QueryString)
                    .HasColumnName("query_string")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.HasOne(d => d.Module)
                    .WithMany(p => p.SysMenuitem)
                    .HasForeignKey(d => d.ModuleId)
                    .HasConstraintName("qms_menuitem_module_fk");

                entity.HasOne(d => d.Permission)
                    .WithOne(p => p.SysMenuitem)
                    .HasForeignKey<SysMenuitem>(d => d.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("qms_menuitem_permission_fk");
            });

            modelBuilder.Entity<SysModule>(entity =>
            {
                entity.HasKey(e => e.ModuleId);

                entity.ToTable("sys_module", "aca");

                entity.Property(e => e.ModuleId)
                    .HasColumnName("module_id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.ActionName)
                    .IsRequired()
                    .HasColumnName("action_name")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ControllerName)
                    .IsRequired()
                    .HasColumnName("controller_name")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

                entity.Property(e => e.DisplayOrder)
                    .HasColumnName("display_order")
                    .HasColumnType("tinyint(3)");

                entity.Property(e => e.ModuleCode)
                    .IsRequired()
                    .HasColumnName("module_code")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ModuleLabel)
                    .IsRequired()
                    .HasColumnName("module_label")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.QueryString)
                    .HasColumnName("query_string")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<SysModuleRole>(entity =>
            {
                entity.HasKey(e => e.ModuleRoleId);

                entity.ToTable("sys_module_role", "aca");

                entity.HasIndex(e => e.ModuleId)
                    .HasName("qms_module_moduleid_fk");

                entity.HasIndex(e => e.RoleId)
                    .HasName("qms_module_roleid_fk");

                entity.Property(e => e.ModuleRoleId)
                    .HasColumnName("module_role_id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

                entity.Property(e => e.ModuleId)
                    .HasColumnName("module_id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.RoleId)
                    .HasColumnName("role_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.HasOne(d => d.Module)
                    .WithMany(p => p.SysModuleRole)
                    .HasForeignKey(d => d.ModuleId)
                    .HasConstraintName("qms_module_moduleid_fk");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.SysModuleRole)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("qms_module_roleid_fk");
            });

            modelBuilder.Entity<SysSetting>(entity =>
            {
                entity.HasKey(e => e.SettingId);

                entity.ToTable("sys_setting", "aca");

                entity.HasIndex(e => new { e.SettingTypeId, e.Environment })
                    .HasName("sys_setting_environment_uc")
                    .IsUnique();

                entity.Property(e => e.SettingId).HasColumnType("int(10)");

                entity.Property(e => e.Environment)
                    .IsRequired()
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.SettingTypeId).HasColumnType("int(10)");

                entity.Property(e => e.SettingValue)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(d => d.SettingType)
                    .WithMany(p => p.SysSetting)
                    .HasForeignKey(d => d.SettingTypeId)
                    .HasConstraintName("qms_setting_settingtype_fk");
            });

            modelBuilder.Entity<SysSettingtype>(entity =>
            {
                entity.HasKey(e => e.SettingTypeId);

                entity.ToTable("sys_settingtype", "aca");

                entity.Property(e => e.SettingTypeId).HasColumnType("int(10)");

                entity.Property(e => e.Createdat).HasColumnName("createdat");

                entity.Property(e => e.Deletedat).HasColumnName("deletedat");

                entity.Property(e => e.SettingCode)
                    .IsRequired()
                    .HasColumnName("Setting_Code")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.SettingDescription)
                    .IsRequired()
                    .HasColumnName("Setting_Description")
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });
        }
    }
}
