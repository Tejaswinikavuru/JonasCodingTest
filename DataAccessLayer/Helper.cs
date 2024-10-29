using System.Reflection;

namespace DataAccessLayer
{
    public class Helper
    {
        // Generic Helper Methods

        // Method to update entity properties using reflection
        public static void UpdateEntity<T>(T existingEntity, T newEntity)
        {
            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                var newValue = prop.GetValue(newEntity);
                var oldValue = prop.GetValue(existingEntity);

                if (newValue != null && !newValue.Equals(oldValue))
                {
                    prop.SetValue(existingEntity, newValue);
                }
            }
        }

        // Method to compare two entities using reflection
        public static bool AreEntitiesEqual<T>(T entity1, T entity2)
        {
            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                var value1 = prop.GetValue(entity1);
                var value2 = prop.GetValue(entity2);

                if (value1 == null && value2 != null || value1 != null && !value1.Equals(value2))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
