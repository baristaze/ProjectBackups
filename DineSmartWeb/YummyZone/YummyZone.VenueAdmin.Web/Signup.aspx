<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Signup.aspx.cs" Inherits="YummyZone.VenueAdmin.Web.Signup" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">

<head >
    <title>Dine Smart 365 - Signup</title>
    
    <link href="Styles/Site_0_Common.css?version=1.05" rel="stylesheet" type="text/css" />
    <link href="Styles/Site_1_PublicBrief.css?version=1.04" rel="stylesheet" type="text/css" />

    <script type="text/javascript" src="Scripts/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery.form.js"></script>
    <script type="text/javascript" src="Scripts/common1.js?version=1.04"></script>
    <script type="text/javascript" src="Scripts/common2.js?version=1.04"></script>
    <script type="text/javascript" src="Scripts/SingleActHelper.js?version=1.04"></script>
    <script type="text/javascript" src="Scripts/Signup.js?version=1.04"></script>
        
    <style type="text/css" media="screen">
        .firstSectBody{height:170px;}
        .secondSectBody{height:200px;}
        .thirdSectBody{height:120px;}
        .thanksContent{margin:10px 20px;}
        .animationHelper{height:554px;}
        .firstLine{height:6px;}
        .state{width:60px; text-transform: uppercase;}        
        .zip{width:125px;}
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

            <div class="identityLogoMenuBkg">
                <div class="mainMenuStrip">
                    <div class="logoWrp">
                        <img alt="Dine Smart 365" class="logoImg" src="Images/logo.png" />
                    </div>
                    <div class="menuWrpr">
                        <div class="mainMenuRightInner genericTitle condensedFont">
                            <div class="menuItem"><a href="Default.aspx" class="menuItemLink">Home</a></div>
                            <div class="menuItemSep" runat="server" id="menuSep"><img alt="|" class="menuSepImg" src="Images/menuDivider.png" /></div>
                            <div class="menuItem selectedLink"><a href="javascript:void(0);" class="menuItemLink">Signup</a></div>
                        </div>
                    </div>    
                </div>
            </div>

            <form action="UserHandlers/RegisterVenue.ashx">

            <div class="subSection">
                <div class="subSectHeader genericTitle condensedFont">
                    <div class="preHeaderText"></div>
                    <div class="subSectHeadText">Restaurant Info</div>
                </div>
                <div class="subSectBody firstSectBody">
                    <div class="firstLine"></div>
                    <div class="inputLine">
                        <div class="labelWrp">
                            <span class="secInputLabel">Restaurant Name :</span>
                        </div>
                        <div class="inputWrp">
                            <input id="venueName" class="editBox secInput" type="text" maxlength="100" />
                        </div>
                    </div>
                    <div class="clear"></div>
                    <div class="inputLine">
                        <div class="labelWrp">
                            <span class="secInputLabel">Restaurant Address :</span>
                        </div>
                        <div class="inputWrp">
                            <input id="venueAddressLine1" class="editBox secInput" type="text" maxlength="300" />
                        </div>
                    </div>
                    <div class="clear"></div>
                    <div class="inputLine">
                        <div class="labelWrp">
                            <span class="secInputLabel">Address Line 2 :</span>
                        </div>
                        <div class="inputWrp">
                            <input id="venueAddressLine2" class="editBox secInput" type="text" maxlength="100" />
                        </div>
                    </div>
                    <div class="clear"></div>
                    <div class="inputLine">
                        <div class="labelWrp">
                            <span class="secInputLabel">City :</span>
                        </div>
                        <div class="inputWrp">
                            <input id="venueAddressCity" class="editBox secInput city" type="text" maxlength="50" />
                        </div>
                    </div>
                    <div class="clear"></div>
                    <div class="inputLine">
                        <div class="labelWrp">
                            <span class="secInputLabel">State / ZIP :</span>
                        </div>
                        <div class="inputWrp">
                            <input id="venueAddressState" class="editBox secInput state" type="text" maxlength="2" />
                            <input id="venueAddressZip" class="editBox secInput zip" type="text" maxlength="10" />
                        </div>
                    </div>
                    <div class="clear"></div>
                </div>
                <div class="subSectBottom">
                </div>
            </div>

            <div class="subSection">
                <div class="subSectHeader genericTitle condensedFont">
                    <div class="preHeaderText"></div>
                    <div class="subSectHeadText">User Info</div>
                </div>
                <div class="subSectBody secondSectBody">
                    <div class="firstLine"></div>
                    <div class="inputLine">
                        <div class="labelWrp">
                            <span class="secInputLabel">First Name :</span>
                        </div>
                        <div class="inputWrp">
                            <input id="userFirstName" class="editBox secInput" type="text" maxlength="100" />
                        </div>
                    </div>
                    <div class="clear"></div>
                    <div class="inputLine">
                        <div class="labelWrp">
                            <span class="secInputLabel">Last Name :</span>
                        </div>
                        <div class="inputWrp">
                            <input id="userLastName" class="editBox secInput" type="text" maxlength="100" />
                        </div>
                    </div>
                    <div class="clear"></div>
                     <div class="inputLine">
                        <div class="labelWrp">
                            <span class="secInputLabel">Phone Number :</span>
                        </div>
                        <div class="inputWrp">
                            <input id="userPhone" class="editBox secInput" type="text" maxlength="20" />
                        </div>
                    </div>
                    <div class="clear"></div>
                    <div class="inputLine">
                        <div class="labelWrp">
                            <span class="secInputLabel">Business Email :</span>
                        </div>
                        <div class="inputWrp">
                            <input id="userEmail" class="editBox secInput userEmail" type="text" maxlength="200" />
                        </div>
                    </div>
                    <div class="clear"></div>
                    <div class="inputLine">
                        <div class="labelWrp">
                            <span class="secInputLabel">Password :</span>
                        </div>
                        <div class="inputWrp">
                            <input id="userPassword" class="editBox secInput" type="password" maxlength="100" />
                        </div>
                    </div>
                    <div class="clear"></div>
                    <div class="inputLine">
                        <div class="labelWrp">
                            <span class="secInputLabel">Password (again) :</span>
                        </div>
                        <div class="inputWrp">
                            <input id="userPasswordRepeat" class="editBox secInput" type="password" maxlength="100" />
                        </div>
                    </div>
                    <div class="clear"></div>
                </div>
                <div class="subSectBottom">
                </div>
            </div>

            </form>

            <div id="animationHelper" class="animationHelper hidden"></div>

            <div id="thanks" class="subSection hidden">
                <div class="subSectHeader genericTitle condensedFont">
                    <div class="preHeaderText"></div>
                    <div class="subSectHeadText">Thank You!</div>
                </div>
                <div class="subSectBody thirdSectBody">
                    <div class="firstLine"></div>
                    <div class="thanksContent">
                        We have received your request successfully. 
                        We need to verify that you own this restaurant or you are one of the managers. 
                        We will send you an email once your request is confirmed. Please expect a delay in response. 
                        Thank you for your patience and understanding during this process.
                    </div>
                    <div class="clear"></div>
                </div>
                <div class="subSectBottom">
                </div>
            </div>

            <div id="signUpSect" class="singleGlsActBtnWrp">
                <div class="errorStrip hidden">
                   <div class="errorText"></div>
                </div>
                <div class="singleGlsActBtn">
                    <div class="singleGlsActTxt condensedFont">
                        <a href="javascript:void(0);" class="singleGlsActLink">
                            Sign Up
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


