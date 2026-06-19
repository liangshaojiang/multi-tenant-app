using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jvs.OrgService.Migrations
{
    /// <inheritdoc />
    public partial class AddDeptCompanyFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "dept_type",
                table: "org_dept",
                type: "tinyint unsigned",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<string>(
                name: "second_dept_name",
                table: "org_dept",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "second_tax_number",
                table: "org_dept",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "tax_number",
                table: "org_dept",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "dept_type",
                table: "org_dept");

            migrationBuilder.DropColumn(
                name: "second_dept_name",
                table: "org_dept");

            migrationBuilder.DropColumn(
                name: "second_tax_number",
                table: "org_dept");

            migrationBuilder.DropColumn(
                name: "tax_number",
                table: "org_dept");
        }
    }
}
