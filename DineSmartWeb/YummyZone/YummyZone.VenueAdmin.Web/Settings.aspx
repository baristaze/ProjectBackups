<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Settings.aspx.cs" Inherits="YummyZone.VenueAdmin.Web.Settings" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Dine Smart 365 - Settings</title>
    
    <link href="Styles/Site_0_Common.css?version=1.05" rel="stylesheet" type="text/css" />
    <link href="Styles/Settings.css?version=1.04" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.12.custom.css" rel="stylesheet" type="text/css" />

    <script type="text/javascript" src="Scripts/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui-1.8.12.custom.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery.form.js"></script>
    <script type="text/javascript" src="Scripts/jquery.scrollTo-min.js"></script>    
    <script type="text/javascript" src="Scripts/common2.js?version=1.04"></script>
    <script type="text/javascript" src="Scripts/Settings.js?version=1.04"></script>
    <script type="text/javascript" src="Scripts/MultiBranch.js?version=1.00"></script>

    <style type="text/css" media="screen">
        .ui-autocomplete{background-color:#ffffee; width:255px;margin:0px;padding:0px;list-style: none;border:1px solid #ccc;padding:1px 0px;}
        .ui-menu-item{width:100%;margin:1px 4px;}
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
        
        <!-- below line is NOT a comment but a real CODE -->
        <!--#include file="header.html"-->
        <!-- DO NOT DELETE ABOVE LINE -->
        
        <div class="main">
        
            <div class="hiddenParts hidden">

                <div id="confirmDeletingUserDlg" >
                    <p>Are you sure you want<br/>to delete this user?</p>
                </div>

                <!-- below line is NOT a comment but a real CODE -->
                <!--#include file="mBrnch.html"-->
                <!-- DO NOT DELETE ABOVE LINE -->

                <div class="memberRow">
                    <div class="memberColumnWrp memberColumnLeft">
                        <div class="memberColumnInner memberNameWrp">
                            <span class="memberFirstN"></span>
                            <span class="memberLastN"></span>
                        </div>
                    </div>
                    <div class="memberColumnWrp memberColumnLeft2">
                        <div class="memberColumnInner memberEmailWrp">
                            <span class="memberEmail"></span>
                            <span class="memberStatus">(disabled)</span>
                        </div>
                    </div>
                    <div class="memberColumnWrp memberColumnRight">
                        <div class="memberColumnInner memberActions">
                            <a href="javascript:void(0);" class="memberAction memberActDisable">disable</a>
                            <a href="javascript:void(0);" class="memberAction memberActEnable">enable</a>
                            <a href="javascript:void(0);" class="memberAction memberActDelete">delete</a>
                            &nbsp;
                        </div>
                    </div>
                    <div class="clear"></div>
                </div>
            </div>

            <center>
                <div class="settingsSectionWrp">
                    <div class="settingsSectionHeader">
                        <div class="settingsSectionTitle">Password</div>
                    </div>
                    <div class="settingsSectionContent pswdSectionContent">
                        <div class="currentUserWrp">
                            <div class="pairLine">
                                <div class="pairLineLeftWrp">
                                    <div class="pairLineLeft">Your email: </div>
                                </div>
                                <div class="pairLineRightWrp">
                                    <div class="pairLineRight userNameLineRight">
                                        <span id="currentUserName" class="currentUserName" runat="server"></span>
                                        <a href="javascript:void(0);" class="passwordChangeLink">change password</a>
                                    </div>
                                </div>                            
                                <div class="clear"></div>
                            </div>
                        </div>
                        <form id="changePswdForm" class="hidden">
                            <div class="pairLine">
                                <div class="pairLineLeftWrp">
                                    <div class="pairLineLeft">Current Password: </div>
                                </div>
                                <div class="pairLineRightWrp">
                                    <div class="pairLineRight">
                                        <input id="currentPassword" class="editBox1 inputText" type="password" maxlength="100" runat="server"/>
                                    </div>
                                </div>                            
                                <div class="clear"></div>
                            </div>
                            <div class="pairLine">
                                <div class="pairLineLeftWrp">
                                    <div class="pairLineLeft">New Password: </div>
                                </div>
                                <div class="pairLineRightWrp">
                                    <div class="pairLineRight">
                                        <input id="newPassword1" class="editBox1 inputText" type="password" maxlength="100" runat="server"/>
                                    </div>
                                </div>                            
                                <div class="clear"></div>
                            </div>
                            <div class="pairLine">
                                <div class="pairLineLeftWrp">
                                    <div class="pairLineLeft">New Password (again): </div>
                                </div>
                                <div class="pairLineRightWrp">
                                    <div class="pairLineRight">
                                        <input id="newPassword2" class="editBox1 inputText" type="password" maxlength="100" runat="server"/>
                                    </div>
                                </div>                            
                                <div class="clear"></div>
                            </div>
                            <div class="pairLine">
                                <div class="pairLineLeftWrp">
                                    <div class="pairLineLeft errorStripWrp">
                                        <div class="errorStrip">
                                           <div class="errorText inline"></div>
                                        </div>
                                    </div>
                                </div>
                                <div class="pairLineRightWrp">
                                    <div class="pairLineRight">
                                        <input class="changePasswordButton" type="button" value="change password" />
                                        <a href="javascript:void(0);" class="cancelPasswordChangeLink">cancel</a>
                                    </div>
                                </div>                            
                                <div class="clear"></div>
                            </div>
                        </form>
                    </div>
                </div>

                <div class="settingsSectionWrp">
                    <div class="settingsSectionHeader">
                        <div class="settingsSectionTitle">Logo</div>
                    </div>
                    <div class="settingsSectionContent">
                        <form id="changeLogoForm">                        
                            <div class="pairLine">
                                <div class="pairLineLeftWrp">
                                    <div class="pairLineLeft">
                                        <img id="customerLogoImg" class="customerLogoImg" alt="logo" src="Images/venueDefaultImage.png" runat="server" />
                                    </div>
                                </div>
                                <div class="pairLineRightWrp">
                                    <div class="pairLineRight imageInputWrp">
                                        <div class="imagePair">
                                            <input class="imageSourceRadio" type="radio" name="imageSource" value="1" /><span class="imageSourceRadioText">upload new logo from a web link</span><br/>
                                            <input class="imageSourceRadio" type="radio" name="imageSource" value="2" /><span class="imageSourceRadioText">upload new logo from a local file</span>
                                            <input class="imageInput inputText fromWebLink" type="text" maxlength="500"/>
                                            <input class="imageInput inputText fromLocalFile" type="file" name="imageFileFromLocal" />
                                        </div>
                                        <div>
                                            <input class="changeLogoButton" type="button" value="upload new logo" />
                                            <a href="javascript:void(0);" class="cancelLogoChangeLink">cancel</a>
                                        </div>
                                    </div>
                                </div>                            
                                <div class="clear"></div>
                            </div>
                        </form>
                    </div>
                </div>

                <div class="settingsSectionWrp">
                    <div class="settingsSectionHeader">
                        <div class="settingsSectionTitle">Users</div>
                    </div>
                    <div class="settingsSectionContent">
                        <div class="membersTableWrp">
                            <asp:Repeater id="memberRepeater" runat="server">
                                <ItemTemplate>                                                     
                                    <div class="memberRow">
                                        <div class="memberColumnWrp memberColumnLeft">
                                            <div class="memberColumnInner memberNameWrp">
                                                <span class="memberFirstN"><%#DataBinder.Eval(Container.DataItem, "FirstName")%></span>
                                                <span class="memberLastN"><%#DataBinder.Eval(Container.DataItem, "LastName")%></span>
                                            </div>
                                        </div>
                                        <div class="memberColumnWrp memberColumnLeft2">
                                            <div class="memberColumnInner memberEmailWrp">
                                                <span class="memberEmail"><%#DataBinder.Eval(Container.DataItem, "EmailAddress")%></span>
                                                <span class="memberStatus <%#GetShowHideCssClass(DataBinder.Eval(Container.DataItem, "IsDisabled"))%>">(disabled)</span>
                                            </div>
                                        </div>
                                        <div class="memberColumnWrp memberColumnRight">
                                            <div class="memberColumnInner memberActions">
                                                <a href="javascript:void(0);" class="memberAction memberActDisable <%#GetShowHideCssClass(DataBinder.Eval(Container.DataItem, "CanDisable"))%>">disable</a>
                                                <a href="javascript:void(0);" class="memberAction memberActEnable <%#GetShowHideCssClass(DataBinder.Eval(Container.DataItem, "CanEnable"))%>">enable</a>
                                                <a href="javascript:void(0);" class="memberAction memberActDelete <%#GetShowHideCssClass(DataBinder.Eval(Container.DataItem, "CanDelete"))%>">delete</a>
                                                &nbsp;
                                            </div>
                                        </div>
                                        <div class="clear"></div>
                                    </div>  
                                </ItemTemplate>              
                            </asp:Repeater>
                        </div>
                        <div class="newMemberArea">
                            <div class="addNewWrp">
                                <input class="addNewMemberBtn" type="button" value="add new user" />
                                &nbsp;
                            </div>
                            <div class="clear"></div>
                            <form id="newUserForm" class="hidden">
                                <div class="pairLine">
                                    <div class="pairLineLeftWrp">
                                        <div class="pairLineLeft">Email Address: </div>
                                    </div>
                                    <div class="pairLineRightWrp">
                                        <div class="pairLineRight">
                                            <input id="memberEmailAddress" class="editBox2 inputText" type="text" maxlength="100" runat="server"/>
                                            <span class="inputHint existingUserDesc hidden">(existing user)</span>
                                        </div>
                                    </div>                            
                                    <div class="clear"></div>
                                </div>                             
                                <div class="pairLine forNewUser">
                                    <div class="pairLineLeftWrp">
                                        <div class="pairLineLeft">First Name: </div>
                                    </div>
                                    <div class="pairLineRightWrp">
                                        <div class="pairLineRight">
                                            <input id="memberFirstName" class="editBox2 inputText" type="text" maxlength="100" runat="server"/>
                                        </div>
                                    </div>                            
                                    <div class="clear"></div>
                                </div>
                                <div class="pairLine forNewUser">
                                    <div class="pairLineLeftWrp">
                                        <div class="pairLineLeft">Last Name: </div>
                                    </div>
                                    <div class="pairLineRightWrp">
                                        <div class="pairLineRight">
                                            <input id="memberLastName" class="editBox2 inputText" type="text" maxlength="100" runat="server"/>
                                        </div>
                                    </div>                            
                                    <div class="clear"></div>
                                </div>
                                <div class="pairLine forNewUser">
                                    <div class="pairLineLeftWrp">
                                        <div class="pairLineLeft">Phone Number: </div>
                                    </div>
                                    <div class="pairLineRightWrp">
                                        <div class="pairLineRight">
                                            <input id="memberPhone" class="editBox2 inputText" type="text" maxlength="100" runat="server"/>
                                            <span class="inputHint">(optional)</span>
                                        </div>
                                    </div>                            
                                    <div class="clear"></div>
                                </div>
                                <div class="pairLine forNewUser">
                                    <div class="pairLineLeftWrp">
                                        <div class="pairLineLeft">Temporary Password: </div>
                                    </div>
                                    <div class="pairLineRightWrp">
                                        <div class="pairLineRight">
                                            <input id="memberPassword" class="editBox2 inputText" type="text" maxlength="100" runat="server"/>
                                            <span class="inputHint">(feel free to change)</span>
                                        </div>
                                    </div>                            
                                    <div class="clear"></div>
                                </div>
                                <div class="pairLine">
                                    <div class="pairLineLeftWrp">
                                        <div class="pairLineLeft errorStripWrp2">
                                            <div class="errorStrip2">
                                               <div class="errorText2 inline"></div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="pairLineRightWrp">
                                        <div class="pairLineRight">
                                            <input class="saveUserButton" type="button" value="create user" />
                                            <a href="javascript:void(0);" class="cancelNewMemberLink">cancel</a>
                                        </div>
                                    </div>                            
                                    <div class="clear"></div>
                                </div>
                            </form>
                        </div>
                    </div>
                </div>
            </center>
        </div>

        <div class="clear">
        </div>
    </div>

</body>
</html>
