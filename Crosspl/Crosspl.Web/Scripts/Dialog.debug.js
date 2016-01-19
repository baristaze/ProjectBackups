function showBlockedConfirmDialog(message, callBackFunction) {
    var css = getCssForBlockUI('#000');
    var dlg = $(".confirm-dialog");
    var content = dlg.find(".confirm-dialog-content-inner");
    var result = dlg.find(".confirm-dialog-result");
    content.text(message);
    result.text("0");

    $.blockUI(
        {
            message: dlg,
            css: css,

            overlayCSS: {
                backgroundColor: '#000',
                opacity: 0.6,
                cursor: 'default'
            },

            onUnblock: function () { callBackFunction(result.text()); }
        });
}

function showErrorDialog(errorDialogCss) {
    $.blockUI(
        {
            message: $(".error-dialog"),
            timeout: 6000,
            css: errorDialogCss,

            overlayCSS: {
                backgroundColor: '#000',
                opacity: 0.6,
                cursor:
                'default'
            }
        });
}

function showBlockedErrorMessage(message) {
    var css = getCssForBlockUI('#cb2026');
    var content = $(".error-dialog-content");
    content.text(message);
    showErrorDialog(css);
}

function showBlockedLoginDialog() {

    var css = getCssForBlockUI('#fff');

    $.blockUI(
        {
            message: $(".login-dialog"),
            css: css,

            overlayCSS: {
                backgroundColor: '#000',
                opacity: 0.92,
                cursor:
                'default'
            }
        });
}

function showBlockedInstallDialog() {

    var css = getCssForBlockUI2('#fff', '600px', '-300px', 'fixed', '10%');

    $.blockUI(
    {
        message: $(".install-dialog"),
        css: css,

        overlayCSS: {
            backgroundColor: '#000',
            opacity: 0.92,
            cursor:
            'default'
        }
    });
}

function getCssForBlockUI(bkgColor) {
    return getCssForBlockUI2(bkgColor, '500px', '-250px', 'fixed', '20%');
}

function getCssForBlockUI2(bkgColor, sizeWidth, marginLeft, positionx, topx) {
    var css = {
        position: positionx,
        top: topx,
        width: sizeWidth,
        left: '50%',
        'margin-left': marginLeft,
        border: '1px solid White',
        'box-shadow': '0 0 25px 5px #999',
        '-moz-box-shadow': '0 0 25px 5px #999',
        '-webkit-box-shadow': '0 0 25px 5px #999',
        padding: '0px',
        color: 'White',
        backgroundColor: bkgColor,
        '-webkit-border-radius': '10px',
        '-moz-border-radius': '10px',
        'border-radius': '10px',
        opacity: 0.9,
        color: '#fff',
        cursor: 'default',
        'font-size': '20px'
    };

    if ($(window).width() < 540) {
        css = {
            position: positionx,
            top: topx,
            width: '80%',
            left: '10%',
            border: '1px solid White',
            'box-shadow': '0 0 25px 5px #999',
            '-moz-box-shadow': '0 0 25px 5px #999',
            '-webkit-box-shadow': '0 0 25px 5px #999',
            padding: '0px',
            color: 'White',
            backgroundColor: bkgColor,
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            'border-radius': '10px',
            opacity: 0.99,
            color: '#fff',
            cursor: 'default',
            'font-size': '20px'
        };
    }

    return css;
}

function dialogs_InitCommons() {
    $(".error-dialog").live('click', $.unblockUI);
    $(".dialog-close-button").live('click', $.unblockUI);
    $(".confirm-dialog").live('click', $.unblockUI);
    $(".confirm-dialog-btn-yes").live('click', function () { $(this).closest(".confirm-dialog-content").find(".confirm-dialog-result").text("1"); });
    $(".blockOverlay").attr('title', 'Click to close the error window').live('click', function () {
        if ($(this).next(".blockMsg").find(".strong-modal").length == 0) {
            $.unblockUI();
        }
    });
}