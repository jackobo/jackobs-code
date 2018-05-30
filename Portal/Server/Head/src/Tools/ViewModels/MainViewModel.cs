using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Configurations;

namespace Tools.ViewModels
{
    public class MainViewModel
    {
        IConfigurationReader _configurationReader;
        public MainViewModel(IConfigurationReader configurationReader)
        {
            _configurationReader = configurationReader;
            GameLanguagesGenerator = new GameLanguagesGenerator(_configurationReader);
        }

        public GameLanguagesGenerator GameLanguagesGenerator { get; private set; }
    }
}
