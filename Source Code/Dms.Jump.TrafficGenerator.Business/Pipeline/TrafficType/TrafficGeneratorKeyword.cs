using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sitecore.Analytics.Pipelines.TrafficTypes;
using Sitecore.Analytics.Data;
using Sitecore.Web;
using Sitecore.Diagnostics;

using Dms.Jump.TrafficGenerator.Business.Helper;

namespace Dms.Jump.TrafficGenerator.Business.Pipeline.TrafficType
{
    public class TrafficGeneratorKeyword : TrafficTypeProcessor
    {
        public override void Process(TrafficTypeArgs args)
        {
            try
            {
                // 2. Traffic Generator Traffic Type Processor
                string queryStringValue = QueryStringHelper.SessionKeyword; 

                
            }
            catch (Exception exp)
            {
                Log.Error("Traffic Generator Keyword parse referrer pipeline: " + exp.InnerException, this);
            }

        }
    }
}
