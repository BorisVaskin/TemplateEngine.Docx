﻿using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TemplateEngine.Docx.Processors
{
    internal class ImagesProcessor : IProcessor
    {
        private readonly ProcessContext _context;
        private ProcessResult _processResult;
        public ImagesProcessor(ProcessContext context)
        {
            _context = context;
        }

        private bool _isNeedToRemoveContentControls;

        public IProcessor SetRemoveContentControls(bool isNeedToRemove)
        {
            _isNeedToRemoveContentControls = isNeedToRemove;
            return this;
        }

        public ProcessResult FillContent(XElement contentControl, IEnumerable<IContentItem> items)
        {
            _processResult = new ProcessResult();

            foreach (var contentItem in items)
            {
                FillContent(contentControl, contentItem);
            }


            if (_processResult.Success && _isNeedToRemoveContentControls)
                contentControl.RemoveContentControl();

            return _processResult;
        }

        public void FillContent(XElement contentControl, IContentItem item)
        {
            if (!(item is ImageContent))
            {
                _processResult = ProcessResult.NotHandledResult;
                return;
            }

            var field = item as ImageContent;
            // If image bytes was not provided, then doing nothing
            if (field.Binary == null || field.Binary.Length == 0) { return; }

            // If there isn't a field with that name, add an error to the error string,
            // and continue with next field.
            if (contentControl == null)
            {
                _processResult.Errors.Add(String.Format("Field Content Control '{0}' not found.",
                    field.Name));
                return;
            }


            var blip = contentControl.DescendantsAndSelf(A.blip).First();
            if (blip == null)
            {
                _processResult.Errors.Add(String.Format("Image to replace for '{0}' not found.",
                    field.Name));
                return;
            }

            // Creating a new image part
            var imagePart = _context.WordDocument.MainDocumentPart.AddImagePart(field.MIMEType);
            // Writing image bytes to it
            using (BinaryWriter writer = new BinaryWriter(imagePart.GetStream()))
            {
                writer.Write(field.Binary);
            }
            // Setting reference for CC to newly uploaded image
            blip.Attribute(R.embed).Value = _context.WordDocument.MainDocumentPart.GetIdOfPart(imagePart);

            //var imageId = blip.Attribute(R.embed).Value;
            //var xmlPart = _context.WordDocument.MainDocumentPart.GetPartById(imageId);
            //if (xmlPart is ImagePart)
            //{
            //    ImagePart imagePart = xmlPart as ImagePart;
            //    using (BinaryWriter writer = new BinaryWriter(imagePart.GetStream()))
            //    {
            //        writer.Write(field.Binary);
            //    }
            //}
        }
    }
}
