using Converter.Common.Enums;

namespace Converter.Common.ApiModels
{
    /// <summary>
    /// The response may not contain attachments
    /// </summary>
    public class ResponseDTO
    {
        /// <summary>
        /// Status
        /// </summary>
        public ConversionStatus Status { get; set; }

        /// <summary>
        /// User identifier
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Attachment
        /// </summary>
        public AttachmentDTO Attachment { get; set; }
    }
}
