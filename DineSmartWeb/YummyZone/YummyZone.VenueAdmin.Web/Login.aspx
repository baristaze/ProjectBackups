<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="YummyZone.VenueAdmin.Web.Login" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">

<head >
    
    <title>Dine Smart 365 - Login</title>
    
    <link href="Styles/Site_0_Common.css?version=1.05" rel="stylesheet" type="text/css" />
    <link href="Styles/Site_1_PublicBrief.css?version=1.04" rel="stylesheet" type="text/css" />

    <script type="text/javascript" src="Scripts/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery.form.js"></script>
    <script type="text/javascript" src="Scripts/common1.js?version=1.04"></script>
    <script type="text/javascript" src="Scripts/common2.js?version=1.04"></script>
    <script type="text/javascript" src="Scripts/SingleActHelper.js?version=1.04"></script>
    <script type="text/javascript" src="Scripts/Login.js?version=1.04"></script>

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

        <div class="main">      

            <div class="identityLogoMenuBkg">
                <div class="mainMenuStrip">
                    <div class="logoWrp">
                        <img alt="Dine Smart 365" class="logoImg" src="Images/logo.png" />
                    </div>
                    <div class="menuWrpr">
                        <div class="mainMenuRightInner genericTitle condensedFont">
                            <div class="menuItem"><a href="Default.aspx" class="menuItemLink">Home</a></div>
                            <div class="menuItemSep" runat="server" id="menuSep"><img alt="|" class="menuSepImg" src="Images/menuDivider.png" /></div>
                            <div class="menuItem selectedLink"><a href="javascript:void(0);" class="menuItemLink">Login</a></div>
                        </div>
                    </div>    
                </div>
            </div>

            <div class="hidden">
                <div id="emailHint" runat="server"></div>
                <div id="errorHint" runat="server"></div>
            </div>

            <form class="hidden" runat="server" action="Login.aspx" id="first">
                <input id="userEmail2" class="editBox secInput" type="text" maxlength="200" runat="server"/>
                <input id="userPassword2" class="editBox secInput" type="password" maxlength="100" runat="server"/>
                <input id="submitButton" type="button" value="button" runat="server" onserverclick="HiddenButtonClicked"/>
            </form>

            <form action="UserHandlers/Authenticate.ashx" id="second">

            <div class="subSection">
                <div class="subSectHeader genericTitle condensedFont">
                    <div class="preHeaderText"></div>
                    <div class="subSectHeadText"></div>
                </div>
                <div class="subSectBody firstSectBody">
                    <div class="firstLine"></div>
                    <div class="inputLine">
                        <div class="labelWrp">
                            <span class="secInputLabel">Email Address:</span>
                        </div>
                        <div class="inputWrp">
                            <input id="userEmail" class="editBox secInput userEmail" type="text" maxlength="200" onkeydown="enterKeyDown(event, singleActionPostEx, hideError)" />
                        </div>
                    </div>
                    <div class="clear"></div>
                    <div class="inputLine">
                        <div class="labelWrp">
                            <span class="secInputLabel">Password :</span>
                        </div>
                        <div class="inputWrp">
                            <input id="userPassword" class="editBox secInput" type="password" maxlength="100" onkeydown="enterKeyDown(event, singleActionPostEx, hideError)" />
                        </div>
                    </div>
                    <div class="clear"></div>
                </div>
                <div class="subSectBottom">
                </div>
            </div>

            </form>

            <div id="singleGlsActSect" class="singleGlsActBtnWrp">
                <div class="errorStrip hidden">
                   <div class="errorText"></div>
                </div>
                <div class="singleGlsActBtn">
                    <div class="singleGlsActTxt condensedFont">
                        <a href="javascript:void(0);" class="singleGlsActLink">
                            Login
                        </a>
                        <img class="spinner hidden" alt="..." src="/Images/spinner.gif" />
                    </div>
                </div>
            </div>

        </div>
        
        <!-- below line is NOT a comment but a real CODE -->
        <!--#include file="footer.html"-->
        <!-- DO NOT DELETE ABOVE LINE -->

        <div class="clear"></div>
    </div>
</body>
</html>