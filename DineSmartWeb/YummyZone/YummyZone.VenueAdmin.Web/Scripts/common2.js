$(document).ready(function () {
    $(".menuItem").click(function () {
        if ($(this).hasClass("selectedLink")) {
            return;
        }

        var id = $(this).attr("id");
        if (id == "mi_cpns") { window.location.href = "Coupons.aspx"; }
        if (id == "mi_fdbk") { window.location.href = "Feedbacks.aspx"; }
        if (id == "mi_menu") { window.location.href = "Menus.aspx"; }
        if (id == "mi_srvy") { window.location.href = "Survey.aspx"; }
        if (id == "mi_sett") { window.location.href = "Settings.aspx"; }
    });
});

$(document).ready(function () {
    $(".inputText").live('focus', function () {
        $(".highlightedText").removeClass("highlightedText");
        $(this).addClass("highlightedText");
    });

    $(".inputText").live('focusout', function () {
        $(this).removeClass("highlightedText");
    });
});

function getInternetExplorerVersion() {
    var rv = -1; // Return value assumes failure.    
    if (navigator.appName == 'Microsoft Internet Explorer') {
        var ua = navigator.userAgent;
        var re = new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");
        if (re.exec(ua) != null)
            rv = parseFloat(RegExp.$1);
    }

    return rv;
}

function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regexS = "[\\?&]" + name + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var results = regex.exec(window.location.href);
    if (results == null)
        return "";
    else
        return decodeURIComponent(results[1].replace(/\+/g, " "));
}

$.fn.ForceNumericOnly = function () {
    return this.each(function () {
        $(this).keydown(function (e) {
            var key = e.charCode || e.keyCode || 0;
            // allow backspace, tab, delete, arrows, numbers and keypad numbers ONLY
            return (
                        key == 8 ||
                        key == 9 ||
                        key == 16 ||
                        key == 46 ||
                        key == 190 ||
                        key == 110 ||
                        (key >= 35 && key <= 40) ||
                        (key >= 48 && key <= 57) ||
                        (key >= 96 && key <= 105));
        })
    })
};

function isNumber(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}

function guidGenerator() {
    var S4 = function () {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    };

    return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
}

function randomString(length) {
    var text = "";
    var possible = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz123456789";

    for (var i = 0; i < length; i++) {
        text += possible.charAt(Math.floor(Math.random() * possible.length));
    }

    return text;
} 

function checkFileExt(filePath, extensionWhiteList) {
    if (filePath) {
        var fileName = filePath.replace(/^.*\\/, '');
        if (fileName) {
            var fileExt = /[^.]+$/.exec(fileName);
            if (fileExt) {
                fileExt = fileExt.toString().toLowerCase();
                if ($.inArray(fileExt, extensionWhiteList) >= 0) {
                    return true;
                }
            }
        }
    }

    return false;
}

function isValidEmailAddress(emailAddress) {
    var pattern = new RegExp(/^(("[\w-+\s]+")|([\w-+]+(?:\.[\w-+]+)*)|("[\w-+\s]+")([\w-+]+(?:\.[\w-+]+)*))(@((?:[\w-+]+\.)*\w[\w-+]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][\d]\.|1[\d]{2}\.|[\d]{1,2}\.))((25[0-5]|2[0-4][\d]|1[\d]{2}|[\d]{1,2})\.){2}(25[0-5]|2[0-4][\d]|1[\d]{2}|[\d]{1,2})\]?$)/i);
    return pattern.test(emailAddress);
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

function isValidState(state) {
    return include(__allStates, state);
}

function isValidZipCode(zipCode) {
    var zipCodePattern = new RegExp(/^\d{5}$|^\d{5}-\d{4}$/);
    return zipCodePattern.test(zipCode);
}

function isValidPhoneUSA(phoneNumber) {
    var pattern = new RegExp(/^(?:\+?1\s*(?:[.-]\s*)?)?(?:\(\s*([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9])\s*\)|([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9]))\s*(?:[.-]\s*)?([2-9]1[02-9]|[2-9][02-9]1|[2-9][02-9]{2})\s*(?:[.-]\s*)?([0-9]{4})(?:\s*(?:#|x\.?|ext\.?|extension)\s*(\d+))?$/);
    return pattern.test(phoneNumber);
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

var __guidPattern = "(^([0-9a-fA-F]){32}$|^([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}$)";

function isGuid(text) {
    var regexp = new RegExp(__guidPattern);
    if (regexp.test(text)) {
        return true;
    } else {
        return false;
    }
}