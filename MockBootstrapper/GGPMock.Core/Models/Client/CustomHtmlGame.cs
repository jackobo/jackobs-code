using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.Models.Client
{
    public class CustomHtml5Game
    {
        public CustomHtml5Game()
        {

        }

        public CustomHtml5Game(int gameType, string baseUrl)
        {
            this.GameType = gameType;
            this.BaseUrl = baseUrl;
        }

        public int GameType { get; set; }
        public string BaseUrl { get; set; }
    }


    public class CustomHtmlGamesRepository
    {
        public CustomHtmlGamesRepository()
        {
            this.Games = new List<CustomHtml5Game>();
        }

        public List<CustomHtml5Game> Games { get; set; }
    }
}
