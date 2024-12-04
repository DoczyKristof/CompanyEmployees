using Entities.Models;
using System.Dynamic;

namespace Contracts
{
    public interface IDataShaper<T>
    {
        IEnumerable<Entity> ShapeData(IEnumerable<T> entitites, string fieldString);
        Entity ShapeData(T entitiy, string fieldString);
    }
}
