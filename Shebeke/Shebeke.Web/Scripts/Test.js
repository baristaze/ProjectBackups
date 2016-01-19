function testQueryParamUtil() {
    /*
    var xx = [];
    xx[100] = undefined;
    alert(xx[100]);
    return;
    */

    var hash = [];
    var exph = [];
    
    hash.push('');
    hash.push('#');
    hash.push('#f');
    hash.push('#f=');
    hash.push('#f=1');
    hash.push('#f=1&');
    hash.push('#f=1&g');
    hash.push('#f=1&g=');
    hash.push('#f=1&g=2');
    hash.push('#f=1&g=2&');
    hash.push('#f=1&g=2&&');

    exph.push('');
    exph.push('');
    exph.push('f');
    exph.push('f');
    exph.push('f=1');
    exph.push('f=1');
    exph.push('f=1&g');
    exph.push('f=1&g');
    exph.push('f=1&g=2');
    exph.push('f=1&g=2');
    exph.push('f=1&g=2');

    var link = "http://foobar.com";

    for (var j = 0; j < hash.length; j++) {
        var input = link + hash[j];
        var actual = getHashPart(input);
        var expected = exph[j];
        
        if (actual != expected) {
            alert("Expected: " + expected + " [NOT EQUAL TO] Actual: " + actual);
        }
    }

    alert("done 1");

    exph = [];
    exph.push('');
    exph.push('');
    exph.push('');
    exph.push('');
    exph.push('');
    exph.push('');
    exph.push('#g');
    exph.push('#g');
    exph.push('#g=2');
    exph.push('#g=2');
    exph.push('#g=2');

    for (var j = 0; j < hash.length; j++) {
        var input = link + hash[j];
        var actual = removeHash(input, 'f');
        var expected = link + exph[j];

        if (actual != expected) {
            alert("Expected: " + expected + " [NOT EQUAL TO] Actual: " + actual);
        }

        if (j >= 6) {
            actual = removeHash(actual, 'g');
            expected = link;
            if (actual != expected) {
                alert("Expected: " + expected + " [NOT EQUAL TO] Actual: " + actual);
            }
        }
    }

    alert("done 2");

    exph = [];
    exph.push('#f=2');
    exph.push('#f=2');
    exph.push('#f=2');
    exph.push('#f=2');
    exph.push('#f=2');
    exph.push('#f=2');
    exph.push('#f=2&g=4');
    exph.push('#f=2&g=4');
    exph.push('#f=2&g=4');
    exph.push('#f=2&g=4');
    exph.push('#f=2&g=4');

    for (var j = 0; j < hash.length; j++) {
        var input = link + hash[j];
        var actual = updateHashStringParameter(input, 'f', 2);
        if (j >= 6) {
            actual = updateHashStringParameter(actual, 'g', 4);
        }

        var expected = link + exph[j];
        if (actual != expected) {
            alert("Expected: " + expected + " [NOT EQUAL TO] Actual: " + actual);
        }
    }

    alert("done 3");

    exph = [];
    exph.push('');
    exph.push('#');
    exph.push('#f');
    exph.push('#f');
    exph.push('#f=1');
    exph.push('#f=1');
    exph.push('#f=1&g');
    exph.push('#f=1&g');
    exph.push('#f=1&g=2');
    exph.push('#f=1&g=2');
    exph.push('#f=1&g=2');

    var url = [];
    var rep = [];

    url.push('http://foobar.com');
    url.push('http://foobar.com?');
    url.push('http://foobar.com?a');
    url.push('http://foobar.com?a=');
    url.push('http://foobar.com?a=1');
    url.push('http://foobar.com?a=1&');
    url.push('http://foobar.com?a=1&b');
    url.push('http://foobar.com?a=1&b=');
    url.push('http://foobar.com?a=1&b=2');
    url.push('http://foobar.com?a=1&b=2&');

    rep.push('http://foobar.com?a=5');
    rep.push('http://foobar.com?a=5');
    rep.push('http://foobar.com?a=5');
    rep.push('http://foobar.com?a=5');
    rep.push('http://foobar.com?a=5');
    rep.push('http://foobar.com?a=5');
    rep.push('http://foobar.com?a=5&b=8');
    rep.push('http://foobar.com?a=5&b=8');
    rep.push('http://foobar.com?a=5&b=8');
    rep.push('http://foobar.com?a=5&b=8');

    for (var i = 0; i < url.length; i++) {
        for (var j = 0; j < hash.length; j++) {
            var input = url[i] + hash[j];
            var expected = rep[i] + exph[j];
            var actual = updateQueryStringParameter(input, 'a', 5);
            if (i >= 6) {
                actual = updateQueryStringParameter(actual, 'b', 8);
            }
            if (actual != expected) {
                alert("Expected: " + expected + " [NOT EQUAL TO] Actual: " + actual);
            }
        }
    }

    alert("done 4");

    url = [];
    rep = [];

    url.push('http://foobar.com?c=3');
    url.push('http://foobar.com?c=3&');
    url.push('http://foobar.com?c=3&a');
    url.push('http://foobar.com?c=3&a=');
    url.push('http://foobar.com?c=3&a=1');
    url.push('http://foobar.com?c=3&a=1&');
    url.push('http://foobar.com?c=3&a=1&b');
    url.push('http://foobar.com?c=3&a=1&b=');
    url.push('http://foobar.com?c=3&a=1&b=2');
    url.push('http://foobar.com?c=3&a=1&b=2&');

    rep.push('http://foobar.com?c=3&a=5');
    rep.push('http://foobar.com?c=3&a=5');
    rep.push('http://foobar.com?c=3&a=5');
    rep.push('http://foobar.com?c=3&a=5');
    rep.push('http://foobar.com?c=3&a=5');
    rep.push('http://foobar.com?c=3&a=5');
    rep.push('http://foobar.com?c=3&a=5&b=8');
    rep.push('http://foobar.com?c=3&a=5&b=8');
    rep.push('http://foobar.com?c=3&a=5&b=8');
    rep.push('http://foobar.com?c=3&a=5&b=8');

    for (var i = 0; i < url.length; i++) {
        for (var j = 0; j < hash.length; j++) {
            var input = url[i] + hash[j];
            var expected = rep[i] + exph[j];
            var actual = updateQueryStringParameter(input, 'a', 5);
            if (i >= 6) {
                actual = updateQueryStringParameter(actual, 'b', 8);
            }
            if (actual != expected) {
                alert("Expected: " + expected + " [NOT EQUAL TO] Actual: " + actual);
            }
        }
    }

    alert("done 5");

    url = [];
    rep = [];

    url.push('http://foobar.com');
    url.push('http://foobar.com?');
    url.push('http://foobar.com?c');
    url.push('http://foobar.com?c=3');
    url.push('http://foobar.com?c=3&');
    url.push('http://foobar.com?c=3&a');
    url.push('http://foobar.com?c=3&a=');
    url.push('http://foobar.com?c=3&a=1');
    url.push('http://foobar.com?c=3&a=1&');
    url.push('http://foobar.com?c=3&a=1&b');
    url.push('http://foobar.com?c=3&a=1&b=');
    url.push('http://foobar.com?c=3&a=1&b=2');
    url.push('http://foobar.com?c=3&a=1&b=2&');

    rep.push('http://foobar.com');
    rep.push('http://foobar.com');
    rep.push('http://foobar.com?c');
    rep.push('http://foobar.com?c=3');
    rep.push('http://foobar.com?c=3');
    rep.push('http://foobar.com?c=3');
    rep.push('http://foobar.com?c=3');
    rep.push('http://foobar.com?c=3');
    rep.push('http://foobar.com?c=3');
    rep.push('http://foobar.com?c=3');
    rep.push('http://foobar.com?c=3');
    rep.push('http://foobar.com?c=3');
    rep.push('http://foobar.com?c=3');

    for (var i = 0; i < url.length; i++) {
        for (var j = 0; j < hash.length; j++) {
            var input = url[i] + hash[j];
            var expected = rep[i] + exph[j];
            var actual = removeUrlParameter(input, 'b');
            actual = removeUrlParameter(actual, 'a');
            
            if (actual != expected) {
                alert("Expected: " + expected + " [NOT EQUAL TO] Actual: " + actual);
            }
        }
    }

    alert("done 6");

    /*
    url = [];
    url.push('http://foobar.com');
    url.push('http://foobar.com?');
    url.push('http://foobar.com?c');
    url.push('http://foobar.com?c=3');
    url.push('http://foobar.com?c=3&');
    url.push('http://foobar.com?c=3&a');
    url.push('http://foobar.com?c=3&a=');
    url.push('http://foobar.com?c=3&a=1');
    url.push('http://foobar.com?c=3&a=1&');
    url.push('http://foobar.com?c=3&a=1&b');
    url.push('http://foobar.com?c=3&a=1&b=');
    url.push('http://foobar.com?c=3&a=1&b=2');
    url.push('http://foobar.com?c=3&a=1&b=2&');
    */

    rep = [];
    rep.push('http://foobar.com');
    rep.push('http://foobar.com');
    rep.push('http://foobar.com?c');
    rep.push('http://foobar.com?c=3');
    rep.push('http://foobar.com?c=3');
    rep.push('http://foobar.com?c=3&a');
    rep.push('http://foobar.com?c=3&a='); // we have some flexibility here
    rep.push('http://foobar.com?c=3&a=1');
    rep.push('http://foobar.com?c=3&a=1');
    rep.push('http://foobar.com?c=3&a=1&b');
    rep.push('http://foobar.com?c=3&a=1&b='); // we have some flexibility here
    rep.push('http://foobar.com?c=3&a=1&b=2');
    rep.push('http://foobar.com?c=3&a=1&b=2');

    var utm = 'utm_expid=324534-6&utm_referrer=https%3A%2F%2Fwww.facebook.com%2F';

    for (var i = 0; i < url.length; i++) {
        var utmx = utm;
        if (i < 2) {
            utmx = '?' + utm;
        }
        else {
            utmx = '&' + utm;
        }

        for (var j = 0; j < hash.length; j++) {
            var input = url[i] + utmx + hash[j];
            var expected = rep[i] + exph[j];
            var actual = removeGoogleExperimentParams(input);

            if (actual != expected) {
                alert("Expected: " + expected + " [NOT EQUAL TO] Actual: " + actual);
            }
        }
    }

    alert("done 7");


    alert("finito");
}