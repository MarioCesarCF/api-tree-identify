using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SWIA.Migrations
{
    /// <inheritdoc />
    public partial class CreateTreesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Trees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ScientificName = table.Column<string>(type: "text", nullable: false),
                    CommonName = table.Column<string>(type: "text", nullable: true),
                    Family = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    LeafFormat = table.Column<string>(type: "text", nullable: true),
                    LeafMargin = table.Column<string>(type: "text", nullable: true),
                    LeafArrangement = table.Column<string>(type: "text", nullable: true),
                    LeafType = table.Column<string>(type: "text", nullable: true),
                    WoodType = table.Column<string>(type: "text", nullable: true),
                    TrunkType = table.Column<string>(type: "text", nullable: true),
                    CrownShape = table.Column<string>(type: "text", nullable: true),
                    FlowerColor = table.Column<string>(type: "text", nullable: true),
                    FruitType = table.Column<string>(type: "text", nullable: true),
                    HasFlower = table.Column<bool>(type: "boolean", nullable: true),
                    HasFruit = table.Column<bool>(type: "boolean", nullable: true),
                    Biome = table.Column<string>(type: "text", nullable: true),
                    Region = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trees", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trees");
        }
    }
}
