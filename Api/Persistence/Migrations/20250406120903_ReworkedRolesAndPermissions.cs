using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Api.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ReworkedRolesAndPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RolePermission",
                schema: "public");

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("01960516-468d-7f11-90f2-e166cfda751f"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("01960516-cbce-7251-b410-071a6dd1a200"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("01960517-4409-7812-a4c4-3b3c1b3c676c"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("01960518-2c6e-733c-a464-994496458d89"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("01960513-664b-7304-92de-d2544cfa1ff7"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("01960514-3085-7e45-a2ce-dc3336bdbac3"));

            migrationBuilder.CreateTable(
                name: "PermissionRole",
                schema: "public",
                columns: table => new
                {
                    PermissionsId = table.Column<Guid>(type: "uuid", nullable: false),
                    RolesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionRole", x => new { x.PermissionsId, x.RolesId });
                    table.ForeignKey(
                        name: "FK_PermissionRole_Permissions_PermissionsId",
                        column: x => x.PermissionsId,
                        principalSchema: "public",
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PermissionRole_Roles_RolesId",
                        column: x => x.RolesId,
                        principalSchema: "public",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PermissionRole_RolesId",
                schema: "public",
                table: "PermissionRole",
                column: "RolesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PermissionRole",
                schema: "public");

            migrationBuilder.CreateTable(
                name: "RolePermission",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermission_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalSchema: "public",
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermission_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "public",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "Permissions",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("01960516-468d-7f11-90f2-e166cfda751f"), "read" },
                    { new Guid("01960516-cbce-7251-b410-071a6dd1a200"), "write" },
                    { new Guid("01960517-4409-7812-a4c4-3b3c1b3c676c"), "delete" },
                    { new Guid("01960518-2c6e-733c-a464-994496458d89"), "admin" }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("01960513-664b-7304-92de-d2544cfa1ff7"), "user" },
                    { new Guid("01960514-3085-7e45-a2ce-dc3336bdbac3"), "admin" }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "RolePermission",
                columns: new[] { "Id", "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("0196051a-17cf-72b6-b98d-4cb5a6c65b83"), new Guid("01960516-468d-7f11-90f2-e166cfda751f"), new Guid("01960513-664b-7304-92de-d2544cfa1ff7") },
                    { new Guid("0196051a-5fce-7dd1-bd5d-03280dcc27e9"), new Guid("01960516-cbce-7251-b410-071a6dd1a200"), new Guid("01960513-664b-7304-92de-d2544cfa1ff7") },
                    { new Guid("0196051a-a482-7439-9b61-41d1c45fd105"), new Guid("01960517-4409-7812-a4c4-3b3c1b3c676c"), new Guid("01960513-664b-7304-92de-d2544cfa1ff7") },
                    { new Guid("0196051b-0f46-73e7-8da9-5a467b71157a"), new Guid("01960516-468d-7f11-90f2-e166cfda751f"), new Guid("01960514-3085-7e45-a2ce-dc3336bdbac3") },
                    { new Guid("0196051b-6543-7f2f-92da-37bcef6eb2d3"), new Guid("01960516-cbce-7251-b410-071a6dd1a200"), new Guid("01960514-3085-7e45-a2ce-dc3336bdbac3") },
                    { new Guid("0196051b-ada9-71ab-9022-4e99496803e0"), new Guid("01960517-4409-7812-a4c4-3b3c1b3c676c"), new Guid("01960514-3085-7e45-a2ce-dc3336bdbac3") },
                    { new Guid("0196051b-ebe8-79e9-8913-459bb1cebfb5"), new Guid("01960518-2c6e-733c-a464-994496458d89"), new Guid("01960514-3085-7e45-a2ce-dc3336bdbac3") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_PermissionId",
                schema: "public",
                table: "RolePermission",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_RoleId",
                schema: "public",
                table: "RolePermission",
                column: "RoleId");
        }
    }
}
