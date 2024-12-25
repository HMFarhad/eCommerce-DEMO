using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace Common.DataLayer;

#nullable disable
public abstract class DatabaseContext : IDisposable
{
    #region Constructor
    public DatabaseContext(string connectionString)
    {
        ConnectionString = connectionString;
        Initialize();
    }
    #endregion

    #region Properties
    private Exception _LastException = null;
    private int _RowsAffected = 0;
    private string _LastExceptionMessage = string.Empty;

    public Exception LastException
    {
        get { return _LastException; }
        set
        {
            _LastException = value;
            LastExceptionMessage = (value == null ? string.Empty : value.Message);
        }
    }

    public string LastExceptionMessage
    {
        get { return _LastExceptionMessage; }
        set
        {
            _LastExceptionMessage = value;
        }
    }

    public int RowsAffected
    {
        get { return _RowsAffected; }
        set
        {
            _RowsAffected = value;
        }
    }

    public IDbCommand CommandObject { get; set; }
    public DataSet DataSetObject { get; set; }
    public string ConnectionString { get; set; }
    public object IdentityGenerated { get; set; }
    public string ParameterToken { get; set; }
    public string SQL { get; set; }
    public bool IsInTransaction { get; set; }
    #endregion

    #region Initialize Method
    protected virtual void Initialize()
    {
        ParameterToken = "@";
        IdentityGenerated = null;
        SQL = string.Empty;
    }
    #endregion

    #region Reset Method
    public virtual void Reset()
    {
        Reset(CommandType.Text);
    }

    public virtual void Reset(CommandType type)
    {
        if (CommandObject != null)
        {
            CommandObject.CommandText = string.Empty;
            CommandObject.CommandType = type;
            CommandObject.Parameters.Clear();
        }
        LastExceptionMessage = string.Empty;
        LastException = null;
        RowsAffected = 0;
        IdentityGenerated = null;
        SQL = string.Empty;
    }
    #endregion

    #region CreateParameter Methods
    public abstract IDbDataParameter CreateParameter(string name, object value, bool isNullable);
    public abstract IDbDataParameter CreateParameter(string name, object value, bool isNullable, SqlDbType type, ParameterDirection direction = ParameterDirection.Input);
    public abstract IDbDataParameter CreateParameter(string name, object value, bool isNullable, SqlDbType type, int size, ParameterDirection direction = ParameterDirection.Input);
    #endregion

    #region AddParameter Methods
    public virtual void AddParameter(string name, object value, bool isNullable)
    {
        CommandObject.Parameters.Add(CreateParameter(name, value, isNullable));
    }

    public virtual void AddParameter(string name, object value, bool isNullable, SqlDbType type, ParameterDirection direction = ParameterDirection.Input)
    {
        CommandObject.Parameters.Add(CreateParameter(name, value, isNullable, type, direction));
    }

    public virtual void AddParameter(string name, object value, bool isNullable, SqlDbType type, int size, ParameterDirection direction = ParameterDirection.Input)
    {
        CommandObject.Parameters.Add(CreateParameter(name, value, isNullable, type, size, direction));
    }
    #endregion

    #region GetParameter Method
    public virtual IDataParameter GetParameter(string name)
    {
        if (!name.Contains(ParameterToken))
        {
            name = ParameterToken + name;
        }

        return (IDataParameter)CommandObject.Parameters[name];
    }
    #endregion

    #region GetParameterValue Method
    public abstract T GetParameterValue<T>(string name, object defaultValue);
    #endregion

    #region GetIdentityValue Method
    public virtual T GetIdentityValue<T>()
    {
        return GetIdentityValue<T>((T)default);
    }

    public virtual T GetIdentityValue<T>(object defaultValue)
    {
        T ret = (T)defaultValue;

        if (IdentityGenerated != null)
        {
            ret = (T)Convert.ChangeType(IdentityGenerated, typeof(T));
        }

        return ret;
    }
    #endregion

    #region GetRecords Methods
    public virtual List<T> GetRecords<T>(string sql)
    {
        return GetRecords<T>(sql, CommandType.Text, null);
    }

    public virtual List<T> GetRecords<T>(string sql, params IDbDataParameter[] parameters)
    {
        return GetRecords<T>(sql, CommandType.Text, parameters);
    }

    public virtual List<T> GetRecords<T>(string sql, CommandType type)
    {
        return GetRecords<T>(sql, type, null);
    }

    public virtual List<T> GetRecords<T>(string sql, CommandType type, params IDbDataParameter[] parameters)
    {
        List<T> ret = default;

        // Reset all properties
        Reset(type);

        // Initialize the SQL statement
        SQL = sql;

        // Add any standard parameters
        AddStandardParameters();

        if (parameters != null)
        {
            // Add any custom parameters
            for (int i = 0; i < parameters.Length; i++)
            {
                CommandObject.Parameters.Add(parameters[i]);
            }
        }

        // Execute Query
        using (IDataReader dr = GetDataReader())
        {
            // Use reflection to load Product data
            ret = ToList<T>(dr);
            // Set rows returned
            RowsAffected = ret.Count;
        }

        // Get standard output parameters
        GetStandardOutputParameters();

        return ret;
    }

    public virtual List<T> GetZeroIndexRecords<T>(string sql, CommandType type, params IDbDataParameter[] parameters)
    {
        List<T> ret = default;

        // Reset all properties
        Reset(type);

        // Initialize the SQL statement
        SQL = sql;

        // Add any standard parameters
        AddStandardParameters();

        if (parameters != null)
        {
            // Add any custom parameters
            for (int i = 0; i < parameters.Length; i++)
            {
                CommandObject.Parameters.Add(parameters[i]);
            }
        }

        // Execute Query
        using (IDataReader dr = GetDataReader())
        {
            // Use reflection to load Product data
            ret = ZeroIndexToList<T>(dr);
            // Set rows returned
            RowsAffected = ret.Count;
        }

        // Get standard output parameters
        GetStandardOutputParameters();

        return ret;
    }

    #endregion

    #region GetRecordsUsingDataSet Method
    public virtual List<T> GetRecordsUsingDataSet<T>(string sql)
    {
        return GetRecordsUsingDataSet<T>(sql, CommandType.Text, null);
    }

    public virtual List<T> GetRecordsUsingDataSet<T>(string sql, params IDbDataParameter[] parameters)
    {
        return GetRecordsUsingDataSet<T>(sql, CommandType.Text, parameters);
    }

    public virtual List<T> GetRecordsUsingDataSet<T>(string sql, CommandType type)
    {
        return GetRecordsUsingDataSet<T>(sql, type, null);
    }

    public virtual List<T> GetRecordsUsingDataSet<T>(string sql, CommandType type, params IDbDataParameter[] parameters)
    {
        List<T> ret;

        // Reset all properties
        Reset(type);

        // Assign SQL statement to use
        SQL = sql;

        // Add any standard parameters
        AddStandardParameters();

        if (parameters != null)
        {
            // Add any custom parameters
            for (int i = 0; i < parameters.Length; i++)
            {
                CommandObject.Parameters.Add(parameters[i]);
            }
        }

        // Execute Query
        DataSetObject = GetDataSet();

        ret = ToList<T>(DataSetObject.Tables[0]);
        RowsAffected = ret.Count;

        // Get standard output parameters
        GetStandardOutputParameters();

        return ret;
    }
    #endregion

    #region GetRecord Methods
    public virtual T GetRecord<T>(string sql) where T : new()
    {
        return GetRecord<T>(sql, CommandType.Text, null);
    }

    public virtual T GetRecord<T>(string sql, params IDbDataParameter[] parameters) where T : new()
    {
        return GetRecord<T>(sql, CommandType.Text, parameters);
    }

    public virtual T GetRecord<T>(string sql, CommandType type) where T : new()
    {
        return GetRecord<T>(sql, type, null);
    }

    public virtual T GetRecord<T>(string sql, CommandType type, params IDbDataParameter[] parameters) where T : new()
    {
        T ret = default;

        // Reset all properties
        Reset(type);

        // Create SQL to call stored procedure
        SQL = sql;

        if (parameters != null)
        {
            // Add any custom parameters
            for (int i = 0; i < parameters.Length; i++)
            {
                CommandObject.Parameters.Add(parameters[i]);
            }
        }

        // Add any standard parameters
        AddStandardParameters();

        // Execute Query
        using (IDataReader dr = GetDataReader())
        {
            // Use reflection to load Product data
            List<T> list = ToList<T>(dr);
            if (list.Count > 0)
            {
                // Assign return value
                ret = list[0];
                RowsAffected = list.Count;
            }
        }

        // Get standard output parameters
        GetStandardOutputParameters();

        return ret;
    }
    #endregion

    #region GetRecordUsingDataSet Method
    public virtual T GetRecordUsingDataSet<T>(string sql) where T : new()
    {
        return GetRecordUsingDataSet<T>(sql, CommandType.Text, null);
    }

    public virtual T GetRecordUsingDataSet<T>(string sql, params IDbDataParameter[] parameters) where T : new()
    {
        return GetRecordUsingDataSet<T>(sql, CommandType.Text, parameters);
    }

    public virtual T GetRecordUsingDataSet<T>(string sql, CommandType type) where T : new()
    {
        return GetRecordUsingDataSet<T>(sql, type, null);
    }

    public virtual T GetRecordUsingDataSet<T>(string sql, CommandType type, params IDbDataParameter[] parameters) where T : new()
    {
        T ret = new();

        // Reset all properties
        Reset(type);

        // Assign SQL statement to use
        SQL = sql;

        // Add any standard parameters
        AddStandardParameters();

        if (parameters != null)
        {
            // Add any custom parameters
            for (int i = 0; i < parameters.Length; i++)
            {
                CommandObject.Parameters.Add(parameters[i]);
            }
        }

        // Execute Query
        DataSetObject = GetDataSet();
        if (DataSetObject.Tables.Count > 0)
        {
            List<T> list = ToList<T>(DataSetObject.Tables[0]);
            if (list.Count > 0)
            {
                ret = list[0];
                RowsAffected = list.Count;
            }
        }

        // Get standard output parameters
        GetStandardOutputParameters();

        return ret;
    }
    #endregion

    #region CountRecords Methods
    public virtual int CountRecords(string sql)
    {
        return CountRecords(sql, CommandType.Text);
    }

    public virtual int CountRecords(string sql, params IDbDataParameter[] parameters)
    {
        return CountRecords(sql, CommandType.Text, parameters);
    }

    public virtual int CountRecords(string sql, CommandType type)
    {
        return CountRecords(sql, type, null);
    }

    public virtual int CountRecords(string sql, CommandType type, params IDbDataParameter[] parameters)
    {
        // Reset all properties
        Reset(type);

        // Create SQL to count all records
        SQL = sql;

        if (parameters != null)
        {
            // Add any custom parameters
            for (int i = 0; i < parameters.Length; i++)
            {
                CommandObject.Parameters.Add(parameters[i]);
            }
        }

        // Get the count of the records
        RowsAffected = ExecuteScalar<int>();

        return RowsAffected;
    }
    #endregion

    #region CreateConnection Methods
    public virtual IDbConnection CreateConnection()
    {
        //SetConnectionString(ConnectStringName);

        return CreateConnection(ConnectionString);
    }

    public abstract IDbConnection CreateConnection(string connectString);
    #endregion

    #region CreateCommand Methods    
    public abstract IDbCommand CreateCommand();
    #endregion

    #region CreateDataAdapter Methods   
    public abstract DbDataAdapter CreateDataAdapter(IDbCommand cmd);
    #endregion

    #region BeginTransaction Method
    public IDbTransaction BeginTransaction()
    {
        IsInTransaction = true;
        // Check Command Object
        CheckCommand(CommandObject);

        // Check if connection is open
        if (CommandObject.Connection.State != ConnectionState.Open)
        {
            CommandObject.Connection.Open();
        }

        // Set transaction
        CommandObject.Transaction = CommandObject.Connection.BeginTransaction();

        return CommandObject.Transaction;
    }
    #endregion

    #region Commit Method
    public void Commit()
    {
        IsInTransaction = false;
        CommandObject.Transaction.Commit();
    }
    #endregion

    #region Rollback Method
    public void Rollback()
    {
        IsInTransaction = false;
        CommandObject.Transaction.Rollback();
    }
    #endregion

    #region CheckCommand Method
    protected virtual void CheckCommand(IDbCommand cmd)
    {
        cmd ??= CreateCommand();

        cmd.Connection ??= CreateConnection();

        if (string.IsNullOrEmpty(cmd.CommandText))
        {
            cmd.CommandText = SQL;
        }
    }
    #endregion

    #region ExecuteScalar Methods
    public virtual T ExecuteScalar<T>(string exceptionMsg = "", T defaultValue = default)
    {
        return ExecuteScalar(CommandObject, exceptionMsg, defaultValue);
    }

    public virtual T ExecuteScalar<T>(IDbCommand cmd, string exceptionMsg = "", T defaultValue = default)
    {
        bool isConnectionOpen;
        T ret = defaultValue;

        RowsAffected = 0;
        IdentityGenerated = null;
        try
        {
            // Ensure Command object is correct
            CheckCommand(cmd);

            // Check for open connection
            isConnectionOpen = (cmd.Connection.State == ConnectionState.Open);

            // Open connection if not open
            if (!isConnectionOpen)
            {
                cmd.Connection.Open();
            }

            // Execute the SQL
            ret = (T)cmd.ExecuteScalar();

            // Close connection if it was not open originally
            if (!isConnectionOpen)
            {
                cmd.Connection.Close();
            }
        }
        catch (Exception ex)
        {
            ThrowDbException(ex, cmd, exceptionMsg);
        }

        return ret;
    }
    #endregion

    #region ExecuteNonQuery Methods
    public virtual int ExecuteNonQuery()
    {
        return ExecuteNonQuery(CommandObject, false, string.Empty, string.Empty);
    }

    public virtual int ExecuteNonQuery(string exceptionMsg)
    {
        return ExecuteNonQuery(CommandObject, false, string.Empty, exceptionMsg);
    }

    public virtual int ExecuteNonQuery(bool retrieveIdentity)
    {
        return ExecuteNonQuery(CommandObject, retrieveIdentity, string.Empty, string.Empty);
    }

    public virtual int ExecuteNonQuery(bool retrieveIdentity, string exceptionMsg = "")
    {
        return ExecuteNonQuery(CommandObject, retrieveIdentity, string.Empty, exceptionMsg);
    }

    public virtual int ExecuteNonQuery(bool retrieveIdentity, string identityParamName = "", string exceptionMsg = "")
    {
        return ExecuteNonQuery(CommandObject, retrieveIdentity, identityParamName, exceptionMsg);
    }

    public virtual int ExecuteNonQuery(IDbCommand cmd, bool retrieveIdentity = false, string identityParamName = "", string exceptionMsg = "")
    {
        bool isConnectionOpen;

        RowsAffected = 0;
        IdentityGenerated = null;
        try
        {
            // Ensure Command object is correct
            CheckCommand(cmd);

            // Check for open connection
            isConnectionOpen = (cmd.Connection.State == ConnectionState.Open);

            // Open connection if not open
            if (!isConnectionOpen)
            {
                cmd.Connection.Open();
            }

            if (retrieveIdentity)
            {
                if (string.IsNullOrEmpty(identityParamName))
                {
                    // Use a DataSet if retrieving IDENTITY and using Dynamic SQL
                    RowsAffected = ExecuteNonQueryUsingDataSet(cmd);
                }
                else
                {
                    // Execute the SQL
                    RowsAffected = cmd.ExecuteNonQuery();

                    // Get output parameter for IDENTITY when using a stored procedure
                    IdentityGenerated = ((IDbDataParameter)cmd.Parameters[identityParamName]).Value;
                }
            }
            else
            {
                // Execute the SQL
                RowsAffected = cmd.ExecuteNonQuery();
            }

            // Close connection if it was not open originally
            if (!isConnectionOpen)
            {
                cmd.Connection.Close();
            }
        }
        catch (Exception ex)
        {
            ThrowDbException(ex, cmd, exceptionMsg);
        }

        return RowsAffected;
    }
    #endregion

    #region ExecuteNonQueryUsingDataSet Method
    public int ExecuteNonQueryUsingDataSet(IDbCommand cmd)
    {
        return ExecuteNonQueryUsingDataSet(cmd, string.Empty);
    }

    public int ExecuteNonQueryUsingDataSet(IDbCommand cmd, string exceptionMsg)
    {
        DataSetObject = new DataSet();

        RowsAffected = 0;
        IdentityGenerated = null;

        cmd.CommandText += ";SELECT @@ROWCOUNT As RowsAffected, SCOPE_IDENTITY() AS IdentityGenerated";
        try
        {
            using DbDataAdapter da = CreateDataAdapter(cmd);
            da.Fill(DataSetObject);
            if (DataSetObject.Tables.Count > 0)
            {
                RowsAffected = (int)DataSetObject.Tables[0].Rows[0]["RowsAffected"];
                IdentityGenerated = DataSetObject.Tables[0].Rows[0]["IdentityGenerated"];
            }
        }
        catch (Exception ex)
        {
            ThrowDbException(ex, cmd, exceptionMsg);
        }

        return RowsAffected;
    }
    #endregion

    #region GetDataSet Methods
    public virtual DataSet GetDataSet()
    {
        return GetDataSet(CommandObject, string.Empty);
    }

    public virtual DataSet GetDataSet(string exceptionMsg)
    {
        return GetDataSet(CommandObject, exceptionMsg);
    }

    public virtual DataSet GetDataSet(IDbCommand cmd, string exceptionMsg = "")
    {
        DataSetObject = new DataSet();

        try
        {
            // Ensure Command object is correct
            CheckCommand(cmd);
            // Create SqlDataAdapter
            using DbDataAdapter da = CreateDataAdapter(cmd);
            // Fill the DataSet
            da.Fill(DataSetObject);
        }
        catch (Exception ex)
        {
            ThrowDbException(ex, cmd, exceptionMsg);
        }

        return DataSetObject;
    }
    #endregion

    #region GetDataReader Methods
    public virtual IDataReader GetDataReader()
    {
        return GetDataReader(CommandObject, string.Empty);
    }

    public virtual IDataReader GetDataReader(string exceptionMsg)
    {
        return GetDataReader(CommandObject, exceptionMsg);
    }

    public virtual IDataReader GetDataReader(IDbCommand cmd, string exceptionMsg = "")
    {
        IDataReader dr = null;

        try
        {
            // Ensure Command object is correct
            CheckCommand(cmd);
            // Open Connection
            cmd.Connection.Open();
            // Create DataReader
            dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }
        catch (Exception ex)
        {
            ThrowDbException(ex, cmd, exceptionMsg);
        }

        return dr;
    }
    #endregion

    #region AddStandardParameters
    public abstract void AddStandardParameters();
    #endregion

    #region GetStandardOutputParameters Method
    public abstract void GetStandardOutputParameters();
    #endregion

    #region ToList Methods
    public virtual List<T> ToList<T>(IDataReader rdr)
    {
        List<T> ret = new();
        T entity;
        Type typ = typeof(T);
        List<ColumnMapper> columns = new();
        string name;
        PropertyInfo col;

        // Get all the properties in Entity Class
        PropertyInfo[] props = typ.GetProperties();

        // Get all ColumnAttribute's
        var attrs = props.Where(p => p.GetCustomAttributes(typeof(ColumnAttribute), false).Any()).ToArray();

        for (int index = 0; index < rdr.FieldCount; index++)
        {
            // Get name from data reader
            name = rdr.GetName(index);
            // See if column name maps directly to property name
            col = props.FirstOrDefault(c => c.Name == name);

            // Column!=Property -> See if the column name is in a ColumnAttribute
            if (col == null)
            {
                for (int i = 0; i < attrs.Length; i++)
                {
                    var tmp = attrs[i].GetCustomAttribute(typeof(ColumnAttribute));
                    if (tmp != null && ((ColumnAttribute)tmp).Name == name)
                    {
                        col = props.FirstOrDefault(c => c.Name == attrs[i].Name);
                        break;
                    }
                }
            }

            if (col != null)
            {
                columns.Add(new ColumnMapper
                {
                    ColumnName = name,
                    ColumnProperty = col
                });
            }
        }

        // Loop through all records
        while (rdr.Read())
        {
            // Create new instance of Entity
            entity = Activator.CreateInstance<T>();

            // Loop through columns to assign data
            for (int i = 0; i < columns.Count; i++)
            {
                if (rdr[columns[i].ColumnName].Equals(DBNull.Value))
                {
                    columns[i].ColumnProperty.SetValue(entity, null, null);
                }
                else
                {
                    columns[i].ColumnProperty.SetValue(entity, rdr[columns[i].ColumnName], null);
                }
            }

            ret.Add(entity);
        }

        return ret;
    }

    public virtual List<T> ZeroIndexToList<T>(IDataReader rdr)
    {
        List<T> ret = new();
        T entity;

        // Loop through all records
        while (rdr.Read())
        {
            entity = rdr[0].Equals(DBNull.Value) ? default : (T)rdr[0];
            ret.Add(entity);
        }

        return ret;
    }

    public virtual List<T> ToList<T>(DataTable dt)
    {
        List<T> ret = new();
        T entity;
        Type typ = typeof(T);
        List<ColumnMapper> columns = new();
        string name;
        PropertyInfo col;

        // Get all the properties in Entity Class
        PropertyInfo[] props = typ.GetProperties();

        // Get all ColumnAttribute's
        var attrs = props.Where(p => p.GetCustomAttributes(typeof(ColumnAttribute), false).Any()).ToArray();

        for (int index = 0; index < dt.Columns.Count; index++)
        {
            // Get name from data reader
            name = dt.Columns[index].ColumnName;
            // See if column name maps directly to property name
            col = props.FirstOrDefault(c => c.Name == name);

            // Column!=Property -> See if the column name is in a ColumnAttribute
            if (col == null)
            {
                for (int i = 0; i < attrs.Length; i++)
                {
                    var tmp = attrs[i].GetCustomAttribute(typeof(ColumnAttribute));
                    if (tmp != null && ((ColumnAttribute)tmp).Name == name)
                    {
                        col = props.FirstOrDefault(c => c.Name == attrs[i].Name);
                        break;
                    }
                }
            }

            if (col != null)
            {
                columns.Add(new ColumnMapper
                {
                    ColumnName = name,
                    ColumnProperty = col
                });
            }
        }

        // Loop through all records
        for (int rows = 0; rows < dt.Rows.Count; rows++)
        {
            // Create new instance of Entity
            entity = Activator.CreateInstance<T>();

            // Loop through columns to assign data
            for (int i = 0; i < columns.Count; i++)
            {
                if (dt.Rows[rows][columns[i].ColumnName].Equals(DBNull.Value))
                {
                    columns[i].ColumnProperty.SetValue(entity, null, null);
                }
                else
                {
                    columns[i].ColumnProperty.SetValue(entity, dt.Rows[rows][columns[i].ColumnName], null);
                }
            }

            ret.Add(entity);
        }

        return ret;
    }

    public virtual List<T> ZeroIndexToList<T>(DataTable dt)
    {
        List<T> ret = new();
        T entity;

        // Loop through all records
        for (int rows = 0; rows < dt.Rows.Count; rows++)
        {
            entity = dt.Rows[rows][0].Equals(DBNull.Value) ? default : (T)dt.Rows[rows][0];
            ret.Add(entity);
        }

        return ret;
    }

    #endregion

    #region ToSingle Methods
    public virtual T ToSingle<T>(DataTable dt)
    {
        T ret = default;

        Type typ = typeof(T);
        List<ColumnMapper> columns = new();
        string name;
        PropertyInfo col;

        // Get all the properties in Entity Class
        PropertyInfo[] props = typ.GetProperties();

        // Get all ColumnAttribute's
        var attrs = props.Where(p => p.GetCustomAttributes(typeof(ColumnAttribute), false).Any()).ToArray();

        for (int index = 0; index < dt.Columns.Count; index++)
        {
            // Get name from data reader
            name = dt.Columns[index].ColumnName;
            // See if column name maps directly to property name
            col = props.FirstOrDefault(c => c.Name == name);

            // Column!=Property -> See if the column name is in a ColumnAttribute
            if (col == null)
            {
                for (int i = 0; i < attrs.Length; i++)
                {
                    var tmp = attrs[i].GetCustomAttribute(typeof(ColumnAttribute));
                    if (tmp != null && ((ColumnAttribute)tmp).Name == name)
                    {
                        col = props.FirstOrDefault(c => c.Name == attrs[i].Name);
                        break;
                    }
                }
            }

            if (col != null)
            {
                columns.Add(new ColumnMapper
                {
                    ColumnName = name,
                    ColumnProperty = col
                });
            }
        }

        if (dt.Rows.Count > 0)
        {
            // Create new instance of Entity
            ret = Activator.CreateInstance<T>();

            // Loop through columns to assign data
            for (int i = 0; i < columns.Count; i++)
            {
                if (dt.Rows[0][columns[i].ColumnName].Equals(DBNull.Value))
                {
                    columns[i].ColumnProperty.SetValue(ret, null, null);
                }
                else
                {
                    columns[i].ColumnProperty.SetValue(ret, dt.Rows[0][columns[i].ColumnName], null);
                }
            }
        }

        return ret;
    }
    #endregion

    #region ThrowDbException Method
    public virtual void ThrowDbException(Exception ex, IDbCommand cmd, string exceptionMsg = "")
    {
        // Set the last exception
        LastException = CreateDbException(ex, cmd, exceptionMsg);

        throw LastException;
    }
    #endregion

    #region CreateDbException Method
    public virtual DataException CreateDbException(Exception ex, IDbCommand cmd, string exceptionMsg = "")
    {
        DataException exc;
        exceptionMsg = string.IsNullOrEmpty(exceptionMsg) ? string.Empty : exceptionMsg + " - ";

        exc = new DataException(exceptionMsg + ex.Message, ex)
        {
            ConnectionString = cmd.Connection.ConnectionString,
            Database = cmd.Connection.Database,
            SQL = SQL,
            CommandParameters = cmd.Parameters,
            WorkstationId = Environment.MachineName
        };

        return exc;
    }
    #endregion

    #region Dispose Method
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (CommandObject != null)
            {
                if (CommandObject.Connection != null)
                {
                    if (CommandObject.Transaction != null)
                    {
                        CommandObject.Transaction.Dispose();
                    }
                    CommandObject.Connection.Close();
                    CommandObject.Connection.Dispose();
                }
                CommandObject.Dispose();
            }
        }
    }
    #endregion
}
