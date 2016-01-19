<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FBCanvas.aspx.cs" Inherits="Shebeke.Web.FBCanvas" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Shebeke</title>
    <style type="text/css">        
        html {margin:0 0;padding:0 0;}
        body{margin:0 0;padding:0 0;font-family: Helvetica, Arial, Verdana, sans-serif; line-height:120%;}
        
        .content{margin-top:40px;width:640px;height:100px;background-color:#efefef;}
        .table-cell-wrapper{display:table-cell; vertical-align:middle;}
                
        .logo{width:190px;height:100px;background-color:#be004c; color:White;font-size:32px;font-weight:bold;text-align:center;}
        .logo-inner{padding-top:35px;}
        
        .explain{width:450px;height:100px;text-align:left;display:inline-block;}
        .explain-inner{padding:12px;}
        .hidden{display:none;}
        
        #redirectLink2{display:none;}
    </style>
    <style type="text/css">
        @-moz-document url-prefix() 
        {
            #redirectLink{display:none;}
            #redirectLink2{display:inline;}
        }
    </style>
</head>
<body>  
    <div id="splitResources" runat="server" class="splitResources hidden"></div>      
    <center>
        <div id="applicationPath" class="hidden" runat="server"></div>
        <div id="isAuthenticated" class="hidden" runat="server">0</div>        
        <div class="content">

            <div class="table-cell-wrapper">
                <div class="logo">
                    <div class="logo-inner">shebeke</div>
                </div>
            </div>

            <div class="table-cell-wrapper">
                <div class="explain">
                    <div class="explain-inner">
                        Shebeke, sadece 13 yaş ve üzeri kullanıcılarına hizmet vermektedir. Shebeke üzerindeki içerikten, ekleyen kişiler sorumludur. Kabul ediyorsanız, lütfen şu bağlantıdan <b>devam edin</b>: <a runat="server" target="_top" id="redirectLink" href="http://shebeke.net">http://shebeke.net</a><a runat="server" target="_blank" id="redirectLink2" href="http://shebeke.net">http://shebeke.net</a>
                    </div>                    
                </div>
            </div>
                        
        </div>

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

    </center>
</body>
</html>
