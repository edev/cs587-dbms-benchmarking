using System;
using System.Collections.Generic;
using SQC = System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WisconsinSetup
{
    class TableManager
    {
        private const string CreateTableSql = @"
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

        private const string DropTableSql = @"
            DROP TABLE IF EXISTS {0};
        ";

        private const string BulkInsertSql = @"
            BULK INSERT {0}
                FROM '{1}'
                WITH
                (
                    FORMAT = 'CSV'
                )
        ";

        private const string InsertRowSql = @"
            INSERT INTO {0} VALUES ({1});
        ";

        private const string LogIndent = "    ";

        private readonly SQC.SqlConnection _connection;

        public TableManager(SQC.SqlConnection connection)
        {
            _connection = connection;
        }

        public string CreateTable(string tableName)
        {
            var query = String.Format(CreateTableSql, tableName);
            var cmd = new SQC.SqlCommand(query, _connection);
            return _cmdToString("CREATE TABLE", cmd);
        }

        public string DropTableIfExists(string tableName)
        {
            var query = String.Format(DropTableSql, tableName);
            var cmd = new SQC.SqlCommand(query, _connection);
            return _cmdToString("DROP TABLE IF EXISTS", cmd);
        }

        // Doesn't work correctly. Consider deleting.
        public string BulkInsert(string tableName, string filename)
        {
            var path = Path.GetFullPath(filename);
            var query = String.Format(BulkInsertSql, tableName, path);
            var cmd = new SQC.SqlCommand(query, _connection);
            return _cmdToString("BULK INSERT", cmd);
        }

        public string InsertRows(Relation relation)
        {
            long rowsAffected = 0;
            int errorCount = 0;
            StringBuilder errors = new StringBuilder();
            foreach (Record record in relation)
            {
                var query = String.Format(InsertRowSql, relation.TableName, record.InsertValues);
                var cmd = new SQC.SqlCommand(query, _connection);
                try
                {
                    rowsAffected += cmd.ExecuteNonQuery();
                }
                catch (SQC.SqlException sqe)
                {
                    foreach (SQC.SqlError ex in sqe.Errors)
                    {
                        errors.AppendLine($"{LogIndent}{LogIndent}{ex.Message}");
                    }
                    errorCount++;
                    if (errorCount >= 10)
                    {
                        errors.AppendLine("Encountered 10 errors. Aborting.");
                        break;
                    }
                }
            }
            errors.AppendLine($"{rowsAffected} rows inserted.");

            return errors.ToString();
        }

        private string _cmdToString(string commandDescription, SQC.SqlCommand command)
        {
            try
            {
                int rowsAffected = command.ExecuteNonQuery();
                return $"{LogIndent}{commandDescription}: {rowsAffected} rows affected.";
            }
            catch (SQC.SqlException sqe)
            {
                var errorString = new StringBuilder();
                foreach (SQC.SqlError ex in sqe.Errors)
                {
                    errorString.AppendLine($"{LogIndent}{LogIndent}{ex.Message}");
                }
                return $"{LogIndent}{commandDescription}:\n{errorString.ToString()}";
            }
            catch (Exception e)
            {
                return $"{LogIndent}{commandDescription}: {e.Message}";
            }
        }
    }
}
