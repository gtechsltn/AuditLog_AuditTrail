using AuditTrails.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuditTrails.Database.Mapping;

public class AuditTrailConfiguration : IEntityTypeConfiguration<AuditTrail>
{
	public void Configure(EntityTypeBuilder<AuditTrail> builder)
	{
		builder.ToTable("audit_trails");
		builder.HasKey(e => e.Id);

		builder.HasIndex(e => e.EntityName);

		builder.Property(e => e.Id);

		builder.Property(e => e.UserId);
		builder.Property(e => e.EntityName).HasMaxLength(100).IsRequired();
		builder.Property(e => e.DateUtc).IsRequired();
		builder.Property(e => e.PrimaryKey).HasMaxLength(100);

		builder.Property(e => e.TrailType).HasConversion<string>();

		builder.Property(e => e.ChangedColumns).HasColumnType("jsonb");
		builder.Property(e => e.OldValues).HasColumnType("jsonb");
		builder.Property(e => e.NewValues).HasColumnType("jsonb");

		builder.HasOne(e => e.User)
			.WithMany()
			.HasForeignKey(e => e.UserId)
			.IsRequired(false)
			.OnDelete(DeleteBehavior.SetNull);
	}
}
