namespace EtherGizmos.SqlMonitor.Shared.Utilities.Annotations;

[AttributeUsage(AttributeTargets.Property)]
public class LookupManyAttribute : Attribute
{
    public Type BridgeType { get; }

    public LookupManyAttribute(Type bridgeType)
    {
        BridgeType = bridgeType;
    }
}
