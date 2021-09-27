using Maersk.Sorting.Api.Common;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Maersk.Sorting.Api.Services
{
    public class SortBackgroundService : BackgroundService
    {
        private readonly IMemoryCache _cacheprovider;
        private readonly ISortJobProcessor _sortJobProcessor;
        public SortBackgroundService(IMemoryCache cacheprovider, ISortJobProcessor sortJobProcessor)
        {
            _cacheprovider = cacheprovider;
            _sortJobProcessor = sortJobProcessor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                List<SortJob> jobs;
                if (_cacheprovider.TryGetValue(Constants.ItemName, out jobs))
                {
                    jobs.ForEach(async job =>
                    {
                        if (job.Status == SortJobStatus.Pending)
                        {
                            var res = await _sortJobProcessor.Process(job);
                            var index = jobs.ToList().IndexOf(job);
                            if (index != -1)
                                jobs[index] = res;
                            _cacheprovider.Set(Constants.ItemName, jobs);
                        }
                    });
                }

                await Task.Delay(new TimeSpan(0, 1, 0));
            }

        }

    }
}
