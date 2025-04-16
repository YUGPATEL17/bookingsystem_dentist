using Spectre.Console;

namespace DentistAppointmentManagementSystem.Utilities
{
    /// <summary>
    /// Provides helper methods for creating table layouts with consistent styling.
    /// </summary>
    public static class TableHelper
    {
        /// <summary>
        /// Creates a new table with a rounded border style.
        /// </summary>
        /// <returns>A Table instance with rounded borders applied.</returns>
        public static Table CreateRoundedTable()
        {
            var table = new Table();
            table.Border(TableBorder.Rounded);
            return table;
        }
    }
}
