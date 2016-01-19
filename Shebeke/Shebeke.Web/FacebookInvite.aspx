<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FacebookInvite.aspx.cs" Inherits="Shebeke.Web.FacebookInvite" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="tr" lang="tr">
<head>
    <title></title>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.9.2/jquery-ui.min.js"></script>
    <script type="text/javascript" src="http://connect.facebook.net/tr_TR/all.js"></script>
    <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/Common.debug.js?version=0.985" %>" ></script>
    <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/Url.debug.js?version=0.985" %>" ></script>
    <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/Dialog.debug.js?version=0.985" %>" ></script>

    <style type="text/css" media="all">
        /* Facebook Dialogue Styles */
        .ui-widget-overlay {background: none;}
        .ui-dialog{background: rgba(82, 82, 82, 0.7);padding:10px;border-radius: 8px;-webkit-border-radius: 8px;}
        .ui-dialog .ui-dialog-titlebar{background: #6D84B4;border: 1px solid #3B5998;font-family: "lucida grande",tahoma,verdana,arial,sans-serif;color: white;font-size: 14px;font-weight: bold;}
        #customFacebookDialog{border-left: 1px solid #555;border-right: 1px solid #555;border-bottom: 1px solid #ccc;background-color:#fff;padding:0px;overflow:hidden;display:none;font-family: "lucida grande",tahoma,verdana,arial,sans-serif;}
        #customFacebookDialog p, #customFacebookDialog div{font-family: "lucida grande",tahoma,verdana,arial,sans-serif;font-size:13px;}
        .ui-widget-content{background: none none 50% top repeat-x;border:none;}
        .ui-dialog-titlebar-close{display:none}
        .ui-dialog .ui-corner-all{-moz-border-radius: 0px;-webkit-border-radius: 0px;border-radius: 0px;}
        .ui-dialog .ui-dialog-buttonpane{background: #f2f2f2; margin:0;border-left: 1px solid #555;border-right: 1px solid #555;border-bottom: 1px solid #555;padding:8px}
        .ui-dialog .ui-dialog-buttonpane button{ margin:0 0 0 10px;padding:4px 8px;}
        .ui-dialog-buttonpane{text-align:right;}
        .ui-state-default, .ui-widget-content .ui-state-default{border-color: #29447E;color: #fff;background:#6D84B4;border-style:solid;}
        .ui-state-hover, .ui-widget-content .ui-state-hover{border-color: #29447E;background:#6D84B4;}
        .ui-state-active, .ui-widget-content .ui-state-active{background: #4F6AA3;border-bottom-color: #29447E;box-shadow: 0 1px 0 rgba(0, 0, 0, 0.1); -webkit-box-shadow: 0 1px 0 rgba(0, 0, 0, 0.1);}
        .ui-button-text-only .ui-button-text {padding:0px}
        .ui-dialog .ui-dialog-titlebar {padding: 5px;}
        .ui-button{border-width:1px;font-size:14px;font-weight:bold;outline:none;cursor:pointer;}
    </style>

    <style type="text/css" media="all">
        #customFacebookDialog{}
        #facebookFriendSelectArea{height:400px; overflow-y:scroll;padding:5px 0px;}
        
        .facebookInviteCancelButtonClass.ui-state-default{background-color:#e0e0e0;color:#333;border-color:#999;}
        .facebookInviteCancelButtonClass.ui-state-active{background-color:#ccc;color:#333;border-color:#999;}
        
        #facebookSelectAllFriendsArea{padding:10px 5px;background-color:#eee;}
        #facebookFriendsSelectAllCBox{outline:none;margin-left:2px;position:relative; top:-2px;}
        #facebookFriendsSelectAllImg{margin:0px 5px;position:relative; top:1px;}
        #customFacebookDialog div#facebookFriendsSelectAllLabel{font-size:18px; font-weight:bold;display:inline-block;position:relative; top:-2px; color:#333;}
        
        #facebookFriendSearchArea{padding:10px 15px 10px 10px;background-color:#eee;border-top:1px solid #ccc;border-bottom:1px solid #ccc;}
        #facebookFriendSearchWaterMark{position:relative;}
        #facebookFriendSearchDescr  {width:100%;padding:2px 4px;box-sizing:border-box;-moz-box-sizing:border-box;-webkit-box-sizing:border-box;color:#000;border:1px solid #ccc;position:absolute;top:0px;left:0px;z-index:9998;background-color:White;opacity:0.99;}
        #facebookFriendSearchTextBox{width:100%;padding:2px 4px;box-sizing:border-box;-moz-box-sizing:border-box;-webkit-box-sizing:border-box;color:#666;border:1px solid #ccc;position:relative;z-index:9999;}
        #facebookFriendSearchTextBox:focus{background-color:#ffffdd;}
        .ffs-watermark{background-color:rgba(255, 255, 255, 0.5);}
                
        .ff-wrp{margin:3px 4px;padding:4px 3px;text-align:left;display:inline-block;cursor:pointer;}
        .ff-wrp:hover{background-color:#eceff5;}
        .ff-cbox{float:left;margin-top:6px;outline:none;}
        .ff-img{height:31px; width:30px;float:left;margin:0px 4px 0px 2px;}
        .ff-name{height:31px;width:120px;word-wrap:break-word;float:left;vertical-align:middle;}
        
        .clear{clear:both;}
        .hidden{display:none;}
    </style>
    
    <script type="text/javascript">
        $(document).ready(function () {

            $(".ff-cbox").live('click', function () {
                if ($(this).attr("checked")) {
                    $(this).removeAttr("checked");
                }
                else {
                    $(this).attr("checked", "checked");
                }
            });

            $(".ff-wrp").live('click', function () {
                var inp = $(this).find("input");
                if (inp.attr("checked")) {
                    inp.removeAttr("checked");
                }
                else {
                    inp.attr("checked", "checked");
                }
            });

            $("#facebookFriendsSelectAllCBox").live('click', function () {
                var checked = false;
                if ($(this).attr("checked")) {
                    checked = true;
                }

                var selectArea = $("#facebookFriendSelectArea");
                selectArea.find(".ff-cbox:visible").each(function (index, cbox) {
                    if (checked) {
                        $(cbox).attr("checked", "checked");
                    }
                    else {
                        $(cbox).removeAttr("checked")
                    }
                });
            });
            
            $("#facebookFriendSearchTextBox").live('blur', function () {
                if ($.trim($(this).val()) == "") {
                    $(this).addClass("ffs-watermark");
                }
                else {
                    $(this).removeClass("ffs-watermark");
                }
            });

            $("#facebookFriendSearchTextBox").live('keyup', function () {
                var txt = $.trim($(this).val()).toUpperCase();
                var selectArea = $("#facebookFriendSelectArea");
                if(txt.length == 0){
                    selectArea.find(".ff-wrp").show();
                }
                else{
                    selectArea.find(".ff-wrp").each(function (index, ffw) {
                        if($(ffw).find(".ff-name").text().toUpperCase().indexOf(txt) < 0){
                            $(ffw).hide();
                        }
                        else{
                            $(ffw).show();
                        }
                    });
                }
            });
        });
    </script>

    <script type="text/javascript">

        var port = ((document.location.port == 80) ? "" : (":" + document.location.port));
        var channel = document.location.protocol + "//" + document.location.hostname + port + "/channel.html";

        // prepare FB init options
        var options = {
            appId: '164591543732009', // App ID from the App Dashboard
            status: false, // check the login status upon init?
            cookie: false, // set sessions cookies to allow your server to access the session?
            xfbml: false,  // parse XFBML tags on this page?
            channelUrl: channel,
            frictionlessRequests: true
        };

        // init FB
        FB.init(options);
        FB.login(function (response) {
            if (response.authResponse) {
                retrieveFacebookFriends();
            }
        }); 

        function fbInvitationCancelled($customDlg, topicId) {
            $customDlg.dialog("close");
        }

        var MAX_APP_REQUEST_RECIPIENTS = 2;

        function fbInvitationConfirmed($customDlg, topicId) {

            var selectedFriends = [];
            var selectArea = $("#facebookFriendSelectArea");
            selectArea.find(".ff-cbox[checked='checked']").each(function(index, cbox){
                selectedFriends.push($(cbox).val());
            });

            if(selectedFriends.length == 0){
                alert("please select at least one friend");
                return false;
            }
            
            $customDlg.dialog("close");

            showInvitationSendWindow(topicId, selectedFriends, 0);
        }

        function showInvitationSendWindow(topicId, selectedFriendsAll, currentIterationIndex){
            var sentSoFar = MAX_APP_REQUEST_RECIPIENTS * currentIterationIndex;
            if(selectedFriendsAll.length > sentSoFar){
                var nextSize = selectedFriendsAll.length - sentSoFar;
                if(nextSize > MAX_APP_REQUEST_RECIPIENTS){
                    nextSize = MAX_APP_REQUEST_RECIPIENTS;
                }
                
                var nextFriends = selectedFriendsAll.slice(sentSoFar, sentSoFar + nextSize);
                
                FB.ui({ method: 'apprequests',
                    message: 'join me on shebeke.net',
                    to: nextFriends,
                    data: topicId
                }, fbInvitationSentEx(topicId, selectedFriendsAll, currentIterationIndex));
            }
        }

        function fbInvitationSentEx(topicId, selectedFriendsAll, currentIterationIndex){
            return function fbInvitationSent(response){
                if (response) {
                    if (response.to.length > 0){
                        alert(response.to.length);
                        currentIterationIndex = currentIterationIndex + 1;
                        showInvitationSendWindow(topicId, selectedFriendsAll, currentIterationIndex);
                    }
                }
            }
        }

        //function to show an emulated Facebook dialogue
        function showFacebookDialogue($dlg, topicId) {
            //setup dialogues
            var dialogue = $dlg.dialog({ autoOpen: false, modal: true, draggable: true, resizable: false, bgiframe: true, width: '590px'});

            //setup options for this dialogue
            $dlg.dialog("option", "title", 'Facebook - Invite Friends');
            $dlg.dialog(
                {
                    buttons:
                    {
                        "SendRequests": { text: 'Davet Et', click: function () { fbInvitationConfirmed($(this), topicId); } },
                        "Cancel": { text: 'İptal', class: 'facebookInviteCancelButtonClass', click: function () { fbInvitationCancelled($(this), topicId); } }
                    }
                }
            );

            $dlg.dialog("open");
            $dlg.bind("dialogbeforeclose", function (event, ui) {
                // returning false, cancels the closing... good to make error check...
                // return false;
            });
        }

        function retrieveFacebookFriends(topicId) {
            FB.api('/me/friends?fields=id,name,picture', function (response) {
                if (response && response.data && response.data.length > 0) {
                    var dlg = $("#customFacebookDialog");
                    var friendsArea = dlg.find("#facebookFriendSelectArea");
                    for (var i = 0; i < response.data.length; i++) {
                        var friend = response.data[i];
                        var img = "<img alt='' class='ff-img' src='" + friend.picture.data.url + "' />";
                        var checkbox = "<input class='ff-cbox' type='checkbox' name='friends' value='" + friend.id + "' />";
                        var name = "<div class='ff-name'>" + friend.name + "</div>";
                        var friendDiv = $("<div />", { id: "ff_" + friend.id, class: 'ff-wrp' });
                        friendDiv.html(checkbox + img + name);
                        friendsArea.append(friendDiv);
                    }

                    showFacebookDialogue(dlg, topicId);
                }
            });
        }

    </script>

</head>
<body>
    <!-- Element to run dialogue -->
    <div id="customFacebookDialog"> 
        <div id="facebookSelectAllFriendsArea">
            <input id="facebookFriendsSelectAllCBox" type='checkbox' name='selectallfriends' value='0' />
            <img alt='&larr;' id='facebookFriendsSelectAllImg' src="<%=Request.ApplicationPath.TrimEnd('/') + "/Images/left_arrow.png" %>" />
            <div id="facebookFriendsSelectAllLabel" >Select All</div>
        </div>
        <div id="facebookFriendSearchArea">
            <div id="facebookFriendSearchWaterMark">
                <input id="facebookFriendSearchDescr" type="text" value="Search..." />
                <input id="facebookFriendSearchTextBox" type="text" class="ffs-watermark"/>
            </div>
        </div>
        <div id="facebookFriendSelectArea"></div>
    </div>
</body>
</html>
