using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spark.Infra.Types;

namespace GamesPortal.Service.Artifactory
{
    public static class WellKnownNamesAndValues
    {
        public static readonly string VersionProperty = "version";
        public static readonly string UserProperty = "Common.User";

        public static bool IsVersionProperty(string propertyKey)
        {
            return 0 == string.Compare(propertyKey, VersionProperty, true);
        }

        public static readonly string NDL = "NDL";
        public static readonly string State = "State";
        public static readonly string PMApproved = "PMApproved";
        public static readonly string QAApproved = "QAApproved";
        public static readonly string Production = "Production";

        public static readonly string True = "True";
        public static readonly string False = "False";


        public static bool IsQaApprovedState(string stateValue)
        {
            return QA_APPROVED_STATUSES.Contains(stateValue?.ToUpper());
        }

        internal static bool IsPmApprovedState(string stateValue)
        {
            return string.Compare(stateValue, "true", true) == 0;
        }


        public static bool IsProductionStatus(string statusValue)
        {
            return string.Compare("PRODUCTION", statusValue, true) == 0;
        }

        private static readonly string[] QA_APPROVED_STATUSES = new string[]
          {
                "APPROVED",
                "CERTIFIED",
                "PRODUCTION"
          };

        

        public static string BuildPropertyKey(string propertySet, string propertyName)
        {
            if (string.IsNullOrEmpty(propertySet))
                return propertyName;

            return propertySet + "." + propertyName;
        }

        public static bool IsApprovalProperty(string key)
        {
            var propertyName = ArtifactoryProperty.GetPropertyNameFromKey(key);
            return propertyName == State || propertyName == PMApproved;

        }

        
    }
}
