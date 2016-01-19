<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserActivities.aspx.cs" Inherits="Shebeke.Web.UserActivities" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="tr" lang="tr">
<head runat="server">
    <title id="pageTitle" runat="server">Shebeke - Kullanıcı Aktiviteleri</title>
    <meta name="description" content="Haberler, Gündem, Komik Paylaşımlar, Popüler Kültür" />
    <meta name="keywords" content="Shebeke,Sebeke,Şebeke,Haber,Eğlence,Komik,Hikaye,Trend,Popüler,Konu,Başlık,Gündem,Blog,Sosyal,Paylaşım,Facebook,Twitter" />
    <meta property="og:type" content="blog" />
    <meta property="og:title" content="Shebeke" />
    <meta property="og:description" content="Haberler, Gündem, Komik Paylaşımlar, Popüler Kültür" />
    <meta property="og:image" content="<%=Request.ApplicationPath.TrimEnd('/') + "/Images/logo.png?version=0.81" %>" />
    <meta name="format-detection" content="telephone=no" />
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1" />

<!-- Google Analytics Content Experiment code -->
<!--
<script>    function utmx_section() { } function utmx() { } (function () {
        var 
k = '76168981-0', d = document, l = d.location, c = d.cookie;
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
-->
<!-- End of Google Analytics Content Experiment code -->

    <!-- Google analytics -->
    <script type="text/javascript">
        var _gaq = _gaq || [];
        _gaq.push(['_setAccount', 'UA-43672396-1']);
        _gaq.push(['_trackPageview']);

        (function () {
            var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
            ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
            var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
        })();
    </script>

    <link rel="shortcut icon" href="<%=Request.ApplicationPath.TrimEnd('/') + "/Images/favicon.ico" %>" type="image/x-icon" />
    <link rel="icon" href="<%=Request.ApplicationPath.TrimEnd('/') + "/Images/favicon.ico" %>" type="image/x-icon" />
    <link rel="stylesheet" type="text/css" href="<%=Request.ApplicationPath.TrimEnd('/') + "/css/bootstrap_limited.min.css" %>" />
    <link rel="stylesheet" type="text/css" href="<%=Request.ApplicationPath.TrimEnd('/') + "/Styles/Layout.css?version=0.985" %>" />
    <link rel="stylesheet" type="text/css" href="<%=Request.ApplicationPath.TrimEnd('/') + "/Styles/Theme.css?version=0.985" %>" />
    <link rel="stylesheet" type="text/css" href="<%=Request.ApplicationPath.TrimEnd('/') + "/Styles/UserActivities.css?version=0.985" %>" />

    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.9.2/jquery-ui.min.js"></script> 
    <script type="text/javascript" src="http://connect.facebook.net/tr_TR/all.js"></script>
    <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/js/bootstrap_limited.min.js" %>" ></script>
    <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/jquery.blockUI-2.56.min.js" %>" ></script>

    <% if (HttpContext.Current.IsDebuggingEnabled) { %>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/Common.debug.js?version=0.985" %>" ></script>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/Url.debug.js?version=0.985" %>" ></script>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/Dialog.debug.js?version=0.985" %>" ></script>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/Splitter.debug.js?version=0.985" %>" ></script>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/FacebookConnect.debug.js?version=0.985" %>" ></script>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/EntryMisc.debug.js?version=0.985" %>" ></script>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/TopicSearchAddDelete.debug.js?version=0.985" %>" ></script>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/Menu.debug.js?version=0.9852" %>" ></script>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/UserActivities.debug.js?version=0.985" %>" ></script>
    <% } else { %>
          <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/js-bundle-min.js?version=0.98546" %>" ></script>
    <% } %>

    <!-- below part should be in this file since it has some ASP directives -->
    <script type="text/javascript">
        
        var _lastTopicSearchQuery = null;

        function userActivitiesPage_InitFacebookEx() {
           <% if (HttpContext.Current.IsDebuggingEnabled) { %>
           initFacebook('164591543732009', onAuthStateChangeDefault, onServerAuthDefault);
           <% } else { %>
           initFacebook('493048124120847', onAuthStateChangeDefault, onServerAuthDefault);
           <% } %>
        }

        $(document).ready(function () {
            applyClientSideSplitters();
            userActivitiesPage_InitFacebookEx();
            userActivitiesFacebookConnectInit();
            menu_Init(onAuthStateChangeDefault);
            dialogs_InitCommons();
            entryPage_Init_TopicSearchAddDelete();
            userActivitiesPage_Init();
        });

    </script>

    <style type="text/css" media="all">
        
        .content{padding-top:10px;}
        .mb-aux2{width:0px;}        
        @media all and (min-width:980px)  
        { 
            .mb-inner{width:650px;}
            .tbar-srch-wrp{width:650px;}
        }
        
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
                    <div class="logo txt-logo"><div class="logo-inner">shebeke</div></div>
                    <div class="tbar-srch-wrp">
                        <div class="tbar-srch-inner bbox">
                            <div class="tbar-srch-inp-dsc bbox watermark-helper">başlık ekle</div>
                            <input id="searchTopicBox" class="tbar-srch-inp box-shdw bbox watermark-main semi-trans" type="text" />
                        </div>
                    </div>
                    <div class="tb-login tb-login-mini">
                        <div class="tb-login-none tb-login-none-mini auth-ctrl-none txt-lrx2 rnd-crn mobile-menu-area"> 
                            <span class="mobile-menu-symbol">&#9776;</span>
                            <div class="user-menu user-menu-mini hidden">
                                <div class="signup-facebook user-menu-item menu-item-logout">giriş yap</div>
                            </div>
                        </div>
                        <div class="tb-login-checking auth-ctrl-checking hidden">
                            <img class="check-fb-auth check-fb-auth-mini" alt="..." src="<%=Request.ApplicationPath.TrimEnd('/') + "/Images/busy-red.gif" %>" />
                        </div>
                        <div class="tb-login-success tb-login-success-mini act clr-txt-flu auth-ctrl-success txt-lrx2 rnd-crn mobile-menu-area hidden">
                            <span class="mobile-menu-symbol">&#9776;</span>
                            <div class="user-menu user-menu-mini hidden">
                                <div class="user-menu-item menu-item-home">ana sayfa</div>
                                <div class="logout user-menu-item menu-item-logout">çıkış</div>
                                <div class="user-menu-item menu-item-notif hidden">iletiler</div>
                                <div class="user-menu-item menu-item-settings hidden">ayarlar</div>
                                <div id="currentUserName2" runat="server" class="user-menu-item">misafir</div>
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
                                    <span class="signup-txt">Facebook ile Bağlan</span>
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
                                <div class="logout user-menu-item menu-item-logout">çıkış</div>
                                <div class="user-menu-item menu-item-notif hidden">iletiler</div>
                                <div class="user-menu-item menu-item-settings hidden">ayarlar</div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="page">
        <div id="content" class="content">

            <div class="main-block">
                <div class="mb-aux1"></div>
                <div class="mb-aux2"></div>
                <div class="mb-inner target-user">
                    <div class="table-cell-wrapper">
                        <a id="targetUserFacebookLink" href="" runat="server">
                            <img id="targetUserImage" runat="server" src="" class="act-log-user-img" alt='' title='bu yazarın Facebook sayfasına git' />
                        </a>
                    </div>
                    <div class="table-cell-wrapper">
                        <span id="targetUserName" runat="server" class="act-log-user-name txt-xlr"></span>
                    </div>
                </div>
            </div>    

            <div class="main-block">
                <div class="mb-aux1"></div>
                <div class="mb-aux2"></div>
                <div class="mb-inner">
                    <table class="table table-hover table-condensed act-log-table">
                        <tbody>
                            <asp:Repeater id="activityLogTableRepeater" runat="server">
	                            <ItemTemplate>
		                        <tr class='<%#DataBinder.Eval(Container.DataItem, "ActionTypeColorClass")%>'>
                                    <td class="act-log-data hidden">
                                        <div class="act-log-data-link hidden"><%#DataBinder.Eval(Container.DataItem, "LinkToData")%></div>
                                    </td>
                                    <td class="act-log-action-time clr-txt-flu txt-sml">
                                        <%#DataBinder.Eval(Container.DataItem, "ActionTime")%>
                                    </td>
                                    <td class="act-log-action-name">
                                        <%#DataBinder.Eval(Container.DataItem, "Action")%>
                                    </td>
                                    <td class="act-log-action-data">
                                        <a href='<%#DataBinder.Eval(Container.DataItem, "LinkToData")%>' class="topic-link"><%#DataBinder.Eval(Container.DataItem, "ActionData")%></a>
                                    </td>
                                </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>    
                </div>
            </div>    
        </div>

        <div id="helper" class="helper">
            <div class="helper-inner">
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
                    <button class="confirm-dialog-btn-no rnd-crn txt-lrx">Evet</button>
                    <button class="confirm-dialog-btn-yes rnd-crn txt-lrx">Hayır</button>
                </div>
            </div>
        </div>

        <div class="dialog login-dialog hidden">
            <span class="dialog-close-button box-shadow-big encircle"><span>x</span></span>
            <div class="dialog-content login-dialog-content">
                <div class="login-txt">Bu işlemi gerçekleştirmek için önce giriş yapmanız gerekiyor</div>
                <button class="signup signup-modal rnd-crn-big txt-lrx act">
                    <div class="table-cell-wrapper">
                        <img class="facebook-img-big" alt="F" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Images/facebook.png" %>" />
                    </div>
                    <div class="table-cell-wrapper txt-xlr">
                        <span class="signup-txt desktop-only">Facebook ile Bağlan</span>
                        <span class="signup-txt mobile-only">Bağlan</span>
                    </div>
                </button>
            </div>
        </div>

       <div id="newTopicDialog" class="dialog new-topic-dialog hidden">
            <span class="dialog-close-button box-shadow-big encircle"><span>x</span></span>
            <div class="dialog-content new-topic-dialog-content">
                <div id="newTopicDialogResult" class="hidden">-1</div>
                <div class="social-share-wrp-outside">
                    <div class="new-topic-label clr-txt-flu">Yeni Başlık</div>
                    <div class="social-share-wrp">
                        <div class="table-cell-wrapper">
                            <span class="clr-txt-flu">paylaş: &nbsp;</span>
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
                <button id="newTopicAddBtn" class="new-topic-dialog-btn-add rnd-crn-med txt-lrx">Başlığı Ekle</button>
            </div>
        </div>

    </div>

    <!-- this helps the rest of the UI that is generated by scripts behaves well -->
    <!-- for example: custom facebook invite dialog. otherwise they suffer from previous floats -->
    <div class="clear"></div>  
</body>
</html>
