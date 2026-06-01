using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ONENET.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReaderEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "readers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    reader_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    full_name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    date_of_birth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    phone_number = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    email = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    registration_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expiry_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_modified_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_readers", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_readers_full_name",
                table: "readers",
                column: "full_name");

            migrationBuilder.CreateIndex(
                name: "IX_readers_phone_number",
                table: "readers",
                column: "phone_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_readers_reader_code",
                table: "readers",
                column: "reader_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_readers_status_expiry_date",
                table: "readers",
                columns: new[] { "status", "expiry_date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "readers");
        }
    }
}
