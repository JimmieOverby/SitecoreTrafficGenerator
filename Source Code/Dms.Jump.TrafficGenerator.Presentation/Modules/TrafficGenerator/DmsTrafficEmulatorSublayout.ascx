<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DmsTrafficEmulatorSublayout.ascx.cs" Inherits="DMS.Jump.TrafficGenerator.Presentation.Modules.TrafficGenerator.TrafficGeneratorSublayout" %>

<div class="trafficGenerator">
    <h2>Traffic Generator</h2>
    <br />

    <asp:Panel ID="pnlUploadFile" runat="server">
        <h2><font color="red">Upload File</font><font color="#D0D0D0"> >> Start Generation >> Confirmation </font></h2>
        <br />
        <br />
        <table>
            <tr>
                <td valign="top" width="100">Upload .csv file: &nbsp;&nbsp;&nbsp;
                </td>
                <td colspan="2" width="400">
                    <asp:FileUpload ID="fuCsvFile" Width="500" runat="server" />&nbsp;&nbsp;&nbsp;
                    <asp:Label ID="lblUploadFile" runat="server" />
                </td>
            </tr>
            <tr>
                <td></td>
                <td align="right" width="400">Check to estimate the time to generate the traffic<br />
                    <i>Note: This generates 10 addition requests</i>
                </td>
                <td align="center" valign="middle" width="100">
                    <asp:CheckBox ID="cbEstimateTime" runat="server" />
                </td>
            </tr>
            <tr>
                <td colspan="2"></td>
                <td align="center">
                    <asp:Button ID="btnUpload" runat="server" Text="Upload" OnClick="btnUpload_Click" />
                </td>
            </tr>
        </table>
    </asp:Panel>

    <asp:Panel ID="pnlGenerateTraffic" runat="server">
        <h2><font color="#D0D0D0"> Upload File >> </font><font color="red">Generate Traffic</font><font color="#D0D0D0"> >> Confirmation </font></h2>
        <asp:Label ID="lblGenerateTraffic" runat="server" />

        <asp:Repeater ID="rptData" runat="server" OnItemDataBound="rptData_ItemDataBound">
            <HeaderTemplate>
                <p align="center">
                    <table border="1" style="border-style: solid; border-color: #D0D0D0; font-size: 8px" cellpadding="2" cellspacing="0">
                        <tr>
                            <td colspan="5">File content:</td>
                            <td colspan="2">Date:</td>
                            <td colspan="4" align="center">Traffic</td>
                            <td colspan="3" align="center">Engagement</td>
                        </tr>
                        <tr>
                            <td>Url</td>
                            <td>Ip</td>
                            <td>Type</td>
                            <td>Campaign</td>
                            <td>Keyword</td>
                            <td>From</td>
                            <td>To</td>
                            <td># Requests</td>
                            <td>Same Session</td>
                            <td>Mode</td>
                            <td>Urls</td>
                            <td>Percentage</td>
                            <td>Type</td>
                            <td>Goals</td>
                        </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                        <asp:Label ID="lblReferrer" runat="server" /></td>
                    <td>
                        <asp:Label ID="lblEngagementIp" runat="server" /></td>
                    <td>
                        <asp:Label ID="lblTrafficType" runat="server" /></td>
                    <td>
                        <asp:Label ID="lblCampaign" runat="server" /></td>
                    <td>
                        <asp:Label ID="lblKeyword" runat="server" /></td>
                    <td>
                        <asp:Label ID="lblDateFrom" runat="server" /></td>
                    <td>
                        <asp:Label ID="lblDateTo" runat="server" /></td>
                    <td>
                        <asp:Label ID="lblTrafficNumberOfRequests" runat="server" /></td>
                    <td>
                        <asp:Label ID="lblTrafficInSameSession" runat="server" /></td>
                    <td>
                        <asp:Label ID="lblTrafficMode" runat="server" /></td>
                    <td>
                        <asp:Label ID="lblTrafficUrls" runat="server" /></td>
                    <td>
                        <asp:Label ID="lblEngagementPercentage" runat="server" /></td>
                    <td>
                        <asp:Label ID="lblEngagementMode" runat="server" /></td>
                    <td>
                        <asp:Label ID="lblEngagementGoals" runat="server" /></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table>
             </p>
             <br />
                <p align="right">
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" />
                    <asp:Button ID="btnGenerateTraffic" runat="server" Text="Generate Traffic" OnClick="btnGenerateTraffic_Click" />

                </p>
            </FooterTemplate>

        </asp:Repeater>
    </asp:Panel>

    <asp:Panel ID="pnlConfirmation" runat="server">
        <h2><font color="#D0D0D0"> Upload File >> Start Generation >> </font><font color="red">Confirmation</font></h2>
        <br />
        <br />
        Report:
             <div style="font-size: 8px">
                 <asp:Label ID="lblConfirmation" runat="server" />
                 <br />
             </div>
        <p align="right">
            <asp:Button ID="btnDone" runat="server" Text="Done" OnClick="btnDone_Click" />
        </p>
        Detailed Requests:
             <div style="font-size: 8px">
                 <asp:Label ID="lblConfirmationRequests" runat="server" />
                 <br />
                 <br />
             </div>
    </asp:Panel>


</div>


