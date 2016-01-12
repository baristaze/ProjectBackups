//---------------------------------------------------------//
//              Simple Initializations
//---------------------------------------------------------//

$(document).ready(function () {
    $("#mi_sett").addClass("selectedLink");
    $(".fromWebLink:first").hide();
    $(".fromLocalFile:first").hide();
    $(".changeLogoButton:first").hide();
    $(".cancelLogoChangeLink:first").hide();
});

//---------------------------------------------------------//
//              File Selection Through Radio
//---------------------------------------------------------//

$(document).ready(function () {

    $(".imageSourceRadio").live('change', function () {
        var selectedVal = $(this).attr("value");
        var parent = $(this).closest(".imageInputWrp");
        parent.find(".fromWebLink:first").hide();
        parent.find(".fromLocalFile:first").hide();

        if (selectedVal == 1) {
            parent.find(".fromWebLink:first").show().focus();
        }
        else if (selectedVal == 2) {
            parent.find(".fromLocalFile:first").show().focus();
        }

        parent.find(".changeLogoButton:first").show();
        parent.find(".cancelLogoChangeLink:first").show();
    });
});

//---------------------------------------------------------//
//              Logo Save
//---------------------------------------------------------//

var allowedExtensions = ["jpeg", "jpg", "png", "bmp", "gif", "tif", "tiff"];

$(document).ready(function () {

    function hideInputs() {
        $(".fromWebLink:first").hide();
        $(".fromLocalFile:first").hide();
        $(".changeLogoButton:first").hide();
        $(".cancelLogoChangeLink:first").hide();

        var selection = $(".imageSourceRadio"); // not first, all of them
        $.each(selection, function (selIndex, selItem) {
            selItem.checked = false;
        });
    }

    $(".cancelLogoChangeLink").live('click', function () {
        hideInputs();
        return false;
    });

    $(".changeLogoButton").live('click', function () {

        var form = $(this).closest("#changeLogoForm");

        // validate form...
        var logoFormData = new Array();

        var imageUrlFromWeb = null;
        var imageFrom = form.find(".imageSourceRadio:checked").attr("value");
        if (imageFrom == 1) {
            imageUrlFromWeb = form.find(".fromWebLink:first").attr("value");
            imageUrlFromWeb = $.trim(imageUrlFromWeb);
            if (imageUrlFromWeb.length > 0) {
                logoFormData["LogoImageFromURL"] = imageUrlFromWeb;
            }
            else {
                return false;
            }
        }
        else if (imageFrom == 2) {
            var filePath = $(form[0]["imageFileFromLocal"]).val();
            if (filePath) {
                if (!checkFileExt(filePath, allowedExtensions)) {
                    return false;
                }

                logoFormData["IsLogoImageFromLocal"] = true;
            }
            else {
                return false;
            }
        }
        else {
            return false;
        }

        function logoUploadFailed() {
            alert("Saving the logo has timed out or failed unexpectedly. Please refresh the page and try again.");
            hideInputs();
        }

        // post-submit callback 
        function logoUploadSucceeded(responseText, statusText, xhr, $form) {
            if (statusText == "success") {
                var regexp = new RegExp("^([0-9a-fA-F]){32}$");
                if (regexp.test(responseText)) {

                    // get the logo file id
                    var savedLogoId = responseText;

                    // set it as the image source
                    var imageNode = form.find(".customerLogoImg:first");
                    var randomId = guidGenerator();
                    var fileId = "SettingsHandlers/LogoDownload.ashx?fid=" + savedLogoId.toString() + "&rid=" + randomId;
                    imageNode.attr("src", fileId);

                    hideInputs();
                    return;
                }
                else if (responseText.indexOf("Authenticate.ashx") >= 0) {
                    window.location = "Login.aspx";
                }
                else {
                    alert(responseText);
                    hideInputs();
                }
            }
            else {
                logoUploadFailed();
            }
        }

        var options = {
            url: 'SettingsHandlers/LogoUpload.ashx',
            type: 'POST',
            success: logoUploadSucceeded,   // post-submit callback
            error: logoUploadFailed,
            clearForm: true,                // clear all form fields after successful submit 
            resetForm: true,                // reset the form after successful submit                 
            dataType: null,
            data: logoFormData,
            timeout: 30 * 1000
        };

        // submit the form
        form.ajaxSubmit(options);

        // avoid a postback to the server
        return false;
    });
});

//---------------------------------------------------------//
//              Change Password
//---------------------------------------------------------//

$(document).ready(function () {

    // hide error animation
    function hideError(form) {
        var errorStripDiv = form.find(".errorStrip");
        var errorTextDiv = errorStripDiv.find(".errorText");
        errorTextDiv.html("");
        errorStripDiv.slideUp();
    }

    // animation on error
    function showError(form, message) {
        var errorStripDiv = form.find(".errorStrip");
        var errorTextDiv = errorStripDiv.find(".errorText");
        errorTextDiv.html(message);
        errorStripDiv.slideDown();
    }

    // wiring event to hide error on input focus
    $(".editBox1").focus(function () {
        var form = $(this).closest("#changePswdForm");
        hideError(form);
    });

    function showPswdArea(show) {
        if (show) {
            $(".passwordChangeLink:first").hide();
            $("#changePswdForm").slideDown();
        }
        else {
            $("#changePswdForm").slideUp(function () {
                $(".passwordChangeLink:first").show();
            });
        }
    }

    $(".passwordChangeLink").live('click', function () {
        showPswdArea(true);
        return false;
    });

    $(".cancelPasswordChangeLink").live('click', function () {
        var form = $("#changePswdForm");
        form.find("#currentPassword").attr("value", "");
        form.find("#newPassword1").attr("value", "");
        form.find("#newPassword2").attr("value", "");
        showPswdArea(false);
        return false;
    });

    $(".changePasswordButton").live('click', function () {

        var form = $(this).closest("#changePswdForm");

        // validate form...
        var currentPassword = form.find("#currentPassword:first").attr("value");
        currentPassword = $.trim(currentPassword);
        if (currentPassword.length == 0) {
            showError(form, "Please enter your current password!");
            return false;
        }

        var newPassword1 = form.find("#newPassword1:first").attr("value");
        newPassword1 = $.trim(newPassword1);
        if (newPassword1.length < 6) {
            showError(form, "New password is too short!");
            return false;
        }

        var newPassword2 = form.find("#newPassword2:first").attr("value");
        newPassword2 = $.trim(newPassword2);
        if (newPassword1 != newPassword2) {
            showError(form, "New passwords do not match!");
            return false;
        }

        function changePasswordFailed() {
            showError(form, "Changing the password has timed out or failed unexpectedly.");
        }

        // post-submit callback 
        function changePasswordSucceeded(responseText, statusText, xhr, $form) {
            if (statusText == "success") {
                if (responseText == "1") {
                    showPswdArea(false);
                }
                else if (responseText.indexOf("Authenticate.ashx") >= 0) {
                    window.location = "Login.aspx";
                }
                else {
                    showError(form, responseText);
                }
            }
            else {
                changePasswordFailed();
            }
        }

        var options = {
            url: 'SettingsHandlers/ChangePassword.ashx',
            type: 'POST',
            success: changePasswordSucceeded,   // post-submit callback
            error: changePasswordFailed,
            clearForm: true,                // clear all form fields after successful submit 
            resetForm: true,                // reset the form after successful submit                 
            dataType: null,
            data: null,
            timeout: 30 * 1000
        };

        // submit the form
        form.ajaxSubmit(options);

        // avoid a postback to the server
        return false;
    });
});


//---------------------------------------------------------//
//              Mouse Over Highlights
//---------------------------------------------------------//        

function OnMouseOverVenueMember() {

    // un-highlight previous ones:
    $(".highlightedRow").removeClass("highlightedRow");

    // highlight the selected row
    var columns = $(this).closest(".memberRow").find(".memberColumnWrp"); // all

    columns.each(function () {
        $(this).addClass("highlightedRow");
    });
}

function OnMouseLeaveVenueMember() {

    // un-highlight previous ones:
    $(".highlightedRow").removeClass("highlightedRow");
}

$(document).ready(function () {
    $(".memberColumnWrp").live('mouseover', OnMouseOverVenueMember);
    $(".memberColumnWrp").live('mouseleave', OnMouseLeaveVenueMember);
});


//---------------------------------------------------------//
//             Add New User
//---------------------------------------------------------//

// animation on error
function showError2(form, message) {
    var errorStripDiv = form.find(".errorStrip2");
    var errorTextDiv = errorStripDiv.find(".errorText2");
    errorTextDiv.html(message);
    errorStripDiv.slideDown();
}

// hide error animation
function hideError2(form) {
    var errorStripDiv = form.find(".errorStrip2");
    var errorTextDiv = errorStripDiv.find(".errorText2");
    errorTextDiv.html("");
    errorStripDiv.slideUp();
}

// check text input
function checkMandatoryText(form, name, value) {
    value = $.trim(value);
    if (value.length <= 0) {
        showError2(form, "Please specify " + name);
        return false;
    }

    return true;
}

// show new user area
function showNewUserArea(show) {
    if (show) {
        $(".addNewMemberBtn:first").hide();
        $("#newUserForm").slideDown();
    }
    else {
        $("#newUserForm").slideUp(function () {
            $(".addNewMemberBtn:first").show();
        });
    }
}

// validate form...
function addMemberFailed(form) {
    showError2(form, "Adding user has timed out or failed unexpectedly.");
}

// post-submit callback 
function addMemberSucceeded(responseText, statusText, xhr, $form) {
    if (statusText == "success") {
        if (responseText == "1") {
            // prepare the url that needs to be called
            var userEmail = $form.find("#memberEmailAddress").attr("value");
            var randomId = guidGenerator();
            var url = "SettingsHandlers/GetUser.ashx?email=" + userEmail + "&rid=" + randomId;
            var jsonRequest = $.getJSON(url, function (userResponse) {
                // process the response
                if (userResponse.ErrorCode == "0") {
                    var memberRow = $(".hiddenParts").find(".memberRow");
                    memberRow = memberRow.clone();
                    memberRow.find(".memberEmail:first").text(userResponse.User.EmailAddress);
                    memberRow.find(".memberFirstN:first").text(userResponse.User.FirstName);
                    memberRow.find(".memberLastN:first").text(userResponse.User.LastName);

                    if (!userResponse.User.IsDisabled) {
                        memberRow.find(".memberStatus:first").hide();
                    }

                    if (!userResponse.User.CanDisable) {
                        memberRow.find(".memberActDisable:first").hide();
                    }

                    if (!userResponse.User.CanEnable) {
                        memberRow.find(".memberActEnable:first").hide();
                    }

                    if (!userResponse.User.CanDelete) {
                        memberRow.find(".memberActDelete:first").hide();
                    }

                    $form.slideUp(function () {
                        $(".membersTableWrp").append(memberRow);
                        $(".addNewMemberBtn:first").show();
                    });

                }
                else {
                    // refresh the page since add operation was successfull.
                    window.location = "Settings.aspx";
                }
            });

            jsonRequest.error(function (jqXHR, textStatus, errorThrown) {
                if (errorThrown.indexOf("Authenticate.ashx") >= 0) {
                    window.location = "Login.aspx";
                }
            });
        }
        else if (responseText.indexOf("Authenticate.ashx") >= 0) {
            window.location = "Login.aspx";
        }
        else {
            showError2($form, responseText);
        }
    }
    else {
        addMemberFailed($form);
    }
}

// get new user info
function getNewUserInfo(form, formDataArray) {

    var userEmail = _selectedMemberEmail;

    if (_selectedMemberEmail.length == 0) {

        userEmail = form.find("#memberEmailAddress").attr("value");
        if (!checkMandatoryText(form, "Email Address", userEmail)) {
            return false;
        }

        var userFirstName = form.find("#memberFirstName").attr("value");
        if (!checkMandatoryText(form, "First Name", userFirstName)) {
            return false;
        }

        if (!isValidName(userFirstName)) {
            showError2(form, "No symbol is allowed in First Name");
            return false;
        }

        var userLastName = form.find("#memberLastName").attr("value");
        if (!checkMandatoryText(form, "Last Name", userLastName)) {
            return false;
        }

        if (!isValidName(userLastName)) {
            showError2(form, "No symbol is allowed in Last Name");
            return false;
        }

        var userPhone = form.find("#memberPhone").attr("value");
        userPhone = $.trim(userPhone);
        if (userPhone.length > 0) {
            if (!isValidPhoneUSA(userPhone)) {
                showError2(form, "Invalid Phone Number. Try xxx-xxx-xxxx");
                return false;
            }
        }

        userEmail = $.trim(userEmail);
        userEmail = userEmail.toLowerCase();
        if (!isValidEmailAddress(userEmail)) {
            showError2(form, "Invalid Email Address");
            return false;
        }

        var userPassword = form.find("#memberPassword").attr("value");
        if (!checkMandatoryText(form, "Password", userPassword)) {
            return false;
        }

        var pswdLenBefore = userPassword.length;
        userPassword = $.trim(userPassword);
        if (pswdLenBefore != userPassword.length) {
            showError2(form, "Password may not start or end with a space");
            return false;
        }

        if (userPassword.length < 6) {
            showError2(form, "Password is too short");
            return false;
        }

        formDataArray["userEmail"] = userEmail;
        formDataArray["userFirstName"] = userFirstName;
        formDataArray["userLastName"] = userLastName;
        formDataArray["userPhone"] = userPhone;
        formDataArray["userPassword"] = userPassword;
        return true;
    }
    else {
        formDataArray["userEmail"] = userEmail;
        return true;
    }
}

$(document).ready(function () {

    // wiring event to hide error on input focus
    $(".editBox2").focus(function () {
        var form = $(this).closest("#newUserForm");
        hideError2(form);
    });

    $(".addNewMemberBtn").live('click', function () {

        // generate a random string
        var pswd = randomString(6);
        $("#memberPassword").attr("value", pswd);

        // show
        showNewUserArea(true);
        return false;
    });

    $(".cancelNewMemberLink").live('click', function () {
        var form = $("#newUserForm");
        form.find("#memberPassword").attr("value", "");
        showNewUserArea(false);
        return false;
    });

    $(".saveUserButton").live('click', function () {

        var form = $(this).closest("#newUserForm");
        var formDataArray = new Array();
        if (!getNewUserInfo(form, formDataArray)) {
            return false;
        }

        var options = {
            url: 'SettingsHandlers/AddNewUser.ashx',
            type: 'POST',
            success: addMemberSucceeded,   // post-submit callback
            error: addMemberFailed,
            clearForm: false,                // clear all form fields after successful submit 
            resetForm: false,                // reset the form after successful submit                 
            dataType: null,
            data: formDataArray,
            timeout: 30 * 1000
        };

        // submit the form
        form.ajaxSubmit(options);

        // avoid a postback to the server
        return false;
    });
});

//---------------------------------------------------------//
//             AutoComplete Email
//---------------------------------------------------------//
var _selectedMemberEmail = "";
var _lastMemberEmailSearchQuery = null;

function showHideExistingDesc(show) {
    if (show) {
        $(".existingUserDesc").show();
        $(".forNewUser").hide();
        $(".saveUserButton").attr("value", "add user");
    }
    else {
        $(".existingUserDesc").hide();
        $(".forNewUser").show();
        $(".saveUserButton").attr("value", "create user");
    }
}

function autoCompleteSearchMemberUser(request, response) {
    // reset
    _selectedMemberEmail = "";

    showHideExistingDesc(false);
    if (_lastMemberEmailSearchQuery != null) {
        _lastMemberEmailSearchQuery.abort();
    }

    // get input
    var searchTerm = request.term;
    searchTerm = $.trim(searchTerm);
    if (searchTerm.length <= 2) {
        return false;
    }

    // prepare the url that needs to be called
    var randomId = guidGenerator();
    var url = "SettingsHandlers/SearchUser.ashx?q=" + searchTerm + "&rid=" + randomId;
    url = encodeURI(url);
    _lastMemberEmailSearchQuery = $.getJSON(url, function (searchResponse) {
        // process the response
        if (searchResponse.ErrorCode == "0") {
            // insert fdbks to the html
            if (searchResponse.Items.length > 0) {
                var dataArray = new Array();
                $.each(searchResponse.Items, function (itemIndex, entity) {
                    dataArray[itemIndex] = { 
                        label: entity.EmailAddress,
                        value: entity.EmailAddress};
                });
                // callback
                response(dataArray);
            }
            else {
                response([]); 
            }
        }
        else {
            response([]);
        }
    });

    _lastMemberEmailSearchQuery.error(function (jqXHR, textStatus, errorThrown) {
        if (errorThrown.indexOf("Authenticate.ashx") >= 0) {
            window.location = "Login.aspx";
        }
    });
}

function autoCompleteItemSelectedMemberUser(event, ui) {
    if (ui.item != null) {
        
        _selectedMemberEmail = ui.item.value;

        showHideExistingDesc(true);
    }
}

$(document).ready(function () {
    var options = { select: autoCompleteItemSelectedMemberUser, source: autoCompleteSearchMemberUser };
    $("#memberEmailAddress").autocomplete(options);
    $("#memberEmailAddress").live("focusout", function () {
        var txt = $(this).attr("value");
        if (txt.length == 0) {

            _selectedMemberEmail = "";

            showHideExistingDesc(false);
        }
    });
});

//---------------------------------------------------------//
//             Activate Deactivate
//---------------------------------------------------------//

function toggleStatus(memberRow, act) {

    var randomId = guidGenerator();
    var memberEmail = memberRow.find(".memberEmail").html();
    var url = "SettingsHandlers/ChangeUserStatus.ashx?userEmail=" + memberEmail + "&act=" + act + "&rid=" + randomId;

    var jqxhr = $.get(url, function (data) {
        if (data == 0) {
            if (act == "enable") {
                memberRow.find(".memberActEnable").hide();
                memberRow.find(".memberActDisable").show();
                memberRow.find(".memberStatus").hide();
            }
            else if (act == "disable") {
                memberRow.find(".memberActDisable").hide();
                memberRow.find(".memberActEnable").show();
                memberRow.find(".memberStatus").show();
            }
            else if (act == "delete") {
                memberRow.fadeOut();
                memberRow.fadeOut(null, function () {
                    // remove the row completely
                    memberRow.remove();
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

    // Set error function for the request above
    jqxhr.error(function () {
        alert("Request failed unexpectedly. Please try again later");
    });
}

/* activate and de-activate */
$(document).ready(function () {

    $(".memberActDisable").live('click', function () {
        toggleStatus($(this).closest(".memberRow"), "disable");
        return false;
    });

    $(".memberActEnable").live('click', function () {
        toggleStatus($(this).closest(".memberRow"), "enable");
        return false;
    });

    $(".memberActDelete").live('click', function () {
        toggleStatus($(this).closest(".memberRow"), "delete");
        return false;
    });
});