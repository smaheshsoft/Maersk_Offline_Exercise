using Maersk.Sorting.Api.Common;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Maersk.Sorting.Api.Services
{
    public class CacheService: ICacheService
    {
        private readonly IMemoryCache _cacheprovider;
        public CacheService(IMemoryCache cacheprovider)
        {
            _cacheprovider = cacheprovider;
        }

        public bool Enqueue(SortJob sortJob)
        {
            if (!_cacheprovider.TryGetValue(Constants.ItemName, out List<SortJob> jobs))
            {
                jobs = new List<SortJob>();
            }
            jobs.Add(sortJob);
            _cacheprovider.Set(Constants.ItemName, jobs);
            return true;
        }

        public IEnumerable<SortJob> Get()
        {
            if (!_cacheprovider.TryGetValue(Constants.ItemName, out List<SortJob> jobs))
            {
                jobs = new List<SortJob>();
            }
            return jobs;
        }

        public SortJob? Get(Guid jobId)
        {
            if (_cacheprovider.TryGetValue(Constants.ItemName, out List<SortJob> jobs))
            {
                var job = jobs.Where(x => x.Id == jobId).FirstOrDefault();
                return job;
            }
            return null;
        }
    }
}
