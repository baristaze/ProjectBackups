//---------------------------------------------------------//
//              Simple Initializations
//---------------------------------------------------------//

$(document).ready(function () {
    $("#mi_cpns").addClass("selectedLink");
});

//---------------------------------------------------------//
//              Mouse Over Highlights
//---------------------------------------------------------//

function OnMouseOverCoupon() {

    // un-highlight previous ones:
    $(".highlightedRow").removeClass("highlightedRow");

    // highlight the selected row
    $(this).addClass("highlightedRow");

    // show action menues
    // disabled feature
    //$(this).find(".cpnActions:first").show();
}

function OnMouseLeaveCoupon() {

    // un-highlight this:
    $(this).removeClass("highlightedRow");

    // hide action menues
    // disabled feature
    //$(this).find(".cpnActions:first").hide();
}

$(document).ready(function () {

    $(".cpn").live('mouseover', OnMouseOverCoupon);
    $(".cpn").live('mouseleave', OnMouseLeaveCoupon);
});

//---------------------------------------------------------//
//              Get Count statistics 
//---------------------------------------------------------//

function RetrieveCpnCount(cpnType, cpnTime, offset) {

    // prepare the url that needs to be called
    var randomId = guidGenerator();
    var url = "CouponHandlers/CouponsGetCount.ashx?ctype=" + cpnType + "&ctime=" + cpnTime + "&offset=" + offset + "&rid=" + randomId;

    // make the call to get the count
    var jqxhr = $.get(url, function (data) {
        if (isNumber(data)) {

            var statSect = null;
            if (cpnType == 1) {
                statSect = $(".sentStats:first");
            }
            else if (cpnType == 2) {
                statSect = $(".redeemStats:first");
            }

            var dataDiv = statSect.find(".data");
            $(dataDiv[cpnTime]).html(data);
        }
        else if (data.indexOf("Authenticate.ashx") >= 0) {
            window.location = "Login.aspx";
        }
    });
}

$(document).ready(function () {

    var now = new Date();
    var offset = now.getTimezoneOffset();
    var i = 1;
    var j = 1;
    for (i = 1; i <= 2; i++) {
        for (j = 1; j <= 7; j++) {
            RetrieveCpnCount(i, j, offset);
        }
    }
});

//---------------------------------------------------------//
//              Convert UTC time to local 
//---------------------------------------------------------//

function AdjustUtcTimes(cpn) {
    var now = new Date();
    var offset = now.getTimezoneOffset();

    var sendDateTimeUTC = $(cpn).find(".cpnSendDateTimeUTC:first").text();
    var sendDateTime = new Date(sendDateTimeUTC);
    var minutes = sendDateTime.getMinutes();
    sendDateTime.setMinutes(minutes - offset);

    $(cpn).find(".cpnSentDate:first").text(sendDateTime.format("mm/dd/yyyy"));
    $(cpn).find(".cpnSentTime:first").text(sendDateTime.format("hh:MM TT"));

    if ($(cpn).find(".cpnRedeemInfo:first").hasClass("hidden") != true) {
        var redeemDateTimeUTC = $(cpn).find(".cpnRedeemDateTimeUTC:first").text();
        var redeemDateTime = new Date(redeemDateTimeUTC);
        var minutes = redeemDateTime.getMinutes();
        redeemDateTime.setMinutes(minutes - offset);

        $(cpn).find(".cpnRedeemtDate:first").text(redeemDateTime.format("mm/dd/yyyy"));
        $(cpn).find(".cpnRedeemTime:first").text(redeemDateTime.format("hh:MM TT"));
    }
}

$(document).ready(function () {

    var thisMonth = new Date();
    var lastMonth = new Date(thisMonth.valueOf());
    lastMonth.setMonth(thisMonth.getMonth() - 1);

    var statsRightWrp = $(".statsRightWrp:first");
    var sentStats = statsRightWrp.find(".sentStats:first");
    var redeemStats = statsRightWrp.find(".redeemStats:first");

    sentStats.find(".sinceBegOfMonthLabel:first").text("Since " + thisMonth.format("mmm") + " 1st");
    redeemStats.find(".sinceBegOfMonthLabel:first").text("Since " + thisMonth.format("mmm") + " 1st");

    sentStats.find(".lastMonthLabel:first").text("In " + lastMonth.format("mmmm"));
    redeemStats.find(".lastMonthLabel:first").text("In " + lastMonth.format("mmmm"));

    var cpnContainer = $(".cpns:first");
    var cpnList = cpnContainer.find(".cpn"); // all
    $.each(cpnList, function (cpnIndex, cpn) {
        AdjustUtcTimes(cpn);
    });
});

//---------------------------------------------------------//
//              Show More 
//---------------------------------------------------------//

function GetCouponView(cpn) {
    var cpnViewTemplate = $(".hiddenParts").find(".cpn:first");
    var cpnView = cpnViewTemplate.clone();

    cpnView.find(".cpnId:first").text(cpn.Id);
    cpnView.find(".cpnSendDateTimeUTC:first").text(cpn.SentDateTimeUtcAsText);
    if (cpn.IsRedeemed) {

        // hide new key
        cpnView.find(".cpnNew:first").addClass("hidden");

        // populate redeem date
        cpnView.find(".cpnRedeemDateTimeUTC:first").text(cpn.RedeemDateTimeUtcAsText);
    }
    else {
        // hide redeemed key
        cpnView.find(".cpnRdm:first").addClass("hidden");

        // hide redemeed dates
        cpnView.find(".cpnRedeemInfo:first").addClass("hidden");

        // hide 'see redeem details' action
        var seeRedeemInfo = cpnView.find(".seeRedeemInfo:first");
        var seeRedeemInfoAct = seeRedeemInfo.closest(".cpnAction");
        seeRedeemInfoAct.addClass("hidden");
    }

    cpnView.find(".cpnTitle:first").text(cpn.Title);
    cpnView.find(".dinerType:first").text(cpn.DinerType);
    cpnView.find(".cpnSender:first").text(cpn.SenderFullName);

    AdjustUtcTimes(cpnView);

    return cpnView;
}

function OnShowMore() {
    // get the time flag of the item which is at the bottom of the page
    var cpnList = $(".cpns:first");
    var lastCpn = cpnList.find(".cpn:last");
    var sentTimeText = lastCpn.find(".cpnSendDateTimeUTC:first").text();
    var redeemTimeText = lastCpn.find(".cpnRedeemDateTimeUTC:first").text();
    var sentTimeUTC = new Date(sentTimeText);
    var redeemTimeUTC = new Date(redeemTimeText);
    var maxTimeText = sentTimeText;
    if (redeemTimeUTC > sentTimeUTC) {
        maxTimeText = redeemTimeText;
    }

    // prepare the url that needs to be called
    var randomId = guidGenerator();
    var url = "CouponHandlers/CouponsGetRecentBulk.ashx?time=" + maxTimeText + "&rid=" + randomId;
    var jsonRequest = $.getJSON(url, function (cpnListResponse) {

        // process the response
        if (cpnListResponse.ErrorCode == "0") {
            // adjust show more button
            if (cpnListResponse.Items.length < 10) {
                $(".showMoreBtn").hide();
            }

            // insert cpns to the html
            $.each(cpnListResponse.Items, function (cpnIndex, cpn) {
                var cpnView = GetCouponView(cpn);
                cpnList.append(cpnView);
            });
        }
        else {
            // process the error
            alert(cpnListResponse.ErrorMessage);
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
