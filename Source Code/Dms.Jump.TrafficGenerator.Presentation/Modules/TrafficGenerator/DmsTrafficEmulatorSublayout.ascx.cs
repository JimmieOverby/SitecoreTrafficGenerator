using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI.WebControls;
using Dms.Jump.TrafficGenerator.Business.Entities;
using Dms.Jump.TrafficGenerator.Business.Entities.Requests;
using Dms.Jump.TrafficGenerator.Business.Helper;

namespace DMS.Jump.TrafficGenerator.Presentation.Modules.TrafficGenerator
{

    /// <summary>
    /// The traffic generator sublayout.
    /// </summary>
    public partial class TrafficGeneratorSublayout : System.Web.UI.UserControl
    {
        #region Properties

        private List<Traffic> TrafficParameters
        {
            get
            {
                return
                    Session["SC_TrafficParameters"] != null
                    ? Session["SC_TrafficParameters"] as List<Traffic>
                    : null;
            }
            set
            {
                Session["SC_TrafficParameters"] = value;
            }

        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ResetWorkflow();
            }

        }

        protected void btnGenerateTraffic_Click(object sender, EventArgs e)
        {
            StringBuilder requestsStringBuilder = new StringBuilder();
            StringBuilder confirmationStringBuilder = new StringBuilder();

            int requestCounter = 0;
            int numberOfErrors = 0;

            DateTime start = DateTime.Now;

            foreach (var trafficParameter in this.TrafficParameters)
            {

                CookieContainer cookie = null;
                if (trafficParameter.RequestSource.TrafficInSameSession)
                {
                    cookie = new CookieContainer();
                }

                var uris = trafficParameter.GetUris();
                requestsStringBuilder.Append("<i>From: " + trafficParameter.RequestSource.UrlReferrer + @"</i><br />");

                foreach (Uri uri in uris)
                {
                    try
                    {
                        requestsStringBuilder.Append(requestCounter + " \t");

                        // Wait a sec to allow http to digest requests
                        if (requestCounter % 100 == 0)
                        {
                            Thread.Sleep(2500);
                        }

                        if (trafficParameter.RequestSource.TrafficInSameSession && (uris.IndexOf(uri) == 0))
                        {
                            cookie = ClientSync.PerformRequest(uri, cookie);
                        }
                        else
                        {
                            ClientAsync.PerformRequest(uri, cookie);
                        }
                        requestsStringBuilder.Append(uri + "<br />");
                    }
                    catch (Exception exp)
                    {
                        numberOfErrors++;
                        requestsStringBuilder.Append("Error in: <font color=red>" + uri.ToString() + "</font> " + exp.Message + "<br />");
                    }
                    requestCounter++;
                }

                // Wait a sec to allow http to digest requests
                Thread.Sleep(5000);
            }

            TimeSpan tts = DateTime.Now - start;
            lblConfirmation.Text = "Completed " + this.TrafficParameters.Count() + " imports<br />";
            lblConfirmation.Text += requestCounter + " requests processed in " + tts.TotalSeconds + " sec. ";
            lblConfirmation.Text += "Avg req/sec. " + requestCounter / tts.TotalSeconds + "<br />";
            lblConfirmation.Text += "<font color=red>Detected number of errors = " + numberOfErrors + "</font><br /><br />";
            lblConfirmation.Text += "Imports:<br />";
            lblConfirmation.Text += confirmationStringBuilder.ToString();

            lblConfirmationRequests.Text = requestsStringBuilder.ToString();

            pnlUploadFile.Visible = false;
            pnlGenerateTraffic.Visible = false;
            pnlConfirmation.Visible = true;

            this.TrafficParameters = null;
            rptData.DataSource = this.TrafficParameters;
            rptData.DataBind();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.ResetWorkflow();
        }

        protected void btnDone_Click(object sender, EventArgs e)
        {
            this.ResetWorkflow();
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            //check to make sure a file is selected    
            if (fuCsvFile.HasFile)
            {
                try
                {
                    lblGenerateTraffic.Text = "Loaded filename: " + fuCsvFile.PostedFile.FileName + "<br/>" +
                         "File size; " + fuCsvFile.PostedFile.ContentLength + " kb" + "<br/>" +
                         "Content type:: " + fuCsvFile.PostedFile.ContentType + "<br/><br/>";

                    using (StreamReader sr = new StreamReader(fuCsvFile.PostedFile.InputStream))
                    {
                        CsvHelper csvHelper = new CsvHelper();
                        this.TrafficParameters = csvHelper.GetTrafficParameters(sr);
                    }

                    rptData.DataSource = this.TrafficParameters;
                    rptData.DataBind();

                    int numberOfRequests = 0;
                    foreach (var trafficParameter in this.TrafficParameters)
                        numberOfRequests += trafficParameter.GetUris().Count();

                    double estimatedRequestTime = 0;
                    if (cbEstimateTime.Checked)
                    {
                        var url = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.AbsolutePath, "");

                        if (!String.IsNullOrEmpty(url))
                        {
                            Uri uri = new Uri(url);

                            var start = DateTime.Now;
                            for (int i = 0; i < 10; i++)
                                ClientAsync.PerformRequest(uri);

                            TimeSpan ts = DateTime.Now - start;
                            estimatedRequestTime = ts.TotalSeconds;
                        }
                    }

                    lblGenerateTraffic.Text += "Number of requests = " + numberOfRequests + " <br />";
                    if (estimatedRequestTime > 0)
                        lblGenerateTraffic.Text += "Estimated time to process = " + (int)(numberOfRequests / 10 * estimatedRequestTime) + " sec. <br />";

                    lblGenerateTraffic.Text += "<br />";

                    pnlUploadFile.Visible = false;
                    pnlGenerateTraffic.Visible = true;
                    pnlConfirmation.Visible = false;
                }
                catch (Exception ex)
                {
                    lblUploadFile.Text = "ERROR: " + ex.Message;
                }
            }
        }

        protected void rptData_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item ||
                e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var lblDateFrom = e.Item.FindControl("lblDateFrom") as Label;
                var lblDateTo = e.Item.FindControl("lblDateTo") as Label;
                var lblTrafficType = e.Item.FindControl("lblTrafficType") as Label;
                var lblKeyword = e.Item.FindControl("lblKeyword") as Label;
                var lblCampaign = e.Item.FindControl("lblCampaign") as Label;
                var lblTrafficNumberOfRequests = e.Item.FindControl("lblTrafficNumberOfRequests") as Label;
                var lblTrafficInSameSession = e.Item.FindControl("lblTrafficInSameSession") as Label;
                var lblTrafficMode = e.Item.FindControl("lblTrafficMode") as Label;
                var lblTrafficUrls = e.Item.FindControl("lblTrafficUrls") as Label;
                var lblReferrer = e.Item.FindControl("lblReferrer") as Label;
                var lblEngagementIp = e.Item.FindControl("lblEngagementIp") as Label;
                var lblEngagementPercentage = e.Item.FindControl("lblEngagementPercentage") as Label;
                var lblEngagementMode = e.Item.FindControl("lblEngagementMode") as Label;
                var lblEngagementGoals = e.Item.FindControl("lblEngagementGoals") as Label;

                var trafficParameter = e.Item.DataItem as Traffic;

                lblCampaign.Text = !String.IsNullOrEmpty(trafficParameter.RequestSource.Campaign) ? trafficParameter.RequestSource.Campaign : "-";
                lblDateFrom.Text = trafficParameter.DateRange.DateFrom.ToString();
                lblDateTo.Text = trafficParameter.DateRange.DateTo.ToString();
                lblTrafficType.Text = trafficParameter.RequestSource.TrafficType.ToString();
                lblTrafficNumberOfRequests.Text = trafficParameter.RequestSource.NumberOfRequests.ToString();
                lblTrafficInSameSession.Text = trafficParameter.RequestSource.TrafficInSameSession.ToString();
                lblTrafficMode.Text = trafficParameter.RequestSource.Mode.ToString();
                lblKeyword.Text = !String.IsNullOrEmpty(trafficParameter.RequestSource.Keyword) ? trafficParameter.RequestSource.Keyword : "-";
                lblTrafficUrls.Text = trafficParameter.RequestSource.Urls.Count > 0 ? trafficParameter.RequestSource.Urls.Count.ToString() : "-";
                lblReferrer.Text = trafficParameter.RequestSource.UrlReferrer;
                lblEngagementIp.Text = trafficParameter.Engagement.IP;
                lblEngagementPercentage.Text = trafficParameter.Engagement.Percentage.ToString();
                lblEngagementMode.Text = trafficParameter.Engagement.Mode.ToString();
                lblEngagementGoals.Text = trafficParameter.Engagement.SpecificGoalNames.Count() > 0 ? trafficParameter.Engagement.SpecificGoalNames.Count().ToString() : "-";
            }
            else if (e.Item.ItemType == ListItemType.Header)
            {
                // Do nothing
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                // Do nothing
            }
        }


        #region Private Helpers

        private void ResetWorkflow()
        {
            pnlUploadFile.Visible = true;
            pnlGenerateTraffic.Visible = false;
            pnlConfirmation.Visible = false;

            this.TrafficParameters = null;
            rptData.DataSource = this.TrafficParameters;
            rptData.DataBind();

            cbEstimateTime.Checked = false;
            lblUploadFile.Text = string.Empty;
        }
        #endregion

    }
}
