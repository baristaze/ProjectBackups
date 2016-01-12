// animation on success
function showThanks() {
    $("form").hide();
    $("#signUpSect").hide();
    $("#animationHelper").show();
    $("#animationHelper").slideUp();
    $("#thanks").fadeIn();
};

// check all inputs
function checkInputs(form, errorStripDiv, errorTextDiv, formDataArray) {

    var venueName = form.find("#venueName").attr("value");
    if (!checkMandatoryText("Restaurant Name", venueName, errorStripDiv, errorTextDiv)) {
        return false;
    }

    if (venueName.indexOf(";") >= 0) {
        showError("Symbol ';' is not allowed in Venue Name", errorStripDiv, errorTextDiv);
        return false;
    }

    var venueAddressLine1 = form.find("#venueAddressLine1").attr("value");
    if (!checkMandatoryText("Restaurant Address", venueAddressLine1, errorStripDiv, errorTextDiv)) {
        return false;
    }

    var venueAddressLine2 = form.find("#venueAddressLine2").attr("value");

    var venueAddressCity = form.find("#venueAddressCity").attr("value");
    if (!checkMandatoryText("City", venueAddressCity, errorStripDiv, errorTextDiv)) {
        return false;
    }

    if (!isValidName(venueAddressCity)) {
        showError("No symbol is allowed in City", errorStripDiv, errorTextDiv);
        return false;
    }

    var venueAddressState = form.find("#venueAddressState").attr("value");
    if (!checkMandatoryText("State", venueAddressState, errorStripDiv, errorTextDiv)) {
        return false;
    }

    venueAddressState = venueAddressState.toUpperCase();
    if (!isValidState(venueAddressState)) {
        showError("Invalid State", errorStripDiv, errorTextDiv);
        return false;
    }

    var venueAddressZip = form.find("#venueAddressZip").attr("value");
    if (!checkMandatoryText("Zip Code", venueAddressZip, errorStripDiv, errorTextDiv)) {
        return false;
    }

    venueAddressZip = $.trim(venueAddressZip);
    if (!isValidZipCode(venueAddressZip)) {
        showError("Invalid Zip Code format", errorStripDiv, errorTextDiv);
        return false;
    }

    var userFirstName = form.find("#userFirstName").attr("value");
    if (!checkMandatoryText("First Name", userFirstName, errorStripDiv, errorTextDiv)) {
        return false;
    }

    if (!isValidName(userFirstName)) {
        showError("No symbol is allowed in First Name", errorStripDiv, errorTextDiv);
        return false;
    }

    var userLastName = form.find("#userLastName").attr("value");
    if (!checkMandatoryText("Last Name", userLastName, errorStripDiv, errorTextDiv)) {
        return false;
    }

    if (!isValidName(userLastName)) {
        showError("No symbol is allowed in Last Name", errorStripDiv, errorTextDiv);
        return false;
    }

    var userPhone = form.find("#userPhone").attr("value");
    if (!checkMandatoryText("Phone Number", userPhone, errorStripDiv, errorTextDiv)) {
        return false;
    }

    userPhone = $.trim(userPhone);
    if (!isValidPhoneUSA(userPhone)) {
        showError("Invalid Phone Number. Try xxx-xxx-xxxx", errorStripDiv, errorTextDiv);
        return false;
    }

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

    var pswdLenBefore = userPassword.length;
    userPassword = $.trim(userPassword);
    if (pswdLenBefore != userPassword.length) {
        showError("Password may not start or end with a space", errorStripDiv, errorTextDiv);
        return false;
    }

    if (userPassword.length < 6) {
        showError("Password is too short", errorStripDiv, errorTextDiv);
        return false;
    }

    var userPasswordRepeat = form.find("#userPasswordRepeat").attr("value");
    if (!checkMandatoryText("Password again", userPasswordRepeat, errorStripDiv, errorTextDiv)) {
        return false;
    }

    if (userPasswordRepeat != userPassword) {
        showError("Passwords don't match", errorStripDiv, errorTextDiv);
        return false;
    }

    formDataArray["venueName"] = venueName;
    formDataArray["venueAddressLine1"] = venueAddressLine1;
    formDataArray["venueAddressLine2"] = venueAddressLine2;
    formDataArray["venueAddressCity"] = venueAddressCity;
    formDataArray["venueAddressState"] = venueAddressState;
    formDataArray["venueAddressZip"] = venueAddressZip;
    formDataArray["userFirstName"] = userFirstName;
    formDataArray["userLastName"] = userLastName;
    formDataArray["userPhone"] = userPhone;
    formDataArray["userEmail"] = userEmail;
    formDataArray["userPassword"] = userPassword;

    return true;
}

// call back on success
function signupCallSucceeded(responseText, statusText, xhr, $form) {

    // hide spinner
    $(".spinner").hide();

    // enable for re-click
    $(".main:first").removeClass("workingNow");

    // process the result
    if (statusText == "success") {
        var regexp = new RegExp("^([0-9a-fA-F]){32}$");
        if (regexp.test(responseText)) {
            showThanks();
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

// wire events
$(document).ready(function () {

    // wiring event to hide error on input focus
    $(".editBox").focus(function () {
        hideError();
    });

    // wiring click event for Sign Up
    $(".singleGlsActBtn").click(function () {
        return singleActionPost('UserHandlers/RegisterVenue.ashx', signupCallSucceeded, checkInputs, "form");
    });
});
