function submitLoginForm() {
    /*
    var emailElem = document.getElementById("bizEmail");
    var emailVal = emailElem.value;
    emailVal = emailVal.replace(/^\s+|\s+$/g, '');
    if (emailVal == "" || emailVal == "your email") {
    return false;
    }

    var pswdElem = document.getElementById("password");
    var pswdVal = pswdElem.value;
    pswdVal = pswdVal.replace(/^\s+|\s+$/g, '');
    if (pswdVal == "") {
    return false;
    }
    if (pswdVal == "password" && pswdElem.type == 'text') {
    pswdElem.focus();
    return false;
    }
    */
    var form = document.getElementById("loginForm");
    form.submit();
}

function emailInputFocused(x) {
    var emailElem = document.getElementById(x);
    var emailVal = emailElem.value;
    emailVal = emailVal.replace(/^\s+|\s+$/g, '');
    if (emailVal == "your email") {
        emailElem.value = "";
    }
}

function emailInputLostFocus(x) {
    var emailElem = document.getElementById(x);
    var emailVal = emailElem.value;
    emailVal = emailVal.replace(/^\s+|\s+$/g, '');
    if (emailVal == "") {
        emailElem.value = "your email";
    }
}

function pswdInputFocused(x) {
    var pswdElem = document.getElementById(x);
    var pswdVal = pswdElem.value;
    pswdVal = pswdVal.replace(/^\s+|\s+$/g, '');
    if (pswdVal == "password" && pswdElem.type == 'text') {
        pswdElem.value = "";
        pswdElem.type = 'password';
    }
}

function pswdInputLostFocus(x) {
    var pswdElem = document.getElementById(x);
    var pswdVal = pswdElem.value;
    pswdVal = pswdVal.replace(/^\s+|\s+$/g, '');
    if (pswdVal == "") {
        pswdElem.value = "password";
        pswdElem.type = 'text';
    }
}