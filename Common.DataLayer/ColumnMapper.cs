using System.Reflection;

namespace Common.DataLayer;

#nullable disable
public class ColumnMapper
{
    public string ColumnName { get; set; }
    public PropertyInfo ColumnProperty { get; set; }
}
