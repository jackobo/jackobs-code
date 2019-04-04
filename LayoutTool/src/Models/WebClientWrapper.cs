using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using LayoutTool.Interfaces;
using Spark.Infra.Windows;
using System.Web;

namespace LayoutTool.Models
{
    public class WebClientWrapper : IWebClient
    {
        static  ConcurrentDictionary<string, string> _cachedDownloads = new ConcurrentDictionary<string, string>();
        public WebClientWrapper(IWcfServiceFactory wcfServiceFactory, IOperatingSystemServices environmentServices)
        {
            _wcfServiceFactory = wcfServiceFactory;
            _environmentServices = environmentServices;
        }

        IOperatingSystemServices _environmentServices;
        IWcfServiceFactory _wcfServiceFactory;


        public void Dispose()
        {
            
        }

        public string DownloadString(PathDescriptor pathDescriptor)
        {
            
            //return DownloadWithoutCaching(pathDescriptor.ToHttpUrlFormat());

            return DownloadUsingCaching(pathDescriptor.ToHttpUrlFormat());
        }

        private static string ApplyAntiCacheKey(string url)
        {
            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["antiCacheKey"] = DateTime.Now.Ticks.ToString();
            uriBuilder.Query = query.ToString();
            return uriBuilder.ToString();
        }

        private string DownloadWithoutCaching(string url)
        {
            if (IsLocalUrl(url))
            {
                return DownloadUsingRegularWebClient(url);
            }
            else
            {
                return DownloadUsingLayoutToolService(url);
            }
        }

        private string DownloadUsingCaching(string url)
        {
            if (!_cachedDownloads.ContainsKey(url))
            {

                if (IsLocalUrl(url))
                {
                    _cachedDownloads.TryAdd(url, DownloadUsingRegularWebClient(url));
                }
                else
                {
                    _cachedDownloads.TryAdd(url, DownloadUsingLayoutToolService(url));
                }

            }
            return _cachedDownloads[url];
        }

        private bool IsLocalUrl(string url)
        {
            var uriBuilder = new UriBuilder(url);

            var machineIPs = _environmentServices.MachineInformationProvider.GetAllIPv4Addresses();

            return 0 == string.Compare(uriBuilder.Host, "localhost" , true)
                   || uriBuilder.Host == "127.0.0.1" 
                   || 0 == string.Compare(uriBuilder.Host, Environment.MachineName, true)
                   || machineIPs.Contains(uriBuilder.Host);
        }

        private string DownloadUsingRegularWebClient(string url)
        {
            try
            {
                using (var webClient = new WebClient())
                {
                    return webClient.DownloadString(ApplyAntiCacheKey(url));
                }
            }
            catch(System.Net.WebException webException)
            {
                var resp = (webException.Response as HttpWebResponse);

                if (resp != null)
                {
                    if (resp.StatusCode == HttpStatusCode.NotFound)
                        throw new HttpNotFoundException((int)resp.StatusCode, webException.Message, url);
                }

                throw new ApplicationException($"Failed to download string from url '{url}'", webException);
            }
            catch(Exception ex)
            {
                throw new ApplicationException($"Failed to download string from url '{url}'", ex);
            }
        }

        private string DownloadUsingLayoutToolService(string url)
        {
            using (var proxy = _wcfServiceFactory.CreateLayoutToolService())
            {
                var response = proxy.DownloadFileContent(new LayoutToolService.DownloadFileContentRequest() { Url = ApplyAntiCacheKey(url) });

                if (response.HttpErrorCode == (int)HttpStatusCode.NotFound)
                    throw new HttpNotFoundException(response.HttpErrorCode, response.HttpErrorDescription, url);

                if (response.HttpErrorCode != 0)
                    throw new HttpException(response.HttpErrorCode, response.HttpErrorDescription, url);

                return response.Content;
            }
        }
    }

    public class HttpNotFoundException : HttpException
    {
        public HttpNotFoundException(int errorCode, string errorDescription, string url)
            : base(errorCode, errorDescription, url)
        {

        }
    }


    public class HttpException : ApplicationException
    {
        public HttpException(int errorCode, string errorDescription, string url)
            : base(string.Format("Error code: {0}; Error description: {1}; Url: {2}", errorCode, errorDescription, url))
        {
            this.ErrorCode = errorCode;
            this.ErrorDescription = errorDescription;
            this.Url = url;
        }

        public int ErrorCode { get; private set; }

        public string ErrorDescription { get; private set; }

        public string Url { get; private set; }
    }
}
