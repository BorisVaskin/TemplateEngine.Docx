using DocumentFormat.OpenXml.Packaging;
using System.Collections.Generic;
using System.Xml.Linq;

namespace TemplateEngine.Docx.Processors
{
	internal class ProcessContext
	{
		internal Dictionary<int, int> LastNumIds { get; private set; }
        internal WordprocessingDocument WordDocument { get; private set; }
        internal XDocument MainPart { get; private set; }
		internal XDocument NumberingPart { get; private set; }
		internal XDocument StylesPart { get; private set; }
        internal XDocument HeaderPart { get; private set; }
        internal XDocument FooterPart { get; private set; }
        /// <summary>
        /// List of document parts which contains std parts to place content in
        /// </summary>
        internal List<XElement> ContainerParts { get; private set; } = new List<XElement>();
        internal ProcessContext(WordprocessingDocument wordDocument, XDocument mainPart, XDocument numberingPart, XDocument stylesPart, XDocument headerPart, XDocument footerPart)
        {
			LastNumIds = new Dictionary<int, int>();
            WordDocument = wordDocument;
            MainPart = mainPart;
			NumberingPart = numberingPart;
			StylesPart = stylesPart;
            HeaderPart = headerPart;
            FooterPart = footerPart;

            ContainerParts.Add(MainPart.Root.Element(W.body));
            if (HeaderPart != null) { ContainerParts.Add(HeaderPart?.Root); }
            if (FooterPart != null) { ContainerParts.Add(FooterPart?.Root); }
        }
	}
}
