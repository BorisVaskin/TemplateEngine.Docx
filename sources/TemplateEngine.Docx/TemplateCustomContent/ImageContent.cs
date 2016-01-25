using DocumentFormat.OpenXml.Packaging;

namespace TemplateEngine.Docx
{
    public class ImageContent : IContentItem
    {
        public ImageContent()
        {

        }

        public ImageContent(string name, byte[] binary, ImagePartType type = ImagePartType.Jpeg)
        {
            Name = name;
            Binary = binary;
            Type = type;
        }

        public string Name { get; set; }
        public byte[] Binary { get; set; }
        public ImagePartType Type { get; set; }
    }
}
