using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffManagementN.Migrations
{
    /// <inheritdoc />
    public partial class remove_direct_m2m : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_Employees_EmployeeModelEmployeeID",
                table: "Shifts");

            migrationBuilder.DropIndex(
                name: "IX_Shifts_EmployeeModelEmployeeID",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "EmployeeModelEmployeeID",
                table: "Shifts");

            migrationBuilder.CreateTable(
                name: "EmployeeShiftModel",
                columns: table => new
                {
                    EmployeeShiftID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeID = table.Column<int>(type: "INTEGER", nullable: false),
                    ShiftID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeShiftModel", x => x.EmployeeShiftID);
                    table.ForeignKey(
                        name: "FK_EmployeeShiftModel_Employees_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeShiftModel_Shifts_ShiftID",
                        column: x => x.ShiftID,
                        principalTable: "Shifts",
                        principalColumn: "ShiftID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeShiftModel_EmployeeID",
                table: "EmployeeShiftModel",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeShiftModel_ShiftID",
                table: "EmployeeShiftModel",
                column: "ShiftID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeShiftModel");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeModelEmployeeID",
                table: "Shifts",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_EmployeeModelEmployeeID",
                table: "Shifts",
                column: "EmployeeModelEmployeeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_Employees_EmployeeModelEmployeeID",
                table: "Shifts",
                column: "EmployeeModelEmployeeID",
                principalTable: "Employees",
                principalColumn: "EmployeeID");
        }
    }
}
