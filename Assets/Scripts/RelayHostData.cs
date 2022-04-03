using System;

public struct RelayHostData
{
    public string JoinCode;
    public string IPv4Address;
    public ushort Port;
    public Guid AllocationId;
    public byte[] AllocationIDBytes;
    public byte[] ConnectionData;
    public byte[] Key;
}
