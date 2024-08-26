using System;

namespace Sokoban;

public interface IEntityManager<TEntity> where TEntity : ISokobanEntity
{
    public TEntity GetEntity(int x, int y);
    
    public void MoveEntity(TEntity entity, Direction direction,bool forUndo=false);
    
    public void InitEntities();
    
}