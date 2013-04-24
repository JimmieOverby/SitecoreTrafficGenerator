using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Web;
using System.Net;

using Sitecore.Analytics.Data;

namespace Dms.Jump.TrafficGenerator.Business.Helper
{
    public class QueryStringHelper
    {
        public static int? SessionTrafficType
        {
            get
            {
                int? trafficType = null;

                string strValue = GetQueryStringValue("Analytics.SessionTrafficTypeQueryStringKey");

                if (!String.IsNullOrEmpty(strValue))
                {
                    switch (strValue)
                    {
                        case "Direct":
                            trafficType = TrafficTypeValues.Direct;
                            break;
                        case "Organic":
                            trafficType = TrafficTypeValues.Organic;
                            break;
                        case "Organic Branded":
                            trafficType = TrafficTypeValues.OrganicBranded;
                            break;
                        case "Referred Other":
                            trafficType = TrafficTypeValues.ReferredOther;
                            break;
                        case "Unknown":
                            trafficType = 0;
                            break;
                        case "Referred - Blog":
                            trafficType = 31;
                            break;
                        case "Referred - News":
                            trafficType = 32;
                            break;
                        case "Referred - Conversations":
                            trafficType = 33;
                            break;
                        case "Referred - Community":
                            trafficType = 34;
                            break;
                        case "Referred - Wiki":
                            trafficType = 35;
                            break;
                        case "Referred - Analyst":
                            trafficType = 36;
                            break;
                        case "Rss":
                            trafficType = 40;
                            break;
                        case "Email":
                            trafficType = 50;
                            break;
                        case "Paid":
                            trafficType = 90;
                            break;
                        default:
                            trafficType = 0;
                            break;
                    }
                }

                return trafficType;
            }
        }


        public static string SessionIPAsString
        {
            get
            {
                return GetQueryStringValue("Analytics.SessionIpQueryStringKey");
            }

        }

        public static byte[] SessionIP
        {
            get
            {
                string strValue = GetQueryStringValue("Analytics.SessionIpQueryStringKey");
                return ConvertToIPByteArray(strValue);
            }
        }

        public static string SessionKeyword
        {
            get
            {
                return GetQueryStringValue("Analytics.SessionKeywordQueryStringKey");
            }

        }

        public static string SessionUrlReferrer
        {
            get
            {
                return GetQueryStringValue("Analytics.SessionUrlReferrerStringKey");
            }

        }

        public static DateTime SessionStartDateTime
        {
            get
            {
                string strValue = GetQueryStringValue("Analytics.SessionStartTimeQueryStringKey");
                return ConvertToDateTime(strValue); 
            }
        }

        public static DateTime SessionEndDateTime
        {
            get
            {
                string strValue = GetQueryStringValue("Analytics.SessionEndTimeQueryStringKey");
                return ConvertToDateTime(strValue);
            }
        }


        private static byte[] ConvertToIPByteArray(string ipString)
        {
            byte[] ipAddress = null;

            IPAddress ip = null;
            if (!string.IsNullOrEmpty(ipString))
                if (IPAddress.TryParse(ipString, out ip))
                    ipAddress = ip.GetAddressBytes();

            return ipAddress;
        }

        private static DateTime ConvertToDateTime(string dtString)
        {
            DateTime dt = DateTime.MinValue;

            if (!String.IsNullOrEmpty(dtString))
                DateTime.TryParse(dtString, out dt);
            return dt; 

        }


        private static string GetQueryStringValue(string configKeyFromQueryString) 
        {
            var queryStringKey = Sitecore.Configuration.Settings.GetSetting(configKeyFromQueryString); 
            string queryStringValue = WebUtil.GetQueryString(queryStringKey).Trim();

            return queryStringValue; 
        }
    }
}
