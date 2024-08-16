using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GarmentFactory.Repository.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Task_AssemblyLines_AssemblyLineId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Task_Orders_OrderId",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Task",
                table: "Tasks");

            migrationBuilder.RenameTable(
                name: "Tasks",
                newName: "Tasks");

            migrationBuilder.RenameIndex(
                name: "IX_Task_AssemblyLineId",
                table: "Tasks",
                newName: "IX_Tasks_AssemblyLineId");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Tasks",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Tasks",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_OrderId",
                table: "Tasks",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssemblyLine_Task",
                table: "Tasks",
                column: "AssemblyLineId",
                principalTable: "AssemblyLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Task",
                table: "Tasks",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssemblyLine_Task",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Task",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_OrderId",
                table: "Tasks");

            migrationBuilder.RenameTable(
                name: "Tasks",
                newName: "Tasks");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_AssemblyLineId",
                table: "Tasks",
                newName: "IX_Task_AssemblyLineId");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Task",
                table: "Tasks",
                columns: new[] { "OrderId", "AssemblyLineId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Task_AssemblyLines_AssemblyLineId",
                table: "Tasks",
                column: "AssemblyLineId",
                principalTable: "AssemblyLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Task_Orders_OrderId",
                table: "Tasks",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
