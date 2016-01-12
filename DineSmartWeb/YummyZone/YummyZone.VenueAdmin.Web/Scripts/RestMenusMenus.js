//---------------------------------------------------------//
//              Menu Add
//---------------------------------------------------------//

$(document).ready(function () {

    $(".addNewMenuButton").live('click', function () {
        var menuRowList = $(".menuRowList:first");
        if (menuRowList.hasClass("editingMenuFlag")) {
            return false;
        }

        var menuRowWrapper = $(".hiddenParts").find(".menuRowWrapper:first");
        menuRowWrapper = menuRowWrapper.clone();
        menuRowList.append(menuRowWrapper);

        var editLink = menuRowWrapper.find(".menuRowEditLink:first");
        editLink.click();

        menuRowList.addClass("editingMenuFlag");                
        return false;
    });
});

//---------------------------------------------------------//
//              Menu Edit
//---------------------------------------------------------//

$(document).ready(function () {

    $(".menuRowEditLink").live("click", function () {

        // switch the page's status to edit mode
        $(".menuRowList").addClass("editingMenuFlag");

        // disable other actions
        var menuRowWrapper = $(this).closest(".menuRowWrapper");
        var readOnlyMenuRow = menuRowWrapper.find(".menuRow:first");

        menuRowWrapper.find(".menuRowSelectHint:first").hide();
        menuRowWrapper.find(".menuRowActions:first").hide();
        menuRowWrapper.find(".menuRowHoverActions:first").hide();

        // get editable view
        var editableMenu = $(".hiddenParts").find(".menuEditableRow:first");
        editableMenu = editableMenu.clone();

        // get the name
        var menuName = readOnlyMenuRow.find(".menuName:first").text();
        menuName = $.trim(menuName.toString());

        // get service times
        var serviceStartTime = menuRowWrapper.find(".serviceStartTime:first").text();
        var serviceEndTime = menuRowWrapper.find(".serviceEndTime:first").text();

        // set service times
        var serviceStart = editableMenu.find(".serviceTime:first[name='serviceStartTime']");
        var serviceEnd = editableMenu.find(".serviceTime:first[name='serviceEndTime']");

        if (serviceStartTime == "") {
            serviceStart.val('360');
        }
        else {
            serviceStart.val(serviceStartTime);
        }

        if (serviceEndTime == "") {
            serviceEnd.val('1410');
        }
        else {
            serviceEnd.val(serviceEndTime);
        }

        // append the editable view
        readOnlyMenuRow.before(editableMenu);

        // set it to the input
        var nameInput = editableMenu.find(".menuEditable:first");
        nameInput.attr("value", menuName);
        nameInput.focus();
        nameInput.select();

        // hide the read-only view
        readOnlyMenuRow.hide();

        return false;
    });
});

//---------------------------------------------------------//
//              Menu Delete
//---------------------------------------------------------//

$(document).ready(function () {

    $(".menuRowDeleteLink").live('click', function (e) {

        var dlgW = 240;
        var dlgH = 180;

        var x = e.pageX - (dlgW / 2) + 5;
        var y = e.pageY - (dlgH / 2) - 70;

        var scrollTop = $(window).scrollTop();
        if (y < scrollTop) {
            y = scrollTop;
        }

        var menuRowWrapper = $(this).closest(".menuRowWrapper");

        // ask confirmation
        $("#confirmDeletingMenuDlg").dialog({

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
                    var menuId = menuRowWrapper.find(".menuId:first").text();

                    // prepare the url that needs to be called
                    var randomId = guidGenerator();
                    var url = "MenuHandlers/MenuDeleteDisable.ashx?mid=" + menuId + "&act=delete" + "&rid=" + randomId;

                    // make the call to delete the item
                    var jqxhr = $.get(url, function (data) {

                        if (data == "1") {

                            // fade out first and then remove completely
                            menuRowWrapper.fadeOut(null, function () {

                                // remove the menuRowWrapper completely
                                menuRowWrapper.remove();

                                // clear the left
                                var leftWrapper = $(".leftWrapper");
                                leftWrapper.removeClass("menuCategoryIsInEditMode");
                                leftWrapper.empty();
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
//              Menu Cancel
//---------------------------------------------------------//

$(document).ready(function () {

    $(".menuCancel").live('click', function () {

        // get the row
        var menuRowWrapper = $(this).closest(".menuRowWrapper");
        var readOnlyMenu = menuRowWrapper.find(".menuRow:first");
        var editableMenu = menuRowWrapper.find(".menuEditableRow:first");
        var menuId = menuRowWrapper.find(".menuId:first").text();

        editableMenu.remove();
        readOnlyMenu.show();

        if (menuId) { // if we were editing and existing item
            // enable editablitiy                                  
            menuRowWrapper.find(".menuRowActions:first").show();
        }
        else {
            menuRowWrapper.remove();
        }

        $(".menuRowList:first").removeClass("editingMenuFlag");

        return false;
    });
});

//---------------------------------------------------------//
//              Menu Save
//---------------------------------------------------------//

$(document).ready(function () {

    $(".menuEditable").live('keydown', function (e) {

        var key = e.charCode || e.keyCode || 0;
        if (key == 13) {
            $(this).closest(".menuEditableRow").find(".menuSave:first").click();
        }
    });

    $(".menuSave").live('click', function () {

        // get the row
        var menuRowWrapper = $(this).closest(".menuRowWrapper");
        var readOnlyMenu = menuRowWrapper.find(".menuRow:first");
        var editableMenu = menuRowWrapper.find(".menuEditableRow:first");

        // get the menu name
        var menuName = editableMenu.find(".menuEditable:first").attr("value");
        menuName = $.trim(menuName.toString());
        if (menuName.length <= 0) {
            return false;
        }

        var escapedMenuName = encodeURI(menuName);
        while(escapedMenuName.indexOf("&") >= 0){
            escapedMenuName = escapedMenuName.replace("&", "%26");
        }

        // get service times
        var serviceStart = editableMenu.find(".serviceTime:first[name='serviceStartTime']").val();
        var serviceEnd = editableMenu.find(".serviceTime:first[name='serviceEndTime']").val();

        // prepare the url that needs to be called
        var randomId = guidGenerator();
        var menuId = menuRowWrapper.find(".menuId:first").text();
        var url = "MenuHandlers/MenuAddUpdate.ashx?mid=" + menuId + "&mn=" + escapedMenuName + "&sst=" + serviceStart + "&set=" + serviceEnd + "&rid=" + randomId;

        // make the call to update the menu
        var jqxhr = $.get(url, function (data) {
            var regexp = new RegExp("^([0-9a-fA-F]){32}$");
            if (regexp.test(data)) {
                // set the menu name
                readOnlyMenu.find(".menuName:first").text(menuName);

                // set the menu Id
                menuRowWrapper.find(".menuId:first").text(data);

                // set service times
                menuRowWrapper.find(".serviceStartTime:first").text(serviceStart);
                menuRowWrapper.find(".serviceEndTime:first").text(serviceEnd);

                // update the UI
                editableMenu.remove();
                readOnlyMenu.show();

                // enable editablitiy on other rows        
                $(".menuRowList:first").removeClass("editingMenuFlag");

                if (menuId) {
                    menuRowWrapper.find(".menuRowActions:first").show();
                }
                else {
                    menuRowWrapper.click();
                }
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
            alert("operation to save the menu has failed");
        });

        return false;
    });
});