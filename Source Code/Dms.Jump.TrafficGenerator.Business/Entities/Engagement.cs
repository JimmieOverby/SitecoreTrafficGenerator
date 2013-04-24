using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Sitecore.Web;
using Sitecore.Configuration;

using Dms.Jump.TrafficGenerator.Business.Utilities.Tools;
using Dms.Jump.TrafficGenerator.Business.Helper;
using Dms.Jump.TrafficGenerator.Business.Utilities.Extension; 

namespace Dms.Jump.TrafficGenerator.Business.Entities
{
    public class Engagement : TrafficBase
    {
        #region Properties
        public enum ModeEnum
        {
            Specific,
            Random
        }

        public Traffic TrafficParameter { get; private set; }

        public int Percentage { get; private set; }

        public ModeEnum Mode { get; private set; }

        public List<string> SpecificGoalNames { get; private set; }

        public string IP { get; private set; }

        #endregion

        #region ctor
        public Engagement(string ip, int percentage, string engagementMode, string engagementGoals)
            : this(null, ip, percentage, engagementMode, engagementGoals)
        {
        }

        public Engagement(Traffic trafficParameter, string ip, int percentage, string engagementMode, string engagementGoals)
        {
            this.IP = ip;
            this.TrafficParameter = trafficParameter;
            this.Percentage = percentage;
            this.Mode = this.GetMode(engagementMode);
            this.SpecificGoalNames = StringOperations.SplitStringToEnumerable(engagementGoals, ',');
        }
        #endregion 

        public List<Uri> SetEngagement(List<Uri> uris)
        {
            // 1. Setup goals
            uris = SetPageEvents(uris); 

            // 2. Setup Campaigns

            // 3. Setup Keywords

            // 4. Setup Refferals - Search Header
            uris = SetIP(uris); 

            return uris;
        }


        private List<Uri> SetIP(List<Uri> uris)
        {
            var ipQueryStringKey = Settings.GetSetting("Analytics.SessionIpQueryStringKey").Trim();

            QueryStringBuilder queryStringBuilder = new QueryStringBuilder();
            queryStringBuilder.Add(ipQueryStringKey, this.IP);

            for (int i = 0; i < uris.Count; i++)
                uris[i] = uris[i].AddQueryString(queryStringBuilder);
            
            return uris;
        }

        private List<Uri> SetPageEvents(List<Uri> uris)
        {
            // 1. Setup goals
            //    Example: http://sitecore650dmsjump/en/Products.aspx?sc_trk=Search,07D9A696-A2FE-4A59-88FB-A57FE386B8AD,1,Hallo,World

            var numberOfGoals = uris.Count * (int)this.Percentage / 100;
            var goalNames = this.GenerateGoalNames(numberOfGoals);

            var eventQueryStringKey = Settings.GetSetting("Analytics.EventQueryStringKey").Trim();

            for (int i = 0; i < goalNames.Count; i++)
            {
                QueryStringBuilder queryStringBuilder = new QueryStringBuilder();
                queryStringBuilder.Add(eventQueryStringKey, goalNames[i]); //  + ",00000000-0000-0000-0000-000000000000,13,Traffic,Emulation");
                uris[i] = uris[i].AddQueryString(queryStringBuilder); 
            }
            return uris;
        }


        #region private helpers
        private ModeEnum GetMode(string engagementMode)
        {
            ModeEnum modeEnum;

            switch (engagementMode)
            {
                case "Specific":
                    modeEnum = ModeEnum.Specific;
                    break;
                case "Random":
                default:
                    modeEnum = ModeEnum.Random;
                    break;
            }

            return modeEnum;
        }


        private StringCollection GenerateGoalNames(int numberOfGoals)
        {
            StringCollection result = new StringCollection();

            switch (this.Mode)
            {
                case ModeEnum.Specific:
                    for (int i = 0; i < numberOfGoals; i++)
                    {
                        var index = i % this.SpecificGoalNames.Count;
                        Debug.WriteLine("Engagement: Index=" + index + " Goals:" + this.SpecificGoalNames[index]);
                        result.Add(this.SpecificGoalNames[index]);
                    }
                    break; 
                case ModeEnum.Random:
                default:
                    StringCollection goalNames = ItemHelper.GetRandomGoalNames();
                    for (int i = 0; i < numberOfGoals; i++)
                    {
                        int randomIndex = GetRandomNumber(goalNames.Count - 1);
                        result.Add(goalNames[randomIndex]);
                    }
                    break;
            }

            return result;
        }
        #endregion 

        public override string ToString()
        {
            return "Engagement: IP='" + IP + "', Percentage='" + this.Percentage.ToString() + "', EngagementType='" + this.Mode + "', GoalNames.Count='" + this.SpecificGoalNames.Count() + "'";
        }   


    }
}
