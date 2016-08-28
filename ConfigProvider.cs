using System;

namespace ExcelToSql
{
    public static class ConfigProvider
    {
        public static string ConnectionString()
        {
            //return ConfigurationManager.ConnectionStrings["destinationDatabaseConnectionString"].ConnectionString;
            return
                @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""C:\Visual Studio Projects\ExcelToSql\ExcelToSql\LocalTestDb.mdf"";Integrated Security=True";
        }

        public static string ExcelQuery()
        {

            return "Select [ID],[Name],[price1],[price2],[price3],[price4],[price5] from";
        }

        public static string DestinationTable()
        {
            return "customers";
        }
    }
}