using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Web;
using Dms.Jump.TrafficGenerator.Business.Utilities.Tools;
using Dms.Jump.TrafficGenerator.Business.Entities;

namespace Dms.Jump.TrafficGenerator.Business.Helper
{
    public class CsvHelper
    {
        public List<Traffic> GetTrafficParameters(StreamReader sr)
        {
            List<Traffic> trafficParameters = new List<Traffic>();

            int lineNumber = 0;
            // 2. Read through file
            while (sr.Peek() >= 0)
            {
                string line = sr.ReadLine();

                // 3. Skip first line, as this contains headers only
                if (lineNumber > 0)
                {
                    string[] columns = line.Split(';');

                    if (columns.Length > 0)
                    {   
                        var urlReferrer = columns[0];
                        var ip = columns[1];

                        // 4. Skip empty file lines
                        if (!String.IsNullOrEmpty(ip) && StringOperations.IsIP(ip, true) && columns.Length >= 11)
                        {
                            var trafficType = columns[2];
                            var campaign = columns[3] != "-" ? columns[3].Trim().Replace("\"", "") : string.Empty;
                            var keyword = columns[4] != "-" ? columns[4].Trim().Replace("\"", "") : string.Empty;
                            var dateFrom = columns[5] != "-" ? columns[5].Trim().Replace("\"", "") : string.Empty;
                            var dateTo = columns[6] != "-" ? columns[6].Trim().Replace("\"", "") : string.Empty;
                            
                            int numberOfRequests = 0; 
                            Int32.TryParse(columns[7], out numberOfRequests);

                            bool trafficInSameSession = false;
                            Boolean.TryParse(columns[8], out trafficInSameSession);

                            var trafficMode = columns[9] != "-" ? columns[9].Trim().Replace("\"","") : string.Empty;
                            var trafficUrls = columns[10] != "-" ? columns[10].Trim().Replace("\"", "") : string.Empty;
                            
                            var engagementPercentage = 0;
                            Int32.TryParse(columns[11], out engagementPercentage); 

                            var engagementType = columns[12] != "-" ? columns[12].Trim().Replace("\"", "") : string.Empty;
                            var engagementGoals = columns[13] != "-" ? columns[13].Trim().Replace("\"", "") : string.Empty;

                            // 5. Create Object Representation
                            Traffic trafficParameter = new Traffic(
                                Sitecore.Context.Item,
                                urlReferrer,
                                ip,
                                dateFrom,
                                dateTo,
                                campaign,
                                trafficType, keyword, numberOfRequests, trafficInSameSession, trafficMode, trafficUrls,
                                engagementPercentage, engagementType, engagementGoals
                                );

                            trafficParameters.Add(trafficParameter);
                        }
                    }
                }

                lineNumber++;
            }

            return trafficParameters;
        }
    }
}
