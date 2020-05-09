using System.Linq;

namespace BCI.SLAPS.Server.Helpers
{
    public static class Validators
    {
        /// <summary>
        /// Checks if a given name is contained in the list of forbidden names
        /// </summary>
        /// <param name="name">Name to test</param>
        /// <returns>True if forbidden, false otherwise</returns>
        public static bool CheckForbiddenName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return true;

            // Hard coded names for now, but can be moved to the database in the future if needed.
            var forbiddenNames = new string[]
            {
                "SYSTEM",
                "SLAPS-SYSTEM",
                "SLAPS"
            };
            return forbiddenNames.Contains(name.ToUpperInvariant());
        }
    }
}
