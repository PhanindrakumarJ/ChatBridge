using ChatBridge.Chat.Application.DTOS;
using ChatBridge.Chat.Application.Interfaces;
using ChatBridge.Chat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatBridge.Chat.Infrastructure.Providers
{
    public sealed class VendorProfileProvider : IVendorProfileProvider
    {
        private const string CacheKey = "ChatBridge:VendorProfile";

        private readonly ChatBridgeDbContext _dbContext;
        private readonly IMemoryCache _cache;

        public VendorProfileProvider(
            ChatBridgeDbContext dbContext,
            IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<VendorProfileDto> GetProfileAsync(
            CancellationToken cancellationToken = default)
        {
            if (_cache.TryGetValue(CacheKey, out VendorProfileDto? cachedProfile)
                && cachedProfile is not null)
            {
                return cachedProfile;
            }

            var vendor = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    u => u.Role == "Vendor",
                    cancellationToken);

            if (vendor is null)
            {
                throw new InvalidOperationException(
                    "The seeded vendor profile was not found.");
            }

            var profile = new VendorProfileDto
            {
                Name = vendor.Name,
                Description = vendor.OrganizationDescription ?? string.Empty,
                CertificateName = string.Empty
            };

            _cache.Set(
                CacheKey,
                profile,
                TimeSpan.FromMinutes(30));

            return profile;
        }
    }
}
