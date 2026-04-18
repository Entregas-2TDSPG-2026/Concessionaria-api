using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AutomoveisVendasApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Carros",
                columns: table => new
                {
                    CarroId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Modelo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Marca = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Ano = table.Column<int>(type: "INTEGER", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Placa = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Vendido = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carros", x => x.CarroId);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    ClienteId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Telefone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.ClienteId);
                });

            migrationBuilder.CreateTable(
                name: "Motos",
                columns: table => new
                {
                    MotoId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Modelo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Marca = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Ano = table.Column<int>(type: "INTEGER", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Vendida = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Motos", x => x.MotoId);
                });

            migrationBuilder.CreateTable(
                name: "Vendas",
                columns: table => new
                {
                    VendaId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClienteId = table.Column<int>(type: "INTEGER", nullable: false),
                    CarroId = table.Column<int>(type: "INTEGER", nullable: true),
                    MotoId = table.Column<int>(type: "INTEGER", nullable: true),
                    DataVenda = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, defaultValue: "Pendente")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendas", x => x.VendaId);
                    table.ForeignKey(
                        name: "FK_Vendas_Carros_CarroId",
                        column: x => x.CarroId,
                        principalTable: "Carros",
                        principalColumn: "CarroId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vendas_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "ClienteId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vendas_Motos_MotoId",
                        column: x => x.MotoId,
                        principalTable: "Motos",
                        principalColumn: "MotoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pagamentos",
                columns: table => new
                {
                    PagamentoId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VendaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataPagamento = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagamentos", x => x.PagamentoId);
                    table.ForeignKey(
                        name: "FK_Pagamentos_Vendas_VendaId",
                        column: x => x.VendaId,
                        principalTable: "Vendas",
                        principalColumn: "VendaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Carros",
                columns: new[] { "CarroId", "Ano", "Marca", "Modelo", "Placa", "Valor", "Vendido" },
                values: new object[] { 1, 2022, "Honda", "Civic", "ABC-1234", 95000m, true });

            migrationBuilder.InsertData(
                table: "Carros",
                columns: new[] { "CarroId", "Ano", "Marca", "Modelo", "Placa", "Valor" },
                values: new object[] { 2, 2023, "Toyota", "Corolla", "XYZ-5678", 110000m });

            migrationBuilder.InsertData(
                table: "Clientes",
                columns: new[] { "ClienteId", "Email", "Nome", "Telefone" },
                values: new object[,]
                {
                    { 1, "joao@email.com", "João Silva", "11999990001" },
                    { 2, "maria@email.com", "Maria Souza", "11999990002" }
                });

            migrationBuilder.InsertData(
                table: "Motos",
                columns: new[] { "MotoId", "Ano", "Marca", "Modelo", "Valor" },
                values: new object[,]
                {
                    { 1, 2022, "Kawasaki", "Ninja 400", 35000m },
                    { 2, 2023, "Honda", "CB 500F", 30000m }
                });

            migrationBuilder.InsertData(
                table: "Vendas",
                columns: new[] { "VendaId", "CarroId", "ClienteId", "DataVenda", "MotoId", "Status", "ValorTotal" },
                values: new object[] { 1, 1, 1, new DateTime(2024, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), null, "Finalizada", 95000m });

            migrationBuilder.InsertData(
                table: "Pagamentos",
                columns: new[] { "PagamentoId", "DataPagamento", "Tipo", "Valor", "VendaId" },
                values: new object[] { 1, new DateTime(2024, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Financiamento", 95000m, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Carros_Placa",
                table: "Carros",
                column: "Placa",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Email",
                table: "Clientes",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_VendaId",
                table: "Pagamentos",
                column: "VendaId");

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_CarroId",
                table: "Vendas",
                column: "CarroId");

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_ClienteId",
                table: "Vendas",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_MotoId",
                table: "Vendas",
                column: "MotoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pagamentos");

            migrationBuilder.DropTable(
                name: "Vendas");

            migrationBuilder.DropTable(
                name: "Carros");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Motos");
        }
    }
}
