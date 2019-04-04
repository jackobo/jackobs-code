using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GamesPortal.Service
{
   

    [DataContract]
    public enum OperatorEnum
    {
        [EnumMember]
        Operator888 = 0,
        [EnumMember]
        Bingo = 1
    }

    [DataContract]
    public enum RecordChangeType
    {
        [EnumMember]
        Added = 0,
        [EnumMember]
        Changed = 1,
        [EnumMember]
        Deleted = 2
    }


}
