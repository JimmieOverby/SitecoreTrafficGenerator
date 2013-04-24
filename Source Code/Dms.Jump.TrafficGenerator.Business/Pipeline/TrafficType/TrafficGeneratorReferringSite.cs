using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Analytics.Pipelines.TrafficTypes;
using Sitecore.Analytics.Data;
using Sitecore.Web;
using Sitecore.Diagnostics;
using Sitecore.Analytics.Data.DataAccess.DataSets;

using Dms.Jump.TrafficGenerator.Business.Helper;

namespace Dms.Jump.TrafficGenerator.Business.Pipeline.TrafficType
{
    public class TrafficGeneratorReferringSite : ReferringSite
    {
        public override void Process(TrafficTypeArgs args)
        {
            try
            {
                // 1. Set Traffic Type
                int? trafficType = QueryStringHelper.SessionTrafficType;

                if (trafficType.HasValue)
                    args.Visit.ReferringSite.TrafficType = trafficType.Value;

                base.Process(args); 

            }
            catch (Exception exp)
            {
                Log.Error("Traffic Generator Traffic Type pipeline: " + exp.InnerException, this);
            }

        }
    }
}
