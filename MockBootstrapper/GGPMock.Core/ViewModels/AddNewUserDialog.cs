using System;
using GGPGameServer.ApprovalSystem.Common;

namespace GGPMockBootstrapper.ViewModels
{
    public class AddNewUserDialog : OkCancelDialogViewModel
    {
        public AddNewUserDialog()
        {
            this.Title = "Add new user";
        }
        private string _userName;

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = (value ?? string.Empty).Trim();
                OnPropertyChanged(this.GetPropertyName(t => t.UserName));
            }
        }

        private string _cid;

        public string CID
        {
            get { return _cid; }
            set
            {
                _cid = (value ?? string.Empty).Trim();
                OnPropertyChanged(this.GetPropertyName(t => t.CID));
            }
        }

        protected override bool OK()
        {
            try
            {
                if (!string.IsNullOrEmpty(CID))
                {
                    int intCid;
                    if (!int.TryParse(CID, out intCid))
                    {
                        UIServices.ShowMessage("You must enter a valid CID");
                        return false;
                    }

                    Models.GGPMockDataManager.Singleton.CreateNewUser(intCid, UserName);
                }
                else
                {
                    Models.GGPMockDataManager.Singleton.CreateNewUser(UserName);
                }
            }
            catch (Exception ex)
            {
                UIServices.ShowMessage("Failed to create user: " + ex.Message);
            }
            
            return true;
        }

    }
}
