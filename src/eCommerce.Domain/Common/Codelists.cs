namespace eCommerce.Domain.Common;

public static class Codelists
{
    public static IReadOnlyCollection<string> Gender { get; } = new string[] { "Male", "Female" };

    public static IReadOnlyCollection<string> MaritalStatus { get; } = new string[] { "Married", "Widowed", "Separated", "Divorced", "Single" };
}