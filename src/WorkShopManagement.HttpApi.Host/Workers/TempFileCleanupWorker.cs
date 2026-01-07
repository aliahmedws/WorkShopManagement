using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;
using WorkShopManagement.EntityAttachments.FileAttachments;
using WorkShopManagement.EntityAttachments.FileAttachments.TempFiles;

namespace WorkShopManagement.Workers
{
    public class TempFileCleanupWorker : AsyncPeriodicBackgroundWorkerBase
    {
        private readonly BlobStorageOptions _options;
        public TempFileCleanupWorker(
            AbpAsyncTimer timer,
            IServiceScopeFactory serviceScopeFactory,
            IOptions<BlobStorageOptions> options
            ) : base (timer,serviceScopeFactory)
        {
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));

            var min = (int)_options.TempCleanupIntervalMinutes;
            Timer.Period = (int)TimeSpan.FromMinutes(min).TotalMilliseconds; 
        }
 
        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            var manager = workerContext .ServiceProvider.GetRequiredService<TempFileManager>();
            Logger.LogInformation("TempFileCleanupWorker started.");

            var deleted = await manager.CleanupOldFilesAsync();
            Logger.LogInformation("TempFileCleanupWorker completed. Deleted {Deleted} old temp files.", deleted);
        }
    }
}
