namespace IBDirect.API.Entities;

public class Users
{
    public int Id { get; set; }
    public string Name { get; set; }
    public byte[] PassHash { get; set; }
    public byte[] Salt { get; set; }
    public int Role { get; set; }
}
