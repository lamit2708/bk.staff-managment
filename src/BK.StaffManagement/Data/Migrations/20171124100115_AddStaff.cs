using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BK.StaffManagement.Data.Migrations
{
    public partial class AddStaff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerCode",
                table: "Customer",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "BirthDay",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerCode",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "BirthDay",
                table: "AspNetUsers");
        }
    }
}
