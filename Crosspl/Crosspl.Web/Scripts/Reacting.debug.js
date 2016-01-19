function reactingFinished(btn) {
    var root = $(btn).closest(".main-block");
    root.find(".entry-react").removeAttr("disabled");
}

function reactingFailedEx(btn, reactionId, resultArea, errorMessage) {
    return function reactingFailed() {
        var doneInfo = $(resultArea).find(".act-done");
        doneInfo.text(errorMessage);
        doneInfo.show("fast", "linear", function () {
            $(this).fadeOut(2000, function () {
                var currentResult = $(resultArea).find(".data-me-current");
                currentResult.show();
                var totalResult = resultArea.find(".data-all-current");
                totalResult.show();
                reactingFinished(btn);
            }); // done info faded out
        }); // done info shown
    }
}

function reactingSucceededEx(btn, reactionId, resultArea) {
    return function reactingSucceeded(response, statusText, xhr, $form) {
        var busyAction = $(resultArea).find(".act-busy");
        busyAction.hide();

        // process the result
        if (statusText == "success") {
            if (response.ErrorCode == "0") {
                // update the info
                var currentResult = $(resultArea).find(".data-me-current");
                currentResult.text(response.ReactionSummary.MyReactionsAsText);
                if (response.ReactionSummary.MyReactionsAsText.length != 0) {
                    currentResult.show();
                }

                var totalResult = resultArea.find(".data-all-current");
                if (response.ReactionSummary.TopReactionsAsText.length != 0) {
                    totalResult.text(response.ReactionSummary.TopReactionsAsText);
                    totalResult.show();
                }
                else {
                    totalResult.text("");
                    totalResult.hide();
                }

                toggleButton(btn);
                reactingFinished(btn);

                var doneInfo = $(resultArea).find(".act-done");
                //if (response.CurrentReactions.length != 0) { // there could be previous many + revert this ???
                if (reactionId > 0) {
                    doneInfo.text("Updated successfully!");
                }
                else {
                    doneInfo.text("Reverted successfully!");
                }
                doneInfo.show("fast", "linear", function () {
                    $(this).fadeOut(1500, function () {
                        //if (response.CurrentReactions.length == 0) { // see above
                        if (reactionId < 0) {
                            currentResult.show();
                        } // it is not a revert...
                    });
                }); // done info showed

                // else, no need to say 'sent successfully'...
            } // error code = 0
            else {
                reactingFailedEx(btn, reactionId, resultArea, response.ErrorMessage)(); //call the closure
            }
        } // status = success
        else {
            reactingFailedEx(btn, reactionId, resultArea, "Unknown error occured while saving your reaction!")();
        }
    }        // core 
}  // closure function

function sendReaction(btn, reactionId) {

    if ($(btn).attr("disabled")) {
        return;
    }

    var root = $(btn).closest(".main-block");
    root.find(".entry-react").attr("disabled", "disabled");

    var resultArea = root.find(".reactions-result");
    var currentResult = resultArea.find(".data-me-current");
    var busyAction = resultArea.find(".act-busy");

    currentResult.hide();
    busyAction.show();

    var topicId = $("#topicId").text();
    var entryId = $(btn).closest(".main-block").find(".entry-id").text();
    var url = $("#applicationPath").text() + "/svc/rest/react/" + topicId + "/" + entryId + "/" + reactionId;

    var jsonData = null;
    var jsonText = JSON.stringify(jsonData);
    $.ajax({
        url: url,
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        data: jsonText,
        success: reactingSucceededEx(btn, reactionId, resultArea),
        error: reactingFailedEx(btn, reactionId, resultArea, "Unknown error occured while saving your reaction!")
    });

    if (reactionId > 0) {
        // don't change the Category name below. It has to match to config at Google side
        _gaq.push(['_trackEvent', 'QuickInteractCategory', 'React', 'Reaction_' + reactionId.toString(), 1, null]);
    }
}

function entryPage_Init_Reacting() {
    $(".entry-react").live('click', function () {

        if ($("#isAuthenticated").text() == "0") {
            showBlockedLoginDialog();
            return false;
        }

        var val = $(this).val().toString();
        var reactionId = parseInt(val);
        var isRevert = $(this).attr("pressed");
        if (isRevert) {
            reactionId *= -1;
        }
        sendReaction(this, reactionId);
        return false;
    });
}
