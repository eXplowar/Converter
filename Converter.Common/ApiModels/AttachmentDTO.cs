namespace Converter.Common.ApiModels
{
    /// <summary>
    /// Attachment
    /// </summary>
    public class AttachmentDTO
    {
        /// <summary>
        /// File in bytes
        /// </summary>
        public byte[] Blob { get; set; }

        /// <summary>
        /// Filename
        /// </summary>
        public string FileName { get; set; }
    }
}
