function getRootUrl() {
    var root = window.location.protocol + '//' + window.location.host;
    return root;
}

function updateQueryStringParameter(uri, key, value) {
    var fragment = '';
    var hashIndex = uri.indexOf('#');
    if (hashIndex >= 0) {
        fragment = "#" + getHashPart(uri);
        uri = uri.substring(0, hashIndex);
    }

    var re = new RegExp("([?|&])" + key + "(=.*)?(&|$)", "i");
    if (uri.match(re)) {
        uri = uri.replace(re, '$1' + key + "=" + value + '$3');
    }
    else {
        var separator = "&";
        var index = uri.indexOf('?');
        if (index >= 0) {
            // do not merge this if with the above one. see else!
            if (uri[uri.length - 1] == "?") {
                separator = "";
            }
            if (uri[uri.length - 1] == "&") {
                separator = "";
            }
        }
        else {
            separator = "?";
        }

        uri = uri + separator + key + "=" + value;
    }

    if (fragment) {
        uri = uri + fragment;
    }

    return uri;
}

function getUrlParameters(url) {
    var vars = [];    
    var index = url.indexOf('?');
    if (index >= 0) {
        url = url.slice(index + 1);
        index = url.indexOf('#');
        if (index >= 0) {
            url = url.substring(0, index);
        }

        var hash = [];
        var hashes = url.split('&');
        for (var i = 0; i < hashes.length; i++) {
            hash = hashes[i].split('=');
            vars.push(hash[0]);
            // it is fine if hash.length = 1 because x[out-of-range] doesn't throw but return undefined
            // still let's be safe here
            if (hash.length > 1) {
                vars[hash[0]] = hash[1];
            }
            else {
                vars[hash[0]] = undefined;
            }
        }
    }

    return vars;
}

function removeUrlParameter(url, p) {
    var fragment = '';
    var hashIndex = url.indexOf('#');
    if (hashIndex >= 0) {
        fragment = "#" + getHashPart(url);
        url = url.substring(0, hashIndex);
    }

    var re = new RegExp("([?|&])" + p + "(=.*)?(&|$)", "i");
    if (url.match(re)) {
        if ('$1' == '?') {
            url = url.replace(re, '$1' + '$3');
        }
        else {
            url = url.replace(re, '$3');
        }
    }

    url = url.replace('?&', '?');

    if (url.length > 0 && url[url.length - 1] == '&') {
        url = url.substring(0, url.length - 1);
    }

    if (url.length > 0 && url[url.length - 1] == '?') {
        url = url.substring(0, url.length-1);
    }

    /*
    var arr_hashes = [];
    var paramIndex = url.indexOf('?');
    if (paramIndex >= 0) {
        arr_hashes = getUrlParameters(url);
        url = url.substring(0, paramIndex);
    }

    var params = '';
    for (var i = 0; i < arr_hashes.length; i++) {
        if (p != arr_hashes[i]) {
            if (arr_hashes[arr_hashes[i]]) {
                params += arr_hashes[i] + '=' + arr_hashes[arr_hashes[i]] + '&';
            }
            else {
                params += arr_hashes[i] + '&';
            }
        }
    }

    if (params) {
        if (params[params.length - 1] == '&') {
            params = params.substring(0, fragment.length - 1);
        }

        params = "?" + params;
        url = url + params;
    }
    */

    if (fragment) {
        url = url + fragment;
    }

    return url;
}

function removeSplitParams(url) {
    url = removeUrlParameter(url, 'split');
    return url;
}

function removeGoogleExperimentParams(url) {
    url = removeUrlParameter(url, 'utm_expid');
    url = removeUrlParameter(url, 'utm_referrer');
    return url;
}

function getGoogleExperimentId(url) {
    var urlParams = getUrlParameters(url);
    return getGoogleExperimentId2(urlParams);
}

function getGoogleExperimentId2(urlParams) {
    return urlParams["utm_expid"];
}

function getSplitId(url) {
    var urlParams = getUrlParameters(url);
    return getSplitId2(urlParams);
}

function getSplitId2(urlParams) {
    var splitId = 0;
    var splitParam = urlParams["split"];
    if (splitParam && isInteger(splitParam)) {
        splitId = parseInt(splitParam);
    }

    return splitId;
}

function updateHashStringParameter(uri, key, value) {
    var pre = uri;
    var index = uri.indexOf('#');
    if (index >= 0) {
        pre = uri.substring(0, index);
    }

    var fragment = getHashPart(uri);
    if (fragment) {
        fragment = "#" + fragment;
        var re = new RegExp("([#|&])" + key + "(=.*)?(&|$)", "i");
        if (fragment.match(re)) {
            fragment = fragment.replace(re, '$1' + key + "=" + value + '$3');
        }
        else {
            // this is safe since we are adding # before
            var separator = fragment.indexOf('#') !== -1 ? "&" : "#";
            fragment = fragment + separator + key + "=" + value;
        }
    }
    else {
        fragment = "#" + key + "=" + value;
    }

    return pre + fragment;
}

function getHashParameters(uri) {
    var vars = [];
    var index = uri.indexOf('?');
    if (index >= 0) {
        uri = uri.slice(index + 1);
    }

    index = uri.indexOf('#');
    if (index >= 0) {
        uri = uri.slice(index + 1);
        if (uri) {
            var hash = [];
            var hashes = uri.split('&');
            for (var i = 0; i < hashes.length; i++) {
                hash = hashes[i].split('=');
                if (hash[0]) {
                    vars.push(hash[0]);
                    // it is fine if hash.length = 1 because x[out-of-range] doesn't throw but return undefined
                    // still let's be safe here
                    if (hash.length > 1) {
                        vars[hash[0]] = hash[1];
                    }
                    else {
                        vars[hash[0]] = undefined;
                    }
                }
            }
        }
    }

    return vars;
}

function getHashPart(url) {
    var fragment = '';
    var hashes = getHashParameters(url);
    for (var i = 0; i < hashes.length; i++) {
        if (hashes[hashes[i]]) {
            fragment += hashes[i] + '=' + hashes[hashes[i]] + '&';
        }
        else {
            fragment += hashes[i] + '&';
        }
    }

    if (fragment) {
        if (fragment[fragment.length - 1] == '&') {
            fragment = fragment.substring(0, fragment.length - 1);
        }
    }

    return fragment;
}

function removeHashFromUrl(p) {
    var url = window.location.href.toString();
    url = removeHash(url, p);
    var fragment = getHashPart(url);
    window.location.hash = fragment;
}

function removeHash(url, p) {
    var index = url.indexOf('#');
    if (index < 0) {
        return url;
    }

    var prefix = url.substring(0, index);

    var fragment = '';
    var hashes = getHashParameters(url);
    for (var i = 0; i < hashes.length; i++) {
        if (p != hashes[i]) {
            if (hashes[hashes[i]]) {
                fragment += hashes[i] + '=' + hashes[hashes[i]] + '&';
            }
            else {
                fragment += hashes[i] + '&';
            }
        }
    }

    if (fragment) {
        if (fragment[fragment.length - 1] == '&') {
            fragment = fragment.substring(0, fragment.length - 1);
        }

        prefix += "#" + fragment;
    }

    return prefix;
}