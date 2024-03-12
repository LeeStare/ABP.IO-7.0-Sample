
using Microsoft.Extensions.Configuration;

using Quartz;

using Scriban.Parsing;

using Serilog;

using System;
using System.Threading;
using System.Threading.Tasks;

using Volo.Abp.BackgroundWorkers.Quartz;
using Volo.Abp.Validation;

namespace Sample.Web.Quartz
{
    public class QuartzSample : QuartzBackgroundWorkerBase
    {

        /// <summary>
        /// 多線程用鎖(init, 上限)
        /// </summary>
        private SemaphoreSlim updateCleanDateLock = new SemaphoreSlim(1, 1);

        #region Fields

        /// <summary>
        /// SettingProvider
        /// </summary>
        private readonly IConfiguration appConfiguration;

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger log = Log.ForContext<QuartzSample>();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CleanWorker" /> class
        /// </summary>
        /// <param name="appConfiguration"> SettingProvider </param>
        public QuartzSample(
            IConfiguration appConfiguration)
        {
            this.appConfiguration = appConfiguration;

            JobDetail = JobBuilder.Create<QuartzSample>().WithIdentity(nameof(QuartzSample)).Build();
            // 快速設定
            /*Trigger = TriggerBuilder.Create()
                .WithIdentity(nameof(QuartzSample))
                // 立即開始
                .StartNow()
                .WithSimpleSchedule(options => options
                    .WithIntervalInSeconds(4)
                    .RepeatForever())
                .Build();*/

            Trigger = TriggerBuilder.Create()
                .WithIdentity(nameof(QuartzSample))
                .StartNow()
                .WithDailyTimeIntervalSchedule(options => options
                    .OnEveryDay()
                    // 凌晨00:00開始
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 0))
                    // 早上08:00結束
                    .EndingDailyAt(TimeOfDay.HourAndMinuteOfDay(08, 00))
                    // 設定時程內每4小時作動一次
                    .WithIntervalInHours(4)
                    )
                .Build();
        }

        #endregion Constructor

        /// <summary>
        /// 執行動作
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task Execute(IJobExecutionContext context)
        {
            await updateCleanDateLock.WaitAsync();
            try
            {
            }
            catch (Exception ex)
            {
                this.log.Error(ex.StackTrace ?? "其他錯誤");
            }
            finally
            {
                updateCleanDateLock.Release();
            }
        }
    }
}
