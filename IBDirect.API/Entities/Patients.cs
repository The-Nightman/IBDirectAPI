namespace IBDirect.API.Entities;

public class Patients
{
    public int Id { get; set; }
    public string Name { get; set; }
    public byte[] PassHash { get; set; }
    public byte[] Salt { get; set; }
}
