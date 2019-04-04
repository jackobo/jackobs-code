using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using GGPGameServer.ApprovalSystem.Common;

namespace GGPMockBootstrapper.Models
{
    public class OpenGameParameters : INotifyPropertyChanged
    {

        public OpenGameParameters(int gameTypeId, string gameName, string gamePhisicalPath)
        {
            this.GameTypeId = gameTypeId;
            this.GameName = gameName;
            this.GamePhisicalPath = gamePhisicalPath;
        }

        public int GameTypeId { get; private set; }
        public string GameName { get; private set; }
        public string GamePhisicalPath { get; private set; }

        bool _isFreePlay;

        public bool IsFreePlay
        {
            get { return _isFreePlay; }
            set
            {
                _isFreePlay = value;
                OnPropertyChanged(this.GetPropertyName(t => t.IsFreePlay));
            }
        }

        private string _currency;

        public string Currency
        {
            get { return _currency; }
            set
            {
                _currency = value;
                OnPropertyChanged(this.GetPropertyName(t => t.Currency));
            }
        }


        private BrandInfo _brand;

        public BrandInfo Brand
        {
            get { return _brand; }
            set
            {
                _brand = value;
                OnPropertyChanged(this.GetPropertyName(t => t.Brand));
            }
        }

        private bool _soundEnabled = false;

        public bool SoundEnabled
        {
            get { return _soundEnabled; }
            set
            {
                _soundEnabled = value;
                OnPropertyChanged(this.GetPropertyName(t => t.SoundEnabled));
            }
        }

        GGPMockDataProvider.LanguageMock _language;
        public GGPMockDataProvider.LanguageMock Language
        {
            get { return _language; }
            set
            {
                _language = value;
                OnPropertyChanged(this.GetPropertyName(t => t.Language));
            }
        }

        Models.JoinTypeEnum _joinType = Models.JoinTypeEnum.Regular;

        public Models.JoinTypeEnum JoinType
        {
            get { return _joinType; }
            set
            {
                _joinType = value;
                OnPropertyChanged(this.GetPropertyName(t => t.JoinType));
            }
        }

      

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }



        #endregion
    }
}
