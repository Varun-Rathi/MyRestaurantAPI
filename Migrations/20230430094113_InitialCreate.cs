using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeFirstRestaurantAPI.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryDish");

            migrationBuilder.DropTable(
                name: "CategoryMenu");

            migrationBuilder.AlterColumn<string>(
                name: "MenuName",
                table: "Menus",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MenuCategories",
                table: "MenuCategories",
                columns: new[] { "MenuId", "CategoryId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryDishes",
                table: "CategoryDishes",
                columns: new[] { "CategoryId", "DishId" });

            migrationBuilder.CreateIndex(
                name: "IX_MenuCategories_CategoryId",
                table: "MenuCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryDishes_DishId",
                table: "CategoryDishes",
                column: "DishId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryDishes_Categories_CategoryId",
                table: "CategoryDishes",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryDishes_Dishes_DishId",
                table: "CategoryDishes",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "DishId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCategories_Categories_CategoryId",
                table: "MenuCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCategories_Menus_MenuId",
                table: "MenuCategories",
                column: "MenuId",
                principalTable: "Menus",
                principalColumn: "MenuId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryDishes_Categories_CategoryId",
                table: "CategoryDishes");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryDishes_Dishes_DishId",
                table: "CategoryDishes");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuCategories_Categories_CategoryId",
                table: "MenuCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuCategories_Menus_MenuId",
                table: "MenuCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MenuCategories",
                table: "MenuCategories");

            migrationBuilder.DropIndex(
                name: "IX_MenuCategories_CategoryId",
                table: "MenuCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryDishes",
                table: "CategoryDishes");

            migrationBuilder.DropIndex(
                name: "IX_CategoryDishes_DishId",
                table: "CategoryDishes");

            migrationBuilder.AlterColumn<string>(
                name: "MenuName",
                table: "Menus",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "CategoryDish",
                columns: table => new
                {
                    CategoriesCategoryId = table.Column<int>(type: "int", nullable: false),
                    DishesDishId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryDish", x => new { x.CategoriesCategoryId, x.DishesDishId });
                    table.ForeignKey(
                        name: "FK_CategoryDish_Categories_CategoriesCategoryId",
                        column: x => x.CategoriesCategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryDish_Dishes_DishesDishId",
                        column: x => x.DishesDishId,
                        principalTable: "Dishes",
                        principalColumn: "DishId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CategoryMenu",
                columns: table => new
                {
                    CategoriesCategoryId = table.Column<int>(type: "int", nullable: false),
                    MenusMenuId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryMenu", x => new { x.CategoriesCategoryId, x.MenusMenuId });
                    table.ForeignKey(
                        name: "FK_CategoryMenu_Categories_CategoriesCategoryId",
                        column: x => x.CategoriesCategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryMenu_Menus_MenusMenuId",
                        column: x => x.MenusMenuId,
                        principalTable: "Menus",
                        principalColumn: "MenuId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryDish_DishesDishId",
                table: "CategoryDish",
                column: "DishesDishId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryMenu_MenusMenuId",
                table: "CategoryMenu",
                column: "MenusMenuId");
        }
    }
}
