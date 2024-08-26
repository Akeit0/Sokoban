namespace Sokoban;

public interface ISokobanEntity
{
    public EntityType EntityType { get; }
    public int X { get; }
    public int Y { get; }
}