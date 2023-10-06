using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMS_ActionLayer.Migrations
{
    public partial class addv5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_teams_TeamId",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "FK_users_userRoles_RoleId",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "FK_users_userStatuses_StatusId",
                table: "users");

            migrationBuilder.AlterColumn<int>(
                name: "TeamId",
                table: "users",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "StatusId",
                table: "users",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "RoleId",
                table: "users",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "TruongPhongId",
                table: "teams",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_users_teams_TeamId",
                table: "users",
                column: "TeamId",
                principalTable: "teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_users_userRoles_RoleId",
                table: "users",
                column: "RoleId",
                principalTable: "userRoles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_users_userStatuses_StatusId",
                table: "users",
                column: "StatusId",
                principalTable: "userStatuses",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_teams_TeamId",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "FK_users_userRoles_RoleId",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "FK_users_userStatuses_StatusId",
                table: "users");

            migrationBuilder.AlterColumn<int>(
                name: "TeamId",
                table: "users",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StatusId",
                table: "users",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RoleId",
                table: "users",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TruongPhongId",
                table: "teams",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_users_teams_TeamId",
                table: "users",
                column: "TeamId",
                principalTable: "teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_users_userRoles_RoleId",
                table: "users",
                column: "RoleId",
                principalTable: "userRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_users_userStatuses_StatusId",
                table: "users",
                column: "StatusId",
                principalTable: "userStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
