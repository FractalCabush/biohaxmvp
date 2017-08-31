using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BioHax.Data.Migrations
{
    public partial class Inheritance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Service",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RecordID",
                table: "Service",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Record",
                columns: table => new
                {
                    RecordID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    URI = table.Column<byte[]>(nullable: true),
                    URIIdentifier = table.Column<byte>(nullable: false),
                    URIRecordType = table.Column<byte>(nullable: false),
                    identifierCode = table.Column<byte>(nullable: false),
                    recordLength = table.Column<byte>(nullable: false),
                    recordTypeLength = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Record", x => x.RecordID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Service_RecordID",
                table: "Service",
                column: "RecordID");

            migrationBuilder.AddForeignKey(
                name: "FK_Service_Record_RecordID",
                table: "Service",
                column: "RecordID",
                principalTable: "Record",
                principalColumn: "RecordID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Service_Record_RecordID",
                table: "Service");

            migrationBuilder.DropTable(
                name: "Record");

            migrationBuilder.DropIndex(
                name: "IX_Service_RecordID",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "RecordID",
                table: "Service");
        }
    }
}
