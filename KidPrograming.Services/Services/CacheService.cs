﻿using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;

namespace KidPrograming.Services.Services
{
    public class CacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IDistributedCache _distributedCache;
        public CacheService(IConnectionMultiplexer connectionMultiplexer, IDistributedCache distributedCache)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _distributedCache = distributedCache;
        }
        public async Task<string?> GetCacheResponseAsync(string key)
        {
            var cacheResponse = await _distributedCache.GetStringAsync(key);
            return string.IsNullOrEmpty(cacheResponse) ? null : cacheResponse;
        }

        public async Task RemoveCacheResponseAsync(string pattern)
        {
            if(string.IsNullOrEmpty(pattern) )
               throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Value can not be null or whitespace");
            await foreach (var key in GetKeysAsync(pattern + "*"))
            {
                await _distributedCache.RemoveAsync(key);
            }
        }
        private async IAsyncEnumerable<string> GetKeysAsync(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Value can not be null or whitespace");
            foreach (var endPoint in _connectionMultiplexer.GetEndPoints())
            {
                var server = _connectionMultiplexer.GetServer(endPoint);
                foreach (var key in server.Keys(pattern: pattern))
                {
                    yield return key.ToString();
                }
            }
        }

        public async Task SetCacheResponseAsync(string key, object reponse, TimeSpan timeOut)
        {
            if (reponse == null)
                return;

            var serializedResponse = JsonConvert.SerializeObject(reponse, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            await _distributedCache.SetStringAsync(key, serializedResponse, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = timeOut
            });
        }
    }
}
