function autoCompleteSearchSucceededEx(searchTerm, responseToUI) {
    return function autoCompleteSearchSucceeded(response, statusText, xhr, $form) {
        // process the result
        if (statusText == "success") {
            if (response.ErrorCode == "0") {
                // add current results
                var index = 0;
                var dataArray = new Array();
                var offerAddNew = true;
                var searchTermLower = searchTerm.toLowerCase();
                $.each(response.Topics, function (itemIndex, topic) {
                    dataArray[index++] = { label: topic.Title, value: topic.Title, objectLink: topic.SeoLink, objectId: topic.Id };
                    if (searchTermLower == topic.Title.toLowerCase()) {
                        offerAddNew = false;
                    }
                });

                // add not found stuff
                if (response.Topics.length <= 0) {
                    dataArray[index++] = { label: "Sonuç bulunamadı", value: searchTerm, objectId: -200 };
                }

                // add 'add new' stuff
                if (offerAddNew) {
                    dataArray[index++] = { label: "Ekle", value: searchTerm, objectId: -100 };
                }

                // add them all to the UI
                responseToUI(dataArray);

            } // error code = 0
            else {
                responseToUI([]);
            }
        } // status = success
        else {
            responseToUI([]);
        }
    }        // core     
}

function autoCompleteSearchTopic(requestByUI, responseToUI) {
    if (_lastTopicSearchQuery != null) {
        _lastTopicSearchQuery.abort();
    }

    // get input
    var searchTerm = requestByUI.term;
    searchTerm = $.trim(searchTerm);
    if (searchTerm.length <= 2) {
        return false;
    }

    // prepare url
    var url = $("#applicationPath").text() + "/svc/rest/topic/search/" + searchTerm;
    url = encodeURI(url);

    // send search request
    _lastTopicSearchQuery = $.ajax({
        url: url,
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        data: null,
        success: autoCompleteSearchSucceededEx(searchTerm, responseToUI),
        error: null
    });

    //_lastTopicSearchQuery.fail(function (jqXHR, textStatus, errorThrown) {
    // alert("search failed"); // abort calls this, too
    //});
}

function renderAutoCompleteItem(ul, item) {
    // wrapping staff by an '<a>' tag is important
    if (item.objectId == -100) {
        return $("<li class='ui-menu-item ui-add-new'></li>")
                      .data("item.autocomplete", item)
                      .append("<a><span class='search-add-new-txt'>" + item.value + "</span><button class='rnd-crn search-add-new-btn'>+ bu başlığı ekle</button></a>")
                      .appendTo(ul);
    }
    else if (item.objectId == -200) {
        // wrapping staff by an '<a>' tag is important
        return $("<li class='ui-menu-item'></li>")
                          .data("item.autocomplete", item)
                          .append("<a><span class='search-no-result'>Sonuç bulunamadı</span></a>")
                          .appendTo(ul);
    }

    // wrapping staff by an '<a>' tag is important
    return $("<li class='ui-menu-item'></li>")
                  .data("item.autocomplete", item)
                  .append("<a>" + item.label + "</a>")
                  .appendTo(ul);
}

function autoCompleteItemFocused(event, ui) {
    var items = $(".ui-autocomplete").find(".ui-menu-item");
    items.removeClass("ui-state-focus");
    $.each(items, function (index, item) {
        var data = $(item).data();
        if (data.uiAutocompleteItem.objectId == ui.item.objectId) {
            $(item).addClass("ui-state-focus");
        }
    });
}

function topicAddFailedEx(errorMessage) {
    return function topicAddFailed() {
        $.unblockUI();
        showBlockedErrorMessage(errorMessage);
    }
}

function topicAddSucceededEx(jsonData) {
    return function topicAddSucceeded(response, statusText, xhr, $form) {
        // process the result
        if (statusText == "success") {
            if (response.ErrorCode == "0") {
                $.unblockUI();
                var url = $("#applicationPath").text() + "/" + response.Data.SeoLink;
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

                window.location = url;
            } // error code = 0
            else {
                topicAddFailedEx(response.ErrorMessage)(); //call the closure
            }
        } // status = success
        else {
            topicAddFailedEx("Başlık kaydedilirken beklenmeyen bir hata oluştu!")();
        }
    }     // core 
} // closure

function sendAddTopicRequest(topicTitle, shareOnFacebook, shareOnTwitter) {
    // create new
    var jsonData = { "Title": topicTitle, "Id": 0, "ShareOnFacebook": shareOnFacebook, "ShareOnTwitter": shareOnTwitter };
    var jsonText = JSON.stringify(jsonData);
    var url = $("#applicationPath").text() + "/svc/rest/topic/new";
    $.ajax({
        url: url,
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        data: jsonText,
        success: topicAddSucceededEx(jsonData),
        error: topicAddFailedEx("Başlık kaydedilirken beklenmeyen bir hata oluştu!")
    });

    // don't change the Category name below. It has to match to config at Google side
    _gaq.push(['_trackEvent', 'ContributionCategory', 'Topic', 'Add', 1, null]);
}

function autoCompleteItemSelectedTopic(event, ui) {
    if (ui.item != null) {
        if (ui.item.objectId > 0) {
            var url = $("#applicationPath").text() + "/" + ui.item.objectLink;

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

            window.location = url;
        }
        else if (ui.item.objectId == -100) {

            if ($("#isAuthenticated").text() == "0") {
                showBlockedLoginDialog();
                return;
            }

            showNewTopicDialog(ui.item.value);
        }
    }
}

function watermarkOnBlur(input) {
    if ($(input).val() == "") {
        $(input).addClass("semi-trans");
    }
    else {
        $(input).removeClass("semi-trans");
    }
}

function topicDeleteFailedEx(errorMessage) {
    return function topicDeleteFailed() {
        showBlockedErrorMessage(errorMessage);
    }
}

function topicDeleteSucceeded(response, statusText, xhr, $form) {
    // process the result
    if (statusText == "success") {
        if (response.ErrorCode == "0") {
            // var url = $("#applicationPath").text();
            var url = getRootUrl() + "/latest";

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

            window.location = url;
        } // error code = 0
        else {
            topicDeleteFailedEx(response.ErrorMessage)(); //call the closure
        }
    } // status = success
    else {
        topicDeleteFailedEx("Başlık silinirken beklenmeyen bir hata oluştu!")();
    }
}

function deleteTopic() {
    showBlockedConfirmDialog("Bu başlığı silmek istediğinizden emin misiniz?", function (dlgRes) {
        if (dlgRes == 1) {
            var topicId = $("#topicId").text();
            var url = $("#applicationPath").text() + "/svc/rest/topic/mark/" + topicId + "/delete";
            $.ajax({
                url: url,
                type: "POST",
                dataType: "json",
                contentType: "application/json",
                data: null,
                success: topicDeleteSucceeded,
                error: topicDeleteFailedEx("Başlık silinirken beklenmeyen bir hata oluştu!")
            });
        }
    });
}

function showNewTopicDialog(topicTitle) {
    $("#newTopicInput").val(topicTitle);
    $("#newTopicDialogResult").text("-1");
    var css = getCssForBlockUI('#fff');
    $.blockUI(
    {
        message: $("#newTopicDialog"),
        css: css,
        overlayCSS: { backgroundColor: '#000', opacity: 0.8, cursor: 'default' }
    });
}

function entryPage_Init_TopicSearchAddDelete() {
    var options = { select: autoCompleteItemSelectedTopic, source: autoCompleteSearchTopic, focus: autoCompleteItemFocused };
    $("#searchTopicBox").autocomplete(options).data('autocomplete')._renderItem = renderAutoCompleteItem;

    $("#searchTopicBox").live('blur', function () {
        watermarkOnBlur(this);
    });

    $("#newTopicAddBtn").live('click', function () {
        var topicTitle = $("#newTopicInput").val();
        topicTitle = $.trim(topicTitle);
        if (topicTitle.length > 0) {

            var dlg = $(this).closest(".dialog");
            var fbCheck = dlg.find(".social-share-check[value='1']");
            var twCheck = dlg.find(".social-share-check[value='2']");
            var shareOnFacebook = false;
            var shareOnTwitter = false;
            if (fbCheck.attr("checked")) {
                shareOnFacebook = true;
            }
            if (twCheck.attr("checked")) {
                shareOnTwitter = true;
            }

            sendAddTopicRequest(topicTitle, shareOnFacebook, shareOnTwitter);
        }
        return false;
    });

    $("#topicDelete").live('click', function () {

        if ($("#isAuthenticated").text() == "0") {
            showBlockedLoginDialog();
            return false;
        }

        deleteTopic();
        return false;
    });
    
    /*        
    $(".related-topic").live('click', function () {
        var seo = $(this).find(".rel-topic-seo-link").text();
        var url = $("#applicationPath").text() + "/" + seo;
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

        window.location = url;
        return false;
    });
    */
}