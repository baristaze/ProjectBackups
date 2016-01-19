function closePhotoUploadSection(uploadSection, onComplete) {
    var photoButtonParent = uploadSection.find(".photo-upload-start-wrp");
    var photoButton = photoButtonParent.find(".photo-upload-start-btn");
    var inputArea = uploadSection.find(".photo-upload-input-wrp");
    var button = uploadSection.find(".photo-upload-do-btn");
    button.removeAttr("_pressed");

    hideUploadError(button);

    inputArea.slideUp(400, function () {
        showHideUploadSpinner(button, false);
        photoButton.removeAttr("pressed");
        photoButtonParent.removeAttr("pressed");
        uploadSection.siblings(".entry-add-btn").show();

        if (onComplete) {
            onComplete();
        }
    });
}

function hideUploadError($child) {
    var form = $child.closest("form");
    var errorArea = form.find(".upload-result");
    var errorTextCtrl = errorArea.find(".upload-result-err");
    errorArea.slideUp(400, function () {
        errorTextCtrl.text("");
    });
}

function showUploadError(errorArea, errorMessage) {
    var errorTextCtrl = errorArea.find(".upload-result-err");
    errorTextCtrl.text(errorMessage);
    errorArea.slideDown(400);
}

function showHideUploadSpinner($btn, isShow) {
    if (isShow) {
        $btn.text("");
        var resourceLink = $("#applicationPath").text() + "/Images/spinner.gif";
        $btn.html("<img class='act-busy' alt='...' src='" + resourceLink + "' />");
    }
    else {
        $btn.html("");
        $btn.text("upload");
    }
}

function fileUploadFailedEx($btn, $form, $errorArea, errorMessage, imageSource, imageUrlFromWeb) {
    return function fileUploadFailed() {
        $btn.removeAttr("_pressed");
        showHideUploadSpinner($btn, false);

        // form is cleared, let's re-set the radio button
        if (imageUrlFromWeb != null && imageUrlFromWeb != undefined && imageUrlFromWeb.length > 0) {
            $form.find(".photo-upload-input-url").attr("value", imageUrlFromWeb);
        }
        if (imageSource == 0) {
            var radio1 = $form.find(".photo-upload-choice[value='0']");
            radio1.click();
        }
        else if (imageSource == 1) {
            var radio2 = $form.find(".photo-upload-choice[value='1']");
            radio2.click();
        }

        showUploadError($errorArea, errorMessage);
    }     // core
} // closure


function adjustTextAreaCursor($ctrl) {
    var setCursor = false;
    var text = $ctrl.val();
    if (text.length == 0) {
        setCursor = true;
    }
    else if (text.indexOf("\n[photo-") == 0 && compareHeightTo3Rows($ctrl) == 0 && text.lastIndexOf("\n") == (text.length - 1)) {
        setCursor = true;
    }

    if (setCursor) {
        $ctrl[0].setSelectionRange(0, 0);
    }
}

function _adjustWatermarkDesc($textArea, flag, blurTextArea) {
    if (flag == 1 || flag == 2 || flag == 3) {
        var descrArea = $textArea.siblings(".entry-add-inp-dsc");
        if (flag == 1) {
            descrArea.attr("value", "yazmaya başla");
        }
        else if (flag == 2) {
            descrArea.attr("value", "bu resim hakkında bir şeyler söyle\n\n");
        }
        else if (flag == 3) {
            descrArea.attr("value", "bu resim hakkında bir şeyler söyle\n\nYukarıdaki resim hakkında yazmaya devam et");
        }

        //descrArea.trigger('autosize');
        var textHeight = $textArea.height();
        var descHeight = descrArea.height();
        if (descHeight != textHeight) {
            descrArea.height(textHeight);
        }

        if (blurTextArea) {
            $textArea.addClass("semi-trans");
        }
    }
}

function adjustWatermarkDescriptionEx($textArea, onBlur) {
    var txt = $textArea.val();
    if ($.trim(txt) == "") {
        _adjustWatermarkDesc($textArea, 1, onBlur);
    }
    else if (txt.indexOf("\n[photo-") == 0) {
        if (compareHeightTo3Rows($textArea) == 0 && txt.lastIndexOf("\n") == (txt.length - 1)) {
            _adjustWatermarkDesc($textArea, 3, onBlur);
        }
        else {
            _adjustWatermarkDesc($textArea, 2, onBlur);
        }
    }
    else if (compareHeightTo3Rows($textArea) == -1 && !onBlur) {
        _adjustWatermarkDesc($textArea, 1, onBlur);
    }
    else {
        if (onBlur) {
            $textArea.removeClass("semi-trans");
        }
    }
}

// bug-fix: .rows is always 1 after enabling autosize
// it alters the height instead of rows... therefore we compare height.
function compareHeightTo3Rows($textArea) {
    var height = $textArea.height();
    if (height < 50) {
        return -1; // smaller than 3 rows; i.e. rowsCount < 3
    }
    else if (height > 65) {
        return 1; // bigger than 3 rows; i.e. rowsCount > 3
    }
    else {
        return 0; // rowsCount == 3
    }
}

function adjustTextEditorOnUpload($child, previousAttachNamesWrapped, newAttachNameWrapped) {
    var root = $child.closest(".mb-inner");
    var textArea = root.find(".entry-add-inp");
    var textData = textArea.attr("value");
    var newTextData = textData;
    if ($.trim(textData).length <= 0) {
        if (previousAttachNamesWrapped.length == 0) {
            newTextData = "\n" + newAttachNameWrapped + "\n";
        }
        else {
            newTextData = "\n" + previousAttachNamesWrapped.join("\n\n") + "\n\n" + newAttachNameWrapped + "\n";
        }
    }
    else if (previousAttachNamesWrapped.length > 0) {
        var softDeletedAttachmentsByUser = new Array();
        for (var x = 0; x < previousAttachNamesWrapped.length; x++) {
            if (textData.indexOf(previousAttachNamesWrapped[x]) < 0) {
                softDeletedAttachmentsByUser.push(previousAttachNamesWrapped[x]);
            }
        }

        if (softDeletedAttachmentsByUser.length > 0) {
            newTextData = trimTailingNewLine(newTextData);
            newTextData += "\n\n";
            newTextData += softDeletedAttachmentsByUser.join("\n\n") + "\n";
        }

        if (newTextData.indexOf(newAttachNameWrapped) < 0) {
            newTextData = trimTailingNewLine(newTextData);
            newTextData += "\n\n";
            newTextData += newAttachNameWrapped + "\n";
        }
    }
    else { // only new one with previous text
        if (newTextData.indexOf(newAttachNameWrapped) < 0) {
            newTextData = trimTailingNewLine(newTextData);
            newTextData += "\n\n";
            newTextData += newAttachNameWrapped + "\n";
        }
        else {
            // do nothing: user has typed [photo-x] before uploading the photo. the reference is already there.
            // though add one more line to make sure that the preview area is updated
            newTextData += "\n";
        }
    }

    textArea.attr("value", newTextData);
    textArea.trigger('autosize');

    // description area'sautosize will be called by the callback function of input area's auto-size
    // the below function is not needed anymore
    // adjustWatermarkDescriptionEx(textArea, false);
}

// post-submit callback 
function fileUploadSucceededEx($btn, $form, $errorArea, imageSource, imageUrlFromWeb) {
    return function fileUploadSucceeded(responseText, statusText, xhr, $form) {
        if (statusText == "success") {
            var response = sanitizeJsonResponse(responseText);
            if (response.ErrorCode == 0) {
                var pageResources = $(".pageResources");
                var attachedPhotoTemplate = pageResources.find(".photo-attach-wrp");
                var attachedPhoto = attachedPhotoTemplate.clone();

                var nextViewId = 0;
                var photoUploadSection = $form.closest(".photo-upload-section");
                var attachmentsWrapper = photoUploadSection.find(".photo-attach-list");

                var previousAttachNamesWrapped = new Array();
                var attachments = attachmentsWrapper.find(".photo-attach-wrp");
                if (attachments.length > 0) {
                    $.each(attachments, function (attachIndex, attachx) {
                        var attachViewId = $(attachx).find(".photo-attach-view-id").text();
                        previousAttachNamesWrapped.push("[photo-" + attachViewId + "]");
                        if (attachIndex == attachments.length - 1) {
                            nextViewId = parseInt(attachViewId.toString());
                        }
                    });
                }

                // increase
                nextViewId++;

                var name = "photo-" + nextViewId.toString();
                attachedPhoto.find(".photo-attach-view-id").text(nextViewId.toString());
                attachedPhoto.find(".photo-attach-id").text(response.FileInfo.Id);
                attachedPhoto.find(".photo-attach-url").text(response.FileInfo.CloudUrl);
                attachedPhoto.find(".photo-attach-txt").text(name);
                attachmentsWrapper.append(attachedPhoto);
                var newAttachNameWrapped = "[" + name + "]";

                closePhotoUploadSection(photoUploadSection, function () {
                    adjustTextEditorOnUpload(photoUploadSection, previousAttachNamesWrapped, newAttachNameWrapped);
                });
            }
            else {
                fileUploadFailedEx($btn, $form, $errorArea, response.ErrorMessage, imageSource, imageUrlFromWeb)();
            }
        }
        else {
            fileUploadFailedEx($btn, $form, $errorArea, "Bilinmeyen bir hata oluştu!", imageSource, imageUrlFromWeb)();
        }
    }                             // core
} // closure

function uploadPhoto($btn, $form, $errorArea, imageSource, imageUrlFromWeb) {
    var rid = guidGenerator();
    var url = $("#applicationPath").text() + "/UploadHandlers/FileUpload.ashx?rid=" + rid;

    var options = {
        url: url,
        type: 'POST',
        success: fileUploadSucceededEx($btn, $form, $errorArea, imageSource, imageUrlFromWeb),   // post-submit callback
        error: fileUploadFailedEx($btn, $form, $errorArea, "Bilinmeyen bir hata oluştu!", imageSource, imageUrlFromWeb),
        clearForm: true,                    // clear all form fields after successful submit 
        resetForm: true,                    // reset the form after successful submit                 
        dataType: null,
        data: null,
        timeout: 90 * 1000
    };

    // submit the form
    $form.ajaxSubmit(options);
    return false;
}

function startUploadingPhoto($btn) {
    var form = $btn.closest("form");
    var errorArea = form.find(".upload-result");
    var errorTextCtrl = errorArea.find(".upload-result-err");

    if ($btn.attr("_pressed")) {
        return false;
    }

    var imageUrlFromWeb = null;
    var imageSource = form.find(".photo-upload-choice:checked").attr("value");
    if (imageSource == 0) {
        imageUrlFromWeb = form.find(".photo-upload-input-url").attr("value");
        imageUrlFromWeb = $.trim(imageUrlFromWeb);
        if (imageUrlFromWeb.length <= 0) {
            showUploadError(errorArea, "Resmin web bağlantı adresini giriniz");
            return false;
        }
    }
    else if (imageSource == 1) {
        var filePath = $(form[0]["LocalFile"]).val();
        if (filePath) {
            if (!checkFileExt(filePath, __allowedExtensions)) {
                showUploadError(errorArea, "Bu dosya tipi desteklenmiyor");
                return false;
            }
        }
        else {
            showUploadError(errorArea, "Lütfen bir resim dosyası seçiniz");
            return false;
        }
    }
    else {
        showUploadError(errorArea, "Seçeneklerden birini işaretleyiniz");
        return false;
    }

    // upload...
    $btn.attr("_pressed", "_pressed");
    showHideUploadSpinner($btn, true);
    uploadPhoto($btn, form, errorArea, imageSource, imageUrlFromWeb);
}

function openOrCloseUploadArea($btn) {
    var uploadSection = $btn.closest(".photo-upload-section");
    if ($btn.attr("pressed")) {
        var actionBtn = uploadSection.find(".photo-upload-do-btn");
        if (!actionBtn.attr("_pressed")) {
            closePhotoUploadSection(uploadSection);
        }
        return false;
    }

    $btn.attr("pressed", "pressed");
    $btn.closest(".photo-upload-start-wrp").attr("pressed", "pressed");

    var inputArea = uploadSection.find(".photo-upload-input-wrp");
    var urlInputCtrl = inputArea.find(".photo-upload-input-url");

    var radio1 = inputArea.find(".photo-upload-choice[value='0']");

    uploadSection.siblings(".entry-add-btn").hide();

    inputArea.slideDown(400, function () {
        radio1.click();
        urlInputCtrl.focus();
    });
}

function removeImageFile($btn) {
    var fileId = $btn.siblings(".photo-attach-id").text();
    var url = $("#applicationPath").text() + "/svc/rest/image/discard/" + fileId;

    $.ajax({
        url: url,
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        data: null,
        success: null,
        error: null
    });

    var imgInfoContainer = $btn.closest(".photo-attach-wrp");
    var grandParent = imgInfoContainer.closest(".mb-inner");
    var viewText = imgInfoContainer.find(".photo-attach-txt").text();
    viewText = "[" + viewText + "]";
    imgInfoContainer.remove();

    var inputArea = grandParent.find(".entry-add-inp");
    var text = inputArea.attr("value");
    //text = text.replace(viewText, "");
    var viewTextRegexStr = viewText.replace(/\[/g, "\\[").replace(/\]/g, "\\]").replace(/\-/g, "\\-");
    var viewTextRegex = new RegExp(viewTextRegexStr, "gi");
    text = text.replace(viewTextRegex, "");

    if ($.trim(text).length == 0) {
        text = $.trim(text);
    }

    inputArea.attr("value", text);
    inputArea.trigger('autosize');

    // description area'sautosize will be called by the callback function of input area's auto-size
    // the below function is not needed anymore
    // adjustWatermarkDescriptionEx(inputArea, false);
}

function entryPage_Init_ImageUpload() {
    // this is not auto-sized because, we adjust the height based on height of ".entry-add-inp"
    // $(".entry-add-inp-dsc").autosize();

    $(".entry-add-inp").autosize({ callback: function () { adjustWatermarkDescriptionEx($(this), false); } });

    /*    
    $('.entry-add-inp').live('click', function (e) {
    adjustTextAreaCursor($(this));
    });
    */

    $('.entry-add-inp').live('focus', function (e) {
        adjustTextAreaCursor($(this));
    });

    // Adjust the descr and its height whenever input blurs.... 
    // Do this at least once when page  loaded although there is no blur
    $(".entry-add-inp").live('blur', function () {
        adjustWatermarkDescriptionEx($(this), true);
    })

    $(".entry-add-inp").each(function (index, item) {
        $(this).val("");
        $.when($(this).trigger('autosize')).done(function () {
            // we need to call below func one more time with 'true' param explicitly
            adjustWatermarkDescriptionEx($(this), true);
        });
    });

    // for the initial load, call it once

    $(".photo-upload-input").live('focus', function () {
        hideUploadError($(this));
        return false;
    });

    $(".photo-upload-do-btn").live('click', function () {

        if ($("#isAuthenticated").text() == "0") {
            showBlockedLoginDialog();
            return false;
        }

        startUploadingPhoto($(this));
        return false;
    });

    $(".photo-upload-start-btn").live('click', function () {
        openOrCloseUploadArea($(this));
        return false;
    });

    $(".cancel-upload-btn").live('click', function () {
        var uploadSection = $(this).closest(".photo-upload-section");
        closePhotoUploadSection(uploadSection);
        return false;
    });

    $(".remove-file-btn").live('click', function () {
        removeImageFile($(this));
        return false;
    });

    $(".photo-upload-choice").live('change', function () {
        var selectedVal = $(this).attr("value");
        var parent = $(this).parent();
        var urlInputCtrl = parent.find(".photo-upload-input-url");
        var fileInputCtrl = parent.find(".photo-upload-input-file");

        if (selectedVal == 0) {
            fileInputCtrl.hide();
            urlInputCtrl.show().focus();
        }
        else if (selectedVal == 1) {
            urlInputCtrl.hide();
            fileInputCtrl.show().focus();
        }
    });
}