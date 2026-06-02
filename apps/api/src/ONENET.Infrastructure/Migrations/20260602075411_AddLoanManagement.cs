using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ONENET.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLoanManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "books",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Author = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Publisher = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PublishYear = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ShelfLocation = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_books", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "phieu_muons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DocGiaId = table.Column<Guid>(type: "uuid", nullable: false),
                    NgayMuon = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NgayHenTra = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TrangThaiPhieuMuon = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_phieu_muons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "readers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReaderCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FullName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_readers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "chi_tiet_phieu_muons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PhieuMuonId = table.Column<Guid>(type: "uuid", nullable: false),
                    SachId = table.Column<Guid>(type: "uuid", nullable: false),
                    NgayTraThucTe = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TrangThaiChiTiet = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsQuaHan = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chi_tiet_phieu_muons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_chi_tiet_phieu_muons_phieu_muons_PhieuMuonId",
                        column: x => x.PhieuMuonId,
                        principalTable: "phieu_muons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_books_BookCode",
                table: "books",
                column: "BookCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_chi_tiet_phieu_muons_IsQuaHan",
                table: "chi_tiet_phieu_muons",
                column: "IsQuaHan");

            migrationBuilder.CreateIndex(
                name: "IX_chi_tiet_phieu_muons_PhieuMuonId",
                table: "chi_tiet_phieu_muons",
                column: "PhieuMuonId");

            migrationBuilder.CreateIndex(
                name: "IX_chi_tiet_phieu_muons_SachId",
                table: "chi_tiet_phieu_muons",
                column: "SachId");

            migrationBuilder.CreateIndex(
                name: "IX_chi_tiet_phieu_muons_TrangThaiChiTiet",
                table: "chi_tiet_phieu_muons",
                column: "TrangThaiChiTiet");

            migrationBuilder.CreateIndex(
                name: "IX_phieu_muons_DocGiaId",
                table: "phieu_muons",
                column: "DocGiaId");

            migrationBuilder.CreateIndex(
                name: "IX_phieu_muons_NgayHenTra",
                table: "phieu_muons",
                column: "NgayHenTra");

            migrationBuilder.CreateIndex(
                name: "IX_phieu_muons_TrangThaiPhieuMuon",
                table: "phieu_muons",
                column: "TrangThaiPhieuMuon");

            migrationBuilder.CreateIndex(
                name: "IX_readers_PhoneNumber",
                table: "readers",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_readers_ReaderCode",
                table: "readers",
                column: "ReaderCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "books");

            migrationBuilder.DropTable(
                name: "chi_tiet_phieu_muons");

            migrationBuilder.DropTable(
                name: "readers");

            migrationBuilder.DropTable(
                name: "phieu_muons");
        }
    }
}
