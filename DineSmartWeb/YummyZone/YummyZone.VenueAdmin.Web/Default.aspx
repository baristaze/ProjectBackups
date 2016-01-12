<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="YummyZone.VenueAdmin.Web.Default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">

<head >
    <title>Dine Smart 365 - Home</title>
    
    <meta content="Leave feedback, win coupons!" name="description" />
    <meta content="rate,review,comment,feedback,coupon,deal,special offer,restaurant,cafe,venue,place,bakery,winery,night club,survey,questionnaire,customer,satisfaction,loyalty,question,plate,meal,dish,menu item,menu,dinner,lunch,breakfast,brunch,happy hour,customer satisfaction,customer loyalty,restaurant feedback" name="keywords" />

    <link href="Styles/Site_0_Common.css?version=1.05" rel="stylesheet" type="text/css" />
    <link href="Styles/Default.css?version=1.05" rel="stylesheet" type="text/css" />
    <link href="Styles/iphone.css?version=1.04" rel="stylesheet" type="text/css" media="all and (max-device-width: 480px)" >  
    <link href="Styles/ipad.css?version=1.04" rel="stylesheet" type="text/css" media="all and (min-device-width: 481px) and (max-device-width: 1024px)">


    <script type="text/javascript" src="Scripts/common1.js?version=1.04"></script>
    <script type="text/javascript" src="Scripts/Default.js?version=1.04"></script>

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
    
    <script type="text/javascript">
        window.onload = function () {
            setTimeout(function () {
                var preloaded = getCookie("__resourcesPreloaded");
                if (preloaded == "") {
                    // create a new frame and point to the URL of the static
                    // page that has all components to preload
                    var iframe = document.createElement('iframe');
                    iframe.setAttribute("width", "0");
                    iframe.setAttribute("height", "0");
                    iframe.setAttribute("frameborder", "0");
                    iframe.src = "preload.html";
                    document.body.appendChild(iframe);
                    setCookie("__resourcesPreloaded", "1", 1);
                }
            }, 1500);
        };
    </script>

</head>

<body>
    <div class="page">

        <div class="iphone">
            <img class="iphoneImg" alt=" " src="Images/iphone.png" />
        </div>
            
        <div class="main">      
            <div class="upperStory">
                <div class="topMenuLoginStrip">

                    <div id="loginWrp" class="loginWrp" runat="server">
                        <form id="loginForm" action="Default.aspx?login=1" runat="server">
                            <div class="loginItem">
                                <img alt="managers >" class="managersImg" src="Images/managers.png" />
                            </div>
                            <div class="loginItem">
                                <input id="bizEmail" class="bizEmail roundedCorners4" type="text" value="your email" runat="server"
                                onfocus="emailInputFocused(this.id)" onblur="emailInputLostFocus(this.id)" onkeydown="enterKeyDown(event, submitLoginForm)"/>
                            </div>
                            <div class="loginItem">
                                <input id="password" class="password roundedCorners4" type="text" value="password" runat="server"
                                onfocus="pswdInputFocused(this.id)" onblur="pswdInputLostFocus(this.id)" onkeydown="enterKeyDown(event, submitLoginForm)"/>
                            </div>
                            <div class="loginItem">
                                <a href="javascript:void(0);" class="loginLink" onkeydown="enterKeyDown(event, submitLoginForm)">
                                    <img alt="login >" class="loginBtnImg" src="Images/loginButton.png" onclick="submitLoginForm()" />
                                </a>
                            </div>
                        </form>
                    </div>

                    <div id="identityWrp" class="identityWrp" runat="server">
                        <div class="brnchWrp">
                            <div id="brnchName" class="brnchName inline" runat="server"></div>
                            <div class="changeBrnch inline"><img class="changeBrnchImg changeBrnchImgNonDashboard" alt="change branch" src="Images/TreeExpanded.png" /></div>
                        </div>
                        <div class="signout">
                            <a href="Signout.aspx">
                                <img alt="Sign Out" class="signoutImg" src="Images/signoutButton.png" />
                            </a>
                        </div>
                        <div class="usrWrp">
                            <div class="welcome inline">Welcome,&nbsp;</div>
                            <div id="usrName" class="usrName inline" runat="server"></div>
                        </div>
                    </div>    
                </div>
                <div class="mainMenuStrip">
                    <div class="logoWrp">
                        <img alt="Dine Smart 365" class="logoImg" src="Images/logo.png" />
                    </div>
                    <div class="menuWrpr">
                        <div class="mainMenuRightInner genericTitle condensedFont">
                            <div class="menuItem selectedLink"><a href="javascript:void(0);" class="menuItemLink">Home</a></div>
                            <div class="menuItemSep"><img alt="|" class="menuSepImg" src="Images/menuDivider.png" /></div>
                            <div class="menuItem" id="menuLink2" runat="server"><a href="Feedbacks.aspx" class="menuItemLink">Dashboard</a></div>
                            <div class="menuItemSep" id="menuSep2" runat="server"><img alt="|" class="menuSepImg" src="Images/menuDivider.png" /></div>
                            <div class="menuItem"><a href="About.aspx" class="menuItemLink">About</a></div>
                            <div class="menuItemSep"><img alt="|" class="menuSepImg" src="Images/menuDivider.png" /></div>
                            <div class="menuItem"><a href="Downloads.aspx" class="menuItemLink">Downloads</a></div>
                            <div class="menuItemSep"><img alt="|" class="menuSepImg" src="Images/menuDivider.png" /></div>
                            <div class="menuItem"><a href="Support.aspx" class="menuItemLink">Support</a></div>
                            <div class="menuItemSep"><img alt="|" class="menuSepImg" src="Images/menuDivider.png" /></div>
                            <div class="menuItem"><a href="FAQs.aspx" class="menuItemLink">FAQs</a></div>
                        </div>
                    </div>    
                </div>
            </div>
            <div class="middleSeparatorStrip">
                <div class="leftWrpr">
                    <div class="leftInner midHeaderWrp midHdr1">
                        <span class="midHeader condensedFont">
                            Diners
                        </span>
                    </div>                
                </div>
                <div class="rightWrpr">
                    <div class="rightInner midHeaderWrp midHdr2">
                        <span class="midHeader condensedFont">
                            Restaurant Managers
                        </span>
                    </div>
                </div>
            </div>

            <div class="lowerHalf">
                <div class="leftWrpr">
                    <div class="leftInner">
                        <div class="benefits condensedFont bnfts1">
                            <ul class="bullets">
                                <li class="bulletItem">Leave feedback, win coupons!</li>
                                <li class="bulletItem">Get special offers to your mobile!</li>
                                <li class="bulletItem">View the dishes people liked the most!</li>
                                <li class="bulletItem">See what you've ordered in a restaurant before!</li>
                                <li class="bulletItem">Keep track of your dining journey!</li>
                                <li class="bulletItem">Share with friends via email or Facebook! <span class="soon">(soon)</span></li>
                                <li class="bulletItem">Absolutely <span class="free"><b>FREE!</b></span></li>
                            </ul> 
                        </div>
                    </div>                
                </div>

                <div class="rightWrpr">
                    <div class="rightInner">      
                        <div class="benefits condensedFont bnfts2">
                            <ul class="bullets">
                                <li class="bulletItem">Collect instant and granular feedback!</li>
                                <li class="bulletItem">Send thank you or apology messages!</li>
                                <li class="bulletItem">Send coupons to your loyal customers!</li>
                                <li class="bulletItem nonmobile">Launch sales campaigns, send special offers! <span class="soon">(soon)</span></li>
                                <li class="bulletItem mobile">Launch sales campaigns! <span class="soon">(soon)</span></li>
                                <li class="bulletItem">Replace your old-fashioned survey tools!</li>
                                <li class="bulletItem">Manage customer satisfaction and loyalty!</li>
                            </ul> 
                        </div>
                        <div class="joinNowBtn" id="joinNowBtnDiv" runat="server">
                            <a href="Signup.aspx">
                                <img alt="Join Now" class="joinNowImg" src="Images/joinNowBtn.png" />
                            </a>
                        </div>
                        <div class="ctrlPanelLinkWrp" id="ctrlPanelLinkWrp" runat="server">
                            <div class="fakeHeight"></div>
                                <p class="centerText">
                                    <a class="ctrlPanelLink" href="Feedbacks.aspx">
                                        Go to Dashboard
                                    </a>
                                </p>
                        </div>
                    </div>
                </div>

            </div>

            <div class="appsAndSocialContainer condensedFont">
                <div class="appsTextWrp">
                    <div class="appsTextHeader">
                        Mobile Apps
                    </div>
                    <div class="appsText">
                        Search '<a href ="http://itunes.apple.com/us/app/dine-smart-365/id543828918?ls=1&mt=8">
                        <span class="appName">Dine Smart</span></a>' in your IPhone's App Store! Apps for other smart phones will be coming shortly!..
                    </div>
                </div>
                <div class="socialLinks">
                    <a href="http://facebook.com/pages/DineSmart365com/190167041076884" target="_blank">
                        <img class="socialImg facebook" alt="facebook" src="Images/facebook.png" />
                    </a>
                    <a href="https://twitter.com/#!/DineSmart365" target="_blank">
                        <img class="socialImg twitter" alt="twitter" src="Images/twitter.png" />
                    </a>
                    <a href="mailto:info@dinesmart365.com">
                        <img class="socialImg mail" alt="mail" src="Images/mail.png" />
                    </a>
                </div>
            </div>

            <!-- below line is NOT a comment but a real CODE -->
            <!--#include file="footer.html"-->
            <!-- DO NOT DELETE ABOVE LINE -->

        </div>
        
        <div class="clear"></div>
    </div>
</body>
</html>

