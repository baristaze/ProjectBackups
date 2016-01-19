function topicsFacebookConnectInit() {
    $(".signup-facebook").live('click', function () {
        signupToFacebook(onAuthStateChangeDefault, onServerAuthDefault);
    });

    $(".signup-modal").live('click', function () {
        signupToFacebook(onAuthStateChangeDefault, onServerAuthDefault);
        //$.unblockUI();
        return false;
    });
}

function topics_sharingTopicOnFacebookCallbackEx(topicId, entryId) {
    return function topics_sharingTopicOnFacebookCallback(response) {
        if (response && response['post_id']) {
            // user has posted something on FB
            // if ($("#isAuthenticated").text() != "0") {
            // keep track of this share
            var url = $("#applicationPath").text() + "/svc/rest/topic/entry/log/share/" + topicId + "/0/Facebook";
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

function topics_shareTopicOnFacebook($topicBox) {
    var topicLink = $topicBox.find("a.topic-link");
    var topicTitle = topicLink.text();
    var topicId = $topicBox.find(".topic-id").text();
    var seo = $topicBox.find(".topic-seo-plain").text();
    var url = getRootUrl() + "/" + seo + "?e=0#f=1";
    var imageLink = getRootUrl() + "/Images/logo.png?version=0.81";

    // calling the API ...
    var obj = {
        method: 'feed',
        name: topicTitle,
        link: url,
        picture: imageLink,
        caption: 'shebeke.net'
    };

    FB.ui(obj, topics_sharingTopicOnFacebookCallbackEx(topicId, 0));
}

function topics_shareTopicOnTwitter($topicBox) {
    var topicLink = $topicBox.find("a.topic-link");
    var topicTitle = topicLink.text();
    var topicId = $topicBox.find(".topic-id").text();
    var seo = $topicBox.find(".topic-seo-plain").text();
    var ref = getRootUrl() + "/" + seo + "?e=0#f=1";
    ref = encodeURIComponent(ref);
    // twitter shortens the URL.
    var len = 140 - 22 - 1;
    if (topicTitle.length > len) {
        topicTitle = topicTitle.substring(0, len - 3) + "...";
    }

    var link = "http://www.twitter.com/share?url=" + ref + "&text=" + encodeURIComponent(topicTitle);

    var width = 540;
    var height = 400;
    var left = ($(window).width() - width) / 2;
    var top = ($(window).height() - height) / 2;
    opts = 'status=1' + ',width=' + width + ',height=' + height + ',top=' + top + ',left=' + left;
    window.open(link, "_blank", opts);

    var url = $("#applicationPath").text() + "/svc/rest/topic/entry/log/share/" + topicId + "/0/Twitter";
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

function topicPage_Init() {
    $(".topic-link").live('click', function () {
        var url = $(this).attr("href");
        if (url) {
            window.open(url, "_blank");
        }
        return false;
    });

    $(".topic-box").live('click', function () {
        var url = $(this).find("a.topic-link").attr("href");
        if (url) {
            window.open(url, "_blank");
        }
        return false;
    });

    $(".topic-box").live('mouseover', function () {
        var url = $(this).find(".social-mini").removeClass("hidden");
        return false;
    });

    $(".topic-box").live('mouseleave', function () {
        var url = $(this).find(".social-mini").addClass("hidden");
        return false;
    });

    $(".facebook-icon-wrp-mini").live('click', function () {
        var topicBox = $(this).closest(".topic-box");
        topics_shareTopicOnFacebook(topicBox);
        return false;
    });

    $(".twitter-icon-wrp-mini").live('click', function () {
        var topicBox = $(this).closest(".topic-box");
        topics_shareTopicOnTwitter(topicBox);
        return false;
    });

    $("a.entry-writer").live('click', function () {
        var url = $(this).attr("href");
        if (url) {
            window.open(url, "_blank");
        }
        return false;
    });

    $("a.user-img-link").live('click', function () {
        var url = $(this).attr("href");
        if (url) {
            window.open(url, "_blank");
        }
        return false;
    });

    $(".promoted-topic-link").live('click', function () {
        var href = $(this).attr("href");
        var url = $("#applicationPath").text() + "/" + href;

        // we better put split id and google experiment to avoid an unnecessary redirect and utm_referer
        var currentUrl = window.location.href.toString();
        var urlParams = getUrlParameters(currentUrl);
        var splitId = getSplitId2(urlParams);
        var delim = '?';
        if (splitId > 0) {
            url += delim + "split=" + splitId;
            delim = '&';
        }

        var googleExperimentId = getGoogleExperimentId2(urlParams);
        if (googleExperimentId) {
            url += delim + "utm_expid=" + googleExperimentId;
            delim = '&';
        }

        window.open(url, "_blank");
        return false;
    });
}