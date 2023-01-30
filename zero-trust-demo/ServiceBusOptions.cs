namespace zero_trust_demo;

public class ServiceBusOptions
{
    public const string SectionName = "ServiceBus";
    public string Queue { get; set; } = null!;
    public string Namespace { get; set; } = null!;
}