using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

using Dms.Jump.TrafficGenerator.Business.Utilities.Tools;
using Dms.Jump.TrafficGenerator.Business.Utilities.Extension;
using Dms.Jump.TrafficGenerator.Business.Helper;

using Sitecore.Data.Items;
using Sitecore.Configuration;

namespace Dms.Jump.TrafficGenerator.Business.Entities
{
    public class RequestSource : TrafficBase
    {
        #region Properties
        public Traffic TrafficParameter { get; private set; }

        public enum ModeEnum {
            Specific, 
            Random
        }

        public bool TrafficInSameSession { get; set; }

        public string TrafficType { get; set; }

        public string Keyword { get; set; }

        public string Campaign { get; set; }

        public int NumberOfRequests { get; set; }

        public ModeEnum Mode { get; set; }

        public List<string> Urls { get; set; }

        public Item ItemInstance { get; set; }

        public string UrlReferrer { get; private set; }


        #endregion

        public RequestSource(Item itemInstance, string urlReferrer, string campaign, string trafficType, string keyword, int numberOfRequests, bool trafficInSameSession, string trafficMode, string trafficKeywords, string trafficUrls)
            : this(null, itemInstance, urlReferrer, campaign, trafficType, keyword, numberOfRequests, trafficInSameSession, trafficMode, trafficUrls)
        {
        }

        public RequestSource(Traffic trafficParameter, Item itemInstance, string urlReferrer, string campaign,
            string trafficType, string keyword, int numberOfRequests, bool trafficInSameSession, string trafficMode, string trafficUrls)
        {
            this.TrafficParameter = trafficParameter;
            this.ItemInstance = itemInstance; 
            this.UrlReferrer = urlReferrer; 
            this.Campaign = campaign; 
            this.TrafficType = trafficType;
            this.Keyword = keyword;
            this.NumberOfRequests = numberOfRequests;
            this.TrafficInSameSession = trafficInSameSession; 
            this.Mode = this.GetTrafficMode(trafficMode);
            this.Urls = StringOperations.SplitStringToEnumerable(trafficUrls, ',');
        }

        public List<Uri> GenerateUris()
        {
            List<Uri> uris = new List<Uri>();
            var urlPrefix = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.AbsolutePath, ""); 

            switch (this.Mode)
            {
                case ModeEnum.Specific:
                    for (int i = 0; i < (int)this.NumberOfRequests; i++)
                    {
                        var index = i % this.Urls.Count;
                        Debug.WriteLine("Traffic: Index=" + index + " Urls:" + this.Urls[index]);
                        uris.Add(new Uri(this.Urls[index]));
                    }

                    break; 
                case ModeEnum.Random: 
                default: 
                    StringCollection pathAndQueries = new StringCollection();
                    ItemHelper.GetRandomRelativePaths(Sitecore.Context.Item, ref pathAndQueries);

                    for (int i = 0; i < (int)this.NumberOfRequests; i++)
                    {
                        int randomIndex = GetRandomNumber(pathAndQueries.Count - 1);
                        string url = urlPrefix + pathAndQueries[randomIndex];
                        uris.Add(new Uri(url)); 
                    }
                    break; 
            }

            return uris; 
        }

        public List<Uri> SetTaffic(List<Uri> uris)
        {
            uris = SetTafficType(uris);

            uris = SetKeyword(uris);

            uris = SetCampaign(uris); 

            return uris; 
        }

        private List<Uri> SetCampaign(List<Uri> uris)
        {
            var campaignTypeQueryStringKey = Settings.GetSetting("Analytics.CampaignQueryStringKey").Trim();

            QueryStringBuilder queryStringBuilder = new QueryStringBuilder();
            queryStringBuilder.Add(campaignTypeQueryStringKey, this.Campaign);

            for (int i = 0; i < uris.Count; i++)
                uris[i] = uris[i].AddQueryString(queryStringBuilder);

            return uris;
        }

        private List<Uri> SetKeyword(List<Uri> uris)
        {
            QueryStringBuilder queryStringBuilder = new QueryStringBuilder();

            var keywordQueryStringKey = Settings.GetSetting("Analytics.SessionKeywordQueryStringKey").Trim();
            var urlReferrerStringKey = Settings.GetSetting("Analytics.SessionUrlReferrerStringKey").Trim();

            queryStringBuilder.Add(keywordQueryStringKey, this.Keyword);
            queryStringBuilder.Add(urlReferrerStringKey, this.UrlReferrer);

            for (int i = 0; i < uris.Count; i++)
                uris[i] = uris[i].AddQueryString(queryStringBuilder);

            return uris;
        }

        private List<Uri> SetTafficType(List<Uri> uris)
        {
            var trafficTypeQueryStringKey = Settings.GetSetting("Analytics.SessionTrafficTypeQueryStringKey").Trim();

            QueryStringBuilder queryStringBuilder = new QueryStringBuilder();
            queryStringBuilder.Add(trafficTypeQueryStringKey, this.TrafficType);

            for (int i = 0; i < uris.Count; i++)
                uris[i] = uris[i].AddQueryString(queryStringBuilder);

            return uris;
        }

        private ModeEnum GetTrafficMode(string trafficMode)
        {
            ModeEnum modeEnum; 

            switch (trafficMode)
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


        public override string ToString()
        {
            return "Traffic: TrafficTypec='" + this.TrafficType.ToString() + "', UrlReferrer='" + UrlReferrer + "', LevelType='" + this.NumberOfRequests.ToString() + "', Traffic='" + this.Mode + "', Keyword='" + this.Keyword + "', Urls.Count='" + this.Urls.Count() + "'";
        }
        
    }
}
