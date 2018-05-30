using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using GGPGameServer.ApprovalSystem.Common;


namespace GGPMockBootstrapper.ViewModels
{
    public class GGPLogWorkAreaItem : WorkAreaItemBase, Models.ILoggerAppender
    {
        public GGPLogWorkAreaItem(IWorkArea workArea)
            : base(workArea)
        {
            Models.GGPMockLoggerConnector.Singleton.RegisterLoggerAppender(this);
            PauseResumeAction = new ActionViewModel("Pause", new Command(PauseResume), ResourcesProvider.CreateBitmapImageSource("Pause24x24.png"));
            ClearAction = new ActionViewModel("Clear", new Command(Clear), ResourcesProvider.CreateBitmapImageSource("Clear24x24.png"));
            SaveToFileAction = new ActionViewModel("Save to file", new Command(SaveToFile), ResourcesProvider.CreateBitmapImageSource("Save24x24.png"));
            
        }

        
        #region ILoggerAppender Members

        public void Append(GGPMockLoggerService.GGPMockLoggerMessage message)
        {
            if (_paused)
                return;

            lock (_messagesWriteSync)
            {

                UIServices.InvokeOnMainThread(new Action(() =>
                    {
                        Messages.Add(message);

                        if (Messages.Count >= 200)
                        {
                            for (int i = 0; i < 20; i++)
                            {
                                Messages.RemoveAt(0);
                            }
                        }

                        this.CurrentItem = Messages[Messages.Count - 1]; ;
                    }));
            }

        }

        #endregion


        private GGPMockLoggerService.GGPMockLoggerMessage _currentItem;

        public GGPMockLoggerService.GGPMockLoggerMessage CurrentItem
        {
            get { return _currentItem; }
            set
            {
                _currentItem = value;
                OnPropertyChanged(this.GetPropertyName(t => t.CurrentItem));
                
            }
        }

        private static object _messagesWriteSync = new object();
        private ObservableCollection<GGPMockLoggerService.GGPMockLoggerMessage> _messages = new ObservableCollection<GGPMockLoggerService.GGPMockLoggerMessage>();

        public ObservableCollection<GGPMockLoggerService.GGPMockLoggerMessage> Messages
        {
            get
            {
                return _messages;
            }
        }

        public ActionViewModel PauseResumeAction{get; private set;}
        public ActionViewModel ClearAction { get; private set; }
        public ActionViewModel SaveToFileAction { get; private set; }


        private bool _paused = false;
        public void PauseResume()
        {
            _paused = !_paused;

            if (_paused)
            {
                PauseResumeAction.Caption = "Resume";
                PauseResumeAction.ImageSource = ResourcesProvider.CreateBitmapImageSource("Resume24x24.png");
            }
            else
            {
                PauseResumeAction.Caption = "Pause";
                PauseResumeAction.ImageSource = ResourcesProvider.CreateBitmapImageSource("Pause24x24.png");
            }
        }


        private void Clear()
        {
            lock (_messagesWriteSync)
            {
                UIServices.InvokeOnMainThread(new Action(() =>
                {
                    Messages.Clear();
                }));
            }
        }


        private void SaveToFile()
        {
            var dlg = new System.Windows.Forms.SaveFileDialog();

            dlg.DefaultExt = ".txt";
            if (System.Windows.Forms.DialogResult.Cancel == dlg.ShowDialog())
                return;


            StringBuilder sb = new StringBuilder();

            foreach (var m in this.Messages)
            {
                sb.Append(m.DateAndTime.ToString());
                sb.Append("\t");
                sb.Append(m.Priority.ToString());
                sb.AppendLine();
                sb.Append(m.Message);
                sb.AppendLine();
            }


            System.IO.File.WriteAllText(dlg.FileName, sb.ToString());
            

        }


    }
}
