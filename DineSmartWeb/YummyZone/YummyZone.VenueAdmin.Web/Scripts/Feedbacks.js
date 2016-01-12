//---------------------------------------------------------//
//              Simple Initializations
//---------------------------------------------------------//

$(document).ready(function () {
    $("#mi_fdbk").addClass("selectedLink");
});

//---------------------------------------------------------//
//              Mouse Over Highlights
//---------------------------------------------------------//

function OnMouseOverFeedback() {

    // un-highlight previous ones:
    $(".highlightedRow").removeClass("highlightedRow");

    if ($(this).hasClass("fbkUnread")) {
        $(this).removeClass("fbkUnread");
        $(this).addClass("fbkUnreadHidden");
    }

    // highlight the selected row
    $(this).addClass("highlightedRow");

    // show action menues
    $(this).find(".fbkActions:first").show();

    // scroll
    var firstItemTop = $(this).offset().top;
    var lastItemBottom = $(this).find(".reportAsSpam:first").offset().top + 20;
            
    var viewPortHeight = $(window).height();
    var scrollTop = $(window).scrollTop();
    var windowFold = viewPortHeight + scrollTop;
    var itemHeight = lastItemBottom - firstItemTop;
    if (firstItemTop >= scrollTop && itemHeight < viewPortHeight && lastItemBottom > windowFold) {
        // below the fold... scroll to ensure visibility
        $.scrollTo(scrollTop + lastItemBottom - windowFold + 2, 500, { axis: 'y' });
    }
    else if (firstItemTop < scrollTop && itemHeight < viewPortHeight && lastItemBottom < windowFold) {
        // below the fold... scroll to ensure visibility
        $.scrollTo(firstItemTop, 100, { axis: 'y' });
    }
}

function OnMouseLeaveFeedback() {
    // explicit for the unread one

    if ($(this).hasClass("fbkUnreadHidden")) {
        $(this).removeClass("fbkUnreadHidden");
        $(this).addClass("fbkUnread");
    }

    // un-highlight this:
    $(this).removeClass("highlightedRow");

    // hide action menues
    $(this).find(".fbkActions:first").hide();
}

$(document).ready(function () {

    $(".fbk").live('mouseover', OnMouseOverFeedback);
    $(".fbk").live('mouseleave', OnMouseLeaveFeedback);

    // touch devices
    if (navigator.userAgent.match(/iPhone/i) ||
        navigator.userAgent.match(/iPad/i) ||
        navigator.userAgent.match(/iPod/i)) {

        // make the list sortable to workaround hover-action issues on touch-devices
        $(".fbkList").sortable();
    }
});
    
//---------------------------------------------------------//
//              Local Time Adjustment
//---------------------------------------------------------//
function AdjustTimes() {
    var now = new Date();
    var offset = now.getTimezoneOffset();
    var now = new Date();

    var times = $(".fbkList").find(".dateTimeUTC");
    $(times).each(function () {
        var timeText = $(this).text();
        var dateTime = new Date(timeText);
        var minutes = dateTime.getMinutes();
        dateTime.setMinutes(minutes - offset);

        var difference = now.getTime() - dateTime.getTime();
        var daysDifference = Math.floor(difference / 1000 / 60 / 60 / 24);
        difference -= daysDifference * 1000 * 60 * 60 * 24;
        var hoursDifference = Math.floor(difference / 1000 / 60 / 60);
        difference -= hoursDifference * 1000 * 60 * 60;
        var minutesDifference = Math.floor(difference / 1000 / 60);
        if (minutesDifference == 0) {
            minutesDifference = 1;
        }

        var diffText = "";
        if (daysDifference > 3) {
            diffText = dateTime.format("mm/dd/yyyy hh:MM TT");
        }
        else if (daysDifference > 1) {
            diffText = daysDifference.toString() + " days ago";
        }
        else if (daysDifference == 1) {
            diffText = (hoursDifference + 24).toString() + " hours ago";
        }                
        else if (hoursDifference > 1) {
            diffText = hoursDifference.toString() + " hours ago";
        }
        else if (hoursDifference == 1 ) {
            diffText = (minutesDifference + 60).toString() + " minutes ago";
        }
        else if (minutesDifference > 1) {
            diffText = minutesDifference.toString() + " minutes ago";
        }
        else {
            diffText = "1 minute ago";
        }

        $(this).next(".dateTime:first").text(diffText);

    });
}

setInterval("AdjustTimes()", 20 * 1000);

$(document).ready(function () {
    AdjustTimes();
});

//---------------------------------------------------------//
//              Mark As
//---------------------------------------------------------//

function OnMarkAs(link, marker) {
    // get the id
    var fbk = $(link).closest(".fbk");
    var chid = $(fbk).find(".checkinId:first").text();

    // prepare the url that needs to be called
    var randomId = guidGenerator();
    var url = "FeedbackHandlers/FeedbacksMarkAs.ashx?chid=" + chid.toString() + "&mark=" + marker.toString() + "&rid=" + randomId;

    // make the call to delete the item
    var jqxhr = $.get(url, function (data) {

        if (data == "1") {
            if (marker == 1 || marker == 2) {
                fbk.toggleClass("fbkUnreadHidden");
                var fbkAction = link.closest(".fbkAction");
                fbkAction.toggle();

                if (marker == 1) {
                    fbkAction.next(".fbkAction").toggle();
                }
                else if (marker == 2) {
                    fbkAction.prev(".fbkAction").toggle();
                }
            }
            else if (marker == 3 || marker == 4) {
                fbk.fadeOut(null, function () {
                    // remove the feedback completely
                    fbk.remove();
                });
            }
        }
        else if (data.indexOf("Authenticate.ashx") >= 0) {
            window.location = "Login.aspx";
        }
        else {
            alert(data);
        }
    });
}

function OnMarkAsRead() {
    OnMarkAs($(this), 1);
}

function OnMarkAsUnRead() {
    OnMarkAs($(this), 2);
}

function OnMarkAsSpam(e) {
    // ask confirmation
    var link = e.currentTarget;

    var dlgW = 300;
    var dlgH = 180;
    var x = e.pageX - dlgW + 120;
    var y = e.pageY - (dlgH / 2) - 70;

    var scrollTop = $(window).scrollTop();
    if (y < scrollTop) {
        y = scrollTop;
    }

    $("#confirmReportingAsSpamDlg").dialog({
        position: [x, y - $(window).scrollTop()],
        title: 'Confirmation',
        resizable: true,
        height: dlgH,
        width: dlgW,
        modal: true,
        buttons: {
            OK: function () {

                // close the dialog
                $(this).dialog("close");
                OnMarkAs(link, 3);
            },

            Cancel: function () {

                // close the dialog
                $(this).dialog("close");
            }
        }
    });            
}

function OnMarkAsDeleted(e) {
    // ask confirmation
    var link = e.currentTarget;
            
    var dlgW = 280;
    var dlgH = 180;
    var x = e.pageX - dlgW + 120;
    var y = e.pageY - (dlgH / 2) - 70;

    var scrollTop = $(window).scrollTop();
    if (y < scrollTop) {
        y = scrollTop;
    }

    $("#confirmMarkingAsDeletedDlg").dialog({
        position: [x, y - $(window).scrollTop()],
        title: 'Confirmation',
        resizable: true,
        height: dlgH,
        width: dlgW,
        modal: true,
        buttons: {
            OK: function () {

                // close the dialog
                $(this).dialog("close");
                OnMarkAs(link, 4);
            },

            Cancel: function () {

                // close the dialog
                $(this).dialog("close");
            }
        }
    }); 
}

$(document).ready(function () {

    $(".markAsRead").live('click', OnMarkAsRead);
    $(".markAsUnRead").live('click', OnMarkAsUnRead);
    $(".reportAsSpam").live('click', function (e) { OnMarkAsSpam(e); });
    $(".markAsDeleted").live('click', function (e) { OnMarkAsDeleted(e); });
});


//---------------------------------------------------------//
//             Show More
//---------------------------------------------------------//

$(document).ready(function () {

    var fbkList = $(".fbkList").find(".fbk");
    if (fbkList.length < 10) {
        $(".showMoreBtn").hide();
    }
});

function GetRateItemView(rateItem) {
    var rateItemViewTemplate = $(".hiddenParts").find(".rateItem:first");
    var rateItemView = rateItemViewTemplate.clone();

    rateItemView.find(".fbkItemName:first").text(rateItem.Name);
    var rateItemVal = "Images/stars/star_" + rateItem.Value + ".png"
    rateItemView.find(".starImg:first").attr("src", rateItemVal);

    return rateItemView;
}

function GetNameValueItemView(item, itemViewTemplateName) {
    var itemViewTemplate = $(".hiddenParts").find(itemViewTemplateName);
    var itemView = itemViewTemplate.clone();

    itemView.find(".fbkItemName:first").text(item.Name);
    itemView.find(".fbkItemValue:first").text(item.Value);

    return itemView;
}

function GetFeedbackView(fbk) {
    var fbkViewTemplate = $(".hiddenParts").find(".fbk:first");
    var fbkView = fbkViewTemplate.clone();

    fbkView.find(".checkinId:first").text(fbk.CheckInId);

    var fbkActions = fbkView.find(".fbkActions:first");

    if (fbk.IsRead) {
        fbkView.removeClass("fbkUnread");
        fbkActions.find(".markAsRead:first").closest(".fbkAction").hide();
    }
    else {
        fbkActions.find(".markAsUnRead:first").closest(".fbkAction").hide();
    }

    if (fbk.IsReplied) {
        fbkView.find(".messageImg:first").removeClass("hidden");
        fbkActions.find(".replyWithMessage:first").closest(".fbkAction").hide();
    }

    if (fbk.IsCouponSent) {
        fbkView.find(".couponImg:first").removeClass("hidden");
        fbkActions.find(".replyWithCoupon:first").closest(".fbkAction").hide();
    }

    fbkView.find(".dinerType:first").text(fbk.DinerType);            
    fbkView.find(".dateTimeUTC:first").text(fbk.CheckInTimeUtcAsText);
    fbkView.find(".customerType:first").text(fbk.CustomerType);

    var rateItems = fbkView.find(".rateItems:first");
    $.each(fbk.RateItems, function (rateItemIndex, rateItem) {
        var rateItemView = GetRateItemView(rateItem);
        rateItems.append(rateItemView);
    });

    var yesNoItems = fbkView.find(".yesNoItems:first");
    $.each(fbk.YesNoItems, function (itemIndex, item) {
        var itemView = GetNameValueItemView(item, ".yesNoItem:first");
        yesNoItems.append(itemView);
    });

    var multipleChoiceItems = fbkView.find(".multipleChoiceItems:first");
    $.each(fbk.MultiChoiceItems, function (itemIndex, item) {
        var itemView = GetNameValueItemView(item, ".multiChoiceItem:first");
        multipleChoiceItems.append(itemView);
    });

    var freeFormItems = fbkView.find(".freeFormItems:first");
    $.each(fbk.FreeFormItems, function (itemIndex, item) {
        var itemView = GetNameValueItemView(item, ".multiChoiceItem:first");
        freeFormItems.append(itemView);
    });

    return fbkView;
}

function OnShowMore() {
    // get the checkin time of the item which is at the bottom of the page
    var fbkList = $(".fbkList:first");
    var dateTimeUTC = fbkList.find(".dateTimeUTC:last").text();

    // prepare the url that needs to be called
    var randomId = guidGenerator();
    var url = "FeedbackHandlers/FeedbacksGetRecentBulk.ashx?time=" + dateTimeUTC + "&rid=" + randomId;
    var jsonRequest = $.getJSON(url, function (feedbackListResponse) {

        // process the response
        if (feedbackListResponse.ErrorCode == "0") {
            // adjust show more button
            if (feedbackListResponse.Items.length < 10) {
                $(".showMoreBtn").hide();
            }

            // insert fdbks to the html
            $.each(feedbackListResponse.Items, function (fbkIndex, fbk) {
                var fbkView = GetFeedbackView(fbk);
                var fbkList = $(".fbkList:first");
                fbkList.append(fbkView);
            });

            AdjustTimes();
        }
        else {
            // process the error
            alert(feedbackListResponse.ErrorMessage);
        }
    });

    jsonRequest.error(function (jqXHR, textStatus, errorThrown) {
        if (errorThrown.indexOf("Authenticate.ashx") >= 0) {
            window.location = "Login.aspx";
        }
    });

    return false;            
}

// wire the event
$(document).ready(function () {

    $(".showMoreBtn").live('click', OnShowMore);
});

//---------------------------------------------------------//
//              Get Count of New Feedbacks 
//---------------------------------------------------------//

function CheckNewFeedbacks() {

    // get the id
    var fbk = $(".fbkList:first").find(".fbk:first");
    var chid = $(fbk).find(".checkinId:first").text();

    // prepare the url that needs to be called
    var randomId = guidGenerator();
    var url = "FeedbackHandlers/FeedbacksGetNewCount.ashx?chid=" + chid.toString() + "&rid=" + randomId;

    // make the call to get the count
    var jqxhr = $.get(url, function (data) {
        if (isNumber(data)) {
            if (data == 0) {
                $(".newFbkBtnWrapper:first").hide();
            }
            else if (data > 0) {
                $(".newFbk:first").html("click to see new feedback (" + data + ")");
                $(".newFbkBtnWrapper:first").slideDown();
            }
        }
        else if (data.indexOf("Authenticate.ashx") >= 0) {
            window.location = "Login.aspx";
        }
    });            
}

setInterval("CheckNewFeedbacks()", 30 * 1000);

//---------------------------------------------------------//
//              Send Message 
//---------------------------------------------------------//

// post-submit callback 1
function submitMessageFailed() {
    alert("Sending the message has timed out or failed unexpectedly. Please refresh the page and try again.");
}

// post-submit callback 2
function submitMessageSucceeded(responseText, statusText, xhr, $form) {
    if (statusText == "success") {
        var regexp = new RegExp("^([0-9a-fA-F]){32}$");
        if (regexp.test(responseText)) {
            var checkinId = $form.find(".checkinId:first").text();
            var fbks = $(".fbkList:first").find(".fbk"); // not first, all of them
            $.each(fbks, function (fbkIndex, fbk) {
                var tempCheckinId = $(fbk).find(".checkinId:first").text();
                if (tempCheckinId == checkinId) {
                    var messageImg = $(fbk).find(".messageImg:first");
                    messageImg.removeClass("hidden");
                    var replyWithMessage = $(fbk).find(".replyWithMessage:first");
                    var fbkAction = replyWithMessage.closest(".fbkAction");
                    fbkAction.hide();
                }
            });
        }
        else if (responseText.indexOf("Authenticate.ashx") >= 0) {
            window.location = "Login.aspx";
        }
        else {
            alert(responseText);
        }
    }
    else {
        submitMessageFailed();
    }
}

// submit
function submitMessage(msgDlg) {

    // prepare form to send
    var form = msgDlg.find("form");

    // prepare data to send...
    var messageData = new Array();

    // get checkin Id
    var checkinId = form.find(".checkinId:first").text();
    messageData["CheckinId"] = checkinId;
            
    // get summary
    var msgSubject = form.find(".msgSubjectInput:first").attr("value");
    msgSubject = $.trim(msgSubject);
    if (msgSubject.length <= 0) {
        return false;
    }
    else if (msgSubject.length > 140) {
        return false;
    }
    messageData["MsgSubject"] = msgSubject;

    // get content
    var msgDetails = form.find(".msgContentInput:first").val();
    msgDetails = $.trim(msgDetails);
    if (msgDetails.length <= 0) {
        return false;
    }
    else if (msgDetails.length > 2000) {
        return false;
    }
    messageData["MsgDetails"] = msgDetails;

    // prepare the package
    var options = {
        url: 'FeedbackHandlers/MessageSend.ashx',
        type: 'POST',
        success: submitMessageSucceeded,   // post-submit callback
        error: submitMessageFailed,
        clearForm: true,                    // clear all form fields after successful submit 
        resetForm: true,                    // reset the form after successful submit                 
        dataType: null,
        data: messageData,
        timeout: 10 * 1000
    };

    // submit the form
    form.ajaxSubmit(options);

    return true; // close the dialog or not?
}

function showMessageDialog(e) {

    // get the checkin id
    var fbk = $(this).closest(".fbk");
    var checkinId = fbk.find(".checkinId:first").text();

    var dlgW = 450;
    var dlgH = 383;

    var x = e.pageX - dlgW + 52;
    var y = e.pageY - dlgH + 24;

    var scrollTop = $(window).scrollTop();
    if (y < scrollTop) {
        y = scrollTop;
    }

    // prepare the dialog
    var msgDlgTemplate = $(".hiddenParts:first").find(".msgDlg:first");
    var msgDlg = msgDlgTemplate.clone();

    // set checkin id
    msgDlg.find(".checkinId:first").text(checkinId);

    // ask confirmation
    msgDlg.dialog({

        position: [x, y - $(window).scrollTop()],
        title: 'Message',
        resizable: true,
        height: dlgH,
        width: dlgW,
        modal: true,
        buttons: {
            Send: function () {

                // submit the message
                if (submitMessage(msgDlg)) {

                    // close the dialog
                    $(this).dialog("close");
                }
            },

            Cancel: function () {

                // close the dialog
                $(this).dialog("close");
            }
        }
    });

    return false;
}

$(document).ready(function () {
    $(".replyWithMessage").live('click', showMessageDialog);
});

//---------------------------------------------------------//
//              Send Coupon 
//---------------------------------------------------------//

// post-submit callback 1
function submitCouponFailed() {
    alert("Sending the coupon has timed out or failed unexpectedly. Please refresh the page and try again.");
}

// post-submit callback 2
function submitCouponSucceeded(responseText, statusText, xhr, $form) {
    if (statusText == "success") {
        var regexp = new RegExp("^([0-9a-fA-F]){32}$");
        if (regexp.test(responseText)) {
            var checkinId = $form.find(".checkinId:first").text();
            var fbks = $(".fbkList:first").find(".fbk"); // not first, all of them
            $.each(fbks, function (fbkIndex, fbk) {
                var tempCheckinId = $(fbk).find(".checkinId:first").text();
                if (tempCheckinId == checkinId) {
                    var couponImg = $(fbk).find(".couponImg:first");
                    couponImg.removeClass("hidden");
                    var replyWithCoupon = $(fbk).find(".replyWithCoupon:first");
                    var fbkAction = replyWithCoupon.closest(".fbkAction");
                    fbkAction.hide();
                }
            });
        }
        else if (responseText.indexOf("Authenticate.ashx") >= 0) {
            window.location = "Login.aspx";
        }
        else {
            alert(responseText);
        }
    }
    else {
        submitCouponFailed();
    }
}

// submit
function submitCoupon(cpnDlg) {

    // prepare form to send
    var form = cpnDlg.find("form");

    // prepare data to send...
    var couponData = new Array();

    // get checkin Id
    var checkinId = form.find(".checkinId:first").text();
    couponData["CheckinId"] = checkinId;

    // get summary
    var cpnTitle = form.find(".cpnSummaryInput:first").attr("value");
    cpnTitle = $.trim(cpnTitle);
    if (cpnTitle.length <= 0) {
        return false;
    }
    else if (cpnTitle.length > 140) {
        return false;
    }
    couponData["CpnTitle"] = cpnTitle;
            
    // get content
    var cpnDetails = form.find(".cpnContentInput:first").val();
    cpnDetails = $.trim(cpnDetails);            
    if (cpnDetails.length > 2000) {
        return false;
    }
    couponData["CpnDetails"] = cpnDetails;

    // get valid through
    var cpnValidity = form.find(".cpnValidThroughInput:first").attr("value");
    cpnValidity = $.trim(cpnValidity);
    if (cpnValidity.length <= 0) {
        return false;
    }
    else if (!isNumber(cpnValidity)) {
        return false;
    }

    var validity = parseInt(cpnValidity, 10);
    if (validity < 7 || validity > 120) {
        return false;
    }

    couponData["CpnValidDays"] = validity.toString();

    // prepare the package
    var options = {
        url: 'FeedbackHandlers/CouponSend.ashx',
        type: 'POST',
        success: submitCouponSucceeded,   // post-submit callback
        error: submitCouponFailed,
        clearForm: true,                    // clear all form fields after successful submit 
        resetForm: true,                    // reset the form after successful submit                 
        dataType: null,
        data: couponData,
        timeout: 10 * 1000
    };

    // submit the form
    form.ajaxSubmit(options);

    return true; // close the dialog or not?
}

function showCouponDialog(e) {

    // get the checkin id
    var fbk = $(this).closest(".fbk");
    var checkinId = fbk.find(".checkinId:first").text();

    var dlgW = 450;
    var dlgH = 383;

    var x = e.pageX - dlgW + 52;
    var y = e.pageY - dlgH + 24;

    var scrollTop = $(window).scrollTop();
    if (y < scrollTop) {
        y = scrollTop;
    }

    // prepare the dialog
    var cpnDlgTemplate = $(".hiddenParts:first").find(".cpnDlg:first");
    var cpnDlg = cpnDlgTemplate.clone();

    // set checkin id
    cpnDlg.find(".checkinId:first").text(checkinId);

    // ask confirmation
    cpnDlg.dialog({

        position: [x, y - $(window).scrollTop()],
        title: 'Coupon',
        resizable: true,
        height: dlgH,
        width: dlgW,
        modal: true,
        buttons: {
            Send: function () {

                // submit the coupon
                if (submitCoupon(cpnDlg)) {

                    // close the dialog
                    $(this).dialog("close");
                }
            },

            Cancel: function () {

                // close the dialog
                $(this).dialog("close");
            }
        }
    });

    return false;
}

$(document).ready(function () {
            
    $(".replyWithCoupon").live('click', showCouponDialog);

    $(".cpnSummaryExampleLink").live('click', function () {
        var message = "For Example:\n\n" + "10% off on your next visit,\n" +
            "$10 off on your next visit for a $30+ order,\n" + 
            "Free dessert on your next visit for a 3-course dinner, etc";
        alert(message);
    });
});