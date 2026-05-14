namespace Guessy.Infrastructure.Configuration;

public class DatabaseOption
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
}