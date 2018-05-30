using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace GGPMockBootstrapper.ViewModels
{
    public static class ResourcesProvider
    {
        private static string _assemblyName = null;
        private static string AssemblyName
        {
            get
            {
                if (string.IsNullOrEmpty(_assemblyName))
                {
                    _assemblyName = typeof(ResourcesProvider).Assembly.GetName().Name;
                }

                return _assemblyName;
            }
        }

        private static Dictionary<string, BitmapImage> ImageSources = new Dictionary<string, BitmapImage>();

        public static BitmapImage CreateBitmapImageSource(string imageName)
        {
            try
            {
                if (!ImageSources.ContainsKey(imageName))
                {
                    ImageSources.Add(imageName, CreateImageOnTheMainThread(imageName));
                }

                return ImageSources[imageName];
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());

                try
                {
                    return CreateBitmapImageSource("MissingImage.png");
                }
                catch
                {
                    return null;
                }

            }
        }

        private static BitmapImage CreateImageOnTheMainThread(string imageName)
        {
            return new BitmapImage(new Uri(string.Format("pack://application:,,,/{0};component/Resources/{1}", AssemblyName, imageName)));
        }
    }
}
