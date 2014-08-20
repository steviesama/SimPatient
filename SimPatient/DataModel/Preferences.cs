namespace SimPatient.DataModel
{
    /// <summary>
    /// Global data class that holds the MySQL Connection tab data
    /// which the user will have entered at some point for access
    /// by the LoginControl or any other component that needs
    /// access to this information.
    /// </summary>
    class Preferences
    {
        public static string HostAddress { get; set; }
        public static string PortAddress { get; set; }
        public static string DatabaseName { get; set; }
        public static string Username { get; set; }
        public static string Password { get; set; }
    }
}
