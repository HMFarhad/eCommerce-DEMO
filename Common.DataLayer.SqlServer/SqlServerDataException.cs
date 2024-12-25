using Microsoft.Data.SqlClient;
using System.Text;

namespace Common.DataLayer.SqlServer;

public class SqlServerDataException : DataException
{
    public SqlServerDataException() : base() { }
    public SqlServerDataException(string message) : base(message) { }
    public SqlServerDataException(string message, Exception innerException) : base(message, innerException) { }

    public override string GetDatabaseSpecificError(Exception ex)
    {
        StringBuilder sb = new(1024);

        if (ex is SqlException sqlExp)
        {
            for (int index = 0; index <= sqlExp.Errors.Count - 1; index++)
            {
                sb.AppendLine(new string('*', 40));
                sb.AppendLine("**** BEGIN: SQL Server Exception #" + (index + 1).ToString() + " ****");
                sb.AppendLine("    Type: " + sqlExp.Errors[index].GetType().FullName);
                sb.AppendLine("    Message: " + sqlExp.Errors[index].Message);
                sb.AppendLine("    Source: " + sqlExp.Errors[index].Source);
                sb.AppendLine("    Number: " + sqlExp.Errors[index].Number.ToString());
                sb.AppendLine("    State: " + sqlExp.Errors[index].State.ToString());
                sb.AppendLine("    Class: " + sqlExp.Errors[index].Class.ToString());
                sb.AppendLine("    Server: " + sqlExp.Errors[index].Server);
                sb.AppendLine("    Procedure: " + sqlExp.Errors[index].Procedure);
                sb.AppendLine("    LineNumber: " + sqlExp.Errors[index].LineNumber.ToString());
                sb.AppendLine("**** END: SQL Server Exception #" + (index + 1).ToString() + " ****");
                sb.AppendLine(new string('*', 40));
            }
        }
        else
        {
            sb.Append(ex.Message);
        }

        return sb.ToString();
    }

    public override bool IsDatabaseSpecificError(Exception ex)
    {
        return (ex is SqlException);
    }
}