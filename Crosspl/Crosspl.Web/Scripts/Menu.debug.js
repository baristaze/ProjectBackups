
function gotoTopics() {
    //if ($(this).hasClass("skip-link-with-auth")) {
    //    setCookie("__skipFriendInvitations", "1", 7 * 24 * 60 * 60);
    //}

    // we better put split id and google experiment to avoid an unnecessary redirect and utm_referer
    var targetUrl = getRootUrl() + "/latest";
    var currentUrl = window.location.href.toString();
    var urlParams = getUrlParameters(currentUrl);
    var splitId = getSplitId2(urlParams);
    var googleExperimentId = getGoogleExperimentId2(urlParams);
    var delim = '?';
    if (splitId > 0) {
        targetUrl += delim + "split=" + splitId;
        delim = '&';
    }
    if (googleExperimentId) {
        targetUrl += delim + "utm_expid=" + googleExperimentId;
        delim = '&';
    }
    window.location = targetUrl;
}

function menu_Init(logoutCallback) {
    $(".tb-login-success").live('mouseenter', function () {
        $(this).find(".user-menu").slideDown(200);

        if (!$(this).hasClass("tb-login-success-mini")) {
            var helper = $("#helper");
            if (helper.length > 0) {
                helper.addClass("helper-transparent");
            }
        }
    });

    $(".tb-login-success").live('mouseleave', function () {
        $(this).find(".user-menu").slideUp(200, function () {
            if (!$(this).hasClass("tb-login-success-mini")) {
                var helper = $("#helper");
                if (helper.length > 0) {
                    helper.removeClass("helper-transparent");
                }
            }
        });
    });

    $(".tb-login-none-mini").live('mouseenter', function () {
        $(this).find(".user-menu").slideDown(200);
        //$("#helper").addClass("helper-transparent");
    });

    $(".tb-login-none-mini").live('mouseleave', function () {
        $(this).find(".user-menu").slideUp(200, function () {
            //$("#helper").removeClass("helper-transparent");
        });
    });

    $(".logout").live('click', function () {
        logout(logoutCallback, true);
    });

    $(".txt-logo").live('click', function () {
        gotoTopics();
    });
}