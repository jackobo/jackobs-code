using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalSystemInfoSupportTest
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var proxy = new ApprovalSystemSupportService.ApprovalSystemSupportServiceClient())
            {
                proxy.RegisterCid(1234567);
            }
        }
    }
}
