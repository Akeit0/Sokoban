using System;

namespace Sokoban;


public sealed class Array2D<T>(int width, int height)
{
    public T[] Data { get; } = new T[height* width];
    public int Width { get; } = width;
    public int Height { get; } = height;

    public ref T this[int x, int y] => ref Data[y * Width + x];
    
    public Span<T> GetRow(int y) => Data.AsSpan(y * Width, Width);
}