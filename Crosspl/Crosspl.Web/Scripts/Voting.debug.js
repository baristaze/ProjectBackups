function getVoteMsg(vote) {
    if (vote > 0) {
        return "Your vote: +1";
    }
    else if (vote < 0) {
        return "Your vote: -1";
    }

    return "You haven't voted yet!";
}

function voteFinished(btn) {
    var root = $(btn).closest(".main-block");
    root.find(".vote").removeAttr("disabled");
}

function voteFailedEx(btn, vote, resultArea, errorMessage) {
    return function voteFailed() {
        var doneInfo = $(resultArea).find(".act-done");
        doneInfo.text(errorMessage);
        doneInfo.show("fast", "linear", function () {
            $(this).fadeOut(2000, function () {
                var currentResult = $(resultArea).find(".data-me-current");
                currentResult.show();
                var totalResult = resultArea.find(".data-all-current");
                totalResult.show();
                voteFinished(btn);
            }); // done info faded out
        }); // done info shown
    }
}

function voteSucceededEx(btn, vote, resultArea) {
    return function voteSucceeded(response, statusText, xhr, $form) {
        var busyAction = $(resultArea).find(".act-busy");
        busyAction.hide();

        // process the result
        if (statusText == "success") {
            if (response.ErrorCode == "0") {
                // update the info
                var currentResult = $(resultArea).find(".data-me-current");
                currentResult.text(getVoteMsg(response.VotingSummary.MyVote));
                if (response.VotingSummary.MyVote != 0) {
                    currentResult.show();
                }

                var totalResult = resultArea.find(".data-all-current");
                totalResult.text(response.VotingSummary.UpvotePercentageAsText);

                toggleExclusiveButton(btn);
                voteFinished(btn);

                var doneInfo = $(resultArea).find(".act-done");
                if (response.VotingSummary.MyVote != 0) {
                    doneInfo.text("Sent successfully!");
                }
                else {
                    doneInfo.text("Reverted successfully!");
                }
                doneInfo.show("fast", "linear", function () {
                    $(this).fadeOut(1500, function () {
                        if (response.VotingSummary.MyVote == 0) {
                            currentResult.show();
                        } // it is not a revert...
                    });
                }); // done info showed

                // else, no need to say 'sent successfully'...
            } // error code = 0
            else {
                voteFailedEx(btn, vote, resultArea, response.ErrorMessage)(); //call the closure
            }
        } // status = success
        else {
            voteFailedEx(btn, vote, resultArea, "Unknown error occured while saving your vote!")();
        }
    }       // core 
} // closure function

function sendVote(btn, vote) {

    if ($(btn).attr("disabled")) {
        return;
    }

    var root = $(btn).closest(".main-block");
    root.find(".vote").attr("disabled", "disabled");

    var resultArea = root.find(".vote-result");
    var currentResult = resultArea.find(".data-me-current");
    var busyAction = resultArea.find(".act-busy");

    currentResult.hide();
    busyAction.show();

    var topicId = $("#topicId").text();
    var entryId = root.find(".entry-id").text();
    var url = $("#applicationPath").text() + "/svc/rest/vote/" + topicId + "/" + entryId + "/" + vote;

    var jsonData = null;
    var jsonText = JSON.stringify(jsonData);
    $.ajax({
        url: url,
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        data: jsonText,
        success: voteSucceededEx(btn, vote, resultArea),
        error: voteFailedEx(btn, vote, resultArea, "Unknown error occured while saving your vote!")
    });

    if (vote != 0) {
        var voteType = 'Upvote';
        if (vote < 0) {
            voteType = 'Downvote';
        }
        // don't change the Category name below. It has to match to config at Google side
        _gaq.push(['_trackEvent', 'QuickInteractCategory', 'Vote', voteType, 1, null]);
    }
}

function sendComparisonVote(btn, vote) {

    var root = $(btn).closest(".main-block");
    var topicId = root.find(".random-topic-id").text();
    var entryId = root.find(".entry-id").text();
    var url = $("#applicationPath").text() + "/svc/rest/vote/" + topicId + "/" + entryId + "/" + vote;

    var jsonData = null;
    var jsonText = JSON.stringify(jsonData);
    $.ajax({
        url: url,
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        data: jsonText,
        success: null,
        error: null
    });
}

function showBlockedComparisonDialog() {
    var dlg = $(".compare-entries-dlg").clone();
    var user = $("#currentUserName").text();
    var title = 'Hey ' + user + ', help us to pick the BETTER one!';

    var url = window.location.href.toString();
    var hashParams = getHashParameters(url);
    var senderFBId = hashParams["u"]; // keep this for backward compatibility
    var username = hashParams["n"];
    var photoUrl = hashParams["p"];
    if (username && photoUrl) {
        username = decodeURIComponent(username);
        photoUrl = decodeURIComponent(photoUrl);
        title = username + ': "Hey ' + user + ', which one is BETTER?' + '"';
        var img = dlg.find(".comparison-invite-sender-img");
        img.attr("src", photoUrl);
        img.show();
    }
    else if (username && senderFBId) {
        username = decodeURIComponent(username);
        title = username + ': "Hey ' + user + ', which one is BETTER?' + '"';
        var img = dlg.find(".comparison-invite-sender-img");
        var photoUrl = "https://graph.facebook.com/" + senderFBId + "/picture?type=small";
        img.attr("src", photoUrl);
        img.show();
    }

    dlg.find(".comparison-title").text(title);

    var css = getCssForBlockUI2('#fff', '80%', '-40%', 'absolute', '5%');
    $.blockUI(
            {
                message: dlg,
                css: css,
                overlayCSS: {
                    backgroundColor: '#000',
                    opacity: 0.92,
                    cursor:
                    'default'
                },
                onBlock: function () {
                    if ($(window).width() >= 540) {
                        equateHeights($(this), ".entry-compare");
                        equateHeights($(this), ".topic-word-comparison");
                    }
                }
            });
}

function entryPage_Init_Voting() {
    $(".vote").live('click', function () {

        if ($("#isAuthenticated").text() == "0") {
            showBlockedLoginDialog();
            return false;
        }

        var val = $(this).val().toString();
        var vote = parseInt(val);

        if ($(this).hasClass("vote-compare")) {
            sendComparisonVote(this, vote);

            var url = window.location.href.toString();
            url = removeHash(url, "f");
            url = removeHash(url, "u"); // keep this for a while for backward compatibility... what happens if a user clicks on an old link?
            url = removeHash(url, "n");
            url = removeHash(url, "p");
            var fragment = getHashPart(url);
            window.location.hash = fragment;

            $.unblockUI();

            var title = 'Facebook - Invite Friends';
            if ($(window).width() > 540) {
                title = 'Facebook - Select ALL Friends to send your PICK to!';
            }

            var mainBlock = $(this).closest(".main-block");
            var topicId = mainBlock.find(".random-topic-id").text();
            var entryId = mainBlock.find(".entry-id").text();
            retrieveFacebookFriends(null, topicId, entryId, true, title);

            // don't change the Category name below. It has to match to config at Google side
            _gaq.push(['_trackEvent', 'VoteUponCompareCategory', 'Vote', 'Upvote', 1, null]);
        }
        else {
            var isRevert = $(this).attr("pressed");
            if (isRevert) {
                vote = 0;
            }
            sendVote(this, vote);
        }

        return false;
    });
}
