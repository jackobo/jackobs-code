using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Interfaces
{
    public interface IPublishPayloadSerializer
    {
        byte[] Serialize(IPublishPayload publishPayload);
        IPublishPayload Deserialize(byte[] content);
    }
}
