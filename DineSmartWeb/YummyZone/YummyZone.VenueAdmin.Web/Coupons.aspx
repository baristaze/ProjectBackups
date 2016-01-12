<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Coupons.aspx.cs" Inherits="YummyZone.VenueAdmin.Web.Coupons" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">

<head >
    <title>Dine Smart 365 - Coupons</title>

    <link href="Styles/Site_0_Common.css?version=1.05" rel="stylesheet" type="text/css" />
    <link href="Styles/Coupons.css?version=1.04" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.12.custom.css" rel="stylesheet" type="text/css" />
    
    <script type="text/javascript" src="Scripts/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui-1.8.12.custom.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery.form.js"></script>
    <script type="text/javascript" src="Scripts/jquery.scrollTo-min.js"></script>    
    <script type="text/javascript" src="Scripts/date.format.js"></script>
    <script type="text/javascript" src="Scripts/common2.js?version=1.04"></script>
    <script type="text/javascript" src="Scripts/Coupons.js?version=1.04"></script>
    <script type="text/javascript" src="Scripts/MultiBranch.js?version=1.00"></script>

    <script type="text/javascript">

            var _gaq = _gaq || [];
            _gaq.push(['_setAccount', 'UA-35375215-1']);
            _gaq.push(['_trackPageview']);

            (function () {
                var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
                ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
                var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
            })();

    </script>

</head>

<body>
    <div class="page">
        
        <!-- below line is NOT a comment but a real CODE -->
        <!--#include file="header.html"-->
        <!-- DO NOT DELETE ABOVE LINE -->

        <div class="main">
        
            <div class="hiddenParts hidden">

                <!-- below line is NOT a comment but a real CODE -->
                <!--#include file="mBrnch.html"-->
                <!-- DO NOT DELETE ABOVE LINE -->

                <div class="cpn">
                    <div class="cpnId hidden"></div>
                    <div class="cpnSendDateTimeUTC hidden"></div>
                    <div class="cpnRedeemDateTimeUTC hidden"></div>
                    <div class="cpnInfoWrpr">
                        <div class="cpnInfo">
                            <div class="cpnTitleWrpr">
                                <span class="cpnTitleLabel">
                                    <span class="cpnNew">New</span>
                                    <span class="cpnRdm">Redeemed</span>
                                    coupon:
                                </span>
                                <span class="cpnTitle"></span>
                            </div>
                            <div class="cpnReceiverInfo">
                                This coupon was sent to
                                <span class="dinerType"></span>
                            </div>
                            <div class="cpnSendInfo">
                                by
                                <span class="cpnSender"></span>
                                on
                                <span class="cpnSentDate"></span>
                                at
                                <span class="cpnSentTime"></span>
                            </div>
                            <div class="cpnRedeemInfo">
                                it was <b>redeemed</b> on
                                <span class="cpnRedeemtDate"></span>
                                at
                                <span class="cpnRedeemTime"></span>
                            </div>                                
                        </div>                                    
                    </div>
                    <div class="cpnActionsWrpr">
                        <div class="cpnActions hidden">
                            <div class="cpnAction"><a href="javascript:void(0);" class="seeCouponDetails">see coupon details</a></div>
                            <div class="cpnAction"><a href="javascript:void(0);" class="seeCheckinInfo">see checkin info</a></div>
                            <div class="cpnAction">
                                <a href="javascript:void(0);" class="seeRedeemInfo">see redeem info</a>
                            </div>
                        </div>
                    </div>
                    <div class="clear"></div>
                </div>
            </div>

            <div>
                <div class="cpnsLeftWrp">
                    <div class="cpns">
                        <asp:Repeater id="couponRepeater" runat="server">
                            <ItemTemplate>
                                <div class="cpn">
                                    <div class="cpnId hidden"><%#DataBinder.Eval(Container.DataItem, "Id")%></div>
                                    <div class="cpnSendDateTimeUTC hidden"><%#DataBinder.Eval(Container.DataItem, "SentDateTimeUTC")%></div>
                                    <div class="cpnRedeemDateTimeUTC hidden"><%#DataBinder.Eval(Container.DataItem, "RedeemDateTimeUTC")%></div>
                                    <div class="cpnInfoWrpr">
                                        <div class="cpnInfo">
                                            <div class="cpnTitleWrpr">
                                                <span class="cpnTitleLabel">
                                                    <span class="cpnNew <%#GetShowHideCssClass(DataBinder.Eval(Container.DataItem, "IsRedeemed"), true)%>">New</span>
                                                    <span class="cpnRdm <%#GetShowHideCssClass(DataBinder.Eval(Container.DataItem, "IsRedeemed"), false)%>">Redeemed</span>
                                                    coupon:
                                                </span>
                                                <span class="cpnTitle"><%#DataBinder.Eval(Container.DataItem, "Title")%></span>
                                            </div>
                                            <div class="cpnReceiverInfo">
                                                This coupon was sent to
                                                <span class="dinerType"><%#DataBinder.Eval(Container.DataItem, "DinerType")%></span>
                                            </div>
                                            <div class="cpnSendInfo">
                                                by
                                                <span class="cpnSender"><%#DataBinder.Eval(Container.DataItem, "SenderFullName")%></span>
                                                on
                                                <span class="cpnSentDate"></span>
                                                at
                                                <span class="cpnSentTime"></span>
                                            </div>
                                            <div class="cpnRedeemInfo <%#GetShowHideCssClass(DataBinder.Eval(Container.DataItem, "IsRedeemed"), false)%>">
                                                it was <b>redeemed</b> on
                                                <span class="cpnRedeemtDate"></span>
                                                at
                                                <span class="cpnRedeemTime"></span>
                                            </div>                                
                                        </div>                                    
                                    </div>
                                    <div class="cpnActionsWrpr">
                                        <div class="cpnActions hidden">
                                            <div class="cpnAction"><a href="javascript:void(0);" class="seeCouponDetails">see coupon details</a></div>
                                            <div class="cpnAction"><a href="javascript:void(0);" class="seeCheckinInfo">see checkin info</a></div>
                                            <div class="cpnAction <%#GetShowHideCssClass(DataBinder.Eval(Container.DataItem, "IsRedeemed"), false)%>">
                                                <a href="javascript:void(0);" class="seeRedeemInfo">see redeem info</a>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="clear"></div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>

                    <div class="showMoreWrapper">
                        <div class="showMoreBtn"><a href="javascript:void(0);" class="showMore">Show More</a></div>
                    </div>
                </div>
                
                <div class="statsRightWrp">
                    <div class="stats">
                        <div class="statSect sentStats roundedCorners4">
                            <div class="statRow colHeaders">
                                <div class="periodWrpr">
                                    <div class="period">Period</div>
                                </div>
                                <div class="dataWrpr">
                                    <div class="data"><span class="cpnType">Sent</span> Coupons</div>
                                </div>
                                <div class="clear"></div>
                            </div>
                            <div class="statRowSep"></div>
                            <div class="statRow">
                                <div class="periodWrpr">
                                    <div class="period">Today</div>
                                </div>
                                <div class="dataWrpr">
                                    <div class="data today">. . .</div>
                                </div>
                                <div class="clear"></div>
                            </div>
                            <div class="statRow">
                                <div class="periodWrpr">
                                    <div class="period">Yesterday</div>
                                </div>
                                <div class="dataWrpr">
                                    <div class="data yesterday">. . .</div>
                                </div>
                                <div class="clear"></div>
                            </div>
                            <div class="statRow">
                                <div class="periodWrpr">
                                    <div class="period">Last 7 days</div>
                                </div>
                                <div class="dataWrpr">
                                    <div class="data last7days">. . .</div>
                                </div>
                                <div class="clear"></div>
                            </div>
                            <div class="statRow">
                                <div class="periodWrpr">
                                    <div class="period">Last 30 days</div>
                                </div>
                                <div class="dataWrpr">
                                    <div class="data last30days">. . .</div>
                                </div>
                                <div class="clear"></div>
                            </div>
                            <div class="statRow">
                                <div class="periodWrpr">
                                    <div class="period sinceBegOfMonthLabel">This Month</div>
                                </div>
                                <div class="dataWrpr">
                                    <div class="data sinceBegOfMonth">. . .</div>
                                </div>
                                <div class="clear"></div>
                            </div>
                            <div class="statRow">
                                <div class="periodWrpr">
                                    <div class="period lastMonthLabel">Last Month</div>
                                </div>
                                <div class="dataWrpr">
                                    <div class="data lastMonth">. . .</div>
                                </div>
                                <div class="clear"></div>
                            </div>
                            <div class="statRow">
                                <div class="periodWrpr">
                                    <div class="period">In this calendar year</div>
                                </div>
                                <div class="dataWrpr">
                                    <div class="data sinceBegOfYear">. . .</div>
                                </div>
                                <div class="clear"></div>
                            </div>
                        </div>
                        <div class="clear"></div>
                        <div class="statSect redeemStats roundedCorners4">
                            <div class="statRow colHeaders">
                                <div class="periodWrpr">
                                    <div class="period">Period</div>
                                </div>
                                <div class="dataWrpr">
                                    <div class="data"><span class="cpnType">Redeemed</span> Cpns</div>
                                </div>
                                <div class="clear"></div>
                            </div>
                            <div class="statRowSep"></div>
                            <div class="statRow">
                                <div class="periodWrpr">
                                    <div class="period">Today</div>
                                </div>
                                <div class="dataWrpr">
                                    <div class="data today">. . .</div>
                                </div>
                                <div class="clear"></div>
                            </div>
                            <div class="statRow">
                                <div class="periodWrpr">
                                    <div class="period">Yesterday</div>
                                </div>
                                <div class="dataWrpr">
                                    <div class="data yesterday">. . .</div>
                                </div>
                                <div class="clear"></div>
                            </div>
                            <div class="statRow">
                                <div class="periodWrpr">
                                    <div class="period">Last 7 days</div>
                                </div>
                                <div class="dataWrpr">
                                    <div class="data last7days">. . .</div>
                                </div>
                                <div class="clear"></div>
                            </div>
                            <div class="statRow">
                                <div class="periodWrpr">
                                    <div class="period">Last 30 days</div>
                                </div>
                                <div class="dataWrpr">
                                    <div class="data last30days">. . .</div>
                                </div>
                                <div class="clear"></div>
                            </div>
                            <div class="statRow">
                                <div class="periodWrpr">
                                    <div class="period sinceBegOfMonthLabel">This Month</div>
                                </div>
                                <div class="dataWrpr">
                                    <div class="data sinceBegOfMonth">. . .</div>
                                </div>
                                <div class="clear"></div>
                            </div>
                            <div class="statRow">
                                <div class="periodWrpr">
                                    <div class="period lastMonthLabel">Last Month</div>
                                </div>
                                <div class="dataWrpr">
                                    <div class="data lastMonth">. . .</div>
                                </div>
                                <div class="clear"></div>
                            </div>
                            <div class="statRow">
                                <div class="periodWrpr">
                                    <div class="period">In this calendar year</div>
                                </div>
                                <div class="dataWrpr">
                                    <div class="data sinceBegOfYear">. . .</div>
                                </div>
                                <div class="clear"></div>
                            </div>
                        </div>
                        <div class="clear"></div>
                        <div class="statSect campaignStats roundedCorners4">
                            <div class="statRow colHeaders">
                                <div class="periodWrpr">
                                    <div class="period">Sale Campaigns</div>
                                </div>
                                <div class="dataWrpr">
                                    <div class="data"><span class="cpnType">Coming Soon!..</span></div>
                                </div>
                                <div class="clear"></div>
                            </div>                            
                        </div>
                    </div>                    
                </div>
                <div class="clear"></div>
            </div>
        </div>
        <div class="clear"></div>
    </div>
</body>
</html>
