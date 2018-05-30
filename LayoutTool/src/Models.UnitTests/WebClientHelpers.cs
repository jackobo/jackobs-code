using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using NSubstitute;

namespace LayoutTool.Models
{
    public static class WebClientHelpers
    {
        public static void DownloadStringReturns(this IWebClient webClient, string url, string returns)
        {
            webClient.DownloadString(new PathDescriptor(url)).Returns(returns);
        }
    }
}
