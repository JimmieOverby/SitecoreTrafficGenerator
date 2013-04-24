using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data.Items;
using Sitecore.Data;

using Dms.Jump.TrafficGenerator.Business.Utilities.Tools;

namespace Dms.Jump.TrafficGenerator.Business.Utilities.Extension
{
    public static class Extensions
    {
        public static Uri AddQueryString(this Uri uri, QueryStringBuilder queryStringBuilder)
        {
            string newUri = uri.ToString();

            if (!newUri.Contains("?"))
                newUri = newUri + "?";
            else
                newUri = newUri + "&";

            newUri = newUri + queryStringBuilder.ToString();

            return new Uri(newUri);
        }

    }
}
