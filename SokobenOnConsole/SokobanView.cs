using System.Runtime.CompilerServices;
using Sokoban;

public class SokobanView : IEntityManager<SokobanEntity>
{
    const char WallChar = '#';
    const char EmptyChar = ' ';
    const char BoxChar = '$';
    const char PlayerChar = '@';
    const char GoalChar = '*';
    const char BoxOnGoalChar = '+';
    const char PlayerOnGoalChar = '!';
    
    readonly Array2D<char> textTiles;
    readonly Array2D< SokobanEntity?> initialEntities;
    readonly Array2D< SokobanEntity?> entities;
    
    public SokobanView(string stage,out Array2D<Tile> tiles)
    {
        var lines = stage.Replace("\r\n","\n").Split('\n');
        var width = lines[0].Length;
        var height = lines.Length;
        tiles=new  (width, height);
        textTiles = new (width, height);
        entities = new (width, height);
        initialEntities = new (width, height);
        for (int y = 0; y < height; y++)
        {
            var line = lines[y];
            var x = 0;
            foreach (var c in line)
            {
                bool isValid = true;
                switch (c)
                {
                    case WallChar:
                        tiles[x,y] = Tile.Wall;
                        textTiles[x,y] = WallChar;
                        break;
                    case PlayerChar:
                        tiles[x, y] = Tile.Player;
                        entities[x, y] = new SokobanEntity(EntityType.Player, x, y);
                        textTiles[x, y] = EmptyChar;
                        break;
                    case BoxChar:
                        tiles[x, y] = Tile.Box;
                        entities[x, y] = new SokobanEntity(EntityType.Box, x, y);
                        textTiles[x, y] = EmptyChar;
                        break;
                    case GoalChar:
                        tiles[x, y] = Tile.Goal;
                        textTiles[x, y] = GoalChar;
                        break;
                    case BoxOnGoalChar:
                        tiles[x, y] = Tile.BoxOnGoal;
                        entities[x, y] = new SokobanEntity(EntityType.Box, x, y);
                        textTiles[x, y] = GoalChar;
                        break;
                    case PlayerOnGoalChar:
                        tiles[x, y] = Tile.PlayerOnGoal;
                        entities[x, y] = new SokobanEntity(EntityType.Player, x, y);
                        textTiles[x, y] = GoalChar;
                        break;
                    case EmptyChar:
                        tiles[x, y] = Tile.Empty;
                        textTiles[x, y] = EmptyChar;
                        break;
                    
                    default:isValid=false;
                        Console.WriteLine((ushort)c);
                        break;
                        
                }
                if(isValid) x++;
            }
        }
        for (int y = 0; y < height; y++)
        {
            var entitySpan = entities.GetRow(y);
            var initialEntitySpan = initialEntities.GetRow(y);
            for (int x = 0; x < entitySpan.Length; x++)
            {
                initialEntitySpan[x] = entitySpan[x];
            }
        }
        
    }
    
    public SokobanEntity GetEntity( int x, int y)
    {
        var entity = entities[x, y];
        if (entity == null)
        {
            throw new InvalidOperationException($"There is no entity at {x},{y}");
        }
        return entity;
    }


    public void MoveEntity(SokobanEntity entity, Direction direction,bool forUndo=false)
    {
        var x = entity.X;
        var y = entity.Y;
        entities[x, y] = null;
        var (dx, dy) = direction;
        var nextX = x + dx;
        var nextY = y + dy;
        entity.X = nextX;
        entity.Y = nextY;
        entities[nextX, nextY] = entity;
        
    }
    public string GetCurrentStateString()
    {
        var builder= new DefaultInterpolatedStringHandler(0, 0);
        for (int y = 0; y < textTiles.Height; y++)
        {
            var textSpan = textTiles.GetRow(y);
            var entitySpan = entities.GetRow(y);
            for (int x = 0; x < textSpan.Length; x++)
            {
                var stateChar = textSpan[x];
                var entity = entitySpan[x];
                if (entity != null)
                {
                    var entityType = entity.EntityType;
                    switch (stateChar,entityType)
                    {
                        case (GoalChar, EntityType.Player):
                            builder.AppendFormatted(PlayerOnGoalChar);
                            break;
                        case (GoalChar, EntityType.Box):
                            builder.AppendFormatted(BoxOnGoalChar);
                            break;
                        case (EmptyChar, EntityType.Player):
                            builder.AppendFormatted(PlayerChar);
                            
                            break;
                        case (EmptyChar, EntityType.Box):
                            builder.AppendFormatted(BoxChar);
                            
                            
                            break;
                    }
                }else
                {
                    builder.AppendFormatted(stateChar);
                }
            }
            builder.AppendFormatted('\n');
        }

        return builder.ToStringAndClear();
    }
    public void InitEntities()
    {
        for (int y = 0; y < initialEntities.Height; y++)
        {
            var entitySpan = entities.GetRow(y);
            entitySpan.Clear();
            var initialEntitySpan = initialEntities.GetRow(y);
            for (int x = 0; x < entitySpan.Length; x++)
            {
                var entity = initialEntitySpan[x];
                if(entity==null)continue;
                entity.X = x;
                entity.Y = y;
                entitySpan[x] = entity;
            }
        }
    }
}