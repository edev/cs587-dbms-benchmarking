using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WisconsinSetup
{
    class QueryManager
    {
        private static readonly string CreateTableSql = @"
            CREATE TABLE {0} (
            	unique1			INT			NOT NULL,
	            unique2			INT			NOT NULL    PRIMARY KEY,
	            two				INT			NOT NULL,
	            four			INT			NOT NULL,
	            ten				INT			NOT NULL,
	            twenty			INT			NOT NULL,
	            onePercent		INT			NOT NULL,
	            tenPercent		INT			NOT NULL,
	            twentyPercent	INT			NOT NULL,
	            fiftyPercent	INT			NOT NULL,
	            unique3			INT			NOT NULL,
	            evenOnePercent	INT			NOT NULL,
	            oddOnePercent	INT			NOT NULL,
	            stringu1		CHAR(53)	NOT NULL,
	            stringu2		CHAR(53)	NOT NULL,
	            string4 		CHAR(53)	NOT NULL,
            );
        ";

        private static readonly string DropTableSql = @"
            DROP TABLE IF EXISTS {0};
        ";

        private static readonly string BulkInsertSql = @"
            BULK INSERT {0}
                FROM '{1}'
                WITH
                (
                    FORMAT = 'CSV'
                )
        ";

        public static string CreateTable(SqlConnection connection, string tableName)
        {
            var query = String.Format(CreateTableSql, tableName);
            var cmd = new SqlCommand(query, connection);
            return _cmdToString("CREATE TABLE", cmd);
        }

        public static string DropTableIfExists(SqlConnection connection, string tableName)
        {
            var query = String.Format(DropTableSql, tableName);
            var cmd = new SqlCommand(query, connection);
            return _cmdToString("DROP TABLE IF EXISTS", cmd);
        }

        public static string BulkInsert(SqlConnection connection, string tableName, string filename)
        {
            var path = Path.GetFullPath(filename);
            var query = String.Format(BulkInsertSql, tableName, path);
            var cmd = new SqlCommand(query, connection) {CommandTimeout = 600}; // Be sure to set a long timeout.
            return _cmdToString("BULK INSERT", cmd);
        }

        private static string _cmdToString(string commandDescription, SqlCommand command)
        {
            try
            {
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == -1) return commandDescription; // rowsAffected is invalid.
                return $"{commandDescription}: {rowsAffected} rows affected.";
            }
            catch (SqlException sqe)
            {
                var errorString = new StringBuilder();
                foreach (SqlError ex in sqe.Errors)
                {
                    errorString.AppendLine(ex.Message);
                }
                return $"{commandDescription}:\n{errorString}";
            }
            catch (Exception e)
            {
                return $"{commandDescription}: {e.Message}";
            }
        }
    }
}
