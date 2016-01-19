function userActivitiesFacebookConnectInit() {
    $(".signup-facebook").live('click', function () {
        signupToFacebook(onAuthStateChangeDefault, onServerAuthDefault);
    });

    $(".signup-modal").live('click', function () {
        signupToFacebook(onAuthStateChangeDefault, onServerAuthDefault);
        //$.unblockUI();
        return false;
    });
}

function userActivitiesPage_Init() {
    $(".act-log-action-data").live('click', function () {
        var url = $(this).closest("tr").find("td.act-log-data").find(".act-log-data-link").text();
        if (url) {
            window.open(url, "_blank");
            return false;
        }
    });
}