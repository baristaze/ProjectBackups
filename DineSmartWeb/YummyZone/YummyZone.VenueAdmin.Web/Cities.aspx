<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Cities.aspx.cs" Inherits="YummyZone.VenueAdmin.Web.Cities" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">

<head >
    
    <title>Dine Smart 365 - Cities</title>
    
    <link href="Styles/Site_0_Common.css?version=1.05" rel="stylesheet" type="text/css" />
    <link href="Styles/Site_1_PublicBrief.css?version=1.04" rel="stylesheet" type="text/css" />
    <link href="Styles/Site_1_PublicBrief2.css" rel="stylesheet" type="text/css" />

    <script type="text/javascript" src="Scripts/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery.form.js"></script>
    <script type="text/javascript" src="Scripts/common1.js?version=1.04"></script>
    <script type="text/javascript" src="Scripts/common2.js?version=1.04"></script>

    <style type="text/css" media="screen">
        .menuItem{display:none;}
        #homeMenu{display:inline;}
        #citiesMenu{display:inline;}
        .subSection{margin-top:60px}
        .subSectBodyText{overflow:auto;overflow-y:auto;overflow-x:hidden;}
        .city{margin-left:20px;margin-top:20px;}
        .cityItem{margin-top:10px;}
    </style>

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
        
            <!-- below line is NOT a comment but a real CODE -->
            <!--#include file="header2.html"-->
            <!-- DO NOT DELETE ABOVE LINE -->

            <div class="subSection">
                <div class="subSectHeader genericTitle condensedFont">
                    <div class="preHeaderText"></div>
                    <div class="subSectHeadText">Cities</div>
                </div>
                <div class="subSectBody firstSectBody">
                    <div class="firstLine"></div>
                    <div class="subSectBodyText">
                        <div class="city">
                            <ul>
                                <asp:Repeater id="cityRepeater" runat="server">
                                    <ItemTemplate>
                                        <li class="cityItem"><%# Container.DataItem %></li>
                                    </ItemTemplate>
                                </asp:Repeater>                                
                            </ul>                            
                        </div>                        
                    </div>
                    <div class="clear"></div>
                </div>
                <div class="subSectBottom"></div>
            </div>
        
        </div>
        <!-- below line is NOT a comment but a real CODE -->
        <!--#include file="footer.html"-->
        <!-- DO NOT DELETE ABOVE LINE -->

        <div class="clear"></div>
    </div>
</body>
</html>


