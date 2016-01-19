<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TopicEntries.aspx.cs" Inherits="Crosspl.Web.TopicEntries" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" xmlns:og="http://ogp.me/ns#">
<head>
    <title id="pageTitle" runat="server">Vizibuzz</title>
    <meta name="description" content="News, Fun Stories, Trending Topics, Popular Feeds, Buzz" />
    <meta name="keywords" content="Vizibuzz,Vizi,Buzz,News,Fun,Funny,Story,Trendy,Trending,Topic,Buzz,Popular,Feed,Blog,Social,Facebook,Comics" />
    <meta property="og:type" content="blog" />
    <meta property="og:title" content="<%= this.PageTitleForFacebook %>" />
    <meta property="og:description" content="<%= this.PageDescriptionForFacebook %>" />
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
    <link rel="stylesheet" type="text/css" href="<%=Request.ApplicationPath.TrimEnd('/') + "/Styles/Entry.css?version=0.97" %>" />
        
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.9.2/jquery-ui.min.js"></script>
    <script type="text/javascript" src="http://connect.facebook.net/en_US/all.js"></script>
    <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/jquery.autosize-min.js" %>" ></script>
    <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/jquery.form.min.js" %>" ></script>
    <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/jquery.blockUI-2.56.min.js" %>" ></script>

    <% if (HttpContext.Current.IsDebuggingEnabled) { %>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/Common.debug.js?version=0.97" %>" ></script>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/Base64.debug.js?version=0.97" %>" ></script>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/Url.debug.js?version=0.97" %>" ></script>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/Dialog.debug.js?version=0.97" %>" ></script>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/Splitter.debug.js?version=0.97" %>" ></script>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/FacebookConnect.debug.js?version=0.97" %>" ></script>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/EntryMisc.debug.js?version=0.97" %>" ></script>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/Voting.debug.js?version=0.97" %>" ></script>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/Reacting.debug.js?version=0.97" %>" ></script>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/ImageUpload.debug.js?version=0.97" %>" ></script>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/EntryPreview.debug.js?version=0.983" %>" ></script>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/EntryAddGetDelete.debug.js?version=0.97" %>" ></script>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/TopicSearchAddDelete.debug.js?version=0.97" %>" ></script>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/Sharing.debug.js?version=0.97" %>" ></script>
        <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/Menu.debug.js?version=0.97" %>" ></script>
    <% } else { %>
          <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/js-bundle-min.js?version=0.983" %>" ></script>
    <% } %>
    
    <!-- below part should be in this file since it has some ASP directives -->
    <script type="text/javascript">
        
        var _lastTopicSearchQuery = null;
        var MAX_APP_REQUEST_RECIPIENTS = 50;
        var __allowedExtensions = ["jpeg", "jpg", "png", "bmp", "gif", "tif", "tiff"];

        function entryPage_InitFacebookEx() {
           <% if (HttpContext.Current.IsDebuggingEnabled) { %>
           initFacebook('133423290166132', onAuthStateChanged, onServerAuthDefault);
           <% } else { %>
           initFacebook('327201024060399', onAuthStateChanged, onServerAuthDefault);
           <% } %>
        }

        $(document).ready(function () {
            applyClientSideSplitters();
            entryPage_InitFacebookEx();
            facebookConnect_Init_DefaultActions();
            menu_Init(onAuthStateChanged);
            dialogs_InitCommons();
            entryPage_Init_TopicSearchAddDelete();
            entryPage_Init_ImageUpload();
            entryPage_Init_Preview();
            entryPage_Init_AddGetDelete();
            entryPage_Init_Reacting();
            entryPage_Init_Sharing();
            entryPage_Init_Voting();
        });

    </script>

    <style type="text/css" media="all">
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
    <div class="page">
        <div id="content" class="content">
            <!-- topic -->
            <div class="main-block main-block-top">
                <div class="mb-aux1 topic-aux"></div>
                <div class="mb-aux2 topic-aux"></div>
                <div class="mb-inner">
                    <div class="tpc">
                        <div id="topicId" class="hidden" runat="server"></div>
                        <div id="canDeleteIfNoEntry" class="hidden" runat="server"></div>
                        <div class="cats">
                            <span id="categoriesTitleLabel" class="txt-med clr-txt-sub" runat="server"></span>
                            <asp:Repeater id="categoryRepeater" runat="server">
	                            <ItemTemplate>
                                    <span class="tag-lnk act txt-sml rnd-crn"><%#DataBinder.Eval(Container.DataItem, "Name")%></span>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                        <div class="topic-word txt-xlr txt-title">
                            <span id="topicTitle" runat="server">Topic couldn't be found</span>
                            <span id="topicDelete" runat="server" class="del-lnk act txt-sml rnd-crn">delete</span>
                            <div class="topic-acts hidden">
                                <div class="topic-act tag-lnk act txt-med rnd-crn" title="follow this topic">Follow</div>
                            </div>
                        </div>
                        <div class="desktop-only hidden">
                            <span class="clr-txt-sub txt-med">Disambiguation from:</span>
                            <span class="dcat act txt-med clr-txt-flu">tv episode</span>
                            <span class="dcat act txt-med clr-txt-flu">book</span>
                            <span class="dcat act txt-med clr-txt-flu">café</span>
                        </div>
                    </div>
                </div>
                <div class="clear"></div>
            </div>

            <!-- add yours -->
            <div class="main-block entry-add hidden">
                <div class="mb-aux1">                    
                </div>
                <div class="mb-aux2">
                </div>
                <div class="mb-inner brdT">
                    <div class="entry-add-advertise">
                        <div id="entryAddEncourageBeFirst" class="entry-add-encourage be-first hidden">Be the first person to add an entry!</div>
                        <div id="entryAddEncourageNoFirst" class="entry-add-encourage no-first">Enlighten us!</div>
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

                    <div class="entry-add-wrp">
                        <div class="entry-add-inp-wrp">
                            <textarea class="entry-add-inp-dsc txt-lrg bbox watermark-helper" cols="100%" rows="1" disabled="disabled"></textarea>
                            <textarea class="entry-add-inp txt-lrg bbox watermark-main semi-trans" cols="100%" rows="1" ></textarea>
                        </div>
                    </div>
                    <div class="entry-add-btn-wrp">
                        <div class="photo-upload-section">
                            <div class="photo-upload-start-wrp">
                                <button class="photo-upload-start-btn"></button>
                            </div>
                            <div class="photo-attach-list">
                            </div>
                            <!--show others-->
                            <div class="photo-upload-input-wrp shadow bbox hidden">
                                <div class="special-border bbox"></div>
                                <div class="after-special-border bbox">
                                    <button class="mini-cross-btn cancel-upload-btn bbox"></button>
                                    <form class="photo-upload-form" action="">
                                        <input name="IsLocalFile" class="photo-upload-choice bbox" type="radio" checked="checked" value="0"/><span class="photo-upload-choice-text txt-sml">from a web link</span>
                                        <input name="IsLocalFile" class="photo-upload-choice bbox" type="radio" value="1" /><span class="photo-upload-choice-text txt-sml">from a local file</span>
                                        <input class="photo-upload-input photo-upload-input-url bbox" type="text" name="WebFileUrl"/>
                                        <input class="photo-upload-input photo-upload-input-file bbox hidden" type="file" name="LocalFile" />
                                        <button class="btn-soft photo-upload-do-btn bbox">upload</button>
                                        <div class="clear"></div>
                                        <div class="upload-result txt-sml clr-txt-flu bbox hidden">
                                            <span class="upload-result-err"></span>
                                        </div>
                                    </form>
                                </div>
                            </div>
                        </div>
                        <button class="entry-add-btn act rnd-crn txt-med">+ add</button>
                    </div>
                </div>
                <div class="clear"></div>
            </div>

            <!-- preview -->
            <div class="main-block entry-preview-block">
                <div class="mb-aux1">                    
                </div>
                <div class="mb-aux2">                    
                </div>
                <div class="mb-inner">
                    <div class="entry entry-preview txt-lrg hidden">
                    </div>
                </div>
                <div class="clear"></div>
            </div>

            <div class="main-block spinner-block hidden">
                <div class="mb-inner spinner-block-inner">
                    <img class="content-spinner" alt="loading entries..." src="<%=Request.ApplicationPath.TrimEnd('/') + "/Images/spinner-big.gif" %>" />
                    <br/>
                    <span class="txt-sml clr-txt-flu">loading entries</span>
                </div>
                <div class="clear"></div>
            </div>
            
            <!-- add yours -->
            <div class="main-block entry-add last-add hidden">
                <div class="mb-aux1">                    
                </div>
                <div class="mb-aux2">
                </div>
                <div class="mb-inner brdT">
                    <div class="last-add-inn brdT brdB">
                        <div class="entry-add-wrp">
                            <div class="entry-add-inp-wrp">
                                <textarea class="entry-add-inp-dsc txt-lrg bbox watermark-helper" cols="100%" rows="1" disabled="disabled"></textarea>
                                <textarea class="entry-add-inp txt-lrg bbox watermark-main semi-trans" cols="100%" rows="1" ></textarea>
                            </div>
                        </div>
                        <div class="entry-add-btn-wrp">
                            <div class="photo-upload-section">
                                <div class="photo-upload-start-wrp">
                                    <button class="photo-upload-start-btn"></button>
                                </div>
                                <div class="photo-attach-list">
                                </div>
                                <!--show others-->
                                <div class="photo-upload-input-wrp shadow bbox hidden">
                                    <div class="special-border bbox"></div>
                                    <div class="after-special-border bbox">
                                        <button class="mini-cross-btn cancel-upload-btn bbox"></button>
                                        <form class="photo-upload-form" action="">
                                            <input name="IsLocalFile" class="photo-upload-choice bbox" type="radio" checked="checked" value="0"/><span class="photo-upload-choice-text txt-sml">from a web link</span>
                                            <input name="IsLocalFile" class="photo-upload-choice bbox" type="radio" value="1" /><span class="photo-upload-choice-text txt-sml">from a local file</span>
                                            <input class="photo-upload-input photo-upload-input-url bbox" type="text" name="WebFileUrl"/>
                                            <input class="photo-upload-input photo-upload-input-file bbox hidden" type="file" name="LocalFile" />
                                            <button class="btn-soft photo-upload-do-btn bbox">upload</button>
                                            <div class="clear"></div>
                                            <div class="upload-result txt-sml clr-txt-flu bbox hidden">
                                                <span class="upload-result-err"></span>
                                            </div>
                                        </form>
                                    </div>
                                </div>
                            </div>
                            <button class="entry-add-btn act txt-med">+ add</button>
                        </div>
                    </div>
                </div>
                <div class="clear"></div>
            </div>

            <!-- preview -->
            <div class="main-block entry-preview-block hidden">
                <div class="mb-aux1">                    
                </div>
                <div class="mb-aux2">                    
                </div>
                <div class="mb-inner">
                    <div class="entry entry-preview txt-lrg hidden">
                    </div>
                </div>
                <div class="clear"></div>
            </div>

            <!-- invite friends -->
            <div class="main-block invite-block">
                <div class="mb-aux1"></div>
                <div class="mb-aux2"></div>
                <div class="mb-inner">
                    <div id="topicInvite" class="topic-invite topic-invite-big hidden">
	                    <div class="invite-friends-to-topic txt-lrx3 rnd-crn-big">Invite your friends to the discussion!</div>
                    </div>
                </div>
                <div class="clear"></div>
            </div>

        </div>
        <div id="helper" class="helper">
            <div class="helper-inner">
                <div id="relatedTopicsTitle" class="related-word txt-xlr txt-subt hidden">Recommended</div>
                <ul class="related">
                </ul>
                <div id="topicInviteSide" class="topic-invite topic-invite-side brdT brdB bkg1 hidden">
                    <div class="invite-friends-side-link">Invite Friends to Vizibuzz</div>
                </div>
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

        <div class="dialog install-dialog strong-modal hidden">
            <span class="dialog-close-button box-shadow-big encircle"><span>x</span></span>
            <div class="dialog-content install-dialog-content">
                <div class="install-dialog-top brdB">
                    <div class="logo-big txt-logo-big table-cell-wrapper"><div class="logo-inner-big">vizibuzz</div></div>
                    <div class="table-cell-wrapper intall-dialog-curiosity-wrp">
                        <span class="intall-dialog-curiosity clr-txt-reg split-intall-dialog-curiosity">What are your friends talking about??</span>
                    </div>
                </div>
                <div class="install-dialog-mid">
                    <div class="table-cell-wrapper">
                        <img class="pg13-img split-pg13-img" alt="PG-13" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Images/pg13.gif" %>" />
                    </div>
                    <div class="pg13-txt-wrp table-cell-wrapper txt-sml clr-txt-reg split-pg13-txt-wrp">
                        <span class="pg-13-warning desktop-only split-pg-13-warning-desktop">Are you over 13?<br/>This topic maynot be suitable for children. Please login to verify your age.</span>
                        <span class="pg-13-warning mobile-only split-pg-13-warning-mobile">Are you over 13? Login to verify.</span>
                    </div>
                </div>
                <div>
                    <div class="table-cell-wrapper signup-modal-install-wrp-left"></div>
                    <div class="table-cell-wrapper signup-modal-install-wrp">
                        <button class="signup signup-modal signup-modal-install rnd-crn-big txt-lrx act">
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

        <div class="photo-attach-wrp txt-sml clr-txt-flu">
            <div class="photo-attach-view-id hidden"></div>
            <div class="photo-attach-id hidden"></div>
            <div class="photo-attach-url hidden"></div>
            <img class="photo-attach-img" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Images/attach.png" %>" alt="attachment"/>
            <span class="photo-attach-txt"></span>
            <button class="mini-cross-btn remove-file-btn bbox"></button>
        </div>

        <div class="entry-react-list">
            <asp:Repeater id="reactionTypeRepeater" runat="server">
	            <ItemTemplate>
		        <button class="btn-togl-soft entry-react txt-sml" value='<%#DataBinder.Eval(Container.DataItem, "Id")%>'><%#DataBinder.Eval(Container.DataItem, "Name")%></button>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <div class="main-block entry-block">
            <div class="mb-aux1 entry-aux1">
                <button class="btn-togl-excl-soft vote txt-med" title="vote up" value="1">+1</button><br/>
                <button class="btn-togl-excl-soft vote txt-med" title="vote down" value="-1">-1</button><br/>

                <button class="btn-share-entry-facebook rnd-crn desktop-only" title="share this entry on Facebook">
                    <img class="share-entry-facebook-img" alt="F" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Images/facebook-small.png" %>" />
                </button><br class="desktop-only"/>
                
                <button class="btn-share-entry-twitter rnd-crn desktop-only" title="share this entry on Twitter">
                    <img class="share-entry-twitter-img" alt="T" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Images/twitter-small.png" %>" />
                </button><br class="desktop-only"/>
                
                <button class="entry-change-mini-btn entry-edit-mini-btn txt-med rnd-crn mobile-only hidden" title="edit this entry">...</button>
                <button class="entry-change-mini-btn entry-delete-mini-btn txt-med rnd-crn mobile-only hidden" title="delete this entry">x</button>
            </div>
            <div class="mb-aux2 entry-aux2">
                <a href="" class="user-img-link">
                    <img class="user-img act brd" alt="" src="" title="see more info about this writer"/><br/>
                </a>
                
                <button class="btn-share-entry-facebook btn-share-entry-fb2 rnd-crn mobile-only" title="share this entry on Facebook">
                    <img class="share-entry-facebook-img" alt="F" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Images/facebook-small.png" %>" />
                </button>
                
                <button class="btn-share-entry-twitter btn-share-entry-tw2 rnd-crn mobile-only" title="share this entry on Twitter">
                    <img class="share-entry-twitter-img" alt="T" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Images/twitter-small.png" %>" />
                </button>
                
                <button class="entry-change-btn entry-edit-btn desktop-only hidden" title="edit this entry">edit<br/>entry</button>
                <button class="entry-change-btn entry-delete-btn desktop-only hidden" title="delete this entry">
                    delete<br/>entry
                </button>
            </div>
            <div class="mb-inner mb-inner-entry brdT brdB">
                <div class="entry">
                    <div class="entry-id hidden"></div>
                    <div class="vote-result txt-sml clr-txt-flu">
                        <span class="data-all-current"></span>
                        <span class="data-me-current">You haven't voted yet</span>
                        <img class="act-busy hidden" alt="..." src="<%=Request.ApplicationPath.TrimEnd('/') + "/Images/spinner.gif" %>" />
                        <span class="act-busy hidden">updating...</span>
                        <span class="act-done hidden">Sent successfully!</span>
                    </div>
                    <div class="reactions-result txt-sml clr-txt-flu">
                        <span class="data-all-current all-reactions"></span>
                        <span class="act-busy hidden right">&nbsp;updating...</span>
                        <img class="act-busy hidden right" alt="..." src="<%=Request.ApplicationPath.TrimEnd('/') + "/Images/spinner.gif" %>" />
                        <span class="data-me-current right"></span>
                        <span class="act-done hidden right">Sent successfully!</span>
                    </div>
                </div>
            </div>
            <div class="clear"></div>
        </div>

        <!-- Element to run dialogue -->
        <div id="customFacebookDialog"> 
            <div id="facebookSelectAllFriendsArea">
                <input id="facebookFriendsSelectAllCBox" type='checkbox' name='selectallfriends' value='0' />
                <img alt='&larr;' class="desktop-only" id='facebookFriendsSelectAllImg' src="<%=Request.ApplicationPath.TrimEnd('/') + "/Images/left_arrow.png" %>" />
                <div id="facebookFriendsSelectAllLabel" >Select All</div>
                <div id="facebookFriendsFilterPane">
                    <div id="facebookFriendsShowLabel">Show: </div>
                    <div id="facebookFriendsShowAll" class="facebookFriendsCurrentFilter">All</div>
                    <div id="facebookFriendsShowOnlySelecteds">Selected</div>
                </div>
            </div>
            <div id="facebookFriendSearchArea">
                <div id="facebookFriendSearchWaterMark">
                    <input id="facebookFriendSearchDescr" type="text" value="Search..." />
                    <input id="facebookFriendSearchTextBox" type="text" class="ffs-watermark"/>
                </div>
            </div>
            <div id="facebookFriendSelectArea"></div>
        </div>

    </div>
    <div class="popup-content hidden">
        <div class="compare-entries-dlg strong-modal clr-txt-reg">
            <div id="entryComparisonDialogContentIsValid" runat="server" class="hidden">0</div>
            <span class="dialog-close-button box-shadow-big encircle"><span>x</span></span>
            <div class="dialog-content compare-entries-dlg-content rnd-crn-big">
                <div class="comparison-title-block brdB">
                    <div class="comparison-title-block-inner">
                        <div class="table-cell-wrapper">
                            <img class="comparison-invite-sender-img hidden" alt='' src='' />
                        </div>
                        <div class="table-cell-wrapper">
                            <div class="comparison-title txt-title txt-lrx3">Help us to pick the BETTER one</div>
                        </div>
                    </div>
                </div>
                <div class="main-block entry-block entry-block-compare entry-block-compare-left">
                    <div class="topic-word txt-xlr txt-title topic-word-comparison">
                        <span id="randomTopicId1" class="random-topic-id hidden" runat="server"></span>
                        <span id="randomTopicName1" class="random-topic" runat="server"></span>
                    </div>
                    <div class="entry-block-compare-inner entry-block-compare-inner-left">
                        <div class="mb-aux1 entry-aux1 entry-aux1-compare">
                            <button class="btn-soft vote vote-compare txt-med" title="vote up" value="1">+1</button><br/>
                        </div>
                        <div class="mb-inner mb-inner-compare">
                            <div id="randomEntryId1" runat="server" class="entry-id hidden"></div>
                            <div id='randomEntryContent1' runat="server" class="entry entry-compare">
                            </div>
                        </div>
                    </div>
                </div>
                <div class="main-block entry-block entry-block-compare entry-block-compare-right">
                    <div class="topic-word txt-xlr txt-title topic-word-comparison">
                        <span id="randomTopicId2" class="random-topic-id hidden" runat="server"></span>
                        <span id="randomTopicName2" class="random-topic" runat="server"></span>
                    </div>
                    <div class="entry-block-compare-inner entry-block-compare-inner-right">
                        <div class="mb-aux1 entry-aux1 entry-aux1-compare">
                            <button class="btn-soft vote vote-compare txt-med" title="vote up" value="1">+1</button><br/>
                        </div>
                        <div class="mb-inner mb-inner-compare">
                            <div id="randomEntryId2" runat="server" class="entry-id hidden"></div>
                            <div id='randomEntryContent2' runat="server" class="entry entry-compare">
                            </div>
                        </div>
                    </div>
                </div>
                <div class="clear"></div>
            </div>
        </div>
    </div>

    <!-- this helps the rest of the UI that is generated by scripts behaves well -->
    <!-- for example: custom facebook invite dialog. otherwise they suffer from previous floats -->
    <div class="clear"></div>
</body>
</html>