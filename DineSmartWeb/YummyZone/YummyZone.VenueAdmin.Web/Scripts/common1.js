
function enterKeyDown(e, onEnterFunc, onNonEnterFunc) {
    var charCode;
    if (e && e.which) {
        charCode = e.which;
    } else if (window.event) {
        e = window.event;
        charCode = e.keyCode;
    }
    if (charCode == 13) {
        onEnterFunc();
    }
    else {
        if (onNonEnterFunc) {
            onNonEnterFunc(e);
        }
    }
}

function getCookie(name) {
    var re = new RegExp(name + "=[^;]+", "i");
    if (document.cookie.match(re)) {
        return document.cookie.match(re)[0].split("=")[1]
    }
    else {
        return ""
    }
}

function setCookie(name, value, days) {
    var expireDate = new Date();
    var expstring = expireDate.setDate(expireDate.getDate() + parseInt(days));
    document.cookie = name + "=" + value + "; expires=" + expireDate.toGMTString() + "; path=/";
}

function footerLinkClicked(id) {
    if (id == "fmi_terms") { window.location.href = "Terms.aspx"; }
    if (id == "fmi_prvc") { window.location.href = "Privacy.aspx"; }
    if (id == "fmi_discl") { window.location.href = "Disclaimer.aspx"; }
}