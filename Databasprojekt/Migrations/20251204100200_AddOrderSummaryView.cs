using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
/* RADERA KOMMENTARER INNAN INLÄMNING
 *En view är en sparad SELECT-fråga i db.
 * - Förenklar komplexa JOINs
 * - Ger oss färdiga summeringar
 * - Slipper skriva samma SQL om och om igen
 * - Säker visning av information och prestandan blir bättre
 */
namespace Databasprojekt.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderSummaryView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE VIEW IF NOT EXISTS OrderSummaryView AS
            SELECT
                o.OrderId,
                o.OrderDate,
                c.Name AS CustomerName,
                c.Email AS CustomerEmail,
                IFNULL(SUM(orw.Quantity * orw.UnitPrice), 0) AS TotalAmount
            FROM Orders o
            JOIN Customers c on c.CustomerId = o.CustomerId
            LEFT JOIN OrderRows orw ON orw.OrderId = o.OrderId
            GROUP BY o.OrderId, o.OrderDate, c.Name, c.Email;
        END;
            ");
            
            // AFTER INSERT
            migrationBuilder.Sql(@"
            CREATE TRIGGER IF NOT EXISTS trg_OrderRow_Insert
            AFTER INSERT ON OrderRows
            BEGIN
            UPDATE Orders
                SET TotalAmount = (
                                    SELECT IFNULL(SUM(Quantity * UnitPrice), 0) 
                                    FROM OrderRows 
                                    WHERE OrderId = NEW.OrderId
                                  )   
                               WHERE OrderId = NEW.OrderId;  
            END;                              
            ");
            // AFTER UPDATE
            migrationBuilder.Sql(@"
            CREATE TRIGGER IF NOT EXISTS trg_OrderRow_Update
            AFTER UPDATE ON OrderRows
            BEGIN
            UPDATE Orders
                SET TotalAmount = (  
                                    SELECT IFNULL(SUM(Quantity * UnitPrice), 0)
                                    FROM OrderRows 
                                    WHERE OrderId = NEW.OrderId
                                  )
                                WHERE OrderId = NEW.OrderId;
            END;
            ");
            // AFTER DELETE
            migrationBuilder.Sql(@"
            CREATE TRIGGER IF NOT EXISTS trg_OrderRow_Delete
            AFTER DELETE ON OrderRows
            BEGIN
                UPDATE Orders
                SET TotalAmount = (
                                    SELECT IFNULL(SUM(Quantity * UnitPrice), 0)
                                    FROM OrderRows
                                    Where OrderId = NEW.OrderId
                                  )
                                WHERE OrderId = NEW.OrderId;
            END;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            DROP VIEW  IF EXISTS OrderSummaryView;
            ");

            migrationBuilder.Sql(@"
            DROP TRIGGER IF EXISTS trg_OrderRow_Insert
            ");
            
            migrationBuilder.Sql(@"
            DROP TRIGGER IF EXISTS trg_OrderRow_Update
            ");
            
            migrationBuilder.Sql(@"
            DROP TRIGGER IF EXISTS trg_OrderRow_Delete
            ");
        }
    }
}
