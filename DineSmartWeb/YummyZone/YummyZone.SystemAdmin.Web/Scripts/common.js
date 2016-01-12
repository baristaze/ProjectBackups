
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

function guidGenerator() {
    var S4 = function () {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    };

    return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
}

var __allStates = ["AL", "AK", "AS", "AZ", "AR", "CA", "CO", "CT", "DE", "DC", "FM", "FL", "GA", "GU", "HI", "ID", "IL", "IN", "IA", "KS",
                   "KY", "LA", "ME", "MH", "MD", "MA", "MI", "MN", "MS", "MO", "MT", "NE", "NV", "NH", "NJ", "NM", "NY", "NC", "ND", "MP",
                   "OH", "OK", "OR", "PW", "PA", "PR", "RI", "SC", "SD", "TN", "TX", "UT", "VT", "VI", "VA", "WA", "WV", "WI", "WY"];

function include(arr, obj) {
    for (var i = 0; i < arr.length; i++) {
        if (arr[i] == obj) {
            return true;
        }
    }

    return false;
}

function isValidEmailAddress(emailAddress) {
    var pattern = new RegExp(/^(("[\w-+\s]+")|([\w-+]+(?:\.[\w-+]+)*)|("[\w-+\s]+")([\w-+]+(?:\.[\w-+]+)*))(@((?:[\w-+]+\.)*\w[\w-+]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][\d]\.|1[\d]{2}\.|[\d]{1,2}\.))((25[0-5]|2[0-4][\d]|1[\d]{2}|[\d]{1,2})\.){2}(25[0-5]|2[0-4][\d]|1[\d]{2}|[\d]{1,2})\]?$)/i);
    return pattern.test(emailAddress);
}

function isValidState(state) {
    return include(__allStates, state);
}

function isValidZipCode(zipCode) {
    var zipCodePattern = new RegExp(/^\d{5}$|^\d{5}-\d{4}$/);
    return zipCodePattern.test(zipCode);
}

var __specialCharsForName = "~!@#$%^&*()_=+[]{}\|;:\"<>/?";

function isValidName(name) {
    for (var x = 0; x < name.length; x++) {
        if (__specialCharsForName.indexOf(name.charAt(x)) >= 0) {
            return false;
        }
    }

    return true;
}

$(document).ready(function () {
    $(".inputText").live('focus', function () {
        $(".highlightedText").removeClass("highlightedText");
        $(this).addClass("highlightedText");
    });

    $(".inputText").live('focusout', function () {
        $(this).removeClass("highlightedText");
    });
});