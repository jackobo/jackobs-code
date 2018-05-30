using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using Microsoft.Practices.ServiceLocation;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels
{
    public class PlayerStatusTypeViewModel : ViewModelBase
    {
        public PlayerStatusTypeViewModel(PlayerStatusType playerStatus, IPlayerStatusFriendlyNameProvider playerStatusFriendlyNameProvider)
        {
            PlayerStatus = playerStatus;
            _playerStatusFriendlyNameProvider = playerStatusFriendlyNameProvider;
            _playerStatusFriendlyNameProvider.Changed += PlayerStatusFriendlyNameProvider_Changed;
        }

        private void PlayerStatusFriendlyNameProvider_Changed(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(Name));
        }

        private PlayerStatusType PlayerStatus { get; set; }


        public string Id
        {
            get { return PlayerStatus.Id; }
        }


        public int Priority
        {
            get
            {
                return PlayerStatus.Priority;
            }
        }


        public string ActionName
        {
            get { return PlayerStatus.ActionName; }
        }

        public bool IsDynamicLayout
        {
            get { return PlayerStatus.IsDynamicLayout; }
        }


        public string Name
        {
            get
            {
                var friendlyName = _playerStatusFriendlyNameProvider.GetFriendlyName(PlayerStatus);
                if (string.IsNullOrEmpty(friendlyName))
                    return PlayerStatus.Name;
                else
                    return friendlyName;
            }
        }

        IPlayerStatusFriendlyNameProvider _playerStatusFriendlyNameProvider;

        public override bool Equals(object obj)
        {
            var theOther = obj as PlayerStatusTypeViewModel;

            if (theOther == null)
                return false;

            return this.PlayerStatus.Equals(theOther.PlayerStatus);
        }

        public override int GetHashCode()
        {
            return this.PlayerStatus.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
