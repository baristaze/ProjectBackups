function toggleButton(item) {
    if ($(item).attr("pressed")) {
        $(item).removeAttr("pressed");
    }
    else {
        $(item).attr("pressed", "pressed");
    }
}

function toggleExclusiveButton(item) {
    if ($(item).attr("pressed")) {
        $(item).removeAttr("pressed");
    }
    else {
        var sister = $(item).siblings("button[pressed]");
        if (sister.length > 0) {
            sister.removeAttr("pressed");
        }
        $(item).attr("pressed", "pressed");
    }
}