function refreshFBFriendListOnServer() {
    $.ajax({
        url: "svc/rest/friends/refresh",
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        data: null,
        success: function (response, statusText, xhr) { },
        error: function () { }
    }); 
}

function coverPage_FBAuthStateChanged(oathProvider, state, isTriggeredByUser){
    if (state == "success") {     
        var statusWrp = $(".statusWrp");
        statusWrp.hide();

        var signUpWrp = $(".signUpWrp");
        signUpWrp.hide();

        var contentWrp = $(".contentWrp");
        contentWrp.show();
                
        $(".footer").addClass("footer2");

        coverPage_GetAlreadyInvitedFriends();

        if (isTriggeredByUser) {
            // don't change the Category name below. It has to match to config at Google side
            _gaq.push(['_trackEvent', 'SignupCategory', 'Facebook', 'Signup', 1, null]);
        }
    }
    else if (state == "server-success") {   
        if (isTriggeredByUser) {
            // pull friends
            refreshFBFriendListOnServer();
        }             
    }
    else if (state == "failure") {
        if ($("#isAuthenticated").text() != "0") {
            logout(coverPage_FBAuthStateChanged, isTriggeredByUser);
        }

        var statusWrp2 = $(".statusWrp");
        statusWrp2.hide();

        var signUpWrp2 = $(".signUpWrp");
        signUpWrp2.show();
    }
    else if (state == "logout") {
        setCookie("XAuthToken", '', -1);
        $("#isAuthenticated").text("0");

        var signUpWrp3 = $(".signUpWrp");
        var contentWrp3 = $(".contentWrp");
        contentWrp3.hide();
        signUpWrp3.show();
        $(".footer").removeClass("footer2");
    }
}

function coverPage_FBServerAuthCompletedSuccessfully(response)
{
    setCookie("XAuthToken", response.Token, response.ExpiresInSeconds);
    $("#isAuthenticated").text(response.OAuthUserId);
    $("#currentUserName").text(response.UserFriendlyName);
}


function coverPage_GetAlreadyInvitedFriends() {
    var randomId = guidGenerator();
    var url = "svc/rest/friends/invited?rid=" + randomId;
    $.ajax({
        url: url,
        type: "GET",
        dataType: "json",
        contentType: "application/json",
        data: null,
        success: function (response, statusText, xhr) {
            if (statusText == "success") {
                if (response.ErrorCode == "0") {
                    // reset the array
                    __facebookExcludedIds = [];
                    $.each(response.Friends, function (friendIndex, friend) {
                        __facebookExcludedIds.push(friend.FacebookId);
                    });

                    $(".invitedResult").text(__facebookExcludedIds.length.toString());
                }
            }
        },
        error: function () { }
    });
}

function coverPage_MarkAsInvited(friendIds) {
    var friends = [];
    for (var x = 0; x < friendIds.length; x++) {
        var friend = { "FacebookId": friendIds[x] };
        friends.push(friend);
    }

    // var jsonData = { "Friends": friends };
    var jsonData = friends;
    var jsonText = JSON.stringify(jsonData);
    $.ajax({
        url: "svc/rest/mark/friends/invited",
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        data: jsonText,
        success: function (data, textStatus, jqXHR) { },
        error: function () { }
    });
}

/*message cannot be > 60 chars*/
function coverPage_InviteFriends() {
    FB.ui({ method: 'apprequests',
        message: 'Bu sosyal paylaşım sitesini tavsiye ederim',
        //message: 'join me on shebeke.net',
        // exclude_ids: __facebookExcludedIds, // re-invite is allowed
        data: '54321'
    }, coverPage_InviteFriendsCallback);
    return false;
}

function coverPage_InviteFriendsCallback(response) {
    if (response && response.hasOwnProperty('to')) {
        if (response.to.length > 0) {
            __facebookExcludedIds = __facebookExcludedIds.concat(response.to);
            coverPage_MarkAsInvited(response.to);
        }

        $(".invitedResult").text(__facebookExcludedIds.length.toString());
        $(".invitedStats").effect("highlight", {}, 3000);
        $(".skip-link-with-auth").text("Devam Et");
    }
}

function coverPage_DocumentReady() {

    coverPage_InitFacebookEx();

    $(".fbSignUp").live('click', function () {
        signupToFacebook(coverPage_FBAuthStateChanged, coverPage_FBServerAuthCompletedSuccessfully);
    });

    $(".cover-page-logout").live('click', function () {
        logout(coverPage_FBAuthStateChanged, true);
    });

    $(".reload").live('click', function () {
        window.location.reload();
        return false;
    });

    $(".logoWrp").live('click', function () {
        gotoTopics();
        return false;
    });

    $(".skip-link").live('click', function () {
        gotoTopics();
        return false;
    });      

    $(".invite").live('click', coverPage_InviteFriends);
    $(".inviteLink").live('click', coverPage_InviteFriends);
}