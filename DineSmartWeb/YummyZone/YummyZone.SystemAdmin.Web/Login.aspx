<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="YummyZone.SystemAdmin.Web.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Dine Smart 365 - Management Portal</title>

    <link href="Styles/Site.css?version=1.04" rel="stylesheet" type="text/css" />

    <script type="text/javascript" src="Scripts/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui-1.8.12.custom.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery.form.js"></script>
    <script type="text/javascript" src="Scripts/common.js?version=1.04"></script>

    <style type="text/css" media="screen">
        .credSection{width:280px; margin-top:120px;}
        .actionSection{width:280px; margin-top:6px;}
        .inputLine{margin-top:4px;}
        .labelWrp{float:left; width:100px; text-align:right;}
        .inputWrp{float:right;width:178px;margin-left:2px;}
        input#emailInput{width:170px;}
        input#passwordInput{width:170px;}
        .errorStrip{float:right;}
        .loginButtonWrp{float:right;}
        .errorText{color:Red; margin-right:10px;margin-top:2px;}
    </style>

    <script type="text/javascript">

        function hideError() {
            showHideError(false, true, "");
        }

        function showHideError(show, animate, msg) {
            var errorStripDiv = $(".errorStrip");
            var errorTextDiv = errorStripDiv.find(".errorText");

            if (show) {

                if (msg) {
                    errorTextDiv.text(msg);
                }

                if (animate) {
                    errorStripDiv.slideDown();
                }
                else {
                    errorStripDiv.show();
                }
            }
            else {
                
                errorTextDiv.text("");

                if (animate) {
                    errorStripDiv.slideUp();
                }
                else {
                    errorStripDiv.hide();
                }
            }
        }

        function focusToPswd() {
            $("input#passwordInput").focus();
        }

        function checkInputs(form, formDataArray) {

            var userEmail = form.find("#emailInput").attr("value");
            userEmail = $.trim(userEmail);
            if (userEmail.length <= 0) {
                showHideError(true, true, "Please specify Email Address");
                return false;
            }

            userEmail = userEmail.toLowerCase();
            if (!isValidEmailAddress(userEmail)) {
                showHideError(true, true, "Invalid Email Address");
                return false;
            }

            var userPassword = form.find("#passwordInput").attr("value");
            userPassword = $.trim(userPassword);
            if (userPassword.length <= 0) {
                showHideError(true, true, "Please specify Password");
                return false;
            }

            formDataArray["userEmail"] = userEmail;
            formDataArray["userPassword"] = userPassword;

            return true;
        }

        // call back on failure
        function loginFailed() {

            showHideError(true, true, "Login request failed. Please try again.");
        }

        // call back on success
        function loginCallSucceeded(responseText, statusText, xhr, $form) {

            // process the result
            if (statusText == "success") {
                var regexp = new RegExp("^([0-9a-fA-F]){32}$");
                if (regexp.test(responseText)) {
                    
                    // copy email to other form
                    var email = $("#emailInput").attr("value");
                    email = $.trim(email);
                    email = email.toLowerCase();
                    $("#userEmail2").attr("value", email);

                    // copy pswd to other form
                    var pswd = $("#passwordInput").attr("value");
                    $("#userPassword2").attr("value", pswd);

                    // post the aspx form
                    document.getElementById('submitButton').click();
                    
                    return;
                }
                else {
                    showHideError(true, true, responseText);
                }
            }
            else {
                loginFailed();
            }
        }

        function submitLoginEx() {
            $("input#loginButton").focus();
            submitLogin();
            return false;
        }

        function submitLogin() {

            // beginning from scratch: clear error                
            showHideError(false, false, "");

            // get the form
            var form = $("form#loginForm");

            // check inputs
            var formDataArray = new Array();
            if (!checkInputs(form, formDataArray)) {
                // error is already shown. no need to re-shown. just return
                return false;
            }

            // post inputs
            var options = {
                url: "OrgHandlers/Authenticate.ashx",
                type: 'POST',
                success: loginCallSucceeded,   // post-submit callback
                error: loginFailed,
                clearForm: false,    // clear all form fields after successful submit 
                resetForm: false,    // reset the form after successful submit 
                dataType: null,
                data: formDataArray,
                timeout: 5 * 1000
            };

            // submit the form
            form.ajaxSubmit(options);

            // avoid a postback to the server
            return false;
        }


    </script>

</head>
<body>
    <center>

    <form class="hidden" runat="server" action="Login.aspx" id="first">
        <input id="userEmail2" class="editBox secInput" type="text" maxlength="200" runat="server"/>
        <input id="userPassword2" class="editBox secInput" type="password" maxlength="100" runat="server"/>
        <input id="submitButton" type="button" value="button" runat="server" onserverclick="HiddenButtonClicked"/>
    </form>

    <form id="loginForm" action="OrgHandlers/Authenticate.ashx">
    <div class="credSection">
        <div class="inputLine">
            <div class="labelWrp">
                <span class="inputLabel">Email Address:</span>
            </div>
            <div class="inputWrp">
                <input id="emailInput" type="text" maxlength="200" onkeydown="enterKeyDown(event, focusToPswd, hideError)" onfocus="hideError()" />
            </div>
        </div>
        <div class="clear"></div>
        <div class="inputLine">
            <div class="labelWrp">
                <span class="inputLabel">Password :</span>
            </div>
            <div class="inputWrp">
                <input id="passwordInput" type="password" maxlength="100" onkeydown="enterKeyDown(event, submitLoginEx, hideError)" onfocus="hideError()" />
            </div>
        </div>
        <div class="clear"></div>
    </div>

    <div id="actionSection" class="actionSection">
        <div class="loginButtonWrp">
            <input id="loginButton" class="loginButton" type="button" value="Login" onclick="submitLogin()" />
        </div>
        <div class="errorStrip hidden">
            <div class="errorText">No matching email and password</div>
        </div>
    </div>
    </form>
    </center>

</body>
</html>
