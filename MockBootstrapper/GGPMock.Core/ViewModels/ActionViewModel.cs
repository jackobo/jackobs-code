using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using GGPGameServer.ApprovalSystem.Common;

namespace GGPMockBootstrapper.ViewModels
{
    public class ActionViewModel : IActionViewModel
    {
        public ActionViewModel(string caption, ICommand command)
            : this(caption, command, null)
        {
        }

        public ActionViewModel(string caption, ICommand command, ImageSource imageSource)
        {
            this.Caption = caption;
            this.Command = command;
            this.ImageSource = imageSource;
        }


        public ICommand Command { get; private set; }


        string _caption;

        public virtual string Caption
        {
            get { return _caption; }
            set
            {
                _caption = value;
                OnPropertyChanged(this.GetPropertyName(t => t.Caption));
            }
        }


        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }


        System.Windows.Media.ImageSource _imageSource;

        public System.Windows.Media.ImageSource ImageSource
        {
            get { return _imageSource; }
            set
            {
                _imageSource = value;
                OnPropertyChanged(this.GetPropertyName(t => t.ImageSource));
            }
        }
        

    }
}
