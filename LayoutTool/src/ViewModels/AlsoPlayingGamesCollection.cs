using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool.ViewModels
{
    public class AlsoPlayingGamesCollection : DropableObservableCollection<AvailableGameViewModel>, IDisposable
    {
        public void Dispose()
        {
            
        }
    }
}
