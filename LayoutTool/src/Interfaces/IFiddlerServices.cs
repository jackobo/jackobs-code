using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool.Interfaces
{
    public interface IFiddlerServices
    {
        void Start();
        void Stop();
        void RegisterFilesOverrideProvider(IFiddlerOverrideProvider provider);
        void UnregisterFilesOverrideProvider(IFiddlerOverrideProvider provider);
    }

    public interface IFiddlerOverrideProvider
    {
        FiddlerOverrideMode GetOverrideMode(string url);
        FiddlerOverrideContent GetOverrideContent(string url, string currentBodyContent);
    }

    public class FiddlerOverrideContent
    {
        public FiddlerOverrideContent(object content, params KeyValuePair<string, string>[] extraHttpHeaders)
        {
            Content = content;
            ExtraHttpHeaders = extraHttpHeaders;
        }

        public object Content { get; private set; }
        public KeyValuePair<string, string>[] ExtraHttpHeaders { get; private set; }

    }

    public enum FiddlerOverrideMode
    {
        NoOverride,
        Normal,
        BypassServer,
        HeadersOnly
    }
}
