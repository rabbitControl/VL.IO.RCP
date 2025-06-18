namespace RCP.Protocol;

public record InfoData
{
    public Version RCPVersion;
    public Version HandshakeVersion;
    public string AppId;
    public string AppVersion;

    //public string ToString()
    //{
    //    return AppId + AppVersion
    //}
}