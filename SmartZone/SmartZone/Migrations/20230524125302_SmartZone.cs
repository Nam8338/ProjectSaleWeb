using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartZone.Migrations
{
    public partial class SmartZone : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Attribute",
                columns: table => new
                {
                    AttributeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attribute", x => x.AttributeID);
                });

            migrationBuilder.CreateTable(
                name: "Brand",
                columns: table => new
                {
                    BrandID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Logo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brand", x => x.BrandID);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    CateID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descriptions = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ParentID = table.Column<int>(type: "int", nullable: false),
                    Levels = table.Column<int>(type: "int", nullable: true),
                    Ordering = table.Column<int>(type: "int", nullable: true),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "(CONVERT([bit],(0)))"),
                    ThumbImg = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Alias = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Cover = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.CateID);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustommerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BirthDay = table.Column<DateTime>(type: "datetime", nullable: true),
                    Avatar = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "varchar(13)", unicode: false, maxLength: 13, nullable: true),
                    Province = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    District = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ward = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastLogin = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActived = table.Column<bool>(type: "bit", nullable: false),
                    Randomkey = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustommerID);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Descriptions = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, defaultValueSql: "(N'')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Shipper",
                columns: table => new
                {
                    ShipperID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShipperName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "varchar(13)", unicode: false, maxLength: 13, nullable: true),
                    Company = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Shipdate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipper", x => x.ShipperID);
                });

            migrationBuilder.CreateTable(
                name: "TransactStatus",
                columns: table => new
                {
                    TransactionStatusID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Descriptions = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactStatus", x => x.TransactionStatusID);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    ProductID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ShortDesc = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Descriptions = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CateID = table.Column<int>(type: "int", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SalesPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Discount = table.Column<int>(type: "int", nullable: true),
                    ThumbImg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Video = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: true),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsBestsellers = table.Column<bool>(type: "bit", nullable: false),
                    Homeflag = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "(CONVERT([bit],(0)))"),
                    IsActived = table.Column<bool>(type: "bit", nullable: false),
                    Tag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Alias = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitInStock = table.Column<int>(type: "int", nullable: true),
                    BrandID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.ProductID);
                    table.ForeignKey(
                        name: "FK_Product_Brand",
                        column: x => x.BrandID,
                        principalTable: "Brand",
                        principalColumn: "BrandID");
                    table.ForeignKey(
                        name: "FK_Product_Category",
                        column: x => x.CateID,
                        principalTable: "Category",
                        principalColumn: "CateID");
                });

            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false, defaultValueSql: "(N'')"),
                    Phone = table.Column<string>(type: "varchar(13)", unicode: false, maxLength: 13, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastLogin = table.Column<DateTime>(type: "datetime", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: true),
                    IsActived = table.Column<bool>(type: "bit", nullable: false),
                    Randomkey = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.UserID);
                    table.ForeignKey(
                        name: "FK_Account_Roles",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId");
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    OrderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerID = table.Column<int>(type: "int", nullable: true),
                    OrderDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "('0001-01-01T00:00:00.000')"),
                    TransactionStatusID = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    IsPaid = table.Column<bool>(type: "bit", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalMoney = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Province = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    District = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ward = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.OrderID);
                    table.ForeignKey(
                        name: "FK_Order_Customers",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "CustommerID");
                    table.ForeignKey(
                        name: "FK_Order_TransactStatus",
                        column: x => x.TransactionStatusID,
                        principalTable: "TransactStatus",
                        principalColumn: "TransactionStatusID");
                });

            migrationBuilder.CreateTable(
                name: "AttributePrices",
                columns: table => new
                {
                    AttributePriceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttributeID = table.Column<int>(type: "int", nullable: true),
                    ProductID = table.Column<int>(type: "int", nullable: true),
                    Price = table.Column<int>(type: "int", nullable: true),
                    IsActived = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributePrices", x => x.AttributePriceID);
                    table.ForeignKey(
                        name: "FK_AttributePrices_Attribute",
                        column: x => x.AttributeID,
                        principalTable: "Attribute",
                        principalColumn: "AttributeID");
                    table.ForeignKey(
                        name: "FK_AttributePrices_Product",
                        column: x => x.ProductID,
                        principalTable: "Product",
                        principalColumn: "ProductID");
                });

            migrationBuilder.CreateTable(
                name: "ImportTicket",
                columns: table => new
                {
                    TicketID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImportDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    TotalMoney = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportTicket", x => x.TicketID);
                    table.ForeignKey(
                        name: "FK_ImportTicket_Account",
                        column: x => x.UserId,
                        principalTable: "Account",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    OrderDetailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderID = table.Column<int>(type: "int", nullable: true),
                    ProductID = table.Column<int>(type: "int", nullable: true),
                    OrderNumber = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    Discount = table.Column<int>(type: "int", nullable: true),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.OrderDetailID);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Order",
                        column: x => x.OrderID,
                        principalTable: "Order",
                        principalColumn: "OrderID");
                    table.ForeignKey(
                        name: "FK_OrderDetails_Product",
                        column: x => x.ProductID,
                        principalTable: "Product",
                        principalColumn: "ProductID");
                });

            migrationBuilder.CreateTable(
                name: "ImportDetails",
                columns: table => new
                {
                    TicketDetailsID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketID = table.Column<int>(type: "int", nullable: true),
                    ProductID = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    ImportCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportDetails", x => x.TicketDetailsID);
                    table.ForeignKey(
                        name: "FK_ImportDetails_ImportTicket",
                        column: x => x.TicketID,
                        principalTable: "ImportTicket",
                        principalColumn: "TicketID");
                    table.ForeignKey(
                        name: "FK_ImportDetails_Product",
                        column: x => x.ProductID,
                        principalTable: "Product",
                        principalColumn: "ProductID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_RoleId",
                table: "Account",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributePrices_AttributeID",
                table: "AttributePrices",
                column: "AttributeID");

            migrationBuilder.CreateIndex(
                name: "IX_AttributePrices_ProductID",
                table: "AttributePrices",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_ImportDetails_ProductID",
                table: "ImportDetails",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_ImportDetails_TicketID",
                table: "ImportDetails",
                column: "TicketID");

            migrationBuilder.CreateIndex(
                name: "IX_ImportTicket_UserId",
                table: "ImportTicket",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_CustomerID",
                table: "Order",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_Order_TransactionStatusID",
                table: "Order",
                column: "TransactionStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderID",
                table: "OrderDetails",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductID",
                table: "OrderDetails",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_Product_BrandID",
                table: "Product",
                column: "BrandID");

            migrationBuilder.CreateIndex(
                name: "IX_Product_CateID",
                table: "Product",
                column: "CateID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttributePrices");

            migrationBuilder.DropTable(
                name: "ImportDetails");

            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "Shipper");

            migrationBuilder.DropTable(
                name: "Attribute");

            migrationBuilder.DropTable(
                name: "ImportTicket");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "TransactStatus");

            migrationBuilder.DropTable(
                name: "Brand");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
