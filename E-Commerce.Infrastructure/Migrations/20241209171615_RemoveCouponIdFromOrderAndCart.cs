using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCouponIdFromOrderAndCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Supprimer la clé étrangère de la table OrderHeaders
            migrationBuilder.DropForeignKey(
                name: "FK_OrderHeaders_Coupons_CouponId",
                table: "OrderHeaders");

            // Supprimer la clé étrangère de la table CartHeaders
            migrationBuilder.DropForeignKey(
                name: "FK_CartHeaders_Coupons_CouponId",
                table: "CartHeaders");

            // Supprimer la colonne CouponId de la table OrderHeaders
            migrationBuilder.DropColumn(
                name: "CouponId",
                table: "OrderHeaders");

            // Supprimer la colonne CouponId de la table CartHeaders
            migrationBuilder.DropColumn(
                name: "CouponId",
                table: "CartHeaders");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Ajouter à nouveau la colonne CouponId dans la table OrderHeaders
            migrationBuilder.AddColumn<int>(
                name: "CouponId",
                table: "OrderHeaders",
                type: "int",
                nullable: true);

            // Ajouter à nouveau la colonne CouponId dans la table CartHeaders
            migrationBuilder.AddColumn<int>(
                name: "CouponId",
                table: "CartHeaders",
                type: "int",
                nullable: true);

            // Recréer la clé étrangère dans la table OrderHeaders
            migrationBuilder.AddForeignKey(
                name: "FK_OrderHeaders_Coupons_CouponId",
                table: "OrderHeaders",
                column: "CouponId",
                principalTable: "Coupons",
                principalColumn: "CouponId");

            // Recréer la clé étrangère dans la table CartHeaders
            migrationBuilder.AddForeignKey(
                name: "FK_CartHeaders_Coupons_CouponId",
                table: "CartHeaders",
                column: "CouponId",
                principalTable: "Coupons",
                principalColumn: "CouponId");
        }

    }
}
