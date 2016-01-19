<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Topics.aspx.cs" Inherits="Crosspl.Web.Topics" %>

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
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1" />

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
    <link rel="stylesheet" type="text/css" href="<%=Request.ApplicationPath.TrimEnd('/') + "/Styles/Layout.css?version=0.97" %>" />
    <link rel="stylesheet" type="text/css" href="<%=Request.ApplicationPath.TrimEnd('/') + "/Styles/Theme.css?version=0.97" %>" />
    <link rel="stylesheet" type="text/css" href="<%=Request.ApplicationPath.TrimEnd('/') + "/Styles/Topics.css?version=0.973" %>" />

    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.9.2/jquery-ui.min.js"></script> 
    <script type="text/javascript" src="http://connect.facebook.net/en_US/all.js"></script>
    <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/jquery.blockUI-2.56.min.js" %>" ></script>
    
    <% if (HttpContext.Current.IsDebuggingEnabled) { %>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/Common.debug.js?version=0.97" %>" ></script>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/Url.debug.js?version=0.97" %>" ></script>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/Dialog.debug.js?version=0.97" %>" ></script>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/Splitter.debug.js?version=0.97" %>" ></script>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/FacebookConnect.debug.js?version=0.97" %>" ></script>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/EntryMisc.debug.js?version=0.97" %>" ></script>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/TopicSearchAddDelete.debug.js?version=0.97" %>" ></script>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/Menu.debug.js?version=0.97" %>" ></script>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/Topics.debug.js?version=0.97" %>" ></script>
    <% } else { %>
          <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/js-bundle-min.js?version=0.97" %>" ></script>
    <% } %>

    <!-- below part should be in this file since it has some ASP directives -->
    <script type="text/javascript">
        
        var _lastTopicSearchQuery = null;

        function topicsPage_InitFacebookEx() {
           <% if (HttpContext.Current.IsDebuggingEnabled) { %>
           initFacebook('133423290166132', onAuthStateChangeDefault, onServerAuthDefault);
           <% } else { %>
           initFacebook('327201024060399', onAuthStateChangeDefault, onServerAuthDefault);
           <% } %>
        }

        $(document).ready(function () {
            applyClientSideSplitters();
            topicsPage_InitFacebookEx();
            // facebookConnect_Init_DefaultActions();
            topicsFacebookConnectInit();
            menu_Init(onAuthStateChangeDefault);
            dialogs_InitCommons();
            entryPage_Init_TopicSearchAddDelete();
            topicPage_Init();
        });

    </script>

    <style type="text/css" media="all">
        .btn, .btn:active, .btn:focus{outline:none;}
        a,i,li,li:active,li:focus{outline:none;}
        
        .hidden{display:none;}
    </style>
    
    <!-- This has to be the latest CSS link in this document -->
    <link rel="stylesheet" type="text/css" href=<%=GetCssForSplitTesting()%> />
    <!-- We better not add any CSS after this line. For readability, do not add scripts either-->
</head>
<body>
    <div id="applicationPath" class="hidden" runat="server"></div>
    <div id="isAuthenticated" class="hidden" runat="server">0</div>
    <div id="splitResources" runat="server" class="splitResources hidden"></div>
    <div class="tb-strip bkg1">
        <div class="topbar">
            <div class="tbi-left-wrp">
                <div class="tb-left">
                    <div class="logo txt-logo"><div class="logo-inner">vizibuzz</div></div>
                    <div class="tbar-srch-wrp">
                        <div class="tbar-srch-inner bbox">
                            <div class="tbar-srch-inp-dsc bbox watermark-helper">add a topic</div>
                            <input id="searchTopicBox" class="tbar-srch-inp box-shdw bbox watermark-main semi-trans" type="text" />
                        </div>
                    </div>
                    <div class="tb-login tb-login-mini">
                        <div class="tb-login-none tb-login-none-mini auth-ctrl-none txt-lrx2 rnd-crn mobile-menu-area"> 
                            <span class="mobile-menu-symbol">&#9776;</span>
                            <div class="user-menu user-menu-mini hidden">
                                <div class="signup-facebook user-menu-item menu-item-logout">sign in</div>
                            </div>
                        </div>
                        <div class="tb-login-checking auth-ctrl-checking hidden">
                            <img class="check-fb-auth check-fb-auth-mini" alt="..." src="<%=Request.ApplicationPath.TrimEnd('/') + "/Images/busy-red.gif" %>" />
                        </div>
                        <div class="tb-login-success tb-login-success-mini act clr-txt-flu auth-ctrl-success txt-lrx2 rnd-crn mobile-menu-area hidden">
                            <span class="mobile-menu-symbol">&#9776;</span>
                            <div class="user-menu user-menu-mini hidden">
                                <div class="logout user-menu-item menu-item-logout">logout</div>
                                <div class="user-menu-item menu-item-notif hidden">notifications</div>
                                <div class="user-menu-item menu-item-settings hidden">settings</div>
                                <div id="currentUserName2" runat="server" class="user-menu-item">guest</div>
                            </div>
                        </div>
                    </div>
                    <div class="clear"></div>
                </div>
            </div>
            <div class="tbi-right-wrp">
                <div class="tb-right">
                    <div class="tb-login">
                        <div class="tb-login-none auth-ctrl-none">
                            <button class="signup signup-facebook txt-lrg act">
                                <div class="table-cell-wrapper">
                                    <img class="facebook-img" alt="F" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Images/facebook.png" %>" />
                                </div>
                                <div class="table-cell-wrapper">
                                    <span class="signup-txt">Connect with Facebook</span>
                                </div>
                            </button>
                        </div>
                        <div class="tb-login-checking auth-ctrl-checking hidden">
                            <img class="check-fb-auth" alt="..." src="<%=Request.ApplicationPath.TrimEnd('/') + "/Images/busy-red.gif" %>" />
                        </div>
                        <div class="tb-login-success act clr-txt-flu auth-ctrl-success hidden">
                            <div class="table-cell-wrapper">
                                <span id="currentUserName" class="login-name" runat="server"></span>
                            </div>
                            <div class="table-cell-wrapper">
                                <img id="currentUserPhoto" class="login-photo"  runat="server" alt="user" src="" />
                            </div>
                            <div class="user-menu hidden">
                                <div class="logout user-menu-item menu-item-logout">logout</div>
                                <div class="user-menu-item menu-item-notif hidden">notifications</div>
                                <div class="user-menu-item menu-item-settings hidden">settings</div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="hidden" id="latestTopics" runat="server">
    </div>

    <div class="page">
        <div id="content" class="content">
            <asp:Repeater id="recentPopularTopicsRepeater" runat="server">
                <ItemTemplate> 
                    <div class="main-block topic-block">
                        <div class="mb-aux1 topic-left">
                            <a href="<%#DataBinder.Eval(Container.DataItem, "CreaterActivityLink")%>" class="user-img-link act">
                                <img class="user-img act brd desktop-only" alt="." src='<%#DataBinder.Eval(Container.DataItem, "CreaterPhotoUrl")%>' title="see more info about this writer"/>
                            </a>
                        </div>
                        <div class="mb-aux2"></div>
                        <div class="mb-inner">
                            <div class="topic-box">
                                <div class="hidden">
                                    <div class="topic-id"><%#DataBinder.Eval(Container.DataItem, "Id")%></div>
                                    <div class="topic-seo-plain"><%#DataBinder.Eval(Container.DataItem, "SeoLinkPlain")%></div>
                                </div>
                                <div class="table-cell-wrapper">
                                    <a href='<%#DataBinder.Eval(Container.DataItem, "SeoLink")%>' class="topic-link txt-lrx2"><%#DataBinder.Eval(Container.DataItem, "Title")%></a>
                                    <span class="entry-count txt-sml"><%#DataBinder.Eval(Container.DataItem, "EntryCountText")%></span>
                                </div>
                                <div class="table-cell-wrapper">
                                    <div class="social-share-sep"></div>
                                </div>
                                <div class="table-cell-wrapper social-mini facebook-icon-wrp-mini rnd-crn hidden" title="share this topic on Facebook">
                                    <img class="facebook-icon-img-mini" alt="F" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Images/facebook-small.png" %>" />
                                </div>
                                <div class="table-cell-wrapper">
                                    <div class="social-share-sep"></div>
                                </div>
                                <div class="table-cell-wrapper social-mini twitter-icon-wrp-mini rnd-crn hidden" title="share this topic on Twitter">
                                    <img class="twitter-icon-img-mini" alt="T" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Images/twitter-small.png" %>" />
                                </div>
                                <div class="topic-info-second-line">
                                    <span class="topic-info-title txt-sml clr-txt-flu"><%#DataBinder.Eval(Container.DataItem, "SocialNumbers")%></span>
                                    <span class="topic-top-writers txt-sml clr-txt-flu"><%#DataBinder.Eval(Container.DataItem, "TopWriterNames")%></span>
                                </div>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <div id="helper" class="helper">
            <div class="helper-inner">
                <div id="promotedTopicsTitle" class="promoted-topic-title txt-xlr txt-subt">Quick Info</div>
                <ul class="promoted-topics">
                    <asp:Repeater id="promotedTopicsRepeater" runat="server">
                        <ItemTemplate>
                            <li class='promoted-topic act txt-lrx'>
                                <a href='<%#DataBinder.Eval(Container.DataItem, "SeoLink")%>' class='promoted-topic-link'>
                                    <%#DataBinder.Eval(Container.DataItem, "Title")%>
                                </a>
                                <span class="promoted-topic-id hidden"><%#DataBinder.Eval(Container.DataItem, "Id")%></span>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
            </div>    
        </div>
    </div>
    <!-- this helps the rest of the UI that is generated by scripts behaves well -->
    <!-- for example: custom facebook invite dialog. otherwise they suffer from previous floats -->
    <div class="clear"></div>

    <div class="pageResources hidden">

        <div class="dialog error-dialog hidden">
            <span class="dialog-close-button box-shadow-big encircle"><span>x</span></span>
            <div class="dialog-content error-dialog-content"></div>
        </div>

        <div class="dialog confirm-dialog hidden">
            <span class="dialog-close-button box-shadow-big encircle"><span>x</span></span>
            <div class="dialog-content confirm-dialog-content">
                <div class="confirm-dialog-result hidden">-1</div>
                <div class="confirm-dialog-content-inner"></div>
                <div class="confirm-dialog-buttons">
                    <button class="confirm-dialog-btn-no rnd-crn txt-lrx">No</button>
                    <button class="confirm-dialog-btn-yes rnd-crn txt-lrx">Yes</button>
                </div>
            </div>
        </div>

        <div class="dialog login-dialog hidden">
            <span class="dialog-close-button box-shadow-big encircle"><span>x</span></span>
            <div class="dialog-content login-dialog-content">
                <div class="login-txt">You need to connect first to perform this action</div>
                <button class="signup signup-modal rnd-crn-big txt-lrx act">
                    <div class="table-cell-wrapper">
                        <img class="facebook-img-big" alt="F" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Images/facebook.png" %>" />
                    </div>
                    <div class="table-cell-wrapper txt-xlr">
                        <span class="signup-txt desktop-only">Connect with Facebook</span>
                        <span class="signup-txt mobile-only">Connect</span>
                    </div>
                </button>
            </div>
        </div>

       <div id="newTopicDialog" class="dialog new-topic-dialog hidden">
            <span class="dialog-close-button box-shadow-big encircle"><span>x</span></span>
            <div class="dialog-content new-topic-dialog-content">
                <div id="newTopicDialogResult" class="hidden">-1</div>
                <div class="social-share-wrp-outside">
                    <div class="new-topic-label clr-txt-flu">New Topic</div>
                    <div class="social-share-wrp">
                        <div class="table-cell-wrapper">
                            <span class="clr-txt-flu">share on: &nbsp;</span>
                        </div>
                        <div class="table-cell-wrapper">
                            <input class="social-share-check social-share-check-facebook" type="checkbox" value="1" checked=checked />
                        </div>
                        <div class="table-cell-wrapper facebook-icon-wrp-mini rnd-crn" title="Facebook">
                            <img class="facebook-icon-img-mini" alt="F" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Images/facebook-small.png" %>" />
                        </div>
                        <div class="table-cell-wrapper hidden">
                            <span>&nbsp;&nbsp;</span>
                        </div>
                        <div class="table-cell-wrapper hidden">
                            <input class="social-share-check social-share-check-twitter" type="checkbox" value="2" checked=checked />
                        </div>
                        <div class="table-cell-wrapper twitter-icon-wrp-mini rnd-crn hidden" title="Twitter">
                            <img class="twitter-icon-img-mini" alt="T" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Images/twitter-small.png" %>" />
                        </div>
                    </div>
                    <div class="clear"></div>
                </div>
                <div class="new-topic-inp-wrp bbox">
                    <textarea id="newTopicInput" class="new-topic-inp txt-lrg bbox" rows="3" maxlength="400"></textarea>
                </div>                
                <button id="newTopicAddBtn" class="new-topic-dialog-btn-add rnd-crn-med txt-lrx">Add Topic</button>
            </div>
        </div>

    </div>

    <!-- this helps the rest of the UI that is generated by scripts behaves well -->
    <!-- for example: custom facebook invite dialog. otherwise they suffer from previous floats -->
    <div class="clear"></div>
</body>
</html>
