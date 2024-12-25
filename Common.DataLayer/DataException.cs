using System.Data;

namespace Common.DataLayer;

#nullable disable
public class DataException : Exception
{
    public DataException() : base() { }
    public DataException(string message) : base(message) { }
    public DataException(string message, Exception innerException) : base(message, innerException) { }

    public IDataParameterCollection CommandParameters { get; set; }
    public string SQL { get; set; }

    private string _ConnectionString = string.Empty;
    public string ConnectionString
    {
        get { return HideLoginInfoForConnectionString(_ConnectionString); }
        set { _ConnectionString = value; }
    }
    public string Database { get; set; }
    public string WorkstationId { get; set; }

    public virtual string HideLoginInfoForConnectionString(string connectString)
    {
        int index;
        string[] values;

        connectString = connectString.Trim();
        if (connectString.Length > 0)
        {
            if (!(connectString.EndsWith(";")))
            {
                connectString += ";";
            }

            values = connectString.Split(';');
            for (index = 0; index <= values.Length - 1; index++)
            {
                if (values[index].ToLower().Contains("uid=", StringComparison.CurrentCulture))
                {
                    values[index] = "uid=***********";
                }
                if (values[index].ToLower().Contains("user id=", StringComparison.CurrentCulture))
                {
                    values[index] = "user id=***********";
                }
                if (values[index].ToLower().Contains("pwd=", StringComparison.CurrentCulture))
                {
                    values[index] = "pwd=***********";
                }
                if (values[index].ToLower().Contains("password=", StringComparison.CurrentCulture))
                {
                    values[index] = "password=***********";
                }
            }

            connectString = string.Join(";", values);
        }

        return connectString;
    }

    public virtual string GetDatabaseSpecificError(Exception ex)
    {
        return string.Empty;
    }

    public virtual bool IsDatabaseSpecificError(Exception ex)
    {
        return false;
    }


}