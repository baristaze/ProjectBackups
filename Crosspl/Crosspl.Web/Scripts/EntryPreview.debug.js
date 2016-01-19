function trimTailingNewLine(text) {
    if (text == null) {
        return text;
    }

    while (text.length > 0 && (text[text.length - 1] == '\n' || text[text.length - 1] == '\r')) {
        text = text.substring(0, text.length - 1);
    }

    return text;
}

function getImageUrlFor(viewNameWrapped, $attachments) {
    var result = null;
    $.each($attachments, function (attachIndex, attachx) {
        var viewId = $(attachx).find(".photo-attach-view-id").text();
        viewId = "[photo-" + viewId + "]";
        if (viewId == viewNameWrapped) {
            result = $(attachx).find(".photo-attach-url").text();
            return false; /*means break*/
        }
    });

    return result;
}

function matchNextAndReplace(text, tagChar, replaceOpen, replaceClose) {
    var previous = -1;
    var tag = tagChar + tagChar;
    var isEscaped = false;
    for (var x = 0; x < text.length; x++) {
        if (text[x] == '\\') {
            isEscaped = true;
        }
        else if (isEscaped == true && text[x] == tagChar) {
            // skip... i.e. do nothing
        }
        else {
            isEscaped = false;
        }

        if (!isEscaped && x >= 1) {

            var sub = text.substring(x - 1, x + 1); // x+1 is not included // if [ab] => we are on 'b'
            if (sub == tag) {
                if (previous < 0) {
                    previous = x - 1; // do not include this, since x-1 = * and x = *
                }
                else {
                    var pre = text.substring(0, previous); // [this is ]**bold** and // previous is not included
                    var middle = text.substring(previous + 2, x - 1); // x-1 = * // do not include x-1
                    var post = "";
                    if (x != text.length - 1) {
                        post = text.substring(x + 1, text.length); // text.length is not included
                    }

                    var altered = pre + replaceOpen + middle + replaceClose + post;
                    return altered;
                }
            }
        }
    }
    return text;
}

function matchNextAndReplaceAll(text, tagChar, replaceOpen, replaceClose) {
    var continueToProcess = true;
    while (continueToProcess) {
        var altered = matchNextAndReplace(text, tagChar, replaceOpen, replaceClose);
        if (altered != text) {
            text = altered;
        }
        else {
            continueToProcess = false;
        }
    }

    return text;
}

function _updatePreviewImages($textArea, text) {
    var root = $textArea.closest(".mb-inner");
    var photoUploadSection = root.find(".photo-upload-section");
    var attachments = photoUploadSection.find(".photo-attach-wrp");
    
    var regex = new RegExp();
    text = text.replace(/\[photo\-[1-9][0-9]{0,1}\]/gi, function (matched) {
        var url = getImageUrlFor(matched, attachments);
        if (url != null) {
            return "<img class='entry-img-preview' alt='...' src='" + url + "'/>";
        }
        else {
            // return "";
            return matched;
        }
    });

    return text;
}

function escapeHtmlSpecialChars(text) {
    return $('<div/>').text(text).html();
}

function detectLinksAndExtract(text, placeHolderArray, valueArray) {
    // detect links
    var regex = new RegExp(/((((https?|ftps?)\:\/\/|www\.)[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3})|([a-zA-Z0-9\-\.]+\.(com|net|org|info|co|us|me|tv|mil)(\.tr)?))((\/|\?)\S*)?/gi);
    text = text.replace(regex, function (matched) {
        // below operation is not safe since regarding html encoding... we will encode everything at the end
        //return "<a class='entry-detected-link' target='_blank' href='javascript:void(0);'>" + matched + "</a>";

        // url-encoded
        // var fullLink = encodeURIComponent(matched); // this won't help
        var fullLink = escapeHtmlSpecialChars(matched);

        // trim
        var trimmed = matched;
        if (trimmed.length > 35) {
            trimmed = trimmed.substring(0, 32);
            trimmed += "...";
        }

        // escape
        trimmed = escapeHtmlSpecialChars(trimmed);

        // replace
        var placeholder = guidGeneratorNoDash();
        var resourceLink = $("#applicationPath").text() + "/Images/link.png";
        var value = "<a class='entry-detected-link' target='_blank' href='javascript:void(0);'>" + trimmed + "<span class='link-data hidden'>" + fullLink + "</span></a><img class='entry-detected-link-img' src='" + resourceLink + "' alt='.' />";
        placeHolderArray.push(placeholder);
        valueArray.push(value);
        return placeholder;
    });

    return text;
}

function _updatePreview($textArea, text) {
    var editBlock = $textArea.closest(".main-block");
    var previewBlock = editBlock.next(".entry-preview-block");
    var previewArea = previewBlock.find(".entry-preview");

    if ($.trim(text).length > 0) {
        
        // detect and replace links
        var placeHolderArray = new Array();
        var valueArray = new Array();
        text = detectLinksAndExtract(text, placeHolderArray, valueArray);

        // html encode the rest
        text = escapeHtmlSpecialChars(text);

        // replace image references with real ones
        text = _updatePreviewImages($textArea, text);

        // BOLD
        text = matchNextAndReplaceAll(text, '*', "<b>", "</b>");

        // UNDELINED
        text = matchNextAndReplaceAll(text, '_', "<u>", "</u>");

        // escape http:// and https:// ftp and file
        var protocolGuid = guidGeneratorNoDash();
        text = text.replace(/(https?|ftps?|file)\:\/\//gi, function (matched) { return matched.substring(0, matched.length - 2) + protocolGuid; });

        // handle italic staff
        text = matchNextAndReplaceAll(text, '/', "<i>", "</i>");

        // replace the // which were after the protocol strings like http, https, ftp, ftps, file
        text = text.replace(new RegExp(protocolGuid, "gi"), "//");

        // UPPER CASE
        text = matchNextAndReplaceAll(text, '|', "<span class='upperCase'>", "</span>");

        // re-add html links
        for(var x=0; x<placeHolderArray.length; x++){
            text = text.replace(placeHolderArray[x], valueArray[x]);
        }

        // trim
        text = $.trim(text);
        text = text.replace(new RegExp("\n", "gi"), "<br/>");

        text = "<p class='entry-paragraph'>" + text + "</p>";

        // replace the preview area
        previewArea.html(text);
        previewArea.show();
    }
    else {
        previewArea.slideUp(400, function () {
            previewArea.html("");
        });
    }
}

function updatePreview($textArea) {
    var lastProcessedData = $textArea.data("lastProcessedData");
    var currentData = $textArea.attr("value");
    if (lastProcessedData != currentData) {
        _updatePreview($textArea, currentData);
        $textArea.data("lastProcessedData", currentData);
    }
}

function entryPage_Init_Preview() {
    $(".entry-add-inp").each(function (index, textArea) {
        setInterval(function () { updatePreview($(textArea)); }, 300);
    });

    $(".entry-detected-link").live('click', function () {
        var link = $(this).find(".link-data").text();

        // does it start with an http(s) or ftp(s)?
        var regex = new RegExp(/^(https?|ftps?)/gi);
        if (!regex.test(link)) {
            // add this to let the links work properly
            link = "http://" + link;
        }

        window.open(link, "_blank");
        return false;
    });

    $(".entry-detected-link-img").live('click', function () {
        var prev = $(this).prev(".entry-detected-link");
        if (prev != null && prev != undefined && prev.length == 1) {
            var link = prev.find(".link-data").text();

            // does it start with an http(s) or ftp(s)?
            var regex = new RegExp(/^(https?|ftps?)/gi);
            if (!regex.test(link)) {
                // add this to let the links work properly
                link = "http://" + link;
            }

            window.open(link, "_blank");
            return false;
        }
    });
};