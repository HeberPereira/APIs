namespace hb29.Shared.Services
{
    public class ContainerNameSettings
    {
        /// <summary>
        /// Blob container used to upload and process Node Template Excel files.
        /// </summary>
        public string Container1 { get; set; }

        /// <summary>
        /// Blob container used to store Node Template per si (variable values to be parsed on Templates).
        /// </summary>
        public string Container2 { get; set; }
    }
}
