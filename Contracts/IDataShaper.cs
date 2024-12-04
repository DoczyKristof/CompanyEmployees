using Entities.Models;
using System.Dynamic;

namespace Contracts
{
    public interface IDataShaper<T>
    {
        IEnumerable<ShapedEntity> ShapeData(IEnumerable<T> entitites, string fieldString);
        ShapedEntity ShapeData(T entitiy, string fieldString);
    }
}
