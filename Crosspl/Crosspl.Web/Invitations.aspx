<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Invitations.aspx.cs" Inherits="Crosspl.Web.Invitations" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Invite Friends</title>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.9.2/jquery-ui.min.js"></script> 
    <script type="text/javascript" src="http://connect.facebook.net/en_US/all.js"></script>
    <script type="text/javascript" src="Scripts/jquery.form.min.js"></script>
    <script type="text/javascript" src="Scripts/Common.debug.js"></script>
    <script type="text/javascript" src="Scripts/Url.debug.js"></script>
    <script type="text/javascript" src="Scripts/Dialog.debug.js"></script>
    
    <script type="text/javascript">
        var _gaq = _gaq || [];
        _gaq.push(['_setAccount', 'UA-37929491-1']);
        _gaq.push(['_trackPageview']);

        (function () {
            var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
            ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
            var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
        })();
    </script>

    <style type="text/css" media="all">
        html { height:100%; margin:0 0;padding:0 0; }
        body { height:100%; margin:0 0;padding:0 0; background-color:White;font-family: Helvetica, Arial, Verdana, sans-serif;}
        .theCenter { height:100%;}
        .bodyInner{ height:100%;}
        .topBarStrip{height:60px; width:100%; background-color:Black;}
        .topBarWrp{position:absolute;height:60px;left:50%; margin-left:-512px; width:1024px; }
        .topBarLeftWrp{float:left;width:400px;text-align:left;height:60px;}
        .topBarRightWrp{float:right;width:624px;text-align:right;height:60px;}
        .topBarLeft{height:60px;}
        .topBarRight{height:60px;}
        .mainContentWrp{position:absolute;top:60px; bottom:0px; left:50%; margin-left:-512px; right:0px; overflow-y:auto; width:1024px;}
        .logoWrp {text-align:left; height:60px; width:160px; background-color:#be004c; color:White;font-size:36px; font-weight:bold;}
        .companyName{padding:6px 0px 0px 0px; text-align:center;}
        .loginInfoWrp{display:inline-block; color:White;margin-top:20px;}
        .loginInfo{padding:0px; margin:0px; vertical-align:middle;}
        .logout{color:Orange;}
        .logout:link, .logout:visited{color: Orange;text-decoration: none;}
        .logout:hover{color:Orange;text-decoration:none;}
        .logout:active{color:Orange;}
        /*.foo{display:inline-block;color:White;}*/
    </style>

</head>
<body>
    <center class="theCenter">
        <div class="bodyInner">
            <div class="topBarStrip">
                <div class="topBarWrp">
                    <div class="topBarLeftWrp">
                        <div class="topBarLeft">
                            <div class="logoWrp">
                                <div class="companyName">Vizibuzz</div>
                            </div>
                        </div>
                    </div>
                    <div class="topBarRightWrp">
                        <div class="topBarRight">
                            <!--
                            <div class="foo">foo</div>
                            -->
                            <div class="loginInfoWrp">
                                <p class="loginInfo">
                                    Welcome <span class="userName"></span>!
                                    <a href="javascript:void(0);" class="logout">logout</a>
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="mainContentWrp">
                hello there
            </div>
        </div>
    </center>
</body>
</html>
