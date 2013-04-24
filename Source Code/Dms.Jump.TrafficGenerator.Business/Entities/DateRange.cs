using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Dms.Jump.TrafficGenerator.Business.Utilities.Tools;
using Dms.Jump.TrafficGenerator.Business.Utilities.Extension; 

using Sitecore.Configuration;

namespace Dms.Jump.TrafficGenerator.Business.Entities
{
    public class DateRange : TrafficBase
    {
        public Traffic TrafficParameter { get; private set; }

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        public DateRange(string dateFrom, string dateTo) : this(null, dateFrom, dateTo)
        {
        }

        public DateRange(Traffic trafficParameter, string dateFrom, string dateTo)
        {
            this.TrafficParameter = trafficParameter; 
            this.DateFrom = this.ConvertToDateTime(dateFrom, DateTime.Now.AddMonths(-1));
            this.DateTo = this.ConvertToDateTime(dateTo, DateTime.Now); 
        }

        public List<Uri> SetDateRange(List<Uri> uris)
        {
            TimeSpan span = this.DateTo - this.DateFrom;

            for (int i = 0; i < uris.Count; i++) 
            {
                var delta = this.GetRandomNumber((int)span.TotalDays + 1);

                var startDateTime = DateFrom.AddDays(delta);
                var endDateTime = DateFrom.AddSeconds(2).AddDays(delta);

                System.Diagnostics.Debug.WriteLine("Start date='" + startDateTime + "', End date='" + endDateTime + "'"); 

                // 1. Set Start Date 
                //    Example: http://sitecore650dmsjump/en/Products.aspx?sc_sst=2011-06-09T14:57:27.0725934+02:00
                uris[i] = this.SetStartDate(uris[i], startDateTime);

                // 2. Set End Date
                //    Example: http://sitecore650dmsjump/en/Products.aspx?sc_set=2011-06-09T14:57:27.0725934+02:00
                uris[i] = this.SetEndDate(uris[i], endDateTime);
            }

            return uris; 
        }


        private Uri SetStartDate(Uri uri, DateTime dt)
        {
            var sessionStartTimeQueryStringKey = Settings.GetSetting("Analytics.SessionStartTimeQueryStringKey").Trim();
            return this.SetDateRange(uri, sessionStartTimeQueryStringKey, dt);
        }

        private Uri SetEndDate(Uri uri, DateTime dt)
        {
            var sessionEndTimeQueryStringKey = Settings.GetSetting("Analytics.SessionEndTimeQueryStringKey").Trim();
            return this.SetDateRange(uri, sessionEndTimeQueryStringKey, dt);
        }

        private Uri SetDateRange(Uri uri, string queryStringKey, DateTime dt)
        {
            QueryStringBuilder queryStringBuilder = new QueryStringBuilder();
            queryStringBuilder.Add(queryStringKey, dt.ToString("s"));
            uri = uri.AddQueryString(queryStringBuilder);
            return uri;
        }

        private DateTime ConvertToDateTime(string date, DateTime defaultValue)
        {
            DateTime dtDate = DateTime.MinValue;
            DateTime.TryParse(date, out dtDate);
            if (dtDate == null || dtDate == DateTime.MinValue)
                dtDate = defaultValue;

            return dtDate;
        }

        public override string ToString()
        {
            return "DateRange: DateFrom='" + this.DateFrom.ToString() + "', DateTo='" + this.DateTo.ToString() + "'"; 
        }
    }
}
