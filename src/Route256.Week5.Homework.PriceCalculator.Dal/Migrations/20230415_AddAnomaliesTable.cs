using FluentMigrator;

namespace Route256.Week5.Workshop.PriceCalculator.Dal.Migrations;

[Migration(20230415, TransactionBehavior.None)]
public class AddAnomaliesTable : Migration {
    public override void Up()
    {
        Create.Table("anomalies")
            .WithColumn("id").AsInt64().PrimaryKey("anomalies_pk").Identity()
            .WithColumn("good_id").AsInt64().NotNullable()
            .WithColumn("price").AsDecimal(34, 5).NotNullable();
    }

    public override void Down()
    {
        Delete.Table("anomalies");
    }
}