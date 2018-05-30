using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GamesPortal.Client.Services.GamesPortalService
{
    public partial class GameVersionPropertyEntity
    {
        string _propertySetName = null;
        public string PropertySetName
        {
            get
            {
                if (_propertySetName == null)
                {
                    var components = this.Key.Split('.');
                    if (components.Length < 2)
                        _propertySetName = string.Empty;
                    else
                        _propertySetName = components[0];
                }

                return _propertySetName;
            }
        }
    }
}
