<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Feedbacks.aspx.cs" Inherits="YummyZone.VenueAdmin.Web.Feedbacks" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">

<head >
    <title>Dine Smart 365 - Feedback</title>
    <link href="Styles/Site_0_Common.css?version=1.05" rel="stylesheet" type="text/css" />
    <link href="Styles/Feedbacks.css?version=1.04" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.12.custom.css" rel="stylesheet" type="text/css" />

    <script type="text/javascript" src="Scripts/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui-1.8.12.custom.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery.form.js"></script>
    <script type="text/javascript" src="Scripts/jquery.scrollTo-min.js"></script>    
    <script type="text/javascript" src="Scripts/date.format.js"></script>
    <script type="text/javascript" src="Scripts/common1.js?version=1.04"></script>
    <script type="text/javascript" src="Scripts/common2.js?version=1.04"></script>
    <script type="text/javascript" src="Scripts/Feedbacks.js?version=1.04"></script>
    <script type="text/javascript" src="Scripts/MultiBranch.js?version=1.00"></script>

    <script type="text/javascript">
        /*
        window.onload = function () {
            setTimeout(function () {
                var preloaded = getCookie("__platePicsPreloaded");
                if (preloaded == "") {
                    // create a new frame and point to the URL of the static
                    // page that has all components to preload
                    var iframe = document.createElement('iframe');
                    iframe.setAttribute("width", "0");
                    iframe.setAttribute("height", "0");
                    iframe.setAttribute("frameborder", "0");
                    iframe.src = "PlatePicsPreload.aspx";
                    document.body.appendChild(iframe);
                    setCookie("__platePicsPreloaded", "1", 1);
                }
            }, 2500);
        };*/
    </script>

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
                
                <div class="msgDlg">
                    <form class="msgForm" action="FeedbackHandlers/MessageSend.ashx">
                        <div class="checkinId hidden"></div>   
                        <div class="msgSubject">
                            <div class="msgSubjectLabel">Subject:</div>
                            <input class="msgSubjectInput inputText" type="text" maxlength="140" />
                        </div>                     
                        <div class="msgContent">                            
                            <div class="msgContentLabel">Content:</div>
                            <textarea class="msgContentInput inputText" rows="9" cols="100%" ></textarea>
                        </div>
                    </form>
                </div>

                <div class="cpnDlg">
                    <form class="cpnForm" action="FeedbackHandlers/CouponSend.ashx">
                        <div class="checkinId hidden"></div>
                        <div class="cpnSummary">
                            <div class="cpnSummaryLabel">Coupon Value Summary: 
                                <a href="javascript:void(0);" class="cpnSummaryExampleLink">
                                    <span class="cpnSummaryExampleLabel">(click to see examples)</span>
                                </a>
                            </div>
                            <input class="cpnSummaryInput inputText" type="text" maxlength="140" />
                        </div>                        
                        <div class="cpnContent">
                            <div class="cpnContentLabel">Details <span class="cpnContentLabelExample">(e.g. restrictions, etc. This field is optional)</span></div>
                            <textarea class="cpnContentInput inputText" rows="7" cols="100%" ></textarea>
                        </div>
                        <div class="cpnValidThrough">
                            <span class="cpnValidThroughLabel1">Valid for the next</span>
                            <input class="cpnValidThroughInput inputText" type="text" maxlength="3" value="30" />
                            <span class="cpnValidThroughLabel2">days</span>
                            <span class="cpnValidThroughMinMax">(min:7, max:120)</span>
                        </div>
                    </form>
                </div>

                <div id="confirmReportingAsSpamDlg" >
                    <p>Are you sure that you want to mark<br/>this item as spam and remove it?</p>
                </div>

                <div id="confirmMarkingAsDeletedDlg" >
                    <p>Are you sure that you do not<br/>want to see this item anymore?</p>
                </div>

                <!-- below line is NOT a comment but a real CODE -->
                <!--#include file="mBrnch.html"-->
                <!-- DO NOT DELETE ABOVE LINE -->

                <!--rate item-->
                <div class="fbkItem rateItem">
                    <div class="fbkItemNameWrapper">
                        <div class="fbkItemName fbkRateItemName"></div>
                    </div>
                    <div class="fbkItemValueWrapper">
                        <div class="fbkItemValue">
                            <img class="starImg" src="Images/stars/star_0.png" alt="downloading rate..." />
                        </div>
                    </div>
                    <div class="clear"></div>
                </div>

                <!-- yes-no item -->
                <div class="fbkItem yesNoItem">
                    <div class="fbkItemNameWrapper">
                        <div class="fbkItemName"></div>
                    </div>
                    <div class="fbkItemValueWrapper">
                        <div class="fbkItemValue"></div>
                    </div>
                    <div class="clear"></div>
                </div>

                <!-- multiple choice item -->
                <div class="fbkItem multiChoiceItem">
                    <div class="fbkItemNameWrapper">
                        <div class="fbkItemName"></div>
                    </div>
                    <div class="fbkItemValueWrapper">
                        <div class="fbkItemValue"></div>
                    </div>
                    <div class="clear"></div>
                </div>

                <!-- free form item -->
                <div class="fbkItem freeFormItem">
                    <div class="fbkItemNameWrapper">
                        <div class="fbkItemName"></div>
                    </div>
                    <div class="fbkItemValueWrapper">
                        <div class="fbkItemValue"></div>
                    </div>
                    <div class="clear"></div>
                </div>    

                <!-- feedback item -->
                <div class="fbk fbkUnread">    
                    <div class="checkinId hidden"></div>
                    <div class="fbkInfoWrapper">                
                        <div class="fbkInfo"> 
                            <!--header-->                           
                            <div class="fbkInfoHeader">                                
                                <div class="dinerTypeWrapper">
                                    <div class="interactionStatusWrapper">
                                        <div class="replyStatus">
                                            <img class="messageImg hidden" alt="msg" src="Images/message.png" />
                                        </div>
                                        <div class="couponStatus">
                                            <img class="couponImg hidden" alt="cpn" src="Images/coupon.png" />
                                        </div>
                                    </div>
                                    <div class="dinnerTypeOuter">
                                        <div class="dinerTypeEx">
                                            <div class="dinerTypeStatic">customer type:</div>
                                            <div class="dinerType"></div>
                                        </div>
                                    </div>
                                    <div class="clear"></div>
                                </div>
                                <div class="dateTimeWrapper">
                                    <div class="dateTimeUTC hidden"></div>
                                    <div class="dateTime"></div>
                                </div>
                                <div class="customerTypeWrapper">                                    
                                    <div class="customerType"></div>
                                </div>                                
                                <div class="clear"></div>                    
                            </div>                                                                                    
                            <div class="fbkItemList">
                                <div class="rateItems">
                                </div>
                                <div class="yesNoItems">
                                </div>
                                <div class="multipleChoiceItems">
                                </div>
                                <div class="freeFormItems">         
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="fbkActionsWrapper">
                        <div class="fbkActions hidden">
                            <div class="fbkAction"><a href="javascript:void(0);" class="markAsRead">mark as read</a></div>
                            <div class="fbkAction"><a href="javascript:void(0);" class="markAsUnRead">mark as unread</a></div>
                            <div class="fbkAction"><a href="javascript:void(0);" class="replyWithMessage">reply with a message</a></div>
                            <div class="fbkAction"><a href="javascript:void(0);" class="replyWithCoupon">reply with a coupon</a></div>
                            <div class="fbkAction"><a href="javascript:void(0);" class="markAsDeleted">archive (hide)</a></div>
                            <div class="fbkAction"><a href="javascript:void(0);" class="reportAsSpam">report as spam</a></div>
                        </div>
                    </div>
                    <div class="clear"></div>
                </div>

            </div>
            
            <div class="newFbkBtnWrapper hidden">
                <div class="newFbkBtn"><a href="javascript:void(0);" class="newFbk" onClick="window.location.reload()">click to see new feedback</a></div>
            </div>
            <div class="clear" />
            
            <div class="fbkList">
                <!-- feedback item -->
                <asp:Repeater id="feedBackRepeater" runat="server">
                  <ItemTemplate>
                      <div class="fbk <%#GetReadCssClass(DataBinder.Eval(Container.DataItem, "IsRead"))%>">
                        <div class="checkinId hidden"><%#DataBinder.Eval(Container.DataItem, "CheckInId")%></div>
                        <div class="fbkInfoWrapper">                
                            <div class="fbkInfo">    
                                <!--header-->                        
                                <div class="fbkInfoHeader">                                
                                    <div class="dinerTypeWrapper">
                                        <div class="interactionStatusWrapper">
                                            <div class="replyStatus">
                                                <img class="messageImg <%#GetShowHideCssClass(DataBinder.Eval(Container.DataItem, "IsReplied"), false)%>" 
                                                     alt="msg" src="Images/message.png" />
                                            </div>
                                            <div class="couponStatus" >
                                                <img class="couponImg <%#GetShowHideCssClass(DataBinder.Eval(Container.DataItem, "IsCouponSent"), false) %>" 
                                                     alt="cpn" src="Images/coupon.png" />
                                            </div>
                                        </div>
                                        <div class="dinnerTypeOuter">
                                            <div class="dinerTypeEx">
                                                <div class="dinerTypeStatic">customer type:</div>
                                                <div class="dinerType"><%#DataBinder.Eval(Container.DataItem, "DinerType")%></div>
                                            </div>
                                        </div>
                                        <div class="clear"></div>
                                    </div>
                                    <div class="dateTimeWrapper">
                                        <div class="dateTimeUTC hidden"><%#DataBinder.Eval(Container.DataItem, "CheckInTimeUTC")%></div>
                                        <div class="dateTime"></div>
                                    </div>
                                    <div class="customerTypeWrapper">                                    
                                        <div class="customerType"><%#DataBinder.Eval(Container.DataItem, "CustomerType")%></div>
                                    </div>                                
                                    <div class="clear"></div>                    
                                </div>                                                                                    
                                <div class="fbkItemList">
                                    <div class="rateItems">
                                        <!--rate item-->
                                        <asp:Repeater id="rateRepeater" DataSource='<%#DataBinder.Eval(Container.DataItem, "RateItems")%>' runat="server">
                                          <ItemTemplate>
                                            <div class="fbkItem rateItem">
                                                <div class="fbkItemNameWrapper">
                                                    <div class="fbkItemName fbkRateItemName"><%#DataBinder.Eval(Container.DataItem, "Name")%></div>
                                                </div>
                                                <div class="fbkItemValueWrapper">
                                                    <div class="fbkItemValue">
                                                        <img class="starImg" src="Images/stars/star_<%#DataBinder.Eval(Container.DataItem, "Value")%>.png" alt="downloading rate..." />
                                                    </div>
                                                </div>
                                                <div class="clear"></div>
                                            </div>
                                          </ItemTemplate>
                                        </asp:Repeater>
                                    </div>
                                    <div class="yesNoItems">
                                        <!-- yes-no item -->
                                        <asp:Repeater id="yesNoRepeater" DataSource='<%#DataBinder.Eval(Container.DataItem, "YesNoItems")%>' runat="server">
                                          <ItemTemplate>
                                            <div class="fbkItem yesNoItem">
                                                <div class="fbkItemNameWrapper">
                                                    <div class="fbkItemName"><%#DataBinder.Eval(Container.DataItem, "Name")%></div>
                                                </div>
                                                <div class="fbkItemValueWrapper">
                                                    <div class="fbkItemValue"><%#DataBinder.Eval(Container.DataItem, "Value")%></div>
                                                </div>
                                                <div class="clear"></div>
                                            </div>
                                          </ItemTemplate>
                                        </asp:Repeater>
                                    </div>
                                    <div class="multipleChoiceItems">
                                        <!-- multiple choice item -->
                                        <asp:Repeater id="multipleChoiceRepeater" DataSource='<%#DataBinder.Eval(Container.DataItem, "MultiChoiceItems")%>' runat="server">
                                          <ItemTemplate>
                                            <div class="fbkItem multiChoiceItem">
                                                <div class="fbkItemNameWrapper">
                                                    <div class="fbkItemName"><%#DataBinder.Eval(Container.DataItem, "Name")%></div>
                                                </div>
                                                <div class="fbkItemValueWrapper">
                                                    <div class="fbkItemValue"><%#DataBinder.Eval(Container.DataItem, "Value")%></div>
                                                </div>
                                                <div class="clear"></div>
                                            </div>
                                          </ItemTemplate>
                                        </asp:Repeater>                                        
                                    </div>
                                    <div class="freeFormItems">    
                                        <!-- free form item -->
                                        <asp:Repeater id="freeFormRepeater" DataSource='<%#DataBinder.Eval(Container.DataItem, "FreeFormItems")%>' runat="server">
                                          <ItemTemplate>
                                            <div class="fbkItem freeFormItem">
                                                <div class="fbkItemNameWrapper">
                                                    <div class="fbkItemName"><%#DataBinder.Eval(Container.DataItem, "Name")%></div>
                                                </div>
                                                <div class="fbkItemValueWrapper">
                                                    <div class="fbkItemValue"><%#DataBinder.Eval(Container.DataItem, "Value")%></div>
                                                </div>
                                                <div class="clear"></div>
                                            </div>      
                                          </ItemTemplate>
                                        </asp:Repeater>                                          
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="fbkActionsWrapper">
                            <div class="fbkActions hidden">
                                <div class="fbkAction <%#GetShowHideCssClass(DataBinder.Eval(Container.DataItem, "IsRead"), true)%>"><a href="javascript:void(0);" class="markAsRead">mark as read</a></div>
                                <div class="fbkAction <%#GetShowHideCssClass(DataBinder.Eval(Container.DataItem, "IsRead"), false)%>"><a href="javascript:void(0);" class="markAsUnRead">mark as unread</a></div>
                                <div class="fbkAction <%#GetShowHideCssClass(DataBinder.Eval(Container.DataItem, "IsReplied"), true)%>"><a href="javascript:void(0);" class="replyWithMessage">reply with a message</a></div>
                                <div class="fbkAction <%#GetShowHideCssClass(DataBinder.Eval(Container.DataItem, "IsCouponSent"), true)%>"><a href="javascript:void(0);" class="replyWithCoupon">reply with a coupon</a></div>
                                <div class="fbkAction"><a href="javascript:void(0);" class="markAsDeleted">archive (hide)</a></div>
                                <div class="fbkAction"><a href="javascript:void(0);" class="reportAsSpam">report as spam</a></div>
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

        <div class="clear">
        </div>

    </div>

</body>

</html>
