//---------------------------------------------------------//
//              Category Add
//---------------------------------------------------------//

$(document).ready(function () {

    $(".addCategoryIntoMenuLink").live('click', function () {

        var leftWrapper = $(".leftWrapper:first");
        var isAlreadyOpened = leftWrapper.hasClass("menuCategoryIsInEditMode");
        if (!isAlreadyOpened) {
            var catW = $(".hiddenParts").find(".menuCategoryWrapper:first");
            catW = catW.clone();
            leftWrapper.append(catW);

            var editLink = catW.find(".menuCategoryNameEdit:first");
            editLink.click();

            leftWrapper.addClass("menuCategoryIsInEditMode");

            MakeMenuItemsSortable(catW.find(".menuCategoryContent:first"));
        }

        var lastCatWrapper = leftWrapper.find(".menuCategoryWrapper:last");
        var viewPortHeight = $(window).height();
        var scrollTop = $(window).scrollTop();
        var windowFold = viewPortHeight + scrollTop;
        var lastCatWrapperBottom = lastCatWrapper.offset().top + lastCatWrapper.height();
        if (lastCatWrapperBottom > windowFold) {
            // below the fold... scroll to ensure visibility
            $.scrollTo(scrollTop + lastCatWrapperBottom - windowFold + 2, 1000, { axis: 'y' });
        }

        return false;
    });
});

//---------------------------------------------------------//
//              Category Import
//---------------------------------------------------------//

$(document).ready(function () {

    function ShowCategoryNames(e) {

        // prepare the url that needs to be called
        var menuId = $(".selectedMenuRow").find(".menuId:first").text();
        var randomId = guidGenerator();
        var url = "MenuHandlers/MenuCategoryNames.ashx?mid=" + menuId + "&rid=" + randomId;
        var jsonRequest = $.getJSON(url, function (categoryNamesResponse) {
            if (categoryNamesResponse.ErrorCode == "0") {

                var allCategoryNames = $(".hiddenParts:first").find(".allCategoryNames:first");
                allCategoryNames = allCategoryNames.clone();

                var hasItems = false;
                var addToHeight = 0;
                var categoryNameAndIdTemplate = $(".hiddenParts:first").find(".categoryNameAndId:first");
                $.each(categoryNamesResponse.Categories, function (catIndex, nameIdPair) {
                    hasItems = true;
                    categoryNameAndId = categoryNameAndIdTemplate.clone();
                    categoryNameAndId.find(".categoryId:first").text(nameIdPair.Id);
                    categoryNameAndId.find(".categoryName:first").text(nameIdPair.Name);
                    allCategoryNames.append(categoryNameAndId);
                    addToHeight += 20;
                });

                if (!hasItems) {
                    alert("There is not any category in other menus yet");
                    return;
                }

                var dlgW = 360;
                var dlgH = 120 + addToHeight;
                var x = e.pageX - (dlgW / 2) + 5;
                var y = e.pageY - (dlgH / 2) - 70;

                var scrollTop = $(window).scrollTop();
                if (y < scrollTop) {
                    y = scrollTop;
                }

                // get selected items
                allCategoryNames.dialog({
                    position: [x, y - $(window).scrollTop()],
                    title: 'Select categories...',
                    resizable: true,
                    height: dlgH,
                    width: dlgW,
                    modal: true,
                    buttons: {
                        OK: function () {
                            var catIds = "";
                            var allPairs = $(this).find(".categoryNameAndId");
                            $.each(allPairs, function (pairIndex, pairDiv) {
                                var checkbox = $(pairDiv).find(".categoryNameCB:first");
                                if (checkbox.attr("checked")) {
                                    var catId = $(pairDiv).find(".categoryId:first").text();
                                    catIds += catId + ",";
                                }
                            });

                            if (catIds != "") {
                                // get
                                var newRandomId = guidGenerator();
                                var importUrl = "MenuHandlers/MenuImportCategories.ashx?mid=" + menuId + "&mcids=" + catIds + "&rid=" + newRandomId;
                                var dialog = $(this);

                                var jqxhr = $.get(importUrl, function (data) {
                                    if (data == "1") {
                                        // refresh the page
                                        window.location.href = "Menus.aspx?smid=" + menuId;
                                    }
                                    else if (data.indexOf("Authenticate.ashx") >= 0) {
                                        window.location = "Login.aspx";
                                    }
                                    else {
                                        alert(data);
                                    }

                                    // close the dialog
                                    dialog.dialog("close");
                                });
                            }
                            // end of OK
                            // close is happening within get's callback
                        },

                        Cancel: function () {

                            // close the dialog
                            $(this).dialog("close");
                        }
                    }
                });
            }
            else {
                alert(categoryNamesResponse.ErrorMessage);
            }
        });

        jsonRequest.error(function (jqXHR, textStatus, errorThrown) {
            if (errorThrown.indexOf("Authenticate.ashx") >= 0) {
                window.location = "Login.aspx";
            }
        });
    }

    $(".importCategoryIntoMenuLink").live("click", ShowCategoryNames);
});

//---------------------------------------------------------//
//              Category Collapse and Expand
//---------------------------------------------------------//

$(document).ready(function () {

    $(".menuCategoryNameWithToggle").live("click", function () {
        // get read-only view of the category
        var treeExpanded = $(this).find(".treeExpanded:first");
        var treeCollapsed = $(this).find(".treeCollapsed:first");
        var wasCollapsed = treeExpanded.hasClass("hidden");
        var menuCategoryWrapper = $(this).closest(".menuCategoryWrapper");
        var menuCategoryContent = menuCategoryWrapper.find(".menuCategoryContent:first");

        var totalRows = menuCategoryWrapper.find(".rowWrapper");

        menuCategoryContent.toggle();
        var totalRowCount = totalRows.length;

        var isInEditMode = menuCategoryWrapper.hasClass("menuItemIsInEditMode");
        if (isInEditMode) {
            var menuCategoryContent = menuCategoryWrapper.find(".menuCategoryContent:first");
            var last = menuCategoryContent.find(".rowWrapper:last");
            var rowId = last.find(".rowId:first").text();
            rowId = $.trim(rowId);
            if (rowId.length == 0) {
                // add mode
                totalRowCount = totalRowCount - 1;
            }
        }

        if (wasCollapsed) {

            if (totalRowCount >= 3) {
                // it needs to be visible... 
                menuCategoryWrapper.find(".menuCategoryBottomActions:first").removeClass("hidden");
                menuCategoryWrapper.find(".addMenuItemLinkExtra:first").removeClass("hidden");
            }

            treeCollapsed.addClass("hidden");
            treeExpanded.removeClass("hidden");
        }
        else {

            if (totalRowCount >= 3) {
                // it is visible... collapse it
                menuCategoryWrapper.find(".menuCategoryBottomActions:first").addClass("hidden");
                menuCategoryWrapper.find(".addMenuItemLinkExtra:first").addClass("hidden");
            }

            treeExpanded.addClass("hidden");
            treeCollapsed.removeClass("hidden");
        }

        return false;
    });
});

//---------------------------------------------------------//
//              Category Edit
//---------------------------------------------------------//

$(document).ready(function () {

    $(".menuCategoryNameEdit").live("click", function () {

        // do not allow user to edit multiple items
        var menuCategoryWrapper = $(this).closest(".menuCategoryWrapper");
        menuCategoryWrapper.find(".menuCategoryActions:first").hide();

        // switch the page's status to edit mode                
        menuCategoryWrapper.addClass("menuCategoryIsInEditMode");

        // get editable view
        var editableCategory = $(".hiddenParts").find(".menuCategoryNameEditableRow:first");
        editableCategory = editableCategory.clone();

        // get read-only view of the category
        var readOnlyCat = $(this).closest(".menuCategoryNameRow");

        // get the name
        var catName = readOnlyCat.find(".menuCategoryNameReadOnly:first").text();
        catName = $.trim(catName.toString());

        // append the editable view
        readOnlyCat.before(editableCategory);

        // set it to the input
        var catInput = editableCategory.find(".menuCategoryNameEditable:first");
        catInput.attr("value", catName);
        catInput.focus();
        catInput.select();

        // hide the read-only view
        readOnlyCat.hide();

        return false;
    });            
});

//---------------------------------------------------------//
//              Category Cancel
//---------------------------------------------------------//

$(document).ready(function () {

    $(".menuCategoryNameCancel").live('click', function () {

        // get the row
        var categoryNameWrapper = $(this).closest(".menuCategoryNameWrapper");
        var readOnlyCat = categoryNameWrapper.find(".menuCategoryNameRow:first");
        var editableCat = categoryNameWrapper.find(".menuCategoryNameEditableRow:first");
        var catId = categoryNameWrapper.find(".menuCategoryId:first").text();

        editableCat.remove();
        readOnlyCat.show();

        var catW = categoryNameWrapper.closest(".menuCategoryWrapper");

        if (catId) { // if we were editing and existing item
            // enable editablitiy              
            catW.removeClass("menuCategoryIsInEditMode");
            catW.find(".menuCategoryActions:first").show();
        }
        else {
            var leftWrapper = catW.closest(".leftWrapper");
            leftWrapper.removeClass("menuCategoryIsInEditMode");
            catW.remove();
        }

        return false;
    });
});

//---------------------------------------------------------//
//              Category Delete
//---------------------------------------------------------//

$(document).ready(function () {

    $(".menuCategoryDelete").live('click', function (e) {

        var dlgW = 240;
        var dlgH = 180;

        var x = e.pageX - (dlgW / 2) + 5;
        var y = e.pageY - (dlgH / 2) - 70;

        var scrollTop = $(window).scrollTop();
        if (y < scrollTop) {
            y = scrollTop;
        }

        var menuCategoryNameWrapper = $(this).closest(".menuCategoryNameWrapper");

        // ask confirmation
        $("#confirmDeletingMenuCategoryDlg").dialog({

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

                    // get the Category Id
                    var menuCategoryId = menuCategoryNameWrapper.find(".menuCategoryId:first").text();

                    // prepare the url that needs to be called
                    var randomId = guidGenerator();
                    var url = "MenuHandlers/MenuCategoryDeleteDisable.ashx?mcid=" + menuCategoryId.toString() + "&act=delete" + "&rid=" + randomId;

                    // make the call to delete the item
                    var jqxhr = $.get(url, function (data) {

                        if (data == "1") {

                            var catW = menuCategoryNameWrapper.closest(".menuCategoryWrapper");

                            // fade out first and then remove completely
                            catW.fadeOut(null, function () {

                                // remove the catW completely
                                catW.remove();

                                // menu can now be deleted if there is not any category in it
                                var categoryCount = $(".leftWrapper:first").find(".menuCategoryWrapper").length;
                                if (categoryCount == 0) {
                                    var selectedMenu = $(".menuRowList:first").find(".selectedMenuRow:first");
                                    selectedMenu.find(".menuRowDeleteLink:first").removeClass("hidden");
                                }
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
//              Category Save
//---------------------------------------------------------//

$(document).ready(function () {

    $(".menuCategoryNameEditable").live('keydown', function (e) {

        var key = e.charCode || e.keyCode || 0;
        if (key == 13) {
            $(this).closest(".menuCategoryNameEditableRow").find(".menuCategoryNameSave:first").click();
            return false;
        }
    });

    $(".menuCategoryNameSave").live('click', function () {

        // get the row
        var categoryNameWrapper = $(this).closest(".menuCategoryNameWrapper");
        var readOnlyCat = categoryNameWrapper.find(".menuCategoryNameRow:first");
        var editableCat = categoryNameWrapper.find(".menuCategoryNameEditableRow:first");

        // get the category name
        var catInput = editableCat.find(".menuCategoryNameEditable:first");
        var catName = catInput.attr("value");
        catName = $.trim(catName.toString());
        if (catName.length <= 0) {
            return false;
        }

        var escapedCatName = encodeURI(catName);
        while(escapedCatName.indexOf("&") >= 0){
            escapedCatName = escapedCatName.replace("&", "%26");
        }

        // prepare the url that needs to be called
        var catId = categoryNameWrapper.find(".menuCategoryId:first").text();
        var randomId = guidGenerator();
        var menuId = $(".selectedMenuRow").find(".menuId:first").text();
        var url = "MenuHandlers/MenuCategoryAddUpdate.ashx?mid=" + menuId + "&mcid=" + catId + "&mcn=" + escapedCatName + "&rid=" + randomId;

        // make the call to update the item
        var jqxhr = $.get(url, function (data) {
            var regexp = new RegExp("^([0-9a-fA-F]){32}$");
            if (regexp.test(data)) {
                // set the category name
                readOnlyCat.find(".menuCategoryNameReadOnly:first").text(catName);

                // set the category Id
                categoryNameWrapper.find(".menuCategoryId:first").text(data);

                // update the UI
                editableCat.remove();
                readOnlyCat.show();

                // enable editablitiy on other rows        
                var catW = categoryNameWrapper.closest(".menuCategoryWrapper");
                catW.removeClass("menuCategoryIsInEditMode");

                catW.find(".menuCategoryActions:first").show();

                if (!catId) {
                    var leftWrapper = catW.closest(".leftWrapper");
                    leftWrapper.removeClass("menuCategoryIsInEditMode");
                }

                // menu cannot be deleted now...
                var selectedMenu = $(".menuRowList:first").find(".selectedMenuRow:first");
                selectedMenu.find(".menuRowDeleteLink:first").addClass("hidden");
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
            alert("operation to save the category name failed");
        });

        return false;
    });
});