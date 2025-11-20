using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace KitKap.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Seed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Abouts",
                keyColumn: "AboutId",
                keyValue: 1,
                columns: new[] { "Address", "Description", "Email", "Phone" },
                values: new object[] { "Türkiye", "KitKap, 2025 yılında kurulan ve ikinci el kitap alışverişini kolaylaştırmayı hedefleyen modern bir e-ticaret platformudur. \r\n\r\nMisyonumuz, kitap severlerin okudukları kitapları paylaşmalarını ve yeni kitaplara ulaşmalarını kolaylaştırmaktır. Sürdürülebilir okuma alışkanlıkları geliştirerek hem çevreye katkı sağlamayı hem de kitap kültürünü yaygınlaştırmayı amaçlıyoruz.\r\n\r\nKitKap olarak, her kitabın yeni bir okuyucuya ulaşması gerektiğine inanıyoruz. Platformumuz, güvenli ve kullanıcı dostu yapısıyla kitap alım-satımını herkes için erişilebilir hale getiriyor.", "info@kitkap.com", "+90 (543) 905 71 36" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "Description", "Name" },
                values: new object[] { new DateTime(2025, 11, 20, 13, 54, 52, 45, DateTimeKind.Local).AddTicks(7773), "Roman, hikaye, şiir ve edebiyat eserleri", "Edebiyat" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "Description", "Name" },
                values: new object[] { new DateTime(2025, 11, 20, 13, 54, 52, 45, DateTimeKind.Local).AddTicks(7787), "Bilim, teknoloji ve mühendislik kitapları", "Bilim ve Teknoloji" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "Description", "Name", "ParentCategoryId" },
                values: new object[] { new DateTime(2025, 11, 20, 13, 54, 52, 45, DateTimeKind.Local).AddTicks(7788), "Kişisel gelişim ve motivasyon kitapları", "Kişisel Gelişim", null });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedDate", "Description", "IsDeleted", "Name", "ParentCategoryId" },
                values: new object[,]
                {
                    { 4, new DateTime(2025, 11, 20, 13, 54, 52, 45, DateTimeKind.Local).AddTicks(7789), "Tarih ve biyografi kitapları", false, "Tarih", null },
                    { 5, new DateTime(2025, 11, 20, 13, 54, 52, 45, DateTimeKind.Local).AddTicks(7790), "Çocuklar için hikaye ve eğitim kitapları", false, "Çocuk Kitapları", null },
                    { 6, new DateTime(2025, 11, 20, 13, 54, 52, 45, DateTimeKind.Local).AddTicks(7791), "Türk ve dünya romanları", false, "Roman", 1 },
                    { 7, new DateTime(2025, 11, 20, 13, 54, 52, 45, DateTimeKind.Local).AddTicks(7792), "Kısa hikaye koleksiyonları", false, "Hikaye", 1 },
                    { 8, new DateTime(2025, 11, 20, 13, 54, 52, 45, DateTimeKind.Local).AddTicks(7793), "Şiir kitapları", false, "Şiir", 1 },
                    { 9, new DateTime(2025, 11, 20, 13, 54, 52, 45, DateTimeKind.Local).AddTicks(7794), "Dünya klasikleri", false, "Klasikler", 1 },
                    { 10, new DateTime(2025, 11, 20, 13, 54, 52, 45, DateTimeKind.Local).AddTicks(7795), "Programlama ve yazılım geliştirme", false, "Yazılım", 2 },
                    { 11, new DateTime(2025, 11, 20, 13, 54, 52, 45, DateTimeKind.Local).AddTicks(7796), "Bilim ve araştırma kitapları", false, "Bilim", 2 },
                    { 12, new DateTime(2025, 11, 20, 13, 54, 52, 45, DateTimeKind.Local).AddTicks(7797), "Matematik kitapları", false, "Matematik", 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.UpdateData(
                table: "Abouts",
                keyColumn: "AboutId",
                keyValue: 1,
                columns: new[] { "Address", "Description", "Email", "Phone" },
                values: new object[] { "dsd", "as", "sdas", "sdwq" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "Description", "Name" },
                values: new object[] { new DateTime(2025, 11, 18, 17, 33, 31, 821, DateTimeKind.Local).AddTicks(9733), "Kitapların olduğu kategori", "Kitap" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "Description", "Name" },
                values: new object[] { new DateTime(2025, 11, 18, 17, 33, 31, 821, DateTimeKind.Local).AddTicks(9744), "Teknolojilerin  olduğu kategori", "Teknoloji" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "Description", "Name", "ParentCategoryId" },
                values: new object[] { new DateTime(2025, 11, 18, 17, 33, 31, 821, DateTimeKind.Local).AddTicks(9745), "Romanların olduğu kategori", "Roman", 1 });
        }
    }
}
