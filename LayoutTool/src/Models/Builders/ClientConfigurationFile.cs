using LayoutTool.Interfaces;

namespace LayoutTool.Models.Builders
{
    public class ClientConfigurationFile<TDescriptor> : IClientConfigurationFile
         where TDescriptor : IConfigurationFileDescriptor
    {
        public ClientConfigurationFile(PathDescriptor location, string content, TDescriptor descriptor)
        {
            this.Location = location;
            this.Content = content;
            this.Descriptor = descriptor;
        }

        public TDescriptor Descriptor { get; private set; }

        public string FileName
        {
            get { return this.Descriptor.DefaultFileName; }
        }

        public PathDescriptor Location { get; private set; }
        public string Content { get; private set; }

    }
}
