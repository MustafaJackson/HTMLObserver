using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.System.Threading;

namespace BackgroundDlownloader
{
    public class BackgroundDownloaderHelper : IBackgroundTask
    {
        BackgroundTaskCancellationReason _cancelReason = BackgroundTaskCancellationReason.Abort;
        volatile bool _cancelRequested = false;
        BackgroundTaskDeferral _deferral = null;
        ThreadPoolTimer _periodicTimer = null;
        uint _progress = 0;
        IBackgroundTaskInstance _taskInstance = null;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            Debug.WriteLine("Background " + taskInstance.Task.Name + " Starting...");
            var cost = BackgroundWorkCost.CurrentBackgroundWorkCost;
            var settings = ApplicationData.Current.LocalSettings;
            settings.Values["BackgroundWorkCost"] = cost.ToString();

            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

            _deferral = taskInstance.GetDeferral();
            _taskInstance = taskInstance;
            _periodicTimer = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(PeriodicTimerCallback), TimeSpan.FromSeconds(1));
            throw new NotImplementedException();
        }

        private void PeriodicTimerCallback(ThreadPoolTimer timer)
        {
            if ((_cancelRequested == false) && (_progress < 100))
            {
                _progress += 10;
                _taskInstance.Progress = _progress;
            }
            else
            {
                _periodicTimer.Cancel();

                var key = _taskInstance.Task.Name;

                //
                // Record that this background task ran.
                //
                String taskStatus = (_progress < 100) ? "Canceled with reason: " + _cancelReason.ToString() : "Completed";
                var settings = ApplicationData.Current.LocalSettings;
                settings.Values[key] = taskStatus;
                Debug.WriteLine("Background " + _taskInstance.Task.Name + settings.Values[key]);

                
                _deferral.Complete();
            }
        }
        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            _cancelRequested = true;
            _cancelReason = reason;

            Debug.WriteLine("Background " + sender.Task.Name + " Cancel Requested...");
        }
    }
}
