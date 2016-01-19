function getImageIdFor(viewNameWrapped, $attachments) {
    var result = null;
    $.each($attachments, function (attachIndex, attachx) {
        var viewId = $(attachx).find(".photo-attach-view-id").text();
        viewId = "[photo-" + viewId + "]";
        if (viewId == viewNameWrapped) {
            result = $(attachx).find(".photo-attach-id").text();
            return false; /*means break*/
        }
    });

    return result;
}

function replaceImageViewIDsWithRealIDs(text, $textArea, $attachmentWrp) {
    var attachments = $attachmentWrp.find(".photo-attach-wrp");
    var regex = new RegExp();
    text = text.replace(/\[photo\-[1-9][0-9]{0,1}\]/gi, function (matched) {
        var imageId = getImageIdFor(matched, attachments);
        if (imageId != null) {
            return "[photo-" + imageId + "]";
        }
        else {
            // return "";
            return matched;
        }
    });

    return text;
}

function showHideAddButtonSpinner($btn, isShow) {
    if (isShow) {
        $btn.text("");
        var resourceLink = $("#applicationPath").text() + "/Images/spinner.gif";
        $btn.html("<img class='act-busy' alt='...' src='" + resourceLink + "' /><span>&nbsp;&nbsp;adding...</span>");
    }
    else {
        $btn.html("");
        $btn.text("+ add");
    }
}

function entryAddFailedEx($textArea, $btn, $photoUploadSection, errorMessage) {
    return function entryAddFailed() {
        showBlockedErrorMessage(errorMessage);
        addingEntryFinished($textArea, $btn, $photoUploadSection);
    }
}

function highlightNewEntry($newEntryBlock) {
    var inner = $newEntryBlock.find(".mb-inner");
    inner.before("<div class='new-entry-highlight'/>");
    inner.prev().width(inner.width()).height(inner.height()).css({
        "position": "absolute",
        "background-color": "#ffff99",
        "opacity": ".7"
    }).fadeOut(3000);
}

function entryAddSucceededEx(jsonData, $textArea, $btn, $photoUploadSection, insertAtTop) {
    return function entryAddSucceeded(response, statusText, xhr, $form) {
        // process the result
        if (statusText == "success") {
            if (response.ErrorCode == "0") {

                // reset uploaded images
                var attachmentWrp = $photoUploadSection.find(".photo-attach-list");
                attachmentWrp.html("");

                // prepare the view of new entry
                var pageResources = $(".pageResources");
                var entryTemplate = pageResources.find(".entry-block");
                var newEntryBlock = entryTemplate.clone();
                newEntryBlock.find(".entry-id").text(response.Entry.Id);

                // prepare writer link
                var linkToCreator = getRootUrl() + "/user/" + response.Entry.CreatedBy;                
                var currentUrl = window.location.href.toString();
                var urlParams = getUrlParameters(currentUrl);
                var splitId = getSplitId2(urlParams);
                var googleExperimentId = getGoogleExperimentId2(urlParams);
                var delim = '?';
                if (splitId > 0) {
                    linkToCreator += delim + "split=" + splitId;
                    delim = '&';
                }
                if (googleExperimentId) {
                    linkToCreator += delim + "utm_expid=" + googleExperimentId;
                    delim = '&';
                }
                newEntryBlock.find(".user-img-link").attr("href", linkToCreator);

                newEntryBlock.find(".user-img").attr("src", response.Entry.AuthorInfo.PhotoUrl);
                newEntryBlock.find(".entry-delete-mini-btn").removeClass("hidden");
                newEntryBlock.find(".entry-delete-btn").removeClass("hidden");
                var reactionListTemplate = pageResources.find(".entry-react-list");
                var reactionList = reactionListTemplate.clone();
                var locator = newEntryBlock.find(".reactions-result");
                locator.before(response.Entry.ContentAsEncodedHtml);
                locator.after(reactionList);

                // add new entry to the view
                var content = $(".content");
                if (insertAtTop) {
                    var prev = content.find(".entry-preview-block:first");
                    prev.after(newEntryBlock);
                }
                else {
                    var next = content.find(".last-add");
                    next.before(newEntryBlock);
                }

                if (content.find(".entry-block").length >= 3) {
                    content.find(".last-add").show().next(".entry-preview-block").show();
                }

                // we cannot delete this topic anymore if it has at least one entry
                $("#topicDelete").hide();

                // enable controls
                addingEntryFinished($textArea, $btn, $photoUploadSection);

                // reset text area
                $textArea.val("");
                $textArea.trigger('autosize');
                $textArea.blur();

                // for just in case
                $("#entryAddEncourageBeFirst").hide().next("#entryAddEncourageNoFirst").show();

                // highlight newly added section
                highlightNewEntry(newEntryBlock);

            } // error code = 0
            else {
                entryAddFailedEx($textArea, $btn, $photoUploadSection, response.ErrorMessage)(); //call the closure
            }
        } // status = success
        else {
            entryAddFailedEx($textArea, $btn, $photoUploadSection, "Unknown error occured while saving your entry!")();
        }
    }       // core 
} // closure function

function addingEntryFinished($textArea, $btn, $photoUploadSection) {
    $btn.removeAttr("pressed");
    showHideAddButtonSpinner($btn, false);
    $textArea.removeAttr("disabled");
    $photoUploadSection.find(".photo-upload-start-btn").removeAttr("disabled");
    var attachmentWrp = $photoUploadSection.find(".photo-attach-list");
    attachmentWrp.find(".remove-file-btn").removeAttr("disabled", "disabled");
}

function addEntry(text, $textArea, $btn) {
    $btn.attr("pressed", "pressed");
    showHideAddButtonSpinner($btn, true);
    $textArea.attr("disabled", "disabled");

    var photoUploadSection = $btn.prev(".photo-upload-section");
    photoUploadSection.find(".photo-upload-start-btn").attr("disabled", "disabled");

    var attachmentWrp = photoUploadSection.find(".photo-attach-list");
    attachmentWrp.find(".remove-file-btn").attr("disabled", "disabled");

    // replace images
    text = replaceImageViewIDsWithRealIDs(text, $textArea, attachmentWrp);

    var mbInner = $btn.closest(".mb-inner");
    var socialSection = mbInner.find(".social-share-wrp");
    var fbCheck = socialSection.find(".social-share-check[value='1']");
    var twCheck = socialSection.find(".social-share-check[value='2']");
    var shareOnFacebook = false;
    var shareOnTwitter = false;
    if (fbCheck.attr("checked")) {
        shareOnFacebook = true;
    }
    if (twCheck.attr("checked")) {
        shareOnTwitter = true;
    }

    var insertAtTop = true;
    if (mbInner.closest(".main-block").hasClass("last-add")) {
        insertAtTop = false;
    }

    // send
    var topicId = $("#topicId").text();
    var jsonData = { "TopicId": topicId, "Content": text, "EntryId": 0, "ShareOnFacebook": shareOnFacebook, "ShareOnTwitter": shareOnTwitter };
    var jsonText = JSON.stringify(jsonData);
    var url = $("#applicationPath").text() + "/svc/rest/topic/entry/new";
    $.ajax({
        url: url,
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        data: jsonText,
        success: entryAddSucceededEx(jsonData, $textArea, $btn, photoUploadSection, insertAtTop),
        error: entryAddFailedEx($textArea, $btn, photoUploadSection, "Unknown error occured while saving your entry!")
    });

    // don't change the Category name below. It has to match to config at Google side
    _gaq.push(['_trackEvent', 'ContributionCategory', 'Entry', 'Add', 1, null]);
}

function startAddingEntry($btn) {
    if ($btn.attr("pressed")) {
        return false;
    }

    var root = $btn.closest(".mb-inner");
    var textAreaWrp = root.find(".entry-add-inp-wrp");
    var textArea = textAreaWrp.find(".entry-add-inp");
    var text = textArea.attr("value");
    text = $.trim(text);
    if (text.length > 0) {
        addEntry(text, textArea, $btn);
    }
    else {
        // showBlockedErrorMessage("You haven't entered your");
        // don't animate background color, it damages the 'focus' color
        var options1 = { borderTopColor: '#cb2026', borderLeftColor: '#cb2026', borderRightColor: '#cb2026', borderBottomColor: '#cb2026' };
        var options2 = { borderTopColor: '#ccc', borderLeftColor: '#ccc', borderRightColor: '#ccc', borderBottomColor: '#ccc' };
        textArea.animate(options1, 'slow', function () {
            $(this).stop().animate(options2, 'fast');
        });
    }
}

function loadingEntriesFinished() {
    $(".spinner-block").remove();
}

function loadingEntriesFailedEx(errorMessage) {
    return function loadingEntriesFailed() {
        showBlockedErrorMessage(errorMessage);
        loadingEntriesFinished();
    }
}

function loadingEntriesSucceeded(response, statusText, xhr, $form) {
    // process the result
    if (statusText == "success") {
        if (response.ErrorCode == "0") {

            loadingEntriesFinished();

            // prepare the view of new entry
            var pageResources = $(".pageResources");
            var entryTemplate = pageResources.find(".entry-block");
            var reactionListTemplate = pageResources.find(".entry-react-list");

            // set the locator
            var content = $(".content");
            var blockLocator = content.find(".entry-preview-block:first");
            
            // prepare helper variables
            var rootUserUrl = getRootUrl() + "/user/";
            var currentUrl = window.location.href.toString();
            var urlParams = getUrlParameters(currentUrl);
            var splitId = getSplitId2(urlParams);
            var googleExperimentId = getGoogleExperimentId2(urlParams);
            var postUrl = "";
            var delim = '?';
            if (splitId > 0) {
                postUrl += delim + "split=" + splitId;
                delim = '&';
            }
            if (googleExperimentId) {
                postUrl += delim + "utm_expid=" + googleExperimentId;
                delim = '&';
            }

            $.each(response.Entries, function (index, entry) {
                var newEntryBlock = entryTemplate.clone();

                // prepare writer link
                var linkToCreator = rootUserUrl + entry.CreatedBy + postUrl;                
                newEntryBlock.find(".user-img-link").attr("href", linkToCreator);
                newEntryBlock.find(".user-img").attr("src", entry.AuthorInfo.PhotoUrl);
                newEntryBlock.find(".entry-id").text(entry.Id);
                if (entry.CanDelete) {
                    newEntryBlock.find(".entry-delete-mini-btn").removeClass("hidden");
                    newEntryBlock.find(".entry-delete-btn").removeClass("hidden");
                }
                var reactionList = reactionListTemplate.clone();
                var reactionResultSection = newEntryBlock.find(".reactions-result");
                reactionResultSection.before(entry.ContentAsEncodedHtml);
                reactionResultSection.after(reactionList);

                // update reactions
                var myReactions = reactionResultSection.find(".data-me-current");
                myReactions.text(entry.ReactionSummary.MyReactionsAsText);
                var topReactions = reactionResultSection.find(".data-all-current");
                topReactions.text(entry.ReactionSummary.TopReactionsAsText);

                $.each(entry.ReactionSummary.MyReactions, function (arrIndex, myReactId) {
                    var rb = reactionList.find(".entry-react[value=" + myReactId.toString() + "]");
                    rb.attr("pressed", "pressed");
                });

                // update votes
                var voteResultSection = newEntryBlock.find(".vote-result");
                var myVote = voteResultSection.find(".data-me-current");
                myVote.text(getVoteMsg(entry.VotingSummary.MyVote));
                if (entry.VotingSummary.MyVote > 0) {
                    newEntryBlock.find(".vote[value=1]").attr("pressed", "pressed");
                }
                else if (entry.VotingSummary.MyVote < 0) {
                    newEntryBlock.find(".vote[value=-1]").attr("pressed", "pressed");
                }

                var upvotePercentage = voteResultSection.find(".data-all-current");
                upvotePercentage.text(entry.VotingSummary.UpvotePercentageAsText);
                upvotePercentage.show();

                // add the block to the view
                blockLocator.after(newEntryBlock);
                blockLocator = newEntryBlock;
            });

            $("#topicInvite").show();

            if (response.Entries.length >= 3) {
                content.find(".last-add").show().next(".entry-preview-block").show();
            }
            else if (response.Entries.length == 0) {
                $("#entryAddEncourageBeFirst").show().next("#entryAddEncourageNoFirst").hide();
                var txtArea = content.find(".entry-add-inp:first");
                var txt = txtArea.text();
                if ($.trim(txt).length == 0) {
                    txtArea.val("\n\n\n");
                    txtArea.trigger('autosize');
                    txtArea.focus();
                }
            }

        } // error code = 0
        else {
            loadingEntriesFailedEx(response.ErrorMessage)(); //call the closure
        }
    } // status = success
    else {
        loadingEntriesFailedEx("Entries couldn't be loaded. Please refresh the page!")();
    }
} // core 

function loadInitialEntries() {
    // load
    var topicId = $("#topicId").text();
    if (topicId.length == 0) {
        return;
    }

    $(".spinner-block").show()
    $(".entry-add:first").show();

    var entryId = getEntryIdFromUrl();

    //var url = $("#applicationPath").text() + "/svc/rest/topic/entries/latest/" + topicId + "/50/0?e=" + entryId;
    var url = $("#applicationPath").text() + "/svc/rest/topic/entries/byvote/" + topicId + "/50/0?e=" + entryId;
    $.ajax({
        url: url,
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        data: null,
        success: loadingEntriesSucceeded, // no paranthesis here since we are not calling the closure. this is a function pointer
        error: loadingEntriesFailedEx("Entries couldn't be loaded. Please refresh the page!")
    });
}

function entryDeleteFailedEx($btn, errorMessage) {
    return function entryDeleteFailed() {
        $btn.removeAttr("disabled");
        showBlockedErrorMessage(errorMessage);
    }
}

function entryDeleteSucceededEx($btn, $mainBlock) {
    return function entryDeleteSucceeded(response, statusText, xhr, $form) {
        // process the result
        if (statusText == "success") {
            if (response.ErrorCode == "0") {
                $btn.removeAttr("disabled");
                $mainBlock.fadeOut('slow', function () {
                    var content = $(this).closest(".content");
                    var entryBlocks = content.find(".entry-block");
                    if (entryBlocks.length <= 3) { // including this one
                        content.find(".last-add").hide().next(".entry-preview-block").hide();
                    }

                    if (entryBlocks.length <= 1) // including this one
                    {
                        if ($("#canDeleteIfNoEntry").text() == "1") {
                            $("#topicDelete").show();
                        }

                        $("#entryAddEncourageBeFirst").show().next("#entryAddEncourageNoFirst").hide();
                        var txtArea = content.find(".entry-add-inp:first");
                        var txt = txtArea.text();
                        if ($.trim(txt).length == 0) {
                            txtArea.val("\n\n\n");
                            txtArea.trigger('autosize');
                            txtArea.focus();
                        }
                    }

                    // remove completely from UI
                    $(this).remove();
                });
            } // error code = 0
            else {
                entryDeleteFailedEx($btn, response.ErrorMessage)(); //call the closure
            }
        } // status = success
        else {
            entryDeleteFailedEx($btn, "Unknown error occured while deleting the entry!")();
        }
    }               // core 
} // closure function

function deleteEntry($btn) {

    if ($btn.attr("disabled")) {
        return;
    }

    showBlockedConfirmDialog("Are you sure to delete this entry?", function (dlgRes) {
        if (dlgRes == 1) {

            $btn.attr("disabled", "disabled");

            var topicId = $("#topicId").text();
            var mainBlock = $btn.closest(".main-block");
            var entryId = mainBlock.find(".entry-id").text();
            var url = $("#applicationPath").text() + "/svc/rest/topic/entry/mark/" + topicId + "/" + entryId + "/delete";

            $.ajax({
                url: url,
                type: "POST",
                dataType: "json",
                contentType: "application/json",
                data: null,
                success: entryDeleteSucceededEx($btn, mainBlock),
                error: entryDeleteFailedEx($btn, "Unknown error occured while deleting the entry!")
            });
        }
    });
}

function loadingRecommendedTopicsSucceeded(response, statusText, xhr, $form) {
    // process the result
    if (statusText == "success") {
        if (response.ErrorCode == "0") {

            var relatedTopicsTitle = $("#relatedTopicsTitle");
            if (response.Items.length > 0) {
                relatedTopicsTitle.show();
                var relatedTopicList = relatedTopicsTitle.next(".related");
                var baseUrl = getRootUrl() + "/";
                // we better put split id and google experiment to avoid an unnecessary redirect and utm_referer
                var currentUrl = window.location.href.toString();
                var urlParams = getUrlParameters(currentUrl);
                var splitId = getSplitId2(urlParams);
                var googleExperimentId = getGoogleExperimentId2(urlParams);
                // go through each item
                $.each(response.Items, function (index, topic) {
                    // prepare topic url
                    var relTopicUrl = baseUrl + topic.SeoLink;
                    var delim = '?';
                    if (splitId > 0) {
                        relTopicUrl += delim + "split=" + splitId;
                        delim = '&';
                    }
                    if (googleExperimentId) {
                        relTopicUrl += delim + "utm_expid=" + googleExperimentId;
                        delim = '&';
                    }

                    // prepare UI element
                    var relTopicLinkWrp = "<a href='" + relTopicUrl + "' class='related-topic-link'>" + topic.Title + "</a>"
                    var rt = "<li class='related-topic act txt-lrx'>" + relTopicLinkWrp + " (<span class='topic-entry-count'>" + topic.EntryCount + "</span>)<span class='rel-topic-seo-link hidden'>" + topic.SeoLink + "</span></li>";
                    relatedTopicList.append(rt);
                });
            } // lenght > 0

        } // error code = 0
        else {
            loadingRecommendedTopicsFailed();
        }
    } // status = success
    else {
        loadingRecommendedTopicsFailed();
    }
} // core 

function loadRecommendedTopics() {
    // load
    var topicId = $("#topicId").text();
    if (topicId.length == 0) {
        return;
    }

    var url = $("#applicationPath").text() + "/svc/rest/topic/" + topicId + "/related";
    $.ajax({
        url: url,
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        data: null,
        success: loadingRecommendedTopicsSucceeded,
        error: null
    });
}

function getEntryIdFromUrl() {
    var entryId = -1;
    var url = window.location.href.toString();
    var urlParams = getUrlParameters(url);
    var entryParam = urlParams["e"];
    if (entryParam !== undefined) {
        if (isInteger(entryParam)) {
            entryId = parseInt(entryParam);
        }
    }
    return entryId;
}

function entryPage_Init_AddGetDelete(){
    $(".entry-add-btn").live('click', function () {

        if ($("#isAuthenticated").text() == "0") {
            showBlockedLoginDialog();
            return false;
        }

        startAddingEntry($(this));
        return false;
    });

    $(".entry-delete-btn").live('click', function () {

        if ($("#isAuthenticated").text() == "0") {
            showBlockedLoginDialog();
            return false;
        }

        deleteEntry($(this));
        return false;
    });

    $(".entry-delete-mini-btn").live('click', function () {

        if ($("#isAuthenticated").text() == "0") {
            showBlockedLoginDialog();
            return false;
        }

        deleteEntry($(this));
        return false;
    });

    loadInitialEntries();
    loadRecommendedTopics();
}