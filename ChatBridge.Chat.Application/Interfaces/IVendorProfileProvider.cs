using ChatBridge.Chat.Application.DTOS;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatBridge.Chat.Application.Interfaces
{
    public interface IVendorProfileProvider
    {
        Task<VendorProfileDto> GetProfileAsync(
            CancellationToken cancellationToken = default);
    }
}
