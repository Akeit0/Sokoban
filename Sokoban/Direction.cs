namespace Sokoban;

public readonly record struct Direction(sbyte X, sbyte Y)
{
    public sbyte X { get; } = X;
    public sbyte Y { get; } = Y;
    public static Direction Up => new(0, -1);
    public static Direction Down => new(0, 1);
    public static Direction Left => new(-1, 0);
    public static Direction Right => new(1, 0);
}