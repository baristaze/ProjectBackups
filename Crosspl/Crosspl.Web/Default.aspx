<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Crosspl.Web.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title id="pageTitle" runat="server">Vizibuzz</title>
    <meta name="description" content="News, Fun Stories, Trending Topics, Popular Feeds, Buzz" />
    <meta name="keywords" content="Vizibuzz,Vizi,Buzz,News,Fun,Funny,Story,Trendy,Trending,Topic,Buzz,Popular,Feed,Blog,Social,Facebook,Comics" />
    <meta property="og:type" content="blog" />
    <meta property="og:title" content="Vizibuzz" />
    <meta property="og:description" content="News, Fun Stories, Trending Topics, Popular Feeds, Buzz" />
    <meta property="og:image" content="<%=Request.ApplicationPath.TrimEnd('/') + "/Images/logo.png" %>" />
    <meta name="format-detection" content="telephone=no" />
    <!--<meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1" />-->

<!-- Google Analytics Content Experiment code -->
<script>    function utmx_section() { } function utmx() { } (function () {
        var 
k = '70062152-6', d = document, l = d.location, c = d.cookie;
        if (l.search.indexOf('utm_expid=' + k) > 0) return;
        function f(n) {
            if (c) {
                var i = c.indexOf(n + '='); if (i > -1) {
                    var j = c.
indexOf(';', i); return escape(c.substring(i + n.length + 1, j < 0 ? c.
length : j))
                }
            }
        } var x = f('__utmx'), xx = f('__utmxx'), h = l.hash; d.write(
'<sc' + 'ript src="' + 'http' + (l.protocol == 'https:' ? 's://ssl' :
'://www') + '.google-analytics.com/ga_exp.js?' + 'utmxkey=' + k +
'&utmx=' + (x ? x : '') + '&utmxx=' + (xx ? xx : '') + '&utmxtime=' + new Date().
valueOf() + (h ? '&utmxhash=' + escape(h.substr(1)) : '') +
'" type="text/javascript" charset="utf-8"><\/sc' + 'ript>')
    })();
</script><script>             utmx('url', 'A/B');</script>
<!-- End of Google Analytics Content Experiment code -->

    <!-- Google analytics -->
    <script type="text/javascript">

        var _gaq = _gaq || [];
        _gaq.push(['_setAccount', 'UA-39196391-1']);
        _gaq.push(['_trackPageview']);

        (function () {
            var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
            ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
            var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
        })();

    </script>

    <link rel="shortcut icon" href="<%=Request.ApplicationPath.TrimEnd('/') + "/Images/favicon.ico" %>" type="image/x-icon" />
    <link rel="icon" href="<%=Request.ApplicationPath.TrimEnd('/') + "/Images/favicon.ico" %>" type="image/x-icon" />

    <link rel="stylesheet" type="text/css" href="<%=Request.ApplicationPath.TrimEnd('/') + "/Styles/Cover.css?version=0.97" %>" />

    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.9.2/jquery-ui.min.js"></script> 
    <script type="text/javascript" src="http://connect.facebook.net/en_US/all.js"></script>

    <% if (HttpContext.Current.IsDebuggingEnabled) { %>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/Common.debug.js?version=0.97" %>" ></script>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/Url.debug.js?version=0.97" %>" ></script>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/Dialog.debug.js?version=0.97" %>" ></script>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/Splitter.debug.js?version=0.97" %>" ></script>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/FacebookConnect.debug.js?version=0.97" %>" ></script>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/Menu.debug.js?version=0.97" %>" ></script>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/Cover.debug.js?version=0.97" %>" ></script>
    <% } else { %>
          <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/js-bundle-min.js?version=0.97" %>" ></script>
    <% } %>

    <script type="text/javascript">
        
        // see Cover.debug.js
        var __facebookExcludedIds = [];

        function coverPage_InitFacebookEx() {
            <% if (HttpContext.Current.IsDebuggingEnabled) { %>
            initFacebook('133423290166132', coverPage_FBAuthStateChanged, coverPage_FBServerAuthCompletedSuccessfully);
            <% } else { %>
            initFacebook('327201024060399', coverPage_FBAuthStateChanged, coverPage_FBServerAuthCompletedSuccessfully);
            <% } %>
        }

        $(document).ready(function () {
            applyClientSideSplitters();
            coverPage_DocumentReady();
        });
            
    </script>

    <style type="text/css" media="all">
        .hidden{display:none;}
        @media all and (max-height: 720px) { .footer { display:none; } }
    </style>
    
    <!-- This has to be the latest CSS link in this document -->
    <link rel="stylesheet" type="text/css" href=<%=GetCssForSplitTesting()%> />
    <!-- We better not add any CSS after this line. For readability, do not add scripts either-->

</head>

<body>
    <div id="splitResources" runat="server" class="splitResources hidden"></div>
    <center class="landingWrp">
        <div id="applicationPath" class="hidden" runat="server"></div>
        <div id="isAuthenticated" class="hidden" runat="server">0</div>
        <div class="landing split-landing">
            <div class="leftWrp">
                <div class="leftInner split-left-inner">
                </div>
                <div class="logoWrp split-logo-wrp">
                    <p id="logoTextCtrl" class="companyName" runat="server">Vizibuzz</p>
                </div>
                <div class="ideaWrp roundedCorners split-idea-wrp">
                    <p id="ideaTextCtrl" class="idea" runat="server">
                        Make it visible! <br/>
                        Vizibuzz connects you to the world. 
                    </p>
                </div>
            </div>
            <div class="rightWrp">
                <div class="rightInner">
                    <div class="statusWrp switcher">Checking login status...&nbsp;&nbsp;<a href="javascript:void(0);" class="reload">Retry</a></div>
                    <div class="signUpWrp switcher hidden">
                        
                        <div id="joinTextCtrl" class="reqInvCode" runat="server">
                            Connect with Facebook for a better experience!
                        </div>

                        <div class="signUp fbSignUp roundedCorners split-facebook-signup">
                            <div class="signUpImgWrp">
                                <img alt="F" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Images/facebook.png" %>" />
                            </div>
                            <div class="signUpTextWrp">
                                <span id="signupTextCtrl" class="signUpText" runat="server">
                                    Connect with Facebook
                                </span>
                            </div>
                        </div>

                        <div class="skip-wrp">
                            <a href="<%=Request.ApplicationPath.TrimEnd('/') + "/latest" %>" class="skip-link skip-link-without-auth">Skip</a>
                        </div>

                    </div>
                    <div class="contentWrp switcher hidden">
                        <div class="loginInfoWrp">
                            Hello <span id="currentUserName" class="userName" runat="server"></span>
                            <a href="javascript:void(0);" class="cover-page-logout">logout</a>    
                        </div>
                        <div id="waitListTextCtrl" class="waitingList split-waiting-list" runat="server">
                            Please help us to spread the word by inviting your friends!
                        </div>
                        <div class="contentInner contentBkg">
                            <div class="inviteWrp">
                                <div class="invite roundedCorners split-invite">
                                    <p class="inviteInner">Invite</p>
                                </div>
                            </div>
                        </div>
                        <div class="invitedStats">
                            <span class="invitedLabel"># of invited friends:</span>
                            <span class="invitedResult">?</span>
                        </div>
                        <a href="<%=Request.ApplicationPath.TrimEnd('/') + "/latest" %>" class="skip-link skip-link-with-auth">Skip</a>
                    </div>
                    <div class="footer">
                        <span class="legal privacy">Privacy</span>
                        <span class="legal sep"> | </span>
                        <span class="legal terms">Terms and Conditions</span>
                    </div>
                </div>
            </div>
            <div class="clear"></div>
        </div>
    </center>
    <div class="hidden" id="latestTopics" runat="server">
    </div>
</body>
</html>
