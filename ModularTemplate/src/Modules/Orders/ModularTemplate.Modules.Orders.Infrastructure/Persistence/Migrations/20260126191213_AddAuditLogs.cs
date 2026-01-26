using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModularTemplate.Modules.Orders.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "audit_logs",
                schema: "orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    entity_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    entity_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    action = table.Column<int>(type: "integer", nullable: false),
                    old_values = table.Column<string>(type: "jsonb", nullable: true),
                    new_values = table.Column<string>(type: "jsonb", nullable: true),
                    affected_columns = table.Column<string>(type: "jsonb", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    timestamp_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    correlation_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    trace_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    user_agent = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audit_logs", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_correlation",
                schema: "orders",
                table: "audit_logs",
                column: "correlation_id");

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_entity",
                schema: "orders",
                table: "audit_logs",
                columns: new[] { "entity_name", "entity_id" });

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_timestamp",
                schema: "orders",
                table: "audit_logs",
                column: "timestamp_utc");

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_user",
                schema: "orders",
                table: "audit_logs",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_logs",
                schema: "orders");
        }
    }
}
