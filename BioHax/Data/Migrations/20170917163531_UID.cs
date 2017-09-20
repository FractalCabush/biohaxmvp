using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BioHax.Data.Migrations
{
    public partial class UID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "recordTypeLength",
                table: "Record",
                newName: "RecordTypeLength");

            migrationBuilder.RenameColumn(
                name: "recordLength",
                table: "Record",
                newName: "RecordLength");

            migrationBuilder.RenameColumn(
                name: "identifierCode",
                table: "Record",
                newName: "IdentifierCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RecordTypeLength",
                table: "Record",
                newName: "recordTypeLength");

            migrationBuilder.RenameColumn(
                name: "RecordLength",
                table: "Record",
                newName: "recordLength");

            migrationBuilder.RenameColumn(
                name: "IdentifierCode",
                table: "Record",
                newName: "identifierCode");
        }
    }
}
