using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Spark.Infra.Types;
using Spark.Wpf.Common.ViewModels;

namespace GamesPortal.Client.ViewModels.NotificationArea
{
    public interface IBackgroundAction : INotifyPropertyChanged, IDisposable
    {
        BackgroundActionStatus Status { get; }
        string Caption { get; }
        int ProgressPercentage { get; }
        void Start();
        void Cancel();
        string ErrorMessage { get; }
        string ErrorDetails { get;}
    }

    public enum BackgroundActionStatus
    {
        Waiting,
        InProgress,
        Done,
        Error,
        Canceled
    }
   
    public interface IBackgroundActionsArea : INotificationCategory
    {
        void AddAction(IBackgroundAction action); 
    }



    public class BackgroundActionsArea : ViewModelBase, IBackgroundActionsArea
    {
        public BackgroundActionsArea()
        {
            
            this.Actions = new ObservableCollection<IBackgroundAction>();
            this.Actions.CollectionChanged += Actions_CollectionChanged;
            
            this.RemoveActionCommand = new Command<IBackgroundAction>(RemoveAction);
            this.RemoveAllActionsCommand = new Command(RemoveAll, () => this.Actions.Count > 0);
            this.ResumeUnfinishedCommand = new Command(Resume, () => this.Actions.Any(a => CanResume(a.Status)) && !HasActionsInProgress);
            this.StopCommand = new Command(Stop, () => HasActionsInProgress);
        }

        private void UpdateCommandsCanExecute()
        {
            ((Command)this.RemoveAllActionsCommand).RaiseCanExecuteChanged();
            ((Command)this.StopCommand).RaiseCanExecuteChanged();
            ((Command)this.ResumeUnfinishedCommand).RaiseCanExecuteChanged();
            
        }


        void Actions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateCommandsCanExecute();
        }

        private bool CanResume(BackgroundActionStatus status)
        {
            return status != BackgroundActionStatus.Done
                   && status != BackgroundActionStatus.InProgress;

        }


        private bool _isSelected;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                SetProperty(ref _isSelected, value);
            }
        }

        public bool HasActionsInProgress
        {
            get
            {
                return this.Actions.Any(a => a.Status == BackgroundActionStatus.InProgress);
            }
        }

        private bool _autoStartActionWhenAdded = true;

        public bool AutoStartActionWhenAdded
        {
            get { return _autoStartActionWhenAdded; }
            set 
            {
                SetProperty(ref _autoStartActionWhenAdded, value);
            }
        }
      

        public ICommand RemoveActionCommand { get; private set; }

        public ICommand RemoveAllActionsCommand { get; private set; }
        public ICommand ResumeUnfinishedCommand { get; private set; }
        public ICommand StopCommand { get; private set; }

        private void Stop()
        {
            foreach (var a in this.Actions.Where(a => a.Status == BackgroundActionStatus.InProgress))
            {
                a.Cancel();
            }
        }

        private void Resume()
        {
            if (this.Actions.Any(a => a.Status == BackgroundActionStatus.InProgress))
                throw new InvalidOperationException("You can't resume while an operation is already in progress!");

            var action = this.Actions.FirstOrDefault(a => a.Status != BackgroundActionStatus.Done);
            if (action != null)
            {
                action.Start();
            }
        }


        private void RemoveAll()
        {
            foreach (var a in this.Actions.ToArray())
            {
                RemoveAction(a);
            }
        }

        private void RemoveAction(IBackgroundAction action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            if (object.ReferenceEquals(action, this.CurrentAction))
            {
                action.Cancel();
                this.CurrentAction = null;
            }

            this.Actions.Remove(action);

            action.Dispose();
        }

        private IBackgroundAction _currentAction;

        public IBackgroundAction CurrentAction
        {
            get { return _currentAction; }
            set
            {
                 SetProperty(ref _currentAction, value);
            }
        }


        public ObservableCollection<IBackgroundAction> Actions { get; private set; }

        #region INotificationCategory Members

        public string Caption
        {
            get
            {
                return "Downloads";
            }
        }

        #endregion

        #region IDownloadCategory Members

        public void AddAction(IBackgroundAction action)
        {
            
            this.Actions.Add(action);

            action.PropertyChanged += Action_PropertyChanged;

            
            if (AutoStartActionWhenAdded && !this.Actions.Any(a => a.Status == BackgroundActionStatus.InProgress))
            {
                StartNextAction();
            }
            
            
        }

        void Action_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
         
            var action = sender as IBackgroundAction;

            if (e.PropertyName == action.GetPropertyName(t => t.Status) && (action.Status == BackgroundActionStatus.Done || action.Status == BackgroundActionStatus.Error))
            {
                StartNextAction();
            }

            UpdateCommandsCanExecute();
                
        }

        private void StartNextAction()
        {
            var nextAction = this.Actions.Where((a, index) => CanResume(a.Status) && (this.CurrentAction  == null || index > this.Actions.IndexOf(this.CurrentAction)))
                                         .FirstOrDefault();

            if (nextAction != null)
            {
                nextAction.Start();
                this.CurrentAction = nextAction;
            }
        }

        #endregion
    }

  
}
