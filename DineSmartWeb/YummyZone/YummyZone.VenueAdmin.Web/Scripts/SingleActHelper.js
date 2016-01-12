function menuClicked(menuNumber) {
    if (menuNumber == 1) {
        this.window.location = "Default.aspx";
    }
}

// animation on error
function showError(message, errorStripDiv, errorTextDiv) {
    errorTextDiv.html(message);
    errorStripDiv.slideDown();
}

// hide error animation
function hideError() {
    var errorStripDiv = $(".main").find(".errorStrip");
    var errorTextDiv = errorStripDiv.find(".errorText");
    errorTextDiv.html("");
    errorStripDiv.slideUp();
}

// check text input
function checkMandatoryText(name, value, errorStripDiv, errorTextDiv) {
    value = $.trim(value);
    if (value.length <= 0) {
        showError("Please specify " + name, errorStripDiv, errorTextDiv);
        return false;
    }

    return true;
}

// call back on failure
function singleActionCallFailed() {

    // hide spinner
    $(".spinner").hide();

    // enable for re-click
    $(".main:first").removeClass("workingNow");

    // show error
    var errorStripDiv = $(".errorStrip");
    var errorTextDiv = errorStripDiv.find(".errorText");
    showError("Saving request failed. Please try again later.", errorStripDiv, errorTextDiv);
}

function singleActionPost(url, singleActionCallSucceeded, inputChecker, formCss) {
    var mainDiv = $(".main:first");

    // avoid double post
    if (mainDiv.hasClass("workingNow")) {
        return false;
    }

    // beginning from scratch: clear error                
    var errorStripDiv = mainDiv.find(".errorStrip");
    var errorTextDiv = errorStripDiv.find(".errorText");
    errorTextDiv.html("");
    errorStripDiv.hide();

    // get the form
    var form = mainDiv.find(formCss);

    // check inputs
    var formDataArray = new Array();
    if (!inputChecker(form, errorStripDiv, errorTextDiv, formDataArray)) {
        // error is already shown. no need to re-shown. just return
        return false;
    }

    // post inputs
    var options = {
        url: url,
        type: 'POST',
        success: singleActionCallSucceeded,   // post-submit callback
        error: singleActionCallFailed,
        clearForm: false,    // clear all form fields after successful submit 
        resetForm: false,    // reset the form after successful submit 
        dataType: null,
        data: formDataArray,
        timeout: 30 * 1000
    };

    // disable
    mainDiv.addClass("workingNow");

    // show spinner
    $(".spinner").show();

    // submit the form
    form.ajaxSubmit(options);

    // avoid a postback to the server
    return false;
}
