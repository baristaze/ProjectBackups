// animation on error
function showError(message) {
    var errorStripDiv = $(".errorStrip");
    var errorTextDiv = errorStripDiv.find(".errorText");
    errorTextDiv.html(message);
    errorStripDiv.slideDown();
}

// hide error animation
function hideError() {
    var errorStripDiv = $(".errorStrip");
    var errorTextDiv = errorStripDiv.find(".errorText");
    errorTextDiv.html("");
    errorStripDiv.slideUp();
}

function submitBulkDataFailed() {
    showError("Sending the bulk data has timed out or failed unexpectedly. Please try again.");
}

function submitBulkDataSucceeded(responseText, statusText, xhr, $form) {
    if (statusText == "success") {
        if (isGuid(responseText)) {
            window.location = "Menus.aspx?smid=" + responseText;
        }
        else if (responseText.indexOf("Authenticate.ashx") >= 0) {
            window.location = "Login.aspx";
        }
        else {
            showError(responseText);
        }
    }
    else {
        submitMessageFailed();
    }
}

function submitBulkData(menuId, catId, bulkData) { 
    // prepare form to send
    var form = $(".bulkInsertForm:first");

    // prepare data to send...
    var messageData = new Array();

    // set values            
    messageData["mid"] = menuId;
    messageData["mcid"] = catId;
    messageData["bulk"] = bulkData;

    // prepare the package
    var options = {
        url: 'MenuHandlers/MenuBulkInsert.ashx',
        type: 'POST',
        success: submitBulkDataSucceeded,   // post-submit callback
        error: submitBulkDataFailed,
        clearForm: false,                    // clear all form fields after successful submit?
        resetForm: false,                    // reset the form after successful submit?         
        dataType: null,
        data: messageData,
        timeout: 30 * 1000
    };

    // submit the form
    form.ajaxSubmit(options);

    return true;
}

$(document).ready(function () {

    $(".bulkInsertData").live('focus', function (e) {
        hideError();
    });

    $(".bulkInsertStartLink").live('click', function (e) {

        var bulkInsertDataArea = $(".bulkInsertDataArea:first");
        if (!bulkInsertDataArea.hasClass("hidden")) {
            return false;
        }

        var menuId = $(".selectedMenuRow").find(".menuId:first").text();
        if (menuId.length == 0) {
            alert("there must be at least one selected menu to insert into.");
            return false;
        }

        bulkInsertDataArea.slideDown(function () {
            $(".bulkInsertData:first").focus();
            var bottomMarker = $(".bottomMarker");
            $.scrollTo(bottomMarker);
        });
    });

    $(".bulkInsertCancel").live('click', function (e) {
        $(".bulkInsertDataArea:first").slideUp();
    });

    $(".bulkInsertButton").live('click', function (e) {

        var bulkData = $(".bulkInsertData:first").val();
        bulkData = $.trim(bulkData);
        if (bulkData.length == 0) {
            showError("Please enter data");
            return false;
        }

        var menuId = $(".selectedMenuRow").find(".menuId:first").text();
        if (menuId.length == 0) {
            showError("There must be at least one selected menu to insert into.");
            return false;
        }

        var leftWrapper = $(".leftWrapper:first");
        var lastCatWrapper = leftWrapper.find(".menuCategoryWrapper:last");
        var catId = lastCatWrapper.find(".menuCategoryId:first").text();

        if (catId.length == 0) {
            if (bulkData.indexOf("[C]") < 0) {
                showError("At least one category tagged with [C] is required at this time.");
                return false;
            }
        }

        submitBulkData(menuId, catId, bulkData);
        return false;
    });

});
    