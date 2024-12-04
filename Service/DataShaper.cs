using Contracts;
using Entities.Models;
using System.Reflection;

namespace Service
{
    public class DataShaper<T> : IDataShaper<T> where T : class
    {
        public PropertyInfo[] Properties { get; set; }

        public DataShaper()
        {
            Properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        public IEnumerable<Entity> ShapeData(IEnumerable<T> entitites, string fieldString)
        {
            var requiredProperties = GetRequiredProperties(fieldString);

            return FetchData(entitites, requiredProperties);
        }

        public Entity ShapeData(T entitiy, string fieldString)
        {
            var requiredProperties = GetRequiredProperties(fieldString);

            return FetchDataForEntity(entitiy, requiredProperties);
        }

        private IEnumerable<PropertyInfo> GetRequiredProperties(string fieldString)
        {
            var requiredProperties = new List<PropertyInfo>();

            if (!string.IsNullOrWhiteSpace(fieldString))
            {
                var fields = fieldString.Split(',', StringSplitOptions.RemoveEmptyEntries);

                foreach ( var field in fields)
                {
                    var property = Properties.FirstOrDefault(pi => pi.Name.Equals(field.Trim(), StringComparison.InvariantCultureIgnoreCase));

                    if (property is null)
                        continue;

                    requiredProperties.Add(property);
                }
            }
            else
            {
                requiredProperties = Properties.ToList();
            }

            return requiredProperties;
        }

        private IEnumerable<Entity> FetchData(IEnumerable<T> entitites, IEnumerable<PropertyInfo> requiredProperties)
        {
            var shapedData = new List<Entity>();

            foreach (var entity in entitites)
            {
                var shapedObject = FetchDataForEntity(entity, requiredProperties);
                shapedData.Add(shapedObject);
            }

            return shapedData;
        }

        private Entity FetchDataForEntity(T entitiy, IEnumerable<PropertyInfo> requiredProperties)
        {
            var shapedObject = new Entity();

            foreach (var property in requiredProperties)
            {
                var objectPropertyValue = property.GetValue(entitiy);
                shapedObject.TryAdd(property.Name, objectPropertyValue);
            }

            return shapedObject;
        }
    }
}
