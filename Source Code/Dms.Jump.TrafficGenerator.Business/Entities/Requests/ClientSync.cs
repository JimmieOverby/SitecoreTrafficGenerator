using System;
using System.Net;

namespace Dms.Jump.TrafficGenerator.Business.Entities.Requests
{
    /// <summary>
    /// The class for getting Sitecore cookies after request.
    /// </summary>
    public class ClientSync
    {
        /// <summary>
        /// The perform request and return cookies.
        /// </summary>
        /// <param name="uri">
        /// The uri.
        /// </param>
        /// <param name="cookieContainer">
        /// The cookie container.
        /// </param>
        /// <returns>
        /// The <see cref="CookieContainer"/>.
        /// </returns>
        public static CookieContainer PerformRequest(Uri uri, CookieContainer cookieContainer)
        {
            var newCookies = cookieContainer;

            // Create the request object.
            var request = (HttpWebRequest)WebRequest.Create(uri);
            if (cookieContainer != null)
            {
                request.CookieContainer = cookieContainer;
            }

            var response = (HttpWebResponse)request.GetResponse();
            newCookies.Add(response.Cookies);

            response.Close();
            return newCookies;
        }
    }
}
