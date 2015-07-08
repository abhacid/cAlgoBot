using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using cAlgo.API;

namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.FullAccess)]
    public class TradeCopierReceiver : Robot
    {
        private const int TimerIntervalInMs = 100;
        private const string CopierTextObjectName = "info";
        private readonly TimeSpan _defaultTimerInterval = TimeSpan.FromMilliseconds(TimerIntervalInMs);
        private CopierFileService _copierFileService;
        private ExecutionService _executionService;

        private DateTime _lastWriteTimeCache = DateTime.MinValue;
        private TimeSpan _maxInterval;

        [Parameter("Symbols", DefaultValue = "")]
        public string SymbolsFilter { get; set; }

        [Parameter("Copy Protection (SL, TP)", DefaultValue = true)]
        public bool CopyProtectionEnabled { get; set; }

        [Parameter("Copy Long (Buy) positions", DefaultValue = true)]
        public bool CopyBuy { get; set; }

        [Parameter("Copy Short (Sell) positions", DefaultValue = true)]
        public bool CopySell { get; set; }

        [Parameter("CopierID")]
        public int CopierID { get; set; }

        protected override void OnStart()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            _copierFileService = new CopierFileService(this);
            _executionService = new ExecutionService(this);

            if (_copierFileService.CanProcessFile)
            {
                Timer.Start(_defaultTimerInterval);
                ChartObjects.DrawText(CopierTextObjectName, "Signal Receiver is running...", StaticPosition.TopRight, Colors.Yellow);
            }
            else
                Stop();
        }

        protected override void OnTimer()
        {
            _copierFileService.UpdateState();

            IEnumerable<TraderCopierArguments> args;
            if (!CanProcessArguments(out args))
                return;

            if (_executionService.Execute(args))
                _lastWriteTimeCache = _copierFileService.LastWriteTime;

            SetProcessingFileResult();
        }

        private bool CanProcessArguments(out IEnumerable<TraderCopierArguments> args)
        {
            args = null;

            if (!_copierFileService.CanProcessFile)
                return false;

            if (_copierFileService.LastWriteTime == _lastWriteTimeCache)
                return false;

            try
            {
                args = File.ReadAllLines(_copierFileService.FilePath).Select(TraderCopierArguments.Parse).ToArray();
            } catch (IOException ex)
            {
                Print("Read information failed: " + ex.Message);
                SetProcessingFileResult();
                return false;
            } catch (Exception exception)
            {
                Print("Failed to read file: " + exception.Message);
                Stop();
                return false;
            }
            return true;
        }

        private void SetProcessingFileResult()
        {
            if (_executionService.ExecutedOperationsCount > 0)
                DoubleTimerInterval();
            else
                ResetTimer();
        }

        private void ResetTimer()
        {
            Timer.Start(_defaultTimerInterval);
        }

        private void DoubleTimerInterval()
        {
            var newInterval = TimeSpan.FromMilliseconds(Timer.Interval.TotalMilliseconds * 2);
            _maxInterval = TimeSpan.FromMinutes(1);
            if (newInterval > _maxInterval)
                newInterval = _maxInterval;
            Timer.Start(newInterval);
        }

        protected override void OnStop()
        {
            Timer.Stop();

            ChartObjects.RemoveObject(CopierTextObjectName);
        }
    }
}
