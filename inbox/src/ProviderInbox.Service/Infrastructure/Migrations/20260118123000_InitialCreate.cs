using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProviderInbox.Service.Infrastructure.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "InboxMessages",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Npi = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                OfficeNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                Type = table.Column<int>(type: "int", nullable: false),
                Title = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                Summary = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                ReferenceId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                ActionRequired = table.Column<bool>(type: "bit", nullable: false),
                DueAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                SlaHours = table.Column<int>(type: "int", nullable: true),
                Priority = table.Column<int>(type: "int", nullable: false),
                Pinned = table.Column<bool>(type: "bit", nullable: false),
                Status = table.Column<int>(type: "int", nullable: false),
                ReadAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                CompletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                SnoozedUntil = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_InboxMessages", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_InboxMessages_Npi_OfficeNumber_Pinned_Priority_Status_DueAt_UpdatedAt",
            table: "InboxMessages",
            columns: new[] { "Npi", "OfficeNumber", "Pinned", "Priority", "Status", "DueAt", "UpdatedAt" });

        migrationBuilder.CreateIndex(
            name: "IX_InboxMessages_Npi_OfficeNumber_Type_CreatedAt",
            table: "InboxMessages",
            columns: new[] { "Npi", "OfficeNumber", "Type", "CreatedAt" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "InboxMessages");
    }
}
