namespace RCP.Protocol;

public record Version
{
    public byte Major;
    public byte Minor;
    public float ToFloat()
    {
        return float.Parse(Major.ToString() + "." + Minor.ToString());
    }
}