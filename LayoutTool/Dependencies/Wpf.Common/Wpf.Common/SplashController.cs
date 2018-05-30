using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Prism.Events;

namespace Spark.Wpf.Common
{
    internal class SplashModule<TShell>
        where TShell : Window
    {
        public SplashModule(TShell shell, IEventAggregator eventAggregator)
        {
            Shell = shell;
            EventAggregator = eventAggregator;
        }

        TShell Shell { get; set; }
        IEventAggregator EventAggregator { get; set; }

        private AutoResetEvent WaitForCreation { get; set; }
        public void Start()
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(
              (Action)(() =>
              {
                  Shell.Show();
                  Shell.Activate();
                  Shell.Topmost = true;  // important
                  Shell.Topmost = false; // important
                  Shell.Focus();         // important
                  EventAggregator.GetEvent<CloseSplashEvent>().Publish(new CloseSplashEvent());
              }));

            WaitForCreation = new AutoResetEvent(false);

            ThreadStart showSplash =
              () =>
              {
                  Dispatcher.CurrentDispatcher.BeginInvoke(
                    (Action)(() =>
                    {
                     

                        var splash = new Views.SplashView();
                        EventAggregator.GetEvent<CloseSplashEvent>().Subscribe(e => splash.Dispatcher.BeginInvoke((Action)splash.Close), ThreadOption.PublisherThread, true);

                        splash.Show();

                        WaitForCreation.Set();
                    }));

                  Dispatcher.Run();
              };

            var thread = new Thread(showSplash) { Name = "Splash Thread", IsBackground = true };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            WaitForCreation.WaitOne();
        }

        
    }

    public class CloseSplashEvent : PubSubEvent<CloseSplashEvent>
    {
    }
}
