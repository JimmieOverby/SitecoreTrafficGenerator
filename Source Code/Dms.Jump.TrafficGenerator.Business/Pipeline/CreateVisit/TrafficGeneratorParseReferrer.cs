using System.Web;
using Sitecore.Analytics.Data.DataAccess.DataSets;
using Sitecore.Diagnostics;
using System;

using Sitecore.Analytics;
using Sitecore.Analytics.Pipelines.CreateVisits;
using Sitecore.Analytics.Pipelines.ParseReferrer;
using Sitecore.Web;

using Dms.Jump.TrafficGenerator.Business.Helper;

namespace Dms.Jump.TrafficGenerator.Pipeline.CreateVisit
{
    public class TrafficGeneratorParseReferrer : CreateVisitProcessor
    {
        // Methods
        private void Parse(HttpRequest request, VisitorDataSet.VisitsRow visit)
        {
            Assert.ArgumentNotNull(request, "request");

            string urlReferrerValue = QueryStringHelper.SessionUrlReferrer; 
            
            Uri urlReferrer = null; 
            if (!String.IsNullOrEmpty(urlReferrerValue))
            {
                if( !urlReferrerValue.StartsWith("http://") || !urlReferrerValue.StartsWith("https://") ) 
                    urlReferrerValue = "http://" + urlReferrerValue + HttpContext.Current.Request.Url.Query; 

                urlReferrer = new Uri(urlReferrerValue); 
            }
            else
            {
                urlReferrer = request.UrlReferrer;
            }

            if (urlReferrer == null)
            {
                visit.Keywords = Tracker.Visitor.DataContext.GetKeywords(string.Empty);
                visit.ReferringSite = Tracker.Visitor.DataContext.GetReferringSite(string.Empty);
                visit.Referrer = string.Empty;
            }
            else
            {
                visit.ReferringSite = Tracker.Visitor.DataContext.GetReferringSite(urlReferrer.Host);

                visit.Referrer = urlReferrer.ToString();
                ParseReferrerArgs args2 = new ParseReferrerArgs
                {
                    UrlReferrer = urlReferrer,
                    Visit = visit
                };

                ParseReferrerArgs args = args2;
                ParseReferrerPipeline.Run(args);
                if (visit.Keywords == null)
                {
                    visit.Keywords = Tracker.Visitor.DataContext.GetKeywords(string.Empty);
                }
            }
        }

        public override void Process(CreateVisitArgs args)
        {
            try
            {
                Assert.ArgumentNotNull(args, "args");
                this.Parse(args.Request, args.Visit);
            }
            catch (Exception exp)
            {
                Log.Error("Traffic generator parse referrer pipeline: " + exp.InnerException, this); 
            }
        }
    }

}