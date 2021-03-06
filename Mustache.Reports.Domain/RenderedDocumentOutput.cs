﻿using System;
using System.Collections.Generic;

namespace Mustache.Reports.Domain
{
    public class RenderedDocumentOutput
    {
        public string Base64String { get; set; }
        public List<string> ErrorMessages { get; set; }
        public ContentType ContentType { get; set; }

        public RenderedDocumentOutput()
        {
            ErrorMessages = new List<string>();
        }

        public bool HasErrors()
        {
            return ErrorMessages.Count > 0;
        }

        public byte[] FetchDocumentAsByteArray()
        {
            if (string.IsNullOrEmpty(Base64String))
            {
                return new byte[0];
            }

            return Convert.FromBase64String(Base64String);
        }
    }
}
