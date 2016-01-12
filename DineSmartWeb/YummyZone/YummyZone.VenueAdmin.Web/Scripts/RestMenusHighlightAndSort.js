//---------------------------------------------------------//
//              Mouse Over Highlights
//---------------------------------------------------------//        

function OnMouseOverMenuItem() {

    // if it is in edit mode, do not highlight
    var menuCatW = $(this).closest(".menuCategoryWrapper");
    var catsInEditMode = menuCatW.hasClass("menuItemIsInEditMode") | menuCatW.hasClass("menuCategoryIsInEditMode");
    if (catsInEditMode) {
        return;
    }
                
    // un-highlight previous ones:
    $(".highlightedRow").removeClass("highlightedRow");

    // highlight the selected row
    $(this).addClass("highlightedRow");

    // show action menues
    $(this).find(".menuItemActions:first").show();
}

function OnMouseLeaveMenuItem() {
    // un-highlight this:
    $(this).removeClass("highlightedRow");

    // hide action menues
    $(this).find(".menuItemActions:first").hide();
}

function OnMouseOverMenuCategory() {

    // if it is in edit mode, do not highlight
    var menuCatW = $(this).closest(".menuCategoryWrapper");
    var edit1 = menuCatW.hasClass("menuItemIsInEditMode");
    var edit2 = menuCatW.hasClass("menuCategoryIsInEditMode");
    var catsInEditMode = edit1 | edit2;
    if (catsInEditMode) {
        return;
    }

    // un-highlight previous ones:
    $(".menuCategoryHeader").css("background-color", "#ebebeb");

    // highlight the selected row
    $(this).css("background-color", "#edf8dd");

    // show action menues
    $(this).find(".menuCategoryHooverActions:first").show();
}

function OnMouseLeaveMenuCategory() {
        
    // un-highlight this:
    $(this).css("background-color", "#ebebeb");

    // hide action menues
    $(this).find(".menuCategoryHooverActions:first").hide();
}

function OnMouseOverMenuRow() {

    // disable hover when there is an update going on
    if ($(".menuRowList").hasClass("editingMenuFlag")) {
        return;
    }

    // un-highlight previous ones:
    $(".menuRowWrapper .nonSelectedMenuRow").css("background-color", "#ebebeb");
    $(".menuRowWrapper .selectedMenuRow").css("background-color", "#edf3fe");

    if ($(this).hasClass("selectedMenuRow")) {

        // show action menues
        $(this).find(".menuRowHoverActions:first").show();

        return;
    }
    else {
        $(this).find(".menuRowSelectHint:first").show();
    }

    // highlight the selected row
    $(this).css("background-color", "#ffffdd");
}

function OnMouseLeaveMenuRow() {
    // disable hover when there is an update going on
    if ($(".menuRowList").hasClass("editingMenuFlag")) {
        return;
    }

    // un-highlight this:
    if ($(this).hasClass("nonSelectedMenuRow")) {
        $(this).css("background-color", "#ebebeb");
    }
    else {
        $(this).css("background-color", "#edf3fe");
    }

    // hide action menues
    $(this).find(".menuRowHoverActions:first").hide();
    $(this).find(".menuRowSelectHint:first").hide();
}

$(document).ready(function () {

    $(".rowWrapper").live('mouseover', OnMouseOverMenuItem);
    $(".rowWrapper").live('mouseleave', OnMouseLeaveMenuItem);

    $(".menuCategoryHeader").live('mouseover', OnMouseOverMenuCategory);
    $(".menuCategoryHeader").live('mouseleave', OnMouseLeaveMenuCategory);

    $(".menuRowWrapper").live('mouseover', OnMouseOverMenuRow);
    $(".menuRowWrapper").live('mouseleave', OnMouseLeaveMenuRow);
});

//---------------------------------------------------------//
//              Sortable Plates
//---------------------------------------------------------//        

function MenuItemPositionChanged(event, ui) {
    var menuCategoryContent = $(this).closest(".menuCategoryContent");
    var menuCategoryWrapper = menuCategoryContent.closest(".menuCategoryWrapper");
    var menuCategoryId = menuCategoryWrapper.find(".menuCategoryId:first").text();
    var rowIdControls = menuCategoryContent.find(".rowId"); // not just first one

    if (rowIdControls.length == 0) {
        menuCategoryWrapper.find(".menuCategoryDelete:first").removeClass("hidden");
        return;
    }

    var concatenatedRowIds = "";
    rowIdControls.each(function () {
        var rowId = $(this).text();
        if (rowId) {
            concatenatedRowIds += rowId + ",";
        }
    });

    /*    
    // save the new order
    // prepare the url that needs to be called
    var randomId = guidGenerator();
    var url = "MenuHandlers/MenuItemsReorder.ashx?mcid=" + menuCategoryId + "&miids=" + concatenatedRowIds + "&rid=" + randomId;
    var jqxhr = $.get(url, function (data) {

        if (data == "1") {
            menuCategoryWrapper.find(".menuCategoryDelete:first").addClass("hidden");
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
        alert("re-order operation failed. please refresh the page to see the latest order");
    });
    */
    
    // form data
    var reorderData = new Array();
    reorderData["mcid"] = menuCategoryId;
    reorderData["miids"] = concatenatedRowIds;
    
    function menuItemsReorderFailed() {
        alert("re-order operation failed. please refresh the page to see the latest order.");
    }

    // post-submit callback
    function menuItemsReorderSucceeded(responseText, statusText, xhr, $form) {
        if (statusText == "success") {
            if (responseText == "1") {
                menuCategoryWrapper.find(".menuCategoryDelete:first").addClass("hidden");
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
        url: 'MenuHandlers/MenuItemsReorder.ashx',
        type: 'POST',
        success: menuItemsReorderSucceeded,   // post-submit callback
        error: menuItemsReorderFailed,
        clearForm: true,                    // clear all form fields after successful submit 
        resetForm: true,                    // reset the form after successful submit                 
        dataType: null,
        data: reorderData,
        timeout: 30 * 1000
    };

    var reorderform = menuCategoryWrapper.find(".hiddenReorderForm:first");
    reorderform.ajaxSubmit(options);
}

//connectWith: ".menuCategoryContent" 
var menuItemSortableOptions = { update: MenuItemPositionChanged };

function MakeMenuItemsSortable($menuCategoryContentSelector) {
    $menuCategoryContentSelector.sortable(menuItemSortableOptions);
}

$(document).ready(function () {
    MakeMenuItemsSortable($(".menuCategoryContent"));            
});

//---------------------------------------------------------//
//              Sortable Categories
//---------------------------------------------------------//        

function MenuCategoryPositionChanged(event, ui) {

    var menuId = $(".selectedMenuRow").find(".menuId:first").text();
    var menuCategoryIdControls = $(".leftWrapper").find(".menuCategoryId"); // not just first one
            
    var concatMenuCatIds = "";
    menuCategoryIdControls.each(function () {
        var menuCategoryId = $(this).text();
        if (menuCategoryId) {
            concatMenuCatIds += menuCategoryId + ",";
        }
    });

    // save the new order
    // prepare the url that needs to be called
    var randomId = guidGenerator();
    var url = "MenuHandlers/MenuCategoriesReorder.ashx?mid=" + menuId + "&mcids=" + concatMenuCatIds + "&rid=" + randomId; ;
    var jqxhr = $.get(url, function (data) {

        if (data == "1") {
            // do nothing
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
        alert("re-order operation failed. please refresh the page to see the latest order");
    });
}

var menuCategorySortableOptions = { update: MenuCategoryPositionChanged, handle: ".menuCategoryHeaderWrapper" };

$(document).ready(function () {
    $(".leftWrapper").sortable(menuCategorySortableOptions);
});

//---------------------------------------------------------//
//              Sortable Menus
//---------------------------------------------------------//        

function MenuPositionChanged(event, ui) {

    var menuIdControls = $(".menuRowList").find(".menuId"); // not just first one

    var concatMenuIds = "";
    menuIdControls.each(function () {
        var menuId = $(this).text();
        if (menuId) {
            concatMenuIds += menuId + ",";
        }
    });

    // save the new order
    // prepare the url that needs to be called
    var randomId = guidGenerator();
    var url = "MenuHandlers/MenusReorder.ashx?mids=" + concatMenuIds + "&rid=" + randomId; ;
    var jqxhr = $.get(url, function (data) {

        if (data == "1") {
            // do nothing
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
        alert("re-order operation failed. please refresh the page to see the latest order");
    });
}

var menuSortableOptions = { update: MenuPositionChanged };

$(document).ready(function () {
    $(".menuRowList").sortable(menuSortableOptions);
});
