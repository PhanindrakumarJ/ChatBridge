using System;
using System.Collections.Generic;
using System.Text;

namespace ChatBridge.Chat.Application.DTOS
{
    public class VendorProfileDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CertificateName {  get; set; } = string.Empty;
    }
}
