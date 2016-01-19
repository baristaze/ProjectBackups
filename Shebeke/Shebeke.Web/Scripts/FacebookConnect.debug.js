function authStateChanged(oathProvider, state, isTriggeredByUser, callBackFunc) {
    if (state == "working") {
        $(".auth-ctrl-none").hide();
        $(".auth-ctrl-success").hide();
        $(".auth-ctrl-checking").show();
    }
    else if (state == "success") {
        $(".auth-ctrl-none").hide();
        $(".auth-ctrl-checking").hide();
        $(".auth-ctrl-success").show();
    }
    else if (state == "failure" || state == "cancelled" || state == "logout") {
        $(".auth-ctrl-checking").hide();
        $(".auth-ctrl-success").hide();
        $(".auth-ctrl-none").show();
    }
    else if (state == "server-success") {
        // client auth has been sent to server successfully   
        $(".no-auth-only").hide();
        $(".auth-only").show();
    }

    if (callBackFunc != null) {
        callBackFunc(oathProvider, state, isTriggeredByUser);
    }
}

function initFacebook(appId, callBackFunc, serverAuthCallBack) {

    // update UI
    authStateChanged("Facebook", "working", false, callBackFunc);

    var port = ((document.location.port == 80) ? "" : (":" + document.location.port));
    var channel = document.location.protocol + "//" + document.location.hostname + port + "/channel.html";

    // prepare FB init options
    var options = {
        appId: appId, // App ID from the App Dashboard
        status: false, // check the login status upon init?
        cookie: false, // set sessions cookies to allow your server to access the session?
        xfbml: false,  // parse XFBML tags on this page?
        channelUrl: channel,
        frictionlessRequests: true
    };

    // init FB
    FB.init(options);

    function fbConnectCallback(response) {
        if (response.status === 'connected') {
            // the user is logged in and has authenticated your
            // app, and response.authResponse supplies
            // the user's ID, a valid access token, a signed
            // request, and the time the access token 
            // and signed request each expire

            var isAuthenticated = $("#isAuthenticated");
            if (isAuthenticated.length == 1) {
                if (isAuthenticated.text() != response.authResponse.userID) {
                    // send the authentication info to the server
                    sendClientBasedAuthToServer(
                                "Facebook",
                                response.authResponse.userID,
                                response.authResponse.accessToken,
                                response.authResponse.expiresIn,
                                false,
                                callBackFunc,
                                serverAuthCallBack);

                    // do not update the UI yet
                }
                else {
                    // server know already... just update the UI
                    authStateChanged("Facebook", "success", false, callBackFunc);
                }
            }
            else {
                // there is not a server side part... just update the UI
                authStateChanged("Facebook", "success", false, callBackFunc);
            }
        }
        else if (response.status === 'not_authorized') {
            // the user is logged in to Facebook, 
            // but has not authenticated your app
            authStateChanged("Facebook", "failure", false, callBackFunc);
        }
        else {
            // the user isn't logged in to Facebook.
            authStateChanged("Facebook", "failure", false, callBackFunc);
        }
    }

    // check current login status
    FB.getLoginStatus(fbConnectCallback);
}

function signupToFacebook(callBackFunc, serverAuthCallBack) {
    // login to facebook
    FB.login(function (response) {
        if (response.authResponse) {
            // send the authentication info to the server
            sendClientBasedAuthToServer(
                        "Facebook",
                        response.authResponse.userID,
                        response.authResponse.accessToken,
                        response.authResponse.expiresIn,
                        true,
                        callBackFunc,
                        serverAuthCallBack);
        }
        else {
            // User cancelled login or did not fully authorize.
            authStateChanged("Facebook", "cancelled", true, callBackFunc);
        }
    }, { scope: 'email, publish_actions' });
    // user_birthday, user_hometown, user_location, 
}

// call back on failure
function serverAuthFailedEx(oauthProvider, isTriggeredByUser, errorMessage, callBackFunc) {
    return function serverAuthFailed() {
        if (isTriggeredByUser) {
            showBlockedErrorMessage(errorMessage);
        }

        authStateChanged(oauthProvider, "failure", isTriggeredByUser, callBackFunc);
    }
}

function serverAuthSucceededEx(oauthProvider, isTriggeredByUser, callBackFunc, serverAuthCallBack) {
    return function serverAuthSucceeded(response, statusText, xhr, $form) {
        // process the result
        if (statusText == "success") {
            if (response.ErrorCode == "0") {
                serverAuthCallBack(response);
                authStateChanged(oauthProvider, "success", isTriggeredByUser, callBackFunc);
                authStateChanged(oauthProvider, "server-success", isTriggeredByUser, callBackFunc);
            }
            else {
                var errMsg = oauthProvider + " kimliğiniz teyid edilemedi: " + response.ErrorMessage;
                serverAuthFailedEx(oauthProvider, isTriggeredByUser, errMsg, callBackFunc)();
            }
        }
        else {
            var errMsg2 = oauthProvider + " kimliğinizi kontrol ederken beklenmeyen bir hata oluştu!";
            serverAuthFailedEx(oauthProvider, isTriggeredByUser, errMsg2, callBackFunc)();
        }
    }  // core 
} // closure function

function sendClientBasedAuthToServer(oauthProvider, userId, accessToken, expiresInSeconds, isTriggeredByUser, callBackFunc, serverAuthCallBack) {
    var oathProv = 0;
    if (oauthProvider == "Facebook") {
        oathProv = 1;
    }

    var currentUrl = window.location.href.toString();
    var splitId = getSplitId(currentUrl);

    var defaultErrMsg = "Facebook kimliğinizi kontrol ederken beklenmeyen bir hata oluştu!";
    var jsonData = { "OAuthProvider": oathProv, "OAuthUserId": userId, "OAuthAccessToken": accessToken, "OAuthExpiresInSeconds": expiresInSeconds, "SplitId": splitId };
    var jsonText = JSON.stringify(jsonData);
    var url = $("#applicationPath").text() + "/svc/rest/signin";
    $.ajax({
        url: url,
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        data: jsonText,
        success: serverAuthSucceededEx(oauthProvider, isTriggeredByUser, callBackFunc, serverAuthCallBack),   // post-submit callback
        error: serverAuthFailedEx(oauthProvider, isTriggeredByUser, defaultErrMsg, callBackFunc)
    });

    // avoid a postback to the server
    return false;
}

function logout(callBackFunc, isTriggeredByUser) {
    var url = $("#applicationPath").text() + "/svc/rest/logout";
    $.ajax({
        url: url,
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        data: null,
        success: function () {
            FB.logout();
            authStateChanged("Facebook", "logout", isTriggeredByUser, callBackFunc);
        },
        error: null
    });
}

function onServerAuthDefault(response) {
    setCookie("XAuthToken", response.Token, response.ExpiresInSeconds);
    $("#currentUserName").text(response.UserFriendlyName);
    $("#currentUserName2").text(response.UserFriendlyName);
    $("#currentUserPhoto").attr("src", response.PhotoUrl);
    $("#isAuthenticated").text(response.OAuthUserId);
}

function onAuthStateChangeDefault(oathProvider, state, isTriggeredByUser) {
    if (state == "success") {
        $.unblockUI();

        if (isTriggeredByUser) {
            // don't change the Category name below. It has to match to config at Google side
            _gaq.push(['_trackEvent', 'SignupCategory', 'Facebook', 'Signup', 1, null]);
        }
    }
    else if (state == "failure") {
        if ($("#isAuthenticated").text() != "0") {
            logout(onAuthStateChangeDefault, isTriggeredByUser);
        }
    }
    else if (state == "logout") {
        setCookie("XAuthToken", '', -1);
        $("#isAuthenticated").text("0");
    }
}

// the above section is generic. it can be used in other projects as well.
// the rest is specific to this project...

function onAuthStateChanged(oathProvider, state, isTriggeredByUser) {
    if (state == "success") {
        $.unblockUI();

        var entryId = getEntryIdFromUrl();
        var comparisonDialogFlag = $("#entryComparisonDialogContentIsValid");
        var url = window.location.href.toString();
        var hash = getHashParameters(url)["f"];
        if (entryId >= 0 && comparisonDialogFlag.text() == 1 && hash == 1) {
            showBlockedComparisonDialog();
            // avoid further popup
            comparisonDialogFlag.text("0");
        }

        if (isTriggeredByUser) {
            // don't change the Category name below. It has to match to config at Google side
            _gaq.push(['_trackEvent', 'SignupCategory', 'Facebook', 'Signup', 1, null]);
        }
    }
    else if (state == "server-success") {
        $("#content").find(".entry-block").fadeOut('slow', function () { $(this).remove(); });
        loadInitialEntries();
    }
    else if (state == "failure") {
        if ($("#isAuthenticated").text() != "0") {
            logout(onAuthStateChanged, isTriggeredByUser);
        }

        // 'failure' state is called in two situations: 
        // 1 - User has logged in to FB but the app hasn't been not authorized yet
        // 2 - User hasn't logged in to FB yet; we don't know if the app is authorized or not
        if (!isTriggeredByUser) {
            var url2 = window.location.href.toString();
            var urlParams = getUrlParameters(url2);
            var entryParam = urlParams["e"];
            if (entryParam !== undefined) {
                // it is from social stream
                var entryIdx = -1;
                if (isInteger(entryParam)) {
                    entryIdx = parseInt(entryParam);
                }

                encourageAppInstall(oathProvider, true, entryIdx);
            }
        }
    }
    else if (state == "logout") {
        setCookie("XAuthToken", '', -1);
        if ($("#isAuthenticated").text() != "0") {
            $("#facebookFriendSelectArea").html("");
            $("#content").find(".entry-block").fadeOut('slow', function () { $(this).remove(); });
            loadInitialEntries();
        }

        $("#isAuthenticated").text("0");
    }
}

function encourageAppInstall(oathProvider, isFromSocialChannel, entryId) {
    showBlockedInstallDialog();
}

function printAuthCookie() {
    var looper = 1;
    var regex = new RegExp("XAuthToken=[^;]+", "gi");
    alert(document.cookie);
    document.cookie.replace(regex, function (matched) {
        var ending = "";
        if (matched.length < 10) {
            ending = matched;
        } else {
            ending = matched.substring(matched.length - 10, 10);
        }
        alert(looper.toString() + ": " + ending);
        looper++;
        return matched;
    });
}

function facebookConnect_Init_DefaultActions() {
    $(".signup-facebook").live('click', function () {
        signupToFacebook(onAuthStateChanged, onServerAuthDefault);
    });

    $(".signup-modal").live('click', function () {
        signupToFacebook(onAuthStateChanged, onServerAuthDefault);
        //$.unblockUI();
        return false;
    });
}