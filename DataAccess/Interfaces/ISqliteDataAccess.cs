namespace DataAccess.Interfaces
{
    public interface ISqliteDataAccess
    {
        string GetConnectionString(string id = "Default");
    }
}