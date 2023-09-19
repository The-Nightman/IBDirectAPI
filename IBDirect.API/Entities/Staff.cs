namespace IBDirect.API.Entities;

public class Staff
{
    public int Id { get; set; }
    public string Name { get; set; }
    public byte[] PassHash { get; set; }
    public byte[] Salt { get; set; }
}
