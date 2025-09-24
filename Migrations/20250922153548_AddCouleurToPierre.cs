using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Stone1234.Migrations
{
    /// <inheritdoc />
    public partial class AddCouleurToPierre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "id_utilisateur",
                table: "utilisateur",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateTable(
                name: "carat_",
                columns: table => new
                {
                    id_carat = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    valeur = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_carat_", x => x.id_carat);
                });

            migrationBuilder.CreateTable(
                name: "couleurs",
                columns: table => new
                {
                    id_couleur = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nom_couleur = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_couleurs", x => x.id_couleur);
                });

            migrationBuilder.CreateTable(
                name: "forme",
                columns: table => new
                {
                    id_forme = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nom_forme = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_forme", x => x.id_forme);
                });

            migrationBuilder.CreateTable(
                name: "PierreStockViewModels",
                columns: table => new
                {
                    Id_pierre = table.Column<int>(type: "integer", nullable: false),
                    Nom_pierre = table.Column<string>(type: "text", nullable: true),
                    Prix_vente = table.Column<decimal>(type: "numeric", nullable: false),
                    Qualite = table.Column<string>(type: "text", nullable: true),
                    Carat = table.Column<decimal>(type: "numeric", nullable: false),
                    Forme = table.Column<string>(type: "text", nullable: true),
                    Quantite_totale = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "qualite",
                columns: table => new
                {
                    id_qua = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nom_qualite = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qualite", x => x.id_qua);
                });

            migrationBuilder.CreateTable(
                name: "villes",
                columns: table => new
                {
                    id_villes = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nom_ville = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_villes", x => x.id_villes);
                });

            migrationBuilder.CreateTable(
                name: "pierres",
                columns: table => new
                {
                    id_pierre = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nom_pierre = table.Column<string>(type: "text", nullable: false),
                    prix_vente = table.Column<decimal>(type: "numeric(15,2)", nullable: false),
                    id_qua = table.Column<int>(type: "integer", nullable: false),
                    id_carat = table.Column<int>(type: "integer", nullable: false),
                    id_forme = table.Column<int>(type: "integer", nullable: false),
                    id_couleur = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pierres", x => x.id_pierre);
                    table.ForeignKey(
                        name: "FK_pierres_carat__id_carat",
                        column: x => x.id_carat,
                        principalTable: "carat_",
                        principalColumn: "id_carat",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pierres_couleurs_id_couleur",
                        column: x => x.id_couleur,
                        principalTable: "couleurs",
                        principalColumn: "id_couleur",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pierres_forme_id_forme",
                        column: x => x.id_forme,
                        principalTable: "forme",
                        principalColumn: "id_forme",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pierres_qualite_id_qua",
                        column: x => x.id_qua,
                        principalTable: "qualite",
                        principalColumn: "id_qua",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pavillons",
                columns: table => new
                {
                    id_pavillon = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    matricule = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    nom_pavillon = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    id_villes = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pavillons", x => x.id_pavillon);
                    table.ForeignKey(
                        name: "FK_pavillons_villes_id_villes",
                        column: x => x.id_villes,
                        principalTable: "villes",
                        principalColumn: "id_villes",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mouvement_stock",
                columns: table => new
                {
                    id_mvt = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    date_mouvement = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    type_mouvement = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    total = table.Column<decimal>(type: "numeric(15,2)", nullable: true),
                    id_utilisateur = table.Column<int>(type: "integer", nullable: false),
                    id_pavillon = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mouvement_stock", x => x.id_mvt);
                    table.ForeignKey(
                        name: "FK_mouvement_stock_pavillons_id_pavillon",
                        column: x => x.id_pavillon,
                        principalTable: "pavillons",
                        principalColumn: "id_pavillon",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mouvement_stock_utilisateur_id_utilisateur",
                        column: x => x.id_utilisateur,
                        principalTable: "utilisateur",
                        principalColumn: "id_utilisateur",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mouvement_stock_detail",
                columns: table => new
                {
                    id_mvtdetail = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    quantite = table.Column<decimal>(type: "numeric(15,2)", nullable: false),
                    id_pierre = table.Column<int>(type: "integer", nullable: false),
                    id_mvt = table.Column<int>(type: "integer", nullable: false),
                    prix_unitaire = table.Column<decimal>(type: "numeric(15,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mouvement_stock_detail", x => x.id_mvtdetail);
                    table.ForeignKey(
                        name: "FK_mouvement_stock_detail_mouvement_stock_id_mvt",
                        column: x => x.id_mvt,
                        principalTable: "mouvement_stock",
                        principalColumn: "id_mvt",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mouvement_stock_detail_pierres_id_pierre",
                        column: x => x.id_pierre,
                        principalTable: "pierres",
                        principalColumn: "id_pierre",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mouvement_stock_id_pavillon",
                table: "mouvement_stock",
                column: "id_pavillon");

            migrationBuilder.CreateIndex(
                name: "IX_mouvement_stock_id_utilisateur",
                table: "mouvement_stock",
                column: "id_utilisateur");

            migrationBuilder.CreateIndex(
                name: "IX_mouvement_stock_detail_id_mvt",
                table: "mouvement_stock_detail",
                column: "id_mvt");

            migrationBuilder.CreateIndex(
                name: "IX_mouvement_stock_detail_id_pierre",
                table: "mouvement_stock_detail",
                column: "id_pierre");

            migrationBuilder.CreateIndex(
                name: "IX_pavillons_id_villes",
                table: "pavillons",
                column: "id_villes");

            migrationBuilder.CreateIndex(
                name: "IX_pierres_id_carat",
                table: "pierres",
                column: "id_carat");

            migrationBuilder.CreateIndex(
                name: "IX_pierres_id_couleur",
                table: "pierres",
                column: "id_couleur");

            migrationBuilder.CreateIndex(
                name: "IX_pierres_id_forme",
                table: "pierres",
                column: "id_forme");

            migrationBuilder.CreateIndex(
                name: "IX_pierres_id_qua",
                table: "pierres",
                column: "id_qua");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mouvement_stock_detail");

            migrationBuilder.DropTable(
                name: "PierreStockViewModels");

            migrationBuilder.DropTable(
                name: "mouvement_stock");

            migrationBuilder.DropTable(
                name: "pierres");

            migrationBuilder.DropTable(
                name: "pavillons");

            migrationBuilder.DropTable(
                name: "carat_");

            migrationBuilder.DropTable(
                name: "couleurs");

            migrationBuilder.DropTable(
                name: "forme");

            migrationBuilder.DropTable(
                name: "qualite");

            migrationBuilder.DropTable(
                name: "villes");

            migrationBuilder.AlterColumn<string>(
                name: "id_utilisateur",
                table: "utilisateur",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }
    }
}
