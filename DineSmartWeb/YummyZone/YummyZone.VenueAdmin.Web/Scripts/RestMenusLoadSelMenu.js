//---------------------------------------------------------//
//              Simple Initializations
//---------------------------------------------------------//        
$(document).ready(function () {
    $("#mi_menu").addClass("selectedLink");
    $(".priceInput").ForceNumericOnly();
    $(".menuCategoryHooverActions").hide();
    $(".menuCategoryContent").scrollTo(0);
    $.scrollTo(0);
});

//---------------------------------------------------------//
//              Load Menu Details
//---------------------------------------------------------//        

function OnMouseClickMenuRow(e) {

    if ($(".menuRowList").hasClass("editingMenuFlag")) {
        // do not return false, it affects check-box clicks
        return;
    }

    if ($(this).hasClass("selectedMenuRow")) {
        // do not return false, it affects check-box clicks
        return;
    }

    var menuRowList = $(this).closest(".menuRowList");
    var selectedMenuRow = menuRowList.find(".selectedMenuRow:first");
    selectedMenuRow.removeClass("selectedMenuRow");
    selectedMenuRow.addClass("nonSelectedMenuRow");
    selectedMenuRow.css("background-color", "#ebebeb");
    selectedMenuRow.find(".menuNameSelected:first").removeClass("menuNameSelected");
    selectedMenuRow.find(".menuRowActions:first").hide();
                        
    $(this).removeClass("nonSelectedMenuRow");
    $(this).addClass("selectedMenuRow");
    $(this).css("background-color", "#edf3fe");
    $(this).find(".menuName:first").addClass("menuNameSelected");
    $(this).find(".menuRowActions:first").show();
    $(this).find(".menuRowSelectHint:first").hide();
    var menuRowWrapper = $(this);

    // hide bulk insert area if it is open
    $(".bulkInsertCancel:first").click();

    // remove content
    var leftWrapper = $(".leftWrapper:first");
    leftWrapper.removeClass("menuCategoryIsInEditMode");
    leftWrapper.empty();

    var menuId = $(this).find(".menuId:first").text();

    // prepare the url that needs to be called
    var randomId = guidGenerator();
    var url = "MenuHandlers/MenuDetails.ashx?mid=" + menuId + "&rid=" + randomId;
    var jsonRequest = $.getJSON(url, function (menuDetailsResponse) {
        if (menuDetailsResponse.ErrorCode == "0") {

            var categoryTemplate = $(".hiddenParts").find(".menuCategoryWrapper:first");
            var menuItemTemplate = $(".hiddenParts").find(".rowWrapper:first");
            var hasCategory = false;
            $.each(menuDetailsResponse.Categories, function (catIndex, menuCategory) {
                hasCategory = true;
                var catW = categoryTemplate.clone();
                catW.find(".menuCategoryId:first").text(menuCategory.Id);
                catW.find(".menuCategoryNameReadOnly:first").text(menuCategory.Name);

                var hasItem = false;
                var menuCategoryContent = catW.find(".menuCategoryContent:first");
                $.each(menuCategory.Items, function (menuItemIndex, menuItem) {
                    hasItem = true;
                    var menuItemRow = menuItemTemplate.clone();

                    menuItemRow.find(".rowId:first").text(menuItem.Id);

                    var hasImage = false;
                    if (menuItem.ImageId) {
                        if (menuItem.ImageId != "00000000-0000-0000-0000-000000000000") {
                            hasImage = true;
                        }
                    }
                    var image = menuItemRow.find(".image:first");
                    if (!hasImage) {
                        image.addClass("hidden");
                        image.attr("src", "");
                    }
                    else {
                        image.attr("src", "MenuHandlers/FileDownload.ashx?fid=" + menuItem.ImageId.toString());
                    }

                    if (menuItem.Price) {
                        menuItemRow.find(".price:first").html("$" + menuItem.Price.toFixed(2));
                    }

                    menuItemRow.find(".plateName:first").html(menuItem.Name);

                    if (menuItem.Description) {
                        var itemDescr = menuItem.Description;
                        var newLine = String.fromCharCode(10);
                        var carriageReturn = String.fromCharCode(13);
                        while (itemDescr.indexOf(newLine) >= 0) {
                            itemDescr = itemDescr.replace(newLine, "<br>");
                        }
                        while (itemDescr.indexOf(carriageReturn) >= 0) {
                            itemDescr = itemDescr.replace(carriageReturn, "");
                        }

                        menuItemRow.find(".plateDescription:first").html(itemDescr);
                    }

                    menuCategoryContent.append(menuItemRow);
                });

                if (hasItem) {
                    catW.find(".menuCategoryDelete:first").addClass("hidden");
                }

                if (menuCategory.Items.length >= 3) {
                    catW.find(".addMenuItemLinkExtra:first").removeClass("hidden");
                    catW.find(".menuCategoryBottomActions:first").removeClass("hidden");
                }

                leftWrapper.append(catW);
            });

            if (hasCategory) {
                menuRowWrapper.find(".menuRowDeleteLink:first").addClass("hidden");
            }

            MakeMenuItemsSortable(leftWrapper.find(".menuCategoryContent"));
        }
        else {
            alert(menuDetailsResponse.ErrorMessage);
        }
    });

    jsonRequest.error(function (jqXHR, textStatus, errorThrown) {
        if (errorThrown.indexOf("Authenticate.ashx") >= 0) {
            window.location = "Login.aspx";
        }
    });

    return false;
}

$(document).ready(function () {
    $(".menuRowWrapper").live('click', OnMouseClickMenuRow);

    var latestMenuId = getParameterByName("smid");
    if (latestMenuId) {
        var lastMenuFound = false;
        var allMenuRowWrappers = $(".menuRowList").find(".menuRowWrapper");
        $.each(allMenuRowWrappers, function (menuIndex, menuRowWrapper) {
            var menuId = $(menuRowWrapper).find(".menuId:first").text();
            if (menuId == latestMenuId) {
                lastMenuFound = true;
                $(menuRowWrapper).click();
            }
        });

        if (!lastMenuFound) {
            $(".menuRowList").find(".menuRowWrapper:first").click();
        }
    }
    else {
        $(".menuRowList").find(".menuRowWrapper:first").click();
    }
});
