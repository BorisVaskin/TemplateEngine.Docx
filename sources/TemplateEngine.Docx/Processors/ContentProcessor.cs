﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace TemplateEngine.Docx.Processors
{
	internal class ContentProcessor
	{
		private bool _isNeedToRemoveContentControls;

		private readonly List<IProcessor> _processors;
        private readonly ProcessContext Ctx;

		internal ContentProcessor(ProcessContext context)
		{
            Ctx = context;
			_processors = new List<IProcessor>
			{
				new FieldsProcessor(context),
				new TableProcessor(context),
				new ListProcessor(context)			
			};
		}

		public ContentProcessor SetRemoveContentControls(bool isNeedToRemove)
		{
			_isNeedToRemoveContentControls = isNeedToRemove;
			foreach (var processor in _processors)
			{
				processor.SetRemoveContentControls(_isNeedToRemoveContentControls);
			}
			return this;
		}

        public ProcessResult FillContentEverywhere(Content content)
        {
            List<XElement> documentPartsToInsertContentTo = new List<XElement>();
            documentPartsToInsertContentTo.Add(Ctx.MainPart.Root.Element(W.body));
            if (Ctx.HeaderPart != null) { documentPartsToInsertContentTo.Add(Ctx.HeaderPart?.Root); }
            if (Ctx.FooterPart != null) { documentPartsToInsertContentTo.Add(Ctx.FooterPart?.Root); }

            var errors = new List<string>();
            var data = content.ToList();
            var processedItems = new List<string>();

            foreach (var contentItems in data.GroupBy(d => d.Name))
            {
                if (processedItems.Contains(contentItems.Key)) continue;

                foreach (var documentPart in documentPartsToInsertContentTo)
                {
                    if (documentPart == null) continue;
                    var contentControls = FindContentControls(documentPart, contentItems.Key).ToList();

                    //Need to get error message from processor.
                    if (!contentControls.Any())
                        contentControls.Add(null);

                    foreach (var xElement in contentControls)
                    {
                        if (contentItems.Any(item => item is TableContent) && xElement != null)
                            processedItems.AddRange(ProcessTableFields(data.OfType<FieldContent>(), xElement));


                        foreach (var processor in _processors)
                        {
                            var result = processor.FillContent(xElement, contentItems);
                            if (result.Handled && !result.Success)
                                errors.AddRange(result.Errors);
                        }
                    }
                }
                processedItems.Add(contentItems.Key);
            }
            return errors.Any()
                ? ProcessResult.ErrorResult(errors)
                : ProcessResult.SuccessResult;
        }

        public ProcessResult FillContent(XElement content, IEnumerable<IContentItem> data)
		{
			var errors = new List<string>();
			var processedItems = new List<string>();
			data = data.ToList();

			foreach (var contentItems in data.GroupBy(d=>d.Name))
			{
				if (processedItems.Contains(contentItems.Key)) continue;

				var contentControls = FindContentControls(content, contentItems.Key).ToList();

				//Need to get error message from processor.
				if (!contentControls.Any())
					contentControls.Add(null);

				foreach (var xElement in contentControls)
				{
					if (contentItems.Any(item => item is TableContent) && xElement != null)								
						processedItems.AddRange(ProcessTableFields(data.OfType<FieldContent>(), xElement));
					

					foreach (var processor in _processors)
					{
						var result = processor.FillContent(xElement, contentItems);
						if (result.Handled && !result.Success)
							errors.AddRange(result.Errors);
					}
				}
				processedItems.Add(contentItems.Key);
			}

			return errors.Any()
				? ProcessResult.ErrorResult(errors)
				: ProcessResult.SuccessResult;
		}

		/// <summary>
		/// Processes table data that should not be duplicated
		/// </summary>
		/// <param name="fields">Possible fields</param>
		/// <param name="xElement">Table content control</param>
		/// <returns>List of content items that were processed</returns>
		private IEnumerable<string> ProcessTableFields(IEnumerable<FieldContent> fields, XElement xElement)
		{
			var processedItems = new List<IContentItem>();
			foreach (var fieldContentControl in fields)
			{
				var innerContentControls = FindContentControls(xElement.Element(W.sdtContent), fieldContentControl.Name);
				foreach (var innerContentControl in innerContentControls)
				{
					var processor = _processors.OfType<FieldsProcessor>().FirstOrDefault();
					if (processor != null)
					{
						processor.FillContent(innerContentControl, fieldContentControl);
						processedItems.Add(fieldContentControl);
					}
				}				
			}

			return processedItems.Select(i=>i.Name).Distinct();
		}

		public ProcessResult FillContent(XElement content, Content data)
		{
			return FillContent(content, data.AsEnumerable());
		}

		public ProcessResult FillContent(XElement content, IContentItem data)
		{
			return FillContent(content, new List<IContentItem>{data});
		}
		private IEnumerable<XElement> FindContentControls(XElement content, string tagName)
		{
			return content
				//top level content controls
				.FirstLevelDescendantsAndSelf(W.sdt)
				//with specified tagName
				.Where(sdt => tagName == sdt.SdtTagName());
		}
	}
}
