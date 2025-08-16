using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffManagementN.Migrations
{
    /// <inheritdoc />
    public partial class AddImagePathToEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Members",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Employees",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BorrowedDuringShiftId",
                table: "BorrowedBooks",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProcessedByEmployeeId",
                table: "BorrowedBooks",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReceivedByEmployeeId",
                table: "BorrowedBooks",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReturnedDuringShiftId",
                table: "BorrowedBooks",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Books",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BorrowedBooks_BorrowedDuringShiftId",
                table: "BorrowedBooks",
                column: "BorrowedDuringShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowedBooks_ProcessedByEmployeeId",
                table: "BorrowedBooks",
                column: "ProcessedByEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowedBooks_ReceivedByEmployeeId",
                table: "BorrowedBooks",
                column: "ReceivedByEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowedBooks_ReturnedDuringShiftId",
                table: "BorrowedBooks",
                column: "ReturnedDuringShiftId");

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowedBooks_Employees_ProcessedByEmployeeId",
                table: "BorrowedBooks",
                column: "ProcessedByEmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeID");

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowedBooks_Employees_ReceivedByEmployeeId",
                table: "BorrowedBooks",
                column: "ReceivedByEmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeID");

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowedBooks_Shifts_BorrowedDuringShiftId",
                table: "BorrowedBooks",
                column: "BorrowedDuringShiftId",
                principalTable: "Shifts",
                principalColumn: "ShiftID");

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowedBooks_Shifts_ReturnedDuringShiftId",
                table: "BorrowedBooks",
                column: "ReturnedDuringShiftId",
                principalTable: "Shifts",
                principalColumn: "ShiftID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BorrowedBooks_Employees_ProcessedByEmployeeId",
                table: "BorrowedBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_BorrowedBooks_Employees_ReceivedByEmployeeId",
                table: "BorrowedBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_BorrowedBooks_Shifts_BorrowedDuringShiftId",
                table: "BorrowedBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_BorrowedBooks_Shifts_ReturnedDuringShiftId",
                table: "BorrowedBooks");

            migrationBuilder.DropIndex(
                name: "IX_BorrowedBooks_BorrowedDuringShiftId",
                table: "BorrowedBooks");

            migrationBuilder.DropIndex(
                name: "IX_BorrowedBooks_ProcessedByEmployeeId",
                table: "BorrowedBooks");

            migrationBuilder.DropIndex(
                name: "IX_BorrowedBooks_ReceivedByEmployeeId",
                table: "BorrowedBooks");

            migrationBuilder.DropIndex(
                name: "IX_BorrowedBooks_ReturnedDuringShiftId",
                table: "BorrowedBooks");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "BorrowedDuringShiftId",
                table: "BorrowedBooks");

            migrationBuilder.DropColumn(
                name: "ProcessedByEmployeeId",
                table: "BorrowedBooks");

            migrationBuilder.DropColumn(
                name: "ReceivedByEmployeeId",
                table: "BorrowedBooks");

            migrationBuilder.DropColumn(
                name: "ReturnedDuringShiftId",
                table: "BorrowedBooks");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Books");
        }
    }
}
