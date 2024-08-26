using Sokoban;

public class SokobanEntity(EntityType entityType,int x,int y): ISokobanEntity
{
    public EntityType EntityType { get; } = entityType;
    public int X { get; set; } = x;
    public int Y { get; set; } = y;
}