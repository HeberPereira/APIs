namespace hb29.Shared
{
    public static class StreamToStringExtension
    {
        /// <summary>
        /// Converts and System.IO.Stream to UTF-8 encoded string.
        /// </summary>
        public static string ToEncodedString(this System.IO.Stream stream)
        {
            using var reader = new System.IO.StreamReader(stream, System.Text.Encoding.UTF8);
            
            return reader.ReadToEnd();
        }
    }
}