﻿using System.Collections.Generic;
using System.Xml.Linq;

namespace TemplateEngine.Docx.Processors
{
	internal class ProcessContext
	{
		internal Dictionary<int, int> LastNumIds { get; private set; }
		internal XDocument MainPart { get; private set; }
		internal XDocument NumberingPart { get; private set; }
		internal XDocument StylesPart { get; private set; }
        internal XDocument HeaderPart { get; private set; }
        internal XDocument FooterPart { get; private set; }
        internal ProcessContext(XDocument mainPart, XDocument numberingPart, XDocument stylesPart, XDocument headerPart, XDocument footerPart)
        {
			LastNumIds = new Dictionary<int, int>();
			MainPart = mainPart;
			NumberingPart = numberingPart;
			StylesPart = stylesPart;
            HeaderPart = headerPart;
            FooterPart = footerPart;
        }
	}
}
