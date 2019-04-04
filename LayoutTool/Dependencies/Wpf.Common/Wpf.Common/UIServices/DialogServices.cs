using System;
using Microsoft.Practices.ServiceLocation;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace Spark.Wpf.Common.UIServices
{
    public class DialogServices : IDialogServices
    {
        public DialogServices(IWindowsFactory windowsFactory, IApplicationServices applicationServices)
        {
            _windowsFactory = windowsFactory;
            _applicationServices = applicationServices;
        }


        IWindowsFactory _windowsFactory;
        IApplicationServices _applicationServices;

        public OkCancelDialogBoxResult ShowOkCancelDialogBox(IOkCancelDialogBoxViewModel dialogViewModel)
        {

            var result = OkCancelDialogBoxResult.None;

            var dialogCommands = new DialogBoxCommands(dialogViewModel);
            var window = _windowsFactory.CreateModalWindow();

            
            dialogCommands.OkCommand = new Command(() => 
                            {
                                result = ExecuteOkDialog(dialogViewModel, window);
                            });

            dialogCommands.CancelCommand = new Command(() =>
                            {
                                result = ExecuteCancelDialog(dialogViewModel, window);
                            });


            window.Commands = dialogCommands;


            _applicationServices.ExecuteOnUIThread(() => window.ShowModal());
                                    
            return result;
        }

        private OkCancelDialogBoxResult ExecuteOkDialog(IOkCancelDialogBoxViewModel dialogViewModel, IModalWindow window)
        {
            dialogViewModel.ExecuteOk();
            window.Close();
            return OkCancelDialogBoxResult.Ok;
        }


        private OkCancelDialogBoxResult ExecuteCancelDialog(IOkCancelDialogBoxViewModel dialogViewModel, IModalWindow window)
        {
            dialogViewModel.ExecuteCancel();
            window.Close();
            return OkCancelDialogBoxResult.Cancel;
        }


        #region IDialogServices Members


        public string SelectFolder(bool allowCreate = true)
        {
            var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowserDialog.ShowNewFolderButton = allowCreate;
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                return null;

            return folderBrowserDialog.SelectedPath;
        }

        public string SelectFile(string filter = "All files|*.*")
        {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog();

            openFileDialog.Filter = filter;
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                return null;

            return openFileDialog.FileName;
        }


        public SaveFileResponse SaveFile(string title, byte[] content, string filter = "All files|*.*", string defaultFile = "")
        {
            var openFileDialog = new System.Windows.Forms.SaveFileDialog();

            openFileDialog.Filter = filter;
            openFileDialog.Title = title;
            openFileDialog.FileName = defaultFile;



            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                return new SaveFileResponse();

            using (var stream = openFileDialog.OpenFile())
            {
                foreach(var b in content)
                {
                    stream.WriteByte(b);
                }
            }

            return new SaveFileResponse(openFileDialog.FileName);
        }

        #endregion
    }
}
