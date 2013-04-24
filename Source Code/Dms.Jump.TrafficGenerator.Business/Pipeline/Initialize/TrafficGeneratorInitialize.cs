using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Analytics.Pipelines.InitializeTracker;
using System.Web;
using Sitecore.Analytics.Data.DataAccess;
using Sitecore.Analytics;
using Sitecore.Analytics.Data.DataAccess.DataSets;
using Sitecore.Web;
using Sitecore.Sites;
using Sitecore.Analytics.Pipelines.CreateVisits;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Extensions.StringExtensions;
using System.Net;
using System.Diagnostics;

using Dms.Jump.TrafficGenerator.Business.Helper;

namespace Dms.Jump.TrafficGenerator.Business.Pipeline.Initialize
{
    public class TrafficGeneratorInitializeTracker : Sitecore.Analytics.Pipelines.InitializeTracker.Initialize
    {
        public override void Process(InitializeTrackerArgs args)
        {
            try
            {
                Sitecore.Diagnostics.Assert.ArgumentNotNull(args, "args");
                HttpContext current = HttpContext.Current;

                if (current == null)
                {
                    args.AbortPipeline();
                }
                else
                {
                    Visitor visitor = Tracker.Visitor;
                    if (visitor.Settings.IsNew)
                    {
                        this.CreateVisitor(args, visitor);
                    }

                    VisitorDataSet.VisitsRow visit = null;
                    if (!visitor.Settings.IsFirstRequest)
                    {
                        visit = visitor.GetCurrentVisit();
                    }
                    if (visit == null)
                    {
                        visit = this.CreateVisit(current, visitor);
                    }

                    var startDateTime = QueryStringHelper.SessionStartDateTime; 

                    if (startDateTime != DateTime.MinValue)
                        visit.StartDateTime = startDateTime;

                    this.CreatePage(current, visit);
                }
            }
            catch (Exception exp)
            {
                Sitecore.Diagnostics.Log.Error("Traffic generator initialize pipeline: " + exp.InnerException, this);
            }

        }

        private void CreateVisitor(InitializeTrackerArgs args, Visitor visitor)
        {
            visitor.ExternalUser = string.Empty;
            visitor.VisitCount = 0;
            visitor.SetVisitorClassification(0, 0, true);
            visitor.IntegrationId = Guid.Empty;
            visitor.IntegrationLabel = string.Empty;
        }


        private VisitorDataSet.VisitsRow CreateVisit(HttpContext httpContext, Visitor visitor)
        {
            VisitorDataSet.VisitsRow currentVisit = visitor.CurrentVisit;
            if (currentVisit == null)
            {
                currentVisit = visitor.CreateVisit(visitor.CookieVisitId);
            }
            currentVisit.AspNetSessionId = WebUtil.GetSessionID();
            
            HttpRequest request = httpContext.Request;
            byte[] ip = this.GetIp(httpContext); 

            string browser = request.Browser.Browser;
            string version = request.Browser.Version;
            string browserVersion = request.Browser.Version;
            string platform = request.Browser.Platform;
            string minorName = string.Empty;
            string operatingSystem = string.Empty;

            currentVisit.Ip = ip;
            currentVisit.Browser = visitor.DataContext.GetBrowser(browser, version, browserVersion);
            currentVisit.UserAgent = visitor.DataContext.GetUserAgent(request.UserAgent ?? string.Empty);
            currentVisit.GeoIp = visitor.DataContext.GetGeoIp(ip);

            var spoofedUserHostName = QueryStringHelper.SessionIPAsString;
            if( !String.IsNullOrEmpty(spoofedUserHostName) ) 
                currentVisit.RDNS = spoofedUserHostName;
            else 
                currentVisit.RDNS = request.UserHostName ?? string.Empty;

            currentVisit.OperatingSystem = visitor.DataContext.GetOperatingSystem(platform, minorName, operatingSystem);
            currentVisit.Screen = visitor.DataContext.GetScreen(this.GetDimensions(request));
            
            SiteContext site = Context.Site;
            if (site != null)
            {
                currentVisit.MultiSite = site.Name;
            }
            
            CreateVisitArgs args = new CreateVisitArgs(currentVisit, request);
            CreateVisitPipeline.Run(args);
            return currentVisit;
        }

        private byte[] GetIp(HttpContext httpContext) 
        {
            byte[] ipAddress = QueryStringHelper.SessionIP; 
            
            // If not spoofed use default
            if (ipAddress == null)
            {
                HttpRequest request = httpContext.Request;
                ipAddress = this.GetIp(request.UserHostAddress ?? string.Empty);
            }

            return ipAddress;
        }

        private byte[] GetIp(string userHostAddress)
        {
            IPAddress address;
            if (IPAddress.TryParse(userHostAddress, out address))
            {
                return address.GetAddressBytes();
            }
            Sitecore.Diagnostics.Log.Warn("Failed to parse ip address: " + userHostAddress, this);
            return new byte[4];
        }



        private string GetDimensions(HttpRequest request)
        {
            HttpBrowserCapabilities browser = request.Browser;
            if (browser == null)
            {
                return string.Empty;
            }
            return string.Format("{0}x{1}", browser.ScreenPixelsWidth, browser.ScreenPixelsHeight);
        }


        private void CreatePage(HttpContext httpContext, VisitorDataSet.VisitsRow visit)
        {
            VisitorDataSet.PagesRow row = visit.CreatePage();


            var endDateTime = QueryStringHelper.SessionEndDateTime;

            if (endDateTime != DateTime.MinValue)
                visit.EndDateTime = endDateTime;

            VisitorDataSet.PagesDataTable table = (VisitorDataSet.PagesDataTable)row.Table;
            row.UrlText = WebUtil.GetRawUrl();
            string urlText = row.UrlText;
            int length = urlText.IndexOfAny("?#".ToCharArray());
            if (length >= 0)
            {
                urlText = urlText.Left(length);
            }

            row.Url = urlText.Right(table.UrlColumn.MaxLength);
            DeviceItem device = Context.Device;
            row.DeviceId = (device != null) ? device.ID.Guid : Guid.Empty;
            Item item = Context.Item;
            if (item != null)
            {
                row.ItemId = item.ID.Guid;
                row.ItemLanguage = item.Language.Name;
                row.ItemVersion = item.Version.Number;
            }
        }


    }
}
    