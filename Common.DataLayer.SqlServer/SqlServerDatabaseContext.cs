using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace Common.DataLayer.SqlServer;

public class SqlServerDatabaseContext : DatabaseContext
{
    #region Constructor
    public SqlServerDatabaseContext(string connectionString) : base(connectionString) { }
    #endregion

    #region Properties
    public int ReturnValue { get; set; }
    #endregion

    #region Initialize Method
    protected override void Initialize()
    {
        base.Initialize();

        CommandObject = new SqlCommand();
        ParameterToken = "@";
    }
    #endregion

    #region Reset Methods
    public override void Reset(CommandType type)
    {
        base.Reset(type);

        CommandObject ??= new SqlCommand
        {
            CommandType = type
        };

        ReturnValue = 0;
    }
    #endregion

    #region CreateConnection Method
    public override IDbConnection CreateConnection(string connectString)
    {
        return new SqlConnection(connectString);
    }
    #endregion

    #region CreateCommand Method
    public override IDbCommand CreateCommand()
    {
        return new SqlCommand();
    }
    #endregion

    #region CreateDataAdapter Method
    public override DbDataAdapter CreateDataAdapter(IDbCommand cmd)
    {
        return new SqlDataAdapter((SqlCommand)cmd);
    }
    #endregion

    #region CreateParameter Methods
    public override IDbDataParameter CreateParameter(string name, object value, bool isNullable)
    {
        name = name.Contains(ParameterToken) ? name : ParameterToken + name;
        return new SqlParameter { ParameterName = name, Value = value, IsNullable = isNullable };
    }

    public override IDbDataParameter CreateParameter(string name, object value, bool isNullable, SqlDbType type, ParameterDirection direction = ParameterDirection.Input)
    {
        name = name.Contains(ParameterToken) ? name : ParameterToken + name;
        return new SqlParameter { ParameterName = name, Value = value, IsNullable = isNullable, SqlDbType = type, Direction = direction };
    }

    public override IDbDataParameter CreateParameter(string name, object value, bool isNullable, SqlDbType type, int size, ParameterDirection direction = ParameterDirection.Input)
    {
        name = name.Contains(ParameterToken) ? name : ParameterToken + name;
        return new SqlParameter { ParameterName = name, Value = value, IsNullable = isNullable, SqlDbType = type, Direction = direction, Size = size };
    }
    #endregion

    #region AddStandardParameters Method
    public override void AddStandardParameters()
    {
        if (CommandObject.CommandType == CommandType.StoredProcedure)
        {
            AddParameter("ReturnValue", 0, false, SqlDbType.Int, ParameterDirection.ReturnValue);
        }
    }
    #endregion

    #region GetOutputParameters Method
    public override void GetStandardOutputParameters()
    {
        if (CommandObject.CommandType == CommandType.StoredProcedure)
        {
            ReturnValue = GetParameterValue<int>("ReturnValue", default(int));
        }
    }
    #endregion

    #region GetParameterValue Method
    public override T GetParameterValue<T>(string name, object defaultValue)
    {
        T ret;
        string? value;

        value = ((SqlParameter)GetParameter(name)).Value.ToString();

        ret = string.IsNullOrEmpty(value) ? (T)defaultValue : (T)Convert.ChangeType(value, typeof(T));

        return ret;
    }
    #endregion

    #region ThrowDbException Method
    public override void ThrowDbException(Exception ex, IDbCommand cmd, string exceptionMsg = "")
    {
        DataException exc;
        exceptionMsg = string.IsNullOrEmpty(exceptionMsg) ? string.Empty : exceptionMsg + " - ";

        if (ex is SqlException)
        {
            exc = new SqlServerDataException(exceptionMsg + ex.Message, ex)
            {
                ConnectionString = cmd.Connection?.ConnectionString,
                Database = cmd.Connection?.Database,
                SQL = SQL,
                CommandParameters = cmd.Parameters,
                WorkstationId = Environment.MachineName
            };
        }
        else
        {
            exc = new DataException(exceptionMsg + ex.Message, ex)
            {
                ConnectionString = cmd.Connection?.ConnectionString,
                Database = cmd.Connection?.Database,
                SQL = SQL,
                CommandParameters = cmd.Parameters,
                WorkstationId = Environment.MachineName
            };
        }

        LastException = exc;

        throw exc;
    }
    #endregion
}