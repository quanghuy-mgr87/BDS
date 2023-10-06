using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMS_ActionLayer.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "notificationStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notificationStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "productsStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productsStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "teamsStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teamsStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "userRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "userStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "videoBaiHocStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_videoBaiHocStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "teams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Member = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sologan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TruongPhongId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_teams_teamsStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "teamsStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    TeamId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_users_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_users_userRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "userRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_users_userStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "userStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateId = table.Column<int>(type: "int", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_notifications_notificationStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "notificationStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_notifications_users_CreateId",
                        column: x => x.CreateId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HostName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HostPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Build = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CertificateOfLand1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CertificateOfLand2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    DauChuId = table.Column<int>(type: "int", nullable: false),
                    BatDauBan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GiaBan = table.Column<double>(type: "float", nullable: false),
                    HoaHong = table.Column<double>(type: "float", nullable: false),
                    PhanTramChiaNV = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_products_productsStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "productsStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_products_users_DauChuId",
                        column: x => x.DauChuId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "videoBaiHocs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateId = table.Column<int>(type: "int", nullable: false),
                    URLPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CraeteTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_videoBaiHocs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_videoBaiHocs_users_CreateId",
                        column: x => x.CreateId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_videoBaiHocs_videoBaiHocStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "videoBaiHocStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "phieuXemNhas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustumerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustumerPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustumerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustumerIdImg1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustumerIdImg2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Desciption = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NhaId = table.Column<int>(type: "int", nullable: false),
                    NhanVienId = table.Column<int>(type: "int", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BanThanhCong = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_phieuXemNhas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_phieuXemNhas_products_NhaId",
                        column: x => x.NhaId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "productsImg",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    LinkImg = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productsImg", x => x.Id);
                    table.ForeignKey(
                        name: "FK_productsImg_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_notifications_CreateId",
                table: "notifications",
                column: "CreateId");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_StatusId",
                table: "notifications",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_phieuXemNhas_NhaId",
                table: "phieuXemNhas",
                column: "NhaId");

            migrationBuilder.CreateIndex(
                name: "IX_products_DauChuId",
                table: "products",
                column: "DauChuId");

            migrationBuilder.CreateIndex(
                name: "IX_products_StatusId",
                table: "products",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_productsImg_ProductId",
                table: "productsImg",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_teams_StatusId",
                table: "teams",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_users_RoleId",
                table: "users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_users_StatusId",
                table: "users",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_users_TeamId",
                table: "users",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_videoBaiHocs_CreateId",
                table: "videoBaiHocs",
                column: "CreateId");

            migrationBuilder.CreateIndex(
                name: "IX_videoBaiHocs_StatusId",
                table: "videoBaiHocs",
                column: "StatusId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "phieuXemNhas");

            migrationBuilder.DropTable(
                name: "productsImg");

            migrationBuilder.DropTable(
                name: "videoBaiHocs");

            migrationBuilder.DropTable(
                name: "notificationStatuses");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "videoBaiHocStatuses");

            migrationBuilder.DropTable(
                name: "productsStatus");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "teams");

            migrationBuilder.DropTable(
                name: "userRoles");

            migrationBuilder.DropTable(
                name: "userStatuses");

            migrationBuilder.DropTable(
                name: "teamsStatus");
        }
    }
}
