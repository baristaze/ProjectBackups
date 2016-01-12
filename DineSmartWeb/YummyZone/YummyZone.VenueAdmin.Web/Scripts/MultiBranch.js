//---------------------------------------------------------//
//              MultiBranch Actions Show/Hide
//---------------------------------------------------------//
$(document).ready(function () {

    // prepare the url that needs to be called
    var randomId = guidGenerator();
    var url = "UserHandlers/CheckBranchCount.ashx?rid=" + randomId;

    // make the call to delete the item
    var jqxhr = $.get(url, function (data) {
        if (data > 1) {
            $(".changeBrnchImgDashboard:first").show();
        }
    });
});

//---------------------------------------------------------//
//              MouseClick
//---------------------------------------------------------//
$(document).ready(function () {
    $(".mBrnchRow").live('click', function () {
        $(".allBranchesModalDlg").dialog("close");
    });
});

//---------------------------------------------------------//
//              Switch Branches
//---------------------------------------------------------//
$(document).ready(function () {

    function ShowBranchNames(e) {

        // prepare the url that needs to be called
        var randomId = guidGenerator();
        var url = "UserHandlers/GetBranches.ashx?rid=" + randomId;
        var jsonRequest = $.getJSON(url, function (getBranchesResponse) {
            if (getBranchesResponse.ErrorCode == "0") {

                var allBranches = $(".hiddenParts:first").find(".mBrnchAll:first");
                allBranches = allBranches.clone();
                allBranches.addClass("allBranchesModalDlg");

                var hasItems = false;
                var addToHeight = 0;
                var mBrnchRowTemplate = $(".hiddenParts:first").find(".mBrnchRow:first");
                $.each(getBranchesResponse.Branches, function (branchIndex, branch) {
                    hasItems = true;
                    var mBrnchRow = mBrnchRowTemplate.clone();
                    var mBrnchName = mBrnchRow.find(".mBrnchName:first");
                    mBrnchName.text(branch.Name);
                    mBrnchRow.find(".mBrnchId:first").text(branch.Id);
                    mBrnchRow.find(".mBrnchAddress:first").text(branch.Address);
                    if (branch.IsCurrent) {
                        mBrnchRow.find(".mBrnchTick:first").removeClass("hidden");
                        mBrnchName.removeClass("mBrnchNameNonCurrent");
                        mBrnchName.addClass("mBrnchNameCurrent");
                    }
                    allBranches.append(mBrnchRow);
                    addToHeight += 41;
                });

                if (!hasItems) {
                    alert("This is not any other branch of this venue in the system");
                    return;
                }

                var dlgW = 280;
                var dlgH = 120 + addToHeight;
                if (dlgH > 600) {
                    dlgH = 600;
                }
                var x = e.pageX - (dlgW / 2) + 5;
                var y = e.pageY - (dlgH / 2) - 70;

                var scrollTop = $(window).scrollTop();
                if (y < scrollTop) {
                    y = scrollTop;
                }

                // get selected items
                allBranches.dialog({
                    position: [x, y - $(window).scrollTop()],
                    title: 'Select branch to switch...',
                    resizable: true,
                    height: dlgH,
                    width: dlgW,
                    modal: true,
                    buttons: {
                        Cancel: function () {

                            // close the dialog
                            $(this).dialog("close");
                        }
                    },
                    open: function () {
                        $(".mBrnchRow").click(function () {
                            if ($(this).find(".mBrnchName:first").hasClass("mBrnchNameCurrent")) {
                                $(this).dialog("close");
                            }
                            else {
                                var selectedId = $(this).find(".mBrnchId:first").text();
                                $(this).dialog("close");
                                var url = window.location.pathname;
                                var indexOfSlash = url.indexOf("/");
                                if (indexOfSlash >= 0) {
                                    indexOfSlash++;
                                    url = url.substring(indexOfSlash);
                                    url = url.trim();
                                }

                                if (url.length > 0) {
                                    url = "Login.aspx?branchId=" + selectedId + "&returnUrl=" + url;
                                }
                                else {
                                    url = "Login.aspx?branchId=" + selectedId;
                                }

                                window.location = url;
                            }
                        });
                    }
                });
            }
            else {
                alert(getBranchesResponse.ErrorMessage);
            }
        });

        jsonRequest.error(function (jqXHR, textStatus, errorThrown) {
            if (errorThrown.indexOf("Authenticate.ashx") >= 0) {
                window.location = "Login.aspx";
            }
        });
    }

    $(".changeBrnchImgDashboard").live("click", ShowBranchNames);
});