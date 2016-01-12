//---------------------------------------------------------//
//              File Selection Through Radio
//---------------------------------------------------------//

$(document).ready(function () {

    $(".imageSourceRadio").live('change', function () {
        var selectedVal = $(this).attr("value");
        var parent = $(this).closest(".imagePair");
        parent.find(".fromWebLink:first").hide();
        parent.find(".fromLocalFile:first").hide();

        if (selectedVal == 1) {
            parent.find(".fromWebLink:first").show().focus();
        }
        else if (selectedVal == 2) {
            parent.find(".fromLocalFile:first").show().focus();
        }
    });
});

//---------------------------------------------------------//
//              Plate Delete
//---------------------------------------------------------//

$(document).ready(function () {

    $(".deleteLink").live('click', function (e) {

        var dlgW = 240;
        var dlgH = 180;

        var x = e.pageX - (dlgW / 2) + 5;
        var y = e.pageY - (dlgH / 2) - 70;

        var scrollTop = $(window).scrollTop();
        if (y < scrollTop) {
            y = scrollTop;
        }

        var rowWrapper = $(this).closest(".rowWrapper");

        // ask confirmation
        $("#confirmDeletingMenuItemDlg").dialog({

            position: [x, y - $(window).scrollTop()],
            title: 'Confirmation',
            resizable: true,
            height: dlgH,
            width: dlgW,
            modal: true,
            buttons: {
                OK: function () {

                    // close the dialog
                    $(this).dialog("close");

                    // get the row Id
                    var rowId = rowWrapper.find(".rowId:first").text();

                    // get cat Id
                    var categoryHeader = rowWrapper.closest(".menuCategoryWrapper").find(".menuCategoryHeaderWrapper:first");
                    var categoryId = categoryHeader.find(".menuCategoryId:first").text();

                    // prepare the url that needs to be called
                    var randomId = guidGenerator();
                    var url = "MenuHandlers/MenuItemDeleteDisable.ashx?miid=" + rowId.toString() + "&mcid=" + categoryId.toString() + "&act=delete" + "&rid=" + randomId;

                    // make the call to delete the item
                    var jqxhr = $.get(url, function (data) {

                        if (data == "1") {

                            // update the deletability
                            var menuCategoryWrapper = rowWrapper.closest(".menuCategoryWrapper");
                            var rows = menuCategoryWrapper.find(".rowWrapper");
                            if (rows.length == 1) {
                                // we have just one which is being deleted
                                menuCategoryWrapper.find(".menuCategoryDelete:first").removeClass("hidden");
                            }

                            // menu item count goes below 3
                            if (rows.length <= 3) {
                                menuCategoryWrapper.find(".menuCategoryBottomActions:first").addClass("hidden");
                                menuCategoryWrapper.find(".addMenuItemLinkExtra:first").addClass("hidden");
                            }
                            
                            // fade out first and then remove completely
                            rowWrapper.fadeOut(null, function () {

                                // remove the row completely
                                rowWrapper.remove();
                            });
                        }
                        else if (data.indexOf("Authenticate.ashx") >= 0) {
                            window.location = "Login.aspx";
                        }
                        else {
                            alert(data);
                        }
                    });

                    // Set error function for the request above
                    jqxhr.error(function () {
                        alert("delete operation failed");
                    });
                },

                Cancel: function () {

                    // close the dialog
                    $(this).dialog("close");
                }
            }
        });

        return false;
    });
});

//---------------------------------------------------------//
//              Plate Cancel
//---------------------------------------------------------//

$(document).ready(function () {
    $(".cancelLink").live('click', function () {

        // get the row
        var rowWrapper = $(this).closest(".rowWrapper");

        // get the row Id
        var rowId = rowWrapper.find(".rowId:first").text();

        if (rowId) { // if we were editing and existing item

            // hide the editable view of the row
            var editableRow = rowWrapper.find(".editableRow:first");

            editableRow.slideUp(function () {
                editableRow.remove();
            });

            // show the read-only view of the row
            rowWrapper.find(".row:first").show();
        }
        else { // if we were adding a new item...

            // fade out first and then remove completely
            rowWrapper.fadeOut(null, function () {

                // remove the row completely
                rowWrapper.remove();
            });
        }

        // enable editablitiy                
        var menuCategoryWrapper = rowWrapper.closest(".menuCategoryWrapper");
        menuCategoryWrapper.removeClass("menuItemIsInEditMode");

        return false;
    });
});

//---------------------------------------------------------//
//              Plate Add
//---------------------------------------------------------//

$(document).ready(function () {

    $(".addNewMenuItemButton").live('click', function () {

        var menuCategoryWrapper = $(this).closest(".menuCategoryWrapper");
        var menuCategoryContent = menuCategoryWrapper.find(".menuCategoryContent:first");

        var isAlreadyOpened = menuCategoryWrapper.hasClass("menuItemIsInEditMode");
        if (!isAlreadyOpened) {
            var rowWrapper = $(".hiddenParts").find(".rowWrapper:first");
            menuCategoryContent.append(rowWrapper.clone());

            var last = menuCategoryContent.find(".rowWrapper:last");
            last.find(".editLink:first").click();

            menuCategoryWrapper.addClass("menuItemIsInEditMode");

            // un-highlight and hide action menues
            var menuCategoryHeader = menuCategoryWrapper.find(".menuCategoryHeader:first");
            menuCategoryHeader.css("background-color", "#ebebeb");
            menuCategoryHeader.find(".menuCategoryHooverActions:first").hide();
        }

        var catNameWithToggle = menuCategoryWrapper.find(".menuCategoryNameWithToggle:first");
        var treeExpanded = catNameWithToggle.find(".treeExpanded:first");
        if (treeExpanded.hasClass("hidden")) {
            catNameWithToggle.click();
        }

        // set focus on name
        var lastEditableRow = menuCategoryContent.find(".editableRow:last");
        lastEditableRow.find(".nameInput:first").focus();

        var lastRowWrapper = menuCategoryContent.find(".rowWrapper:last");
        var rowSeparator = lastRowWrapper.find(".rowSeparator:first");
        var viewPortHeight = $(window).height();
        var scrollTop = $(window).scrollTop();
        var windowFold = viewPortHeight + scrollTop;
        var rowSeparatorTop = rowSeparator.offset().top;
        if (rowSeparatorTop > windowFold) {
            // below the fold... scroll to ensure visibility
            $.scrollTo(scrollTop + rowSeparatorTop - windowFold + 2, 1000, { axis: 'y' });
        }

        return false;
    });
});

//---------------------------------------------------------//
//              Plate Edit
//---------------------------------------------------------//
$(document).ready(function () {
    $(".editLink").live('click', function () {
        // do not allow user to edit multiple items
        $(".menuItemActions").hide();

        // switch the page's status to edit mode
        $(this).closest(".menuCategoryWrapper").addClass("menuItemIsInEditMode");

        // get editable view
        var editableRow = $(".hiddenParts").find(".editableRow:first");
        editableRow = editableRow.clone();

        // get read-only view of the row
        var readOnlyRow = $(this).closest(".row");

        // hide the read-only view
        readOnlyRow.hide();

        // append the editable view
        readOnlyRow.before(editableRow);

        // get the current values
        var itemName = $(this).closest(".menuItemContent").find(".plateName:first").text();
        var itemPrice = $(this).closest(".menuItemContent").find(".price:first").text();
        var itemDescr = $(this).closest(".menuItemContent").find(".plateDescription:first").html();
        var imagePath = $(this).closest(".row").find(".image:first").attr("src");

        itemName = $.trim(itemName.toString());
        itemPrice = $.trim(itemPrice.toString());
        itemPrice = itemPrice.replace("$", "");
        itemDescr = $.trim(itemDescr.toString());

        while (itemDescr.indexOf("<br>") >= 0) {
            itemDescr = itemDescr.replace("<br>", String.fromCharCode(10));
        }

        if (imagePath) {
            imagePath = $.trim(imagePath.toString());
        }

        editableRow.find(".nameInput:first").attr("value", itemName);
        editableRow.find(".priceInput:first").attr("value", itemPrice);
        editableRow.find(".descriptionInput:first").val(itemDescr);

        var selection = editableRow.find(".imageSourceRadio"); // not first, all of them
        $.each(selection, function (selIndex, selItem) {
            selItem.checked = false;
        });

        editableRow.find(".noChange:first").hide();
        editableRow.find(".fromWebLink:first").hide();
        editableRow.find(".fromLocalFile:first").hide();
        editableRow.find(".noImage:first").hide();

        if (imagePath) {
            editableRow.find(".noChange:first").show();
            selection[0].checked = true;
        }
        else {
            editableRow.find(".noChange:first").hide();
            editableRow.find(".fromWebLink:first").show();
            editableRow.find(".fromLocalFile:first").hide();
            editableRow.find(".noImage:first").show();
            selection[1].checked = true;
        }

        $(".priceInput").ForceNumericOnly();

        // show editable view of the row                
        editableRow.slideDown(function () {
            var rowSeparator = $(this).closest(".rowWrapper").find(".rowSeparator:first");
            var viewPortHeight = $(window).height();
            var scrollTop = $(window).scrollTop();
            var windowFold = viewPortHeight + scrollTop;
            var rowSeparatorTop = rowSeparator.offset().top;
            if (rowSeparatorTop > windowFold) {
                // below the fold... scroll to ensure visibility
                $.scrollTo(scrollTop + rowSeparatorTop - windowFold + 2, 1000, { axis: 'y' });
            }
        });

        return false;
    });
});

//---------------------------------------------------------//
//              Plate Save
//---------------------------------------------------------//

var allowedExtensions = ["jpeg", "jpg", "png", "bmp", "gif", "tif", "tiff"];

$(document).ready(function () {

    $(".saveButton").live('click', function () {

        var form = $(this).closest("form");
        var editableRow = form.closest(".editableRow");
        var readOnlyRow = form.closest(".rowWrapper").find(".row:first");

        // validate form...
        var menuItemData = new Array();

        // get the row Id
        var rowId = form.closest(".rowWrapper").find(".rowId:first").html();
        menuItemData["MenuItemId"] = rowId;

        // get cat Id
        var categoryHeader = readOnlyRow.closest(".menuCategoryWrapper").find(".menuCategoryHeaderWrapper:first");
        var categoryId = categoryHeader.find(".menuCategoryId:first").text();
        menuItemData["MenuCategoryId"] = categoryId;

        // get name
        var itemName = form.find(".nameInput:first").attr("value");
        itemName = $.trim(itemName);
        if (itemName.length <= 0) {
            return false;
        }
        else if (itemName.length > 100) {
            return false;
        }
        menuItemData["MenuItemName"] = itemName;

        // get price which is optional
        var itemPrice = form.find(".priceInput:first").attr("value");
        itemPrice = $.trim(itemPrice);
        if (itemPrice.length > 0) {
            // but once defined, it needs to be valid
            if (itemPrice.charAt(0) == '$') {
                itemPrice = itemPrice.substr(1, itemPrice.length - 1);
                itemPrice = $.trim(itemPrice);
                if (itemPrice.length == 0) {
                    return false;
                }
            }
            if (!isNumber(itemPrice)) {
                return false;
            }
            else if (itemPrice < 0) {
                return false;
            }
            else if (itemPrice > 10000) {
                return false;
            }

            var pricex = parseFloat(itemPrice);
            menuItemData["MenuItemPrice"] = pricex.toFixed(2).toString();
        }

        var itemDescr = form.find(".descriptionInput:first").val();
        itemDescr = $.trim(itemDescr);
        if (itemDescr.length > 0) {
            if (itemDescr.length > 1000) {
                return false;
            }
            menuItemData["MenuItemDescription"] = itemDescr;
        }

        var imageUrlFromWeb = null;
        var imageFrom = form.find(".imageSourceRadio:checked").attr("value");
        if (imageFrom == 1) {
            imageUrlFromWeb = form.find(".fromWebLink:first").attr("value");
            imageUrlFromWeb = $.trim(imageUrlFromWeb);
            if (imageUrlFromWeb.length > 0) {
                menuItemData["MenuItemImageFromURL"] = imageUrlFromWeb;
            }
        }
        else if (imageFrom == 2) {
            var filePath = $(form[0]["imageFileFromLocal"]).val();
            if (filePath) {
                if (!checkFileExt(filePath, allowedExtensions)) {
                    return false;
                }

                menuItemData["IsMenuItemImageFromLocal"] = true;
            }
        }

        function menuItemUploadFailed() {
            alert("Saving the plate has timed out or failed unexpectedly. Please refresh the page and try again.");
        }

        // post-submit callback 
        function menuItemUploadSucceeded(responseText, statusText, xhr, $form) {
            if (statusText == "success") {
                var regexp = new RegExp("^([0-9a-fA-F]){32}$");
                if (regexp.test(responseText)) {

                    // get the menu item id
                    var savedItemId = responseText;

                    // set it as the row Id
                    readOnlyRow.closest(".rowWrapper").find(".rowId:first").html(savedItemId);
                    readOnlyRow.find(".plateName:first").html(menuItemData["MenuItemName"]);

                    if (menuItemData["MenuItemPrice"]) {
                        readOnlyRow.find(".price:first").html("$" + menuItemData["MenuItemPrice"]);
                    }
                    else {
                        readOnlyRow.find(".price:first").empty();
                    }

                    if (menuItemData["MenuItemDescription"]) {
                        var desc = menuItemData["MenuItemDescription"].toString();
                        var newLine = String.fromCharCode(10);
                        while (desc.indexOf(newLine) >= 0) {
                            desc = desc.replace(newLine, "<br>");
                        }

                        readOnlyRow.find(".plateDescription:first").html(desc);
                    }
                    else {
                        readOnlyRow.find(".plateDescription:first").empty();
                    }

                    var imageNode = readOnlyRow.find(".image:first");
                    if (menuItemData["MenuItemImageFromURL"] || menuItemData["IsMenuItemImageFromLocal"]) {
                        var randomId = guidGenerator();
                        var fileId = "MenuHandlers/FileDownload.ashx?fid=" + savedItemId.toString() + "&rid=" + randomId;
                        imageNode.attr("src", fileId);
                    }

                    if (!imageNode.attr("src")) {
                        imageNode.hide();
                    }
                    else {
                        imageNode.show();
                    }

                    // enable editablitiy on other row
                    var menuCategoryWrapper = editableRow.closest(".menuCategoryWrapper");
                    menuCategoryWrapper.removeClass("menuItemIsInEditMode");

                    // update the deletability
                    menuCategoryWrapper.find(".menuCategoryDelete:first").addClass("hidden");

                    // hide the editable view of the row
                    editableRow.slideUp(function () {
                        editableRow.remove();
                    });

                    // show the read-only view of the row
                    readOnlyRow.show();

                    // menu item count become 3+
                    var totalRows = menuCategoryWrapper.find(".rowWrapper");
                    if (totalRows.length >= 3) {
                        menuCategoryWrapper.find(".addMenuItemLinkExtra:first").removeClass("hidden");
                        menuCategoryWrapper.find(".menuCategoryBottomActions:first").removeClass("hidden");
                    }

                    return;
                }
                else if (responseText.indexOf("Authenticate.ashx") >= 0) {
                    window.location = "Login.aspx";
                }
                else {
                    alert(responseText);
                }
            }
            else {
                menuItemUploadFailed();
            }
        }

        var options = {
            url: 'MenuHandlers/MenuItemAddUpdate.ashx',
            type: 'POST',
            success: menuItemUploadSucceeded,   // post-submit callback
            error: menuItemUploadFailed,
            clearForm: true,                    // clear all form fields after successful submit 
            resetForm: true,                    // reset the form after successful submit                 
            dataType: null,
            data: menuItemData,
            timeout: 30 * 1000
        };

        // submit the form
        form.ajaxSubmit(options);

        // avoid a postback to the server
        return false;
    });
});