using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool.Interfaces
{
    public interface IBrandInformationProvider
    {
        BrandEntity[] GetProductionBrands();
        QAEnvironmentEntity[] GetQAEnvironments();
    }


}
