function GGS4(){
    return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
}

function guidGenerator() {
    return (GGS4() + GGS4() + "-" + GGS4() + "-" + GGS4() + "-" + GGS4() + "-" + GGS4() + GGS4() + GGS4());
}

function guidGeneratorNoDash() {
    return (GGS4() + GGS4() + GGS4() + GGS4() + GGS4() + GGS4() + GGS4() + GGS4());
}

function getUtcTimeNow() {
    var now = new Date();
    return convertToUtcTime(now);
}

function convertToUtcTime(local) {
    return new Date(local.getUTCFullYear(), local.getUTCMonth(), local.getUTCDate(), local.getUTCHours(), local.getUTCMinutes(), local.getUTCSeconds());
}

function convertToLocalTime(utc) {
    var now = new Date();
    var offset = now.getTimezoneOffset();

    var local = new Date(utc);
    var minutes = local.getMinutes();
    local.setMinutes(minutes - offset);
    return local;
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

function setCookie(name, value, seconds) {
    var now = new Date();
    var milliSecs = parseInt(seconds) * 1000;
    var expireDate = new Date(now.getTime() + milliSecs);
    document.cookie = name + "=" + value + "; expires=" + expireDate.toGMTString() + "; path=/";
}

function isValidEmailAddress(emailAddress) {
    var pattern = new RegExp(/^(("[\w-+\s]+")|([\w-+]+(?:\.[\w-+]+)*)|("[\w-+\s]+")([\w-+]+(?:\.[\w-+]+)*))(@((?:[\w-+]+\.)*\w[\w-+]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][\d]\.|1[\d]{2}\.|[\d]{1,2}\.))((25[0-5]|2[0-4][\d]|1[\d]{2}|[\d]{1,2})\.){2}(25[0-5]|2[0-4][\d]|1[\d]{2}|[\d]{1,2})\]?$)/i);
    return pattern.test(emailAddress);
}

function preFetch(divClasses, containerId) {
    for (var i = 0; i < divClasses.length; i++) {
        value = divClasses[i];
        var cookie = "__preloaded_" + value;
        var preloaded = getCookie(cookie);
        if (preloaded == "") {
            var container = document.getElementById(containerId);
            var item = document.createElement('div');
            item.className = value;
            container.appendChild(item);
            setCookie(cookie, "1", 24 * 60 * 60);
        }
    }
}

function escapeJsonString(str) {
    return str
                    .replace(/[\"]/g, '\\"')
                    .replace(/[\\]/g, '\\\\')
                    .replace(/[\/]/g, '\\/')
                    .replace(/[\b]/g, '\\b')
                    .replace(/[\f]/g, '\\f')
                    .replace(/[\n]/g, '\\n')
                    .replace(/[\r]/g, '\\r')
                    .replace(/[\t]/g, '\\t');
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

function setCursorPos(ctrl) {
    var text = $(ctrl).val();
    text = $.trim(text);
    if (text.length == 0) {
        ctrl.setSelectionRange(0, 0);
    }
}

function sanitizeJsonResponse(responseText) {
    var response = responseText;
    if (response.constructor === String) {
        if (response.indexOf("<pre") >= 0) {
            response = $($(response)[0]).html();
        }
        response = $.parseJSON(response);
    };
    return response;
}

function toLocalDateTime(utcTimeText) {
    var now = new Date();
    var offset = now.getTimezoneOffset();
    var dateTime = new Date(utcTimeText);
    var minutes = dateTime.getMinutes();
    dateTime.setMinutes(minutes - offset);
    return dateTime;
}


function equateHeights($parent, itemsClass) {
    var maxHeight = 0;
    var columns = $parent.find(itemsClass);
    columns.each(function (index, column) {
        var h = $(column).height();
        if (h > maxHeight) {
            maxHeight = h;
        }
    });

    columns.each(function (index, column) {
        $(column).css('min-height', maxHeight);
    });
}


function isNumber(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}

function isInteger(n) {
    return !isNaN(parseFloat(n)) && isFinite(n) && (n % 1 === 0);
}

/*
var __isMobile = {
    Android: function() {
        return navigator.userAgent.match(/Android/i);
    },
    BlackBerry: function() {
        return navigator.userAgent.match(/BlackBerry/i);
    },
    iOS: function() {
        return navigator.userAgent.match(/iPhone|iPad|iPod/i);
    },
    Opera: function() {
        return navigator.userAgent.match(/Opera Mini/i);
    },
    Windows: function() {
        return navigator.userAgent.match(/IEMobile/i);
    },
    any: function() {
        return (__isMobile.Android() || __isMobile.BlackBerry() || __isMobile.iOS() || __isMobile.Opera() || __isMobile.Windows());
    }
};
*/