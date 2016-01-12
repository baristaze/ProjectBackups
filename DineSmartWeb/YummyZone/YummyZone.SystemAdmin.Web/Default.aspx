<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="YummyZone.SystemAdmin.Web.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">

<head >
    
    <title>Dine Smart 365 - Management Portal</title>
    
    <link href="Styles/Site.css?version=1.04" rel="stylesheet" type="text/css" />
    <link href="Styles/Default.css?version=1.04" rel="stylesheet" type="text/css" />

    <script type="text/javascript" src="Scripts/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui-1.8.12.custom.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery.scrollTo-min.js"></script>
    <script type="text/javascript" src="Scripts/jquery.form.js"></script>
    <script type="text/javascript" src="Scripts/common.js?version=1.04"></script>
    <script type="text/javascript" src="Scripts/Default.js?version=0.97"></script>
    <script type="text/javascript" src="http://maps.googleapis.com/maps/api/js?key=AIzaSyCbAI153_r8vfKa_4ooXR0cVk_DiIopLmI&sensor=true"></script> 
    
</head>

<body>
    <div class="resources hidden">
        <div class="searchResultWrp">
            <div class="searchResultIdentity hidden">
                <div class="objectId inline"></div>
                <div class="objectType inline"></div>
                <div class="objectStatus inline"></div>
            </div>
            <div class="searchResultLeft">
                <div class="searchResultLeftInner">
                    <div class="entityType inline"></div>
                    <div class="entityName inline"></div>
                    <div class="entityStatus inline"></div>
                </div>
            </div>
            <div class="searchResultMiddle">
                <div class="searchResultMiddleInner">
                    <div class="entityAddress"></div>
                </div>
            </div>
            <div class="searchResultRight">
                <div class="searchResultRightInner hidden">
                    <a class="entityAction activateAction hidden" href="javascript:void(0);">activate</a>
                    <a class="entityAction deactivateAction hidden" href="javascript:void(0);">de-activate</a>
                    <a class="entityAction manageAction hidden" href="javascript:void(0);">manage</a>
                </div>
            </div>
            <div class="clear"></div>
        </div>
    </div>

    <div class="page">
        <div class="header">
            
            <div class="headerLeftWrp">
                <div class="logoSlogan condensedFont">
                    Dine Smart 365 - Admin Console
                </div>
            </div>
            <div class="headerRightWrp">
                <div class="userSection condensedFont">
                    <span id="userFriendlyName" runat="server"></span>
                </div>
            </div>
            <div class="clear"></div>
        </div>

        <div class="main">  
            <div class="searchArea">      
                <div class="searchInputWrp">
                    <p class="centerText searchDescr">  
                        <span>To add a new venue, please firstly search it to avoid a duplicate record</span>
                    </p>
                    <p class="centerText">  
                        <input class="searchInput" type="text" value="" title="type at least 3 letters or click 'search' button" />
                        <input class="searchButton" type="button" value="search" />
                    </p>
                </div>
                <div class="searchStatusWrp">
                    <p class="centerText">
                        <span class="searchStatus">
                        </span>
                    </p>
                </div>            
                <div class="searchResultListWrp">
                </div>
                <p class="rightText">
                    <input class="addNewButton hidden" type="button" value="add new" onclick="addNewLinkClicked()" />
                </p>
            </div>
            <div class="venueArea hidden">
                <center>
                <form id="venueInfoForm" action="javascript:void(0);">
                <div class="venueBasicInfo">
                    <div class="inputLine">
                        <div class="labelWrp">
                            <span class="secInputLabel">Restaurant Name :</span>
                        </div>
                        <div class="inputWrp">
                            <input id="venueName" class="inputText secInput" type="text" maxlength="100" />
                        </div>
                    </div>
                    <div class="clear"></div>
                    <div class="inputLine">
                        <div class="labelWrp">
                            <span class="secInputLabel">Web Site :</span>
                        </div>
                        <div class="inputWrp">
                            <input id="venueUrl" class="inputText secInput" type="text" maxlength="300" />
                        </div>
                    </div>
                    <div class="clear"></div>
                    <div class="inputLine">
                        <div class="labelWrp">
                            <span class="secInputLabel">Restaurant Address :</span>
                        </div>
                        <div class="inputWrp">
                            <input id="venueAddressLine1" class="inputText secInput" type="text" maxlength="300" />
                        </div>
                    </div>
                    <div class="clear"></div>
                    <div class="inputLine">
                        <div class="labelWrp">
                            <span class="secInputLabel">Address Line 2 :</span>
                        </div>
                        <div class="inputWrp">
                            <input id="venueAddressLine2" class="inputText secInput" type="text" maxlength="100" />
                        </div>
                    </div>
                    <div class="clear"></div>
                    <div class="inputLine">
                        <div class="labelWrp">
                            <span class="secInputLabel">City :</span>
                        </div>
                        <div class="inputWrp">
                            <input id="venueAddressCity" class="inputText secInput city" type="text" maxlength="50" />
                        </div>
                    </div>
                    <div class="clear"></div>
                    <div class="inputLine">
                        <div class="labelWrp">
                            <span class="secInputLabel">State / ZIP :</span>
                        </div>
                        <div class="inputWrp">
                            <div class="stateWrp">
                                <input id="venueAddressState" class="inputText secInput state" type="text" maxlength="2" />
                            </div>
                            <div class="zipWrp">
                                <input id="venueAddressZip" class="inputText secInput zip" type="text" maxlength="10" />
                            </div>
                        </div>
                    </div>
                    <div class="clear"></div>
                    <div class="inputLine">
                        <div class="labelWrp">
                            <span class="secInputLabel">Time Zone :</span>
                        </div>
                        <div class="inputWrp" id="timeZoneComboWrp" runat="server">
                        </div>
                    </div>
                    <div class="clear"></div>
                    <div class="actionWrp">
                        <input class="nextButton" type="button" value="next" onclick="nextToMapClicked()" />
                        <input class="cancelButton" type="button" value="cancel" onclick="cancelAndBackToSearchClicked()" />
                        <div class="errorStrip">
                            <span class="errorText"></span>
                        </div>
                    </div>
                    <div class="clear"></div>
                </div>
                </form>
                </center>
            </div>
            <div class="mapArea hidden">
                <center>
                    <p>Please locate the restaurant precisely by panning and zooming in. Target zoom level should be >= 19-21</p>
                    <div class="mapWrp">
                        <div class="gmap" id="gmap"></div>
                        <div class="cross" id="cross"></div>
                    </div>
                    <div class="outputAndAction">
                        <div class="outputWrp">
                            <div class="inline"><strong>Latitude :</strong> <span id="lat"></span></div>
                            <div class="inline"><strong>Longitude :</strong> <span id="long"></span></div>
                            <div class="inline"><strong>Zoom Level :</strong> <span id="zoom"></span></div>
                        </div>
                        <div class="nextToOrgWrp">
                            <input class="backToInfoButton" type="button" value="back" onclick="backToInfoClicked()" />
                            <input class="nextToOrgButton" type="button" value="next" onclick="nextToOrgClicked()" />
                        </div>
                    </div>
                </center>
            </div>
            <div class="orgAreaWrp hidden">
                <center>
                    <div class="orgArea">
                        <div class="orgAreaDesc roundedCorners4 inline">Please specify the organization structure of this restaurant:</div><br/>
                        <input type="radio" class="orgTypeRadio" name="orgType" value="1">This restaurant doesn't have another branch and it is not part of a group</input><br/>
                        <input type="radio" class="orgTypeRadio" name="orgType" value="2">This restaurant is part of a <b>chain</b> but the chain is not part of a group</input><br/>
                        <input type="radio" class="orgTypeRadio" name="orgType" value="3">This restaurant doesn't have another branch but it is part of a <b>group</b></input><br/>
                        <input type="radio" class="orgTypeRadio" name="orgType" value="4">This restaurant is part of <b>chain</b> and the chain is part of a <b>group</b></input><br/>
                        <div class="chainAndVenue hidden">
                            <div class="chainLine">
                                <div class="labelWrp2">
                                    <span class="secInputLabel2">Chain Name :</span>
                                </div>
                                <div class="inputWrp2">
                                    <input id="chainName" class="inputText secInput2" type="text" maxlength="100" />
                                    <a href="javascript:void(0);" onclick="copyVenueNameClicked('#chainName')" title="copy venue name">&#8629;</a>
                                    <span class="descChainExisting hidden">(existing)</span>
                                </div>
                            </div>
                            <div class="clear"></div>
                            <div class="groupLine">
                                <div class="labelWrp2">
                                    <span class="secInputLabel2">Group Name :</span>
                                </div>
                                <div class="inputWrp2">
                                    <input id="groupName" class="inputText secInput2" type="text" maxlength="100" />
                                    <a href="javascript:void(0);" onclick="copyVenueNameClicked('#groupName')" title="copy venue name">&#8629;</a>
                                    <span class="descGroupExisting hidden">(existing)</span>
                                </div>
                            </div>
                            <div class="clear"></div>
                        </div>                         
                        <div class="saveAllWrp">
                            <input class="saveAllButton" type="button" value="save" onclick="saveAllClicked()" />
                            <input class="backToMapButton" type="button" value="back" onclick="backToMapClicked()" />
                            <div class="errorStrip2">
                                <span class="errorText2"></span>
                            </div>
                        </div>
                    </div>
                </center>
            </div>
        </div>
        <div class="clear"></div>
    </div>
</body>
</html>
