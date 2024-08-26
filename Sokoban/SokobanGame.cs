using System.Collections.Generic;

namespace Sokoban;

public class SokobanGame<TEntity> where TEntity : class, ISokobanEntity
{
    public int Width { get; }
    public int Height { get; }
    Array2D<Tile> initialMap { get; }
    Array2D<Tile> map { get; }

    IEntityManager<TEntity> manager { get; }

    List<TEntity> players { get; } = [];

    HashSet<TEntity> alreadyCalculated { get; } = [];
    List<(int, int)> goals { get; } = [];

    List<Direction> undoes { get; } = [];

    public SokobanGame(Array2D<Tile> map, IEntityManager<TEntity> manager)
    {
        initialMap = new(map.Width, map.Height);
        for (int y = 0; y < map.Height; y++)
        {
            var row = map.GetRow(y);
            row.CopyTo(initialMap.GetRow(y));
        }

        this.manager = manager;
        Height = map.Height;
        Width = map.Width;
        for (int y = 0; y < Height; y++)
        for (int x = 0; x < Width; x++)
        {
            var tile = map[x, y];
            if (tile != Tile.Empty)
            {
                if (tile is Tile.Player or Tile.PlayerOnGoal)
                {
                    players.Add(manager.GetEntity(x, y));
                }

                if (tile is Tile.Goal or Tile.BoxOnGoal or Tile.PlayerOnGoal)
                {
                    //manager.CreateGoal(x, y);
                    goals.Add((x, y));
                }
            }
        }

        this.map = map;
    }

    public MoveResult MovePlayer(Direction direction, bool forUndo = false)
    {
        var (dx, dy) = direction;
        alreadyCalculated.Clear();
        bool hasMoved = false;

        bool TryMoveEntity(TEntity entity)
        {
            if (alreadyCalculated.Contains(entity))
                return false;
            var x = entity.X;
            var y = entity.Y;
            var nextX = x + dx;
            var nextY = y + dy;
            if (nextX < 0 || nextX >= Width || nextY < 0 || nextY >= Height)
            {
                alreadyCalculated.Add(entity);
                return false;
            }

            var nextTile = map[nextX, nextY];
            if (nextTile is Tile.Wall)
            {
                alreadyCalculated.Add(entity);
                return false;
            }

            if (nextTile is Tile.Empty or Tile.Goal)
            {
                manager.MoveEntity(entity, direction, forUndo);
                hasMoved = true;
                var currentTile = map[x, y];
                switch (currentTile)
                {
                    case Tile.Player:
                        map[x, y] = Tile.Empty;
                        map[nextX, nextY] = nextTile == Tile.Goal ? Tile.PlayerOnGoal : Tile.Player;
                        break;
                    case Tile.Box:
                        map[x, y] = Tile.Empty;
                        map[nextX, nextY] = nextTile == Tile.Goal ? Tile.BoxOnGoal : Tile.Box;
                        break;
                    case Tile.PlayerOnGoal:
                        map[x, y] = Tile.Goal;
                        map[nextX, nextY] = nextTile == Tile.Goal ? Tile.PlayerOnGoal : Tile.Player;
                        break;
                    case Tile.BoxOnGoal:
                        map[x, y] = Tile.Goal;
                        map[nextX, nextY] = nextTile == Tile.Goal ? Tile.BoxOnGoal : Tile.Box;
                        break;
                }

                alreadyCalculated.Add(entity);
                return true;
            }

            if (entity.EntityType == EntityType.Player && nextTile is Tile.Box or Tile.Player or Tile.BoxOnGoal or Tile.PlayerOnGoal)
            {
                var nextEntity = manager.GetEntity(nextX, nextY);
                if (TryMoveEntity(nextEntity))
                {
                    return TryMoveEntity(entity);
                }
            }

            return false;
        }

        foreach (var player in players)
        {
            TryMoveEntity(player);
        }

        if (!forUndo && hasMoved)
        {
            undoes.Add(direction);
        }

        foreach (var (x, y) in goals)
        {
            if (map[x, y] != Tile.BoxOnGoal)
            {
                return hasMoved ? MoveResult.Success : MoveResult.Blocked;
            }
        }

        return MoveResult.Win;
    }

    public void Reset()
    {
        for (int y = 0; y < Height; y++)
        {
            var row = initialMap.GetRow(y);
            row.CopyTo(map.GetRow(y));
        }

        undoes.Clear();
        manager.InitEntities();
    }

    public bool TryUndo()
    {
        if (undoes.Count == 0)
            return false;
        for (int y = 0; y < Height; y++)
        {
            var row = initialMap.GetRow(y);
            row.CopyTo(map.GetRow(y));
        }

        manager.InitEntities();


        undoes.RemoveAt(undoes.Count - 1);
        foreach (var d in undoes)
        {
            MovePlayer(d, true);
        }

        return true;
    }
}