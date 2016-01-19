
function sharingOnFacebookCallbackEx(topicId, entryId) {
    return function sharingOnFacebookCallback(response) {
        if (response && response['post_id']) {
            // user has posted something on FB
            // if ($("#isAuthenticated").text() != "0") {
                // keep track of this share
                var url = $("#applicationPath").text() + "/svc/rest/topic/entry/log/share/" + topicId + "/" + entryId + "/Facebook";
                $.ajax({
                    url: url,
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json",
                    data: null,
                    success: null,
                    error: null
                });
            //}

            // don't change the Category name below. It has to match to config at Google side
            _gaq.push(['_trackEvent', 'QuickInteractCategory', 'Post', 'Facebook', 1, null]);
        }
    }
}

function shareEntryOnFacebook($childItem) {
    var topicTitle = $("#topicTitle").text();
    var topicId = $("#topicId").text();
    var rootBlock = $childItem.closest(".main-block");
    var entryId = rootBlock.find(".entry-id").text();
    var entryContent = rootBlock.find(".entry-paragraph");
    var entryContentText = entryContent.text();
    var entryImage = entryContent.find(".entry-img-preview:first");
    var entryFirstImageLink = null;
    if (entryImage.length > 0) {
        entryFirstImageLink = entryImage.attr("src");
    }
    else {
        entryFirstImageLink = getRootUrl() + "/Images/logo.png?version=0.8";
    }

    var url = window.location.toString();
    url = removeSplitParams(url);
    url = removeGoogleExperimentParams(url);
    url = updateQueryStringParameter(url, "e", entryId);
    url = updateHashStringParameter(url, "f", "1");
    
    // calling the API ...
    var obj = {
        method: 'feed',
        name: topicTitle,
        link: url,
        picture: entryFirstImageLink,
        caption: 'vizibuzz.com',
        description: entryContentText
    };

    FB.ui(obj, sharingOnFacebookCallbackEx(topicId, entryId));
}

function shareEntryOnTwitter($childItem) {
    var topicTitle = $("#topicTitle").text();
    var topicId = $("#topicId").text();
    var rootBlock = $childItem.closest(".main-block");
    var entryId = rootBlock.find(".entry-id").text();
    var entryContent = rootBlock.find(".entry-paragraph");
    var entryContentText = entryContent.text();
    var twitText = topicTitle + " - " + entryContentText;

    var ref = window.location.toString();
    ref = removeSplitParams(ref);
    ref = removeGoogleExperimentParams(ref);
    ref = updateQueryStringParameter(ref, "e", entryId);
    ref = updateHashStringParameter(ref, "f", "1");
    ref = encodeURIComponent(ref);
    
    /*
    if (ref.length > 140) {
    var topicId = $("#topicId").text();
    ref = getRootUrl() + "?topicId=" + topicId;
    }

    var len = 140 - ref.length - 1;
    */

    // twitter shortens the URL.
    var len = 140 - 22 - 1;
    if (twitText.length > len) {
        twitText = twitText.substring(0, len - 3) + "...";
    }

    var link = "http://www.twitter.com/share?url=" + ref + "&text=" + encodeURIComponent(twitText);

    var width = 480;
    var height = 300;
    var left = ($(window).width() - width) / 2;
    var top = ($(window).height() - height) / 2;
    opts = 'status=1' + ',width=' + width + ',height=' + height + ',top=' + top + ',left=' + left;
    window.open(link, "_blank", opts);

    var url = $("#applicationPath").text() + "/svc/rest/topic/entry/log/share/" + topicId + "/" + entryId + "/Twitter";
    $.ajax({
        url: url,
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        data: null,
        success: null,
        error: null
    });

    // don't change the Category name below. It has to match to config at Google side
    _gaq.push(['_trackEvent', 'QuickInteractCategory', 'Post', 'Twitter', 1, null]);
}

function savingTopicInvitationSucceededEx(topicId, entryId, selectedFriendsAll, sentCountSoFar) {
    return function savingTopicInvitationSucceeded(response, statusText, xhr, $form) {
        if (statusText == "success") {
            if (response.ErrorCode == "0") {
                if (sentCountSoFar == selectedFriendsAll.length) {
                    // last trip
                    $(".invite-friends-to-topic").text("Invite More Friends");
                }
            }
        }
    }
}

function fbInvitationSentEx(topicId, entryId, selectedFriendsAll, sentCountSoFar) {
    return function fbInvitationSent(response) {
        if (response) {
            if (response.to.length > 0) {

                sentCountSoFar = sentCountSoFar + response.to.length;

                if (response.request > 0) {
                    // save this in our DB [response.request + topicId + create time]
                    var reqData = response.request.toString() + "/" + response.to.length.toString();
                    var url = $("#applicationPath").text() + "/svc/rest/topic/invitation/" + topicId + "/" + entryId + "/" + reqData;
                    $.ajax({
                        url: url,
                        type: "POST",
                        dataType: "json",
                        contentType: "application/json",
                        data: null,
                        success: savingTopicInvitationSucceededEx(topicId, entryId, selectedFriendsAll, sentCountSoFar),
                        error: null
                    });
                }

                if (sentCountSoFar < selectedFriendsAll.length) {
                    showInvitationSendWindow(topicId, entryId, selectedFriendsAll, sentCountSoFar);
                }

                // don't change the Category name below. It has to match to config at Google side
                _gaq.push(['_trackEvent', 'InviteFriendsCategory', 'Invite', 'Facebook', response.to.length, null]);
            }
        }
    }
}

function showInvitationSendWindow(topicId, entryId, selectedFriendsAll, sentCountSoFar) {
    if (selectedFriendsAll.length > sentCountSoFar) {
        var nextSize = selectedFriendsAll.length - sentCountSoFar;
        if (nextSize > MAX_APP_REQUEST_RECIPIENTS) {
            nextSize = MAX_APP_REQUEST_RECIPIENTS;
        }

        var nextFriends = selectedFriendsAll.slice(sentCountSoFar, sentCountSoFar + nextSize);

        FB.ui({ method: 'apprequests',
            message: 'I believe you can shed some light on this topic!',
            to: nextFriends,
            data: topicId
        }, fbInvitationSentEx(topicId, entryId, selectedFriendsAll, sentCountSoFar));
    }
}

function fbInvitationConfirmed($customDlg, topicId, entryId) {

    var selectedFriends = [];
    var selectArea = $("#facebookFriendSelectArea");
    selectArea.find(".ff-cbox[checked='checked']").each(function (index, cbox) {
        selectedFriends.push($(cbox).val());
    });

    if (selectedFriends.length == 0) {
        alert("please select at least one friend");
        return false;
    }

    $customDlg.dialog("close");

    showInvitationSendWindow(topicId, entryId, selectedFriends, 0);
}

function fbInvitationCancelled($customDlg, topicId, entryId) {
    $customDlg.dialog("close");
}

//function to show an emulated Facebook dialogue
function showFacebookDialogue($dlg, topicId, entryId, dialogTitle, dlgWidth) {
    //setup dialogues
    if ($(window).width() < 540) {
        dlgWidth = '90%';
    }

    var dialogue = $dlg.dialog({ autoOpen: false, modal: true, draggable: true, resizable: false, bgiframe: true, width: dlgWidth });

    //setup options for this dialogue
    $dlg.dialog("option", "title", dialogTitle);
    $dlg.dialog(
                {
                    buttons:
                    {
                        "SendRequests": { 'text': 'Send Requests', 'class': 'facebookInviteButton', 'click': function () { fbInvitationConfirmed($(this), topicId, entryId); } },
                        "Cancel": { 'text': 'Cancel', 'class': 'facebookInviteButton facebookInviteCancelButtonClass', 'click': function () { fbInvitationCancelled($(this), topicId, entryId); } }
                    }
                }
            );
            
    $dlg.dialog("open");
}

function retrieveFacebookFriends($btn, topicId, entryId, includePics, dialogTitle) {
    if ($btn) {
        $btn.find(".invite-friends-to-topic").text("Please wait...");
    }

    var dlgWidth = '590px';
    if (!includePics) {
        dlgWidth = '530px';
    }

    var dlg = $("#customFacebookDialog");
    var friendsArea = dlg.find("#facebookFriendSelectArea");
    var friends = friendsArea.find(".ff-wrp");
    if (friends.length > 0) {
        // the content of the dialog has already been populated before. just reset the items' states
        dlg.find("#facebookFriendsShowOnlySelecteds").removeClass("facebookFriendsCurrentFilter");
        dlg.find("#facebookFriendsShowAll").addClass("facebookFriendsCurrentFilter");
        dlg.find("#facebookFriendsSelectAllCBox").removeAttr("checked");
        var searchBox = dlg.find("#facebookFriendSearchTextBox");
        searchBox.val("");
        searchBox.addClass("ffs-watermark");
        friends.show().find(".ff-cbox").removeAttr("checked");

        // show dialog
        showFacebookDialogue(dlg, topicId, entryId, dialogTitle, dlgWidth);

        if ($btn) {
            $btn.find(".invite-friends-to-topic").text("Invite your friends to the discussion!");
        }
    }
    else {

        FB.api('/me/friends?fields=id,name,picture', function (response) {
            if (response && response.data && response.data.length > 0) {
                for (var i = 0; i < response.data.length; i++) {
                    var friend = response.data[i];
                    var checkbox = "<input class='ff-cbox' type='checkbox' name='friends' value='" + friend.id + "' />";
                    var name = "<div class='ff-name'>" + friend.name + "</div>";

                    var friendDiv = $("<div />", { 'id': "ff_" + friend.id, 'class': 'ff-wrp' });
                    if (includePics) {
                        var img = "<img alt='' class='ff-img' src='" + friend.picture.data.url + "' />";
                        friendDiv.html(checkbox + img + name);
                    }
                    else {
                        friendDiv.html(checkbox + name);
                    }
                    friendsArea.append(friendDiv);
                }

                showFacebookDialogue(dlg, topicId, entryId, dialogTitle, dlgWidth);
            }

            if ($btn) {
                $btn.find(".invite-friends-to-topic").text("Invite your friends to the discussion!");
            }
        });
    }
}

function inviteFriendsToTopic($btn) {
    var topicId = $("#topicId").text();
    retrieveFacebookFriends($btn, topicId, 0, true, 'Facebook - Invite Friends');
}

function entryPage_Init_Sharing() {
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
                $(cbox).removeAttr("checked");
            }
        });
    });

    $("#facebookFriendsShowOnlySelecteds").live('click', function () {
        $("#facebookFriendsShowAll").removeClass("facebookFriendsCurrentFilter");
        $(this).addClass("facebookFriendsCurrentFilter");

        var dlg = $(this).closest("#customFacebookDialog");
        var searchBox = dlg.find("#facebookFriendSearchTextBox");
        searchBox.val("");
        searchBox.addClass("ffs-watermark");

        var friendsArea = dlg.find("#facebookFriendSelectArea");
        friendsArea.find(".ff-wrp").each(function (index, ffw) {
            if ($(ffw).find(".ff-cbox").attr("checked")) {
                $(ffw).show();
            }
            else {
                $(ffw).hide();
            }
        });
    });

    $("#facebookFriendsShowAll").live('click', function () {
        $("#facebookFriendsShowOnlySelecteds").removeClass("facebookFriendsCurrentFilter");
        $(this).addClass("facebookFriendsCurrentFilter");

        var dlg = $(this).closest("#customFacebookDialog");
        var searchBox = dlg.find("#facebookFriendSearchTextBox");
        searchBox.val("");
        searchBox.addClass("ffs-watermark");

        var friendsArea = dlg.find("#facebookFriendSelectArea");
        var friends = friendsArea.find(".ff-wrp");
        friends.show();
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
        if (txt.length == 0) {
            selectArea.find(".ff-wrp").show();
        }
        else {
            var filterSel = $("#facebookFriendsShowOnlySelecteds");
            if (filterSel.hasClass("facebookFriendsCurrentFilter")) {
                filterSel.removeClass("facebookFriendsCurrentFilter");
                $("#facebookFriendsShowAll").addClass("facebookFriendsCurrentFilter");
            }

            selectArea.find(".ff-wrp").each(function (index, ffw) {
                if ($(ffw).find(".ff-name").text().toUpperCase().indexOf(txt) < 0) {
                    $(ffw).hide();
                }
                else {
                    $(ffw).show();
                }
            });
        }
    });

    $(".btn-share-entry-facebook").live('click', function () {
        shareEntryOnFacebook($(this));
        return false;
    });

    $(".btn-share-entry-twitter").live('click', function () {
        shareEntryOnTwitter($(this));
        return false;
    });

    $(".topic-invite").live('click', function () {

        if ($("#isAuthenticated").text() == "0") {
            showBlockedLoginDialog();
            return false;
        }

        inviteFriendsToTopic($(this));
        return false;
    });
}