using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Visma.IdentityServer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ids");

            migrationBuilder.CreateTable(
                name: "personLanguages",
                schema: "ids",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_personLanguages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "personRoles",
                schema: "ids",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_personRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "persons",
                schema: "ids",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonRoleId = table.Column<int>(type: "int", nullable: false),
                    PersonLanguageId = table.Column<int>(type: "int", nullable: false),
                    CanViewAccountingRecords = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreationDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    ModificationDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_persons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_persons_personLanguages_PersonLanguageId",
                        column: x => x.PersonLanguageId,
                        principalSchema: "ids",
                        principalTable: "personLanguages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_persons_personRoles_PersonRoleId",
                        column: x => x.PersonRoleId,
                        principalSchema: "ids",
                        principalTable: "personRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "personEmails",
                schema: "ids",
                columns: table => new
                {
                    PersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_personEmails", x => x.PersonId);
                    table.ForeignKey(
                        name: "FK_personEmails_persons_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "ids",
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "personNames",
                schema: "ids",
                columns: table => new
                {
                    PersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_personNames", x => x.PersonId);
                    table.ForeignKey(
                        name: "FK_personNames_persons_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "ids",
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "personPasswords",
                schema: "ids",
                columns: table => new
                {
                    PersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_personPasswords", x => x.PersonId);
                    table.ForeignKey(
                        name: "FK_personPasswords_persons_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "ids",
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "ids",
                table: "personLanguages",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "en" },
                    { 2, "dk" }
                });

            migrationBuilder.InsertData(
                schema: "ids",
                table: "personRoles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "standard" },
                    { 2, "accountant" },
                    { 3, "administrator" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_persons_PersonLanguageId",
                schema: "ids",
                table: "persons",
                column: "PersonLanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_persons_PersonRoleId",
                schema: "ids",
                table: "persons",
                column: "PersonRoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "personEmails",
                schema: "ids");

            migrationBuilder.DropTable(
                name: "personNames",
                schema: "ids");

            migrationBuilder.DropTable(
                name: "personPasswords",
                schema: "ids");

            migrationBuilder.DropTable(
                name: "persons",
                schema: "ids");

            migrationBuilder.DropTable(
                name: "personLanguages",
                schema: "ids");

            migrationBuilder.DropTable(
                name: "personRoles",
                schema: "ids");
        }
    }
}
