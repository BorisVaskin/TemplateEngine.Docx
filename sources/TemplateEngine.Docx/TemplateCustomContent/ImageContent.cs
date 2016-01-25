using DocumentFormat.OpenXml.Packaging;

namespace TemplateEngine.Docx
{
    public class ImageContent : IContentItem
    {
        public ImageContent()
        {

        }

        public ImageContent(string name, byte[] binary)
        {
            Name = name;
            Binary = binary;
            MIMEType = "image/jpeg";
        }

        public ImageContent(string name, byte[] binary, string mimeType)
            :this(name, binary)
        {
            if (!string.IsNullOrEmpty(mimeType)) { MIMEType = mimeType; }
        }

        public string Name { get; set; }
        public byte[] Binary { get; set; }
        public string MIMEType { get; set; }
    }
}
