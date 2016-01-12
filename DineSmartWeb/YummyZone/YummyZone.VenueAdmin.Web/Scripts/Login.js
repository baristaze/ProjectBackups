// check all inputs
function checkInputs(form, errorStripDiv, errorTextDiv, formDataArray) {

    var userEmail = form.find("#userEmail").attr("value");
    if (!checkMandatoryText("Email Address", userEmail, errorStripDiv, errorTextDiv)) {
        return false;
    }

    userEmail = $.trim(userEmail);
    userEmail = userEmail.toLowerCase();
    if (!isValidEmailAddress(userEmail)) {
        showError("Invalid Email Address", errorStripDiv, errorTextDiv);
        return false;
    }

    var userPassword = form.find("#userPassword").attr("value");
    if (!checkMandatoryText("Password", userPassword, errorStripDiv, errorTextDiv)) {
        return false;
    }

    formDataArray["userEmail"] = userEmail;
    formDataArray["userPassword"] = userPassword;

    return true;
}

// call back on success
function loginCallSucceeded(responseText, statusText, xhr, $form) {

    // hide spinner
    $(".spinner").hide();

    // enable for re-click
    $(".main:first").removeClass("workingNow");

    // process the result
    if (statusText == "success") {
        var regexp = new RegExp("^([0-9a-fA-F]){32}$");
        if (regexp.test(responseText)) {

            // copy email to other form
            var email = $("#userEmail").attr("value");
            email = $.trim(email);
            email = email.toLowerCase();
            $("#userEmail2").attr("value", email);

            // copy pswd to other form
            var pswd = $("#userPassword").attr("value");
            $("#userPassword2").attr("value", pswd);

            // post the aspx form
            document.getElementById('submitButton').click();

            return;
        }
        else {
            var errorStripDiv = $(".errorStrip");
            var errorTextDiv = errorStripDiv.find(".errorText");
            showError(responseText, errorStripDiv, errorTextDiv);
        }
    }
    else {
        singleActionCallFailed();
    }
}

function singleActionPostEx() {
    return singleActionPost('UserHandlers/Authenticate.ashx', loginCallSucceeded, checkInputs, "form#second");
}

// wire events
$(document).ready(function () {

    // set inputs if we are redirected from another page to this
    var emailHintElem = $("#emailHint");
    var emailHint = emailHintElem.html();
    if (emailHint != "") {
        emailHintElem.html("");
        $("#userEmail").attr("value", emailHint);
    }

    var errorHintElem = $("#errorHint");
    var errorHint = errorHintElem.html();
    if (errorHint != "") {
        errorHintElem.html("");
        var errorStripDiv = $(".main").find(".errorStrip");
        var errorTextDiv = errorStripDiv.find(".errorText");
        showError(errorHint, errorStripDiv, errorTextDiv);
    }

    // wiring event to hide error on input focus
    $(".editBox").focus(function () {
        hideError();
    });

    // wiring click event for Sign Up
    $(".singleGlsActBtn").click(function () {
        singleActionPostEx();
    });
});