namespace DaikinNet;

public class Device
{
    public string Id { get; set; }
    public string LocationId { get; set; }
    public string Name { get; set; }
    public string Model { get; set; }
    public string Brand { get; set; }
    public string FirmwareVersion { get; set; }
    public long CreatedDate { get; set; }
    public bool HasOwner { get; set; }
    public bool HasWrite { get; set; }
}