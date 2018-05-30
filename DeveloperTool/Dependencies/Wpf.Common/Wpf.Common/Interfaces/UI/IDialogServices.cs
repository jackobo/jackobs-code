using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Wpf.Common.Interfaces.UI
{
    public interface IDialogServices
    {
        OkCancelDialogBoxResult ShowOkCancelDialogBox(IOkCancelDialogBoxViewModel dialog);
        string SelectFolder(bool allowCreate = true);

        string SelectFile(string filter = "All files|*.*");

        SaveFileResponse SaveFile(string title, byte[] content, string filter = "All files|*.*", string defaultFile = "");
    }

    public class SaveFileResponse
    {
        public SaveFileResponse(string fileName)
        {
            FileName = fileName;
            Response = OkCancelDialogBoxResult.Ok;
        }

        public SaveFileResponse()
        {
            Response = OkCancelDialogBoxResult.Cancel;
        }
        public string FileName { get; private set; }

        public OkCancelDialogBoxResult Response { get; private set; }
    }


    public enum OkCancelDialogBoxResult
    {
        None,
        Ok,
        Cancel
    }
}
