using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sitecore.Data.Items;

namespace Dms.Jump.TrafficGenerator.Business.Entities
{
    public class Traffic : TrafficBase
    {
        public RequestSource RequestSource { get; set; }
        public DateRange DateRange { get; set; }
        public Engagement Engagement { get; set; }

        public Traffic(DateRange dateRange, RequestSource requestSource, Engagement engagement)
        {
            this.DateRange = dateRange;
            this.RequestSource = requestSource; 
            this.Engagement = engagement;
        }

        public Traffic(Item itemInstance, string urlReferrer, string ip, string dateFrom, string dateTo, string campaign,
            string trafficType, string keyword, int numberOfRequests, bool trafficInSameSession, string trafficMode, string trafficUrls,
            int engagementPercentage, string engagementMode, string engagementGoals) 
        {
            this.DateRange = new DateRange(this, dateFrom, dateTo);
            this.RequestSource = new RequestSource(this, itemInstance, urlReferrer, campaign, trafficType, keyword, numberOfRequests, trafficInSameSession, trafficMode, trafficUrls);
            this.Engagement = new Engagement(this, ip, engagementPercentage, engagementMode, engagementGoals);
        }

        public List<Uri> GetUris()
        {
            List<Uri> uris = new List<Uri>();

            // 1. Generate all the Uris base
            uris = this.RequestSource.GenerateUris();

            // 2. Set Traffic Type
            uris = this.RequestSource.SetTaffic(uris); 

            // 3. Setup Date Range
            uris = this.DateRange.SetDateRange(uris);

            // 3. Setup Engagement
            uris = this.Engagement.SetEngagement(uris);

            return uris;
        }

        public override string ToString()
        {
            return "TrafficParameter: [" + this.DateRange.ToString() + "], [" + this.RequestSource.ToString() + "], [" + this.Engagement.ToString() + "]"; 
        }
    }
}
