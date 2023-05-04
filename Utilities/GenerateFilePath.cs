namespace CodeFirstRestaurantAPI.Utilities
{
    static public class GenerateFilePath
    {
        public static string GenerateFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
            + "_"
            + Guid.NewGuid().ToString().Substring(0, 4)
            + Path.GetExtension(fileName);
        }
    }
}
