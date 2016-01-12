//---------------------------------------------------//
//            Mouse Over Highlights
//---------------------------------------------------//
function OnMouseOverSearchResult() {

    // un-highlight previous ones:
    $(".highlightedRow").removeClass("highlightedRow");

    // highlight the selected row
    $(this).addClass("highlightedRow");

    // show action menues
    $(this).find(".searchResultRightInner:first").show();
}

function OnMouseLeaveSearchResult() {

    // un-highlight this:
    $(this).removeClass("highlightedRow");

    // hide action menues
    $(this).find(".searchResultRightInner:first").hide();
}

$(document).ready(function () {

    $(".searchResultWrp").live('mouseover', OnMouseOverSearchResult);
    $(".searchResultWrp").live('mouseleave', OnMouseLeaveSearchResult);
    $(".searchResultListWrp").sortable(); // good for ios devices!!!
    
    // touch devices
    if (navigator.userAgent.match(/iPhone/i) ||
        navigator.userAgent.match(/iPad/i) ||
        navigator.userAgent.match(/iPod/i)) {
    }
});

//---------------------------------------------------//
//               Mouse Over Actions
//---------------------------------------------------//    

$(".manageAction").live('click', function () {
    var searchResultWrp = $(this).closest(".searchResultWrp");
    var objType = searchResultWrp.find(".objectType").html();
    if (objType == 1) {
        var objId = searchResultWrp.find(".objectId").html();
        if (objId) {
            window.location = "Default.aspx?redirect=" + objId;
        }
    }
    return false;
});

function toggleActivation(searchResultWrp) {
    var objType = searchResultWrp.find(".objectType").html();
    var objId = searchResultWrp.find(".objectId").html();
    var objStat = searchResultWrp.find(".objectStatus").html();

    var act = 1;
    if (objStat == 1) {
        act = 0;
    }

    var randomId = guidGenerator();
    var url = "OrgHandlers/ToggleActivation.ashx?type=" + objType + "&id=" + objId + "&act=" + act + "&rid=" + randomId;

    var jqxhr = $.get(url, function (data) {
        if (data == 0) {
            if (act == 0) {
                searchResultWrp.find(".objectStatus").html("0");
                searchResultWrp.find(".entityStatus").html("(draft)");
                searchResultWrp.find(".activateAction").show();
                searchResultWrp.find(".deactivateAction").hide();
            }
            else {
                searchResultWrp.find(".objectStatus").html("1");
                searchResultWrp.find(".entityStatus").html("");
                searchResultWrp.find(".activateAction").hide();
                searchResultWrp.find(".deactivateAction").show();
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
        alert("Request failed unexpectedly. Please try again later");
    });
}
        
/* manage, activate and de-activate */
$(document).ready(function () {

    $(".activateAction").live('click', function () {
        toggleActivation($(this).closest(".searchResultWrp"));
        return false;
    });

    $(".deactivateAction").live('click', function () {
        toggleActivation($(this).closest(".searchResultWrp"));
        return false;
    });
});

//---------------------------------------------------//
//                initial search
//---------------------------------------------------//    
var _lastSearchQuery;
var _lastSearchTerm;

function GetEntityView(entity) {
    var resources = $(".resources");
    var searchResultWrp = resources.find(".searchResultWrp");
    searchResultWrp = searchResultWrp.clone();

    searchResultWrp.find(".objectId").html(entity.ObjectId);
    searchResultWrp.find(".objectType").html(entity.ObjectTypeAsInt);
    searchResultWrp.find(".objectStatus").html(entity.StatusAsInt);
    searchResultWrp.find(".entityType").html(entity.ObjectTypeAsText);
    searchResultWrp.find(".entityName").html(entity.Name);
    searchResultWrp.find(".entityStatus").html(entity.StatusAsText);
    searchResultWrp.find(".entityAddress").html(entity.AddressAsText);
            
    if (entity.ObjectTypeAsInt == 1) {
        searchResultWrp.find(".manageAction").removeClass("hidden");

        if (entity.StatusAsInt == 0) {
            searchResultWrp.find(".activateAction").removeClass("hidden");
        }
        else if (entity.StatusAsInt == 1) {
            searchResultWrp.find(".deactivateAction").removeClass("hidden");
        }
    }

    return searchResultWrp;
}

function SearchBizs(key) {
    // prepare the url that needs to be called
    var randomId = guidGenerator();
    var url = "OrgHandlers/SearchBiz.ashx?q=" + key + "&rid=" + randomId;
    url = encodeURI(url);
    _lastSearchQuery = $.getJSON(url, function (searchResponse) {
        // process the response
        if (searchResponse.ErrorCode == "0") {
            // insert fdbks to the html
            if (searchResponse.Items.length > 0) {
                $(".searchStatus").html("");
                $(".addNewButton").show();
                $.each(searchResponse.Items, function (itemIndex, entity) {
                    var entityView = GetEntityView(entity);
                    var entityList = $(".searchResultListWrp");
                    entityList.append(entityView);
                });
            }
            else {
                // process the error
                $(".searchStatus").html("No result found. Please revise your search or click <a href='javascript:void(0);' class='addNewLink' onclick='addNewLinkClicked()'>add new</a>");
            }
        }
        else {
            // process the error
            $(".searchStatus").html(searchResponse.ErrorMessage);
        }
    });

    _lastSearchQuery.error(function (jqXHR, textStatus, errorThrown) {
        if (errorThrown.indexOf("Authenticate.ashx") >= 0) {
            window.location = "Login.aspx";
        }
    });

    return false;
}

function startSearch(minInput, force) {
    var txt = $(".searchInput").attr("value");
    txt = $.trim(txt);
    if (force || (txt != _lastSearchTerm)) {

        _lastSearchTerm = txt;

        $(".searchStatus").html("");
        $(".searchResultListWrp").html("");
        $(".addNewButton").hide();
        if (_lastSearchQuery != null) {
            _lastSearchQuery.abort();
        }

        if (txt.length > minInput) {
            $(".searchStatus").html("Searching...");
            SearchBizs(txt);
        }
    }
}

function OnSearchInputKeyUp(event, data) {
    startSearch(2, false);
}

function searchButtonClicked() {
    startSearch(0, true);
    return false;
}

$(document).ready(function () {
    $(".searchInput").keyup(OnSearchInputKeyUp);
    $(".searchButton").click(searchButtonClicked);
});

//---------------------------------------------------//
//                MAP
//---------------------------------------------------//   
var _gmap = null;
var _geocoder = null;
var _geocodeInvoked = false;

function geocodeComplete(results, status) {
    if (status == google.maps.GeocoderStatus.OK) {
        _gmap.setCenter(results[0].geometry.location);
        _gmap.setZoom(17);
    }
    else {
        _gmap.setCenter(new google.maps.LatLng(35.69299463209881, -101.513671875));
        _gmap.setZoom(4);
        alert("Geocoding was not successful. Please go back and\r\ncorrect the address or spot the address on the map manually.");
    }
}

function initMap(streetAddress) {

    var zoomCtrlOpt = { style: google.maps.ZoomControlStyle.SMALL };

    var mapOptions =
    {
        center: new google.maps.LatLng(35.69299463209881, -101.513671875),
        zoom: 4,
        mapTypeControl: false,
        panControl: false,
        rotateControl: false,
        streetViewControl: false,
        overviewMapControl: false,
        zoomControlOptions: zoomCtrlOpt,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };

    _geocoder = new google.maps.Geocoder();
    _gmap = new google.maps.Map(document.getElementById("gmap"), mapOptions);

    // register events
    google.maps.event.addListener(_gmap, "bounds_changed", function () {
        var center = _gmap.getCenter();
        var zoomLevel = _gmap.getZoom();
        document.getElementById("lat").innerHTML = center.lat();
        document.getElementById("long").innerHTML = center.lng();
        document.getElementById("zoom").innerHTML = zoomLevel;
    });
}

//---------------------------------------------------//
//                SAVE ALL
//---------------------------------------------------// 

function saveAllClicked() {

    var errStripDiv = $(".errorStrip2");
    var errTextDiv = errStripDiv.find(".errorText2");
    errTextDiv.html("");
    errStripDiv.hide();

    /* this needs to be ':[checked=checked]' in 1.7.1 version */
    var val = $(".orgTypeRadio:[checked=true]").attr("value");
    if (val == null) {
        val = 0;
    }

    if (val < 1 || val > 4) {
        showError("Please select organization structure", errStripDiv, errTextDiv);
        return false;
    }

    var chainName = null;
    if (val == 2 || val == 4) {
        chainName = $("#chainName").attr("value");
        chainName = $.trim(chainName);
        if (chainName.length == 0) {
            showError("Please specify chain name", errStripDiv, errTextDiv);
            return false;
        }

        if (_selectedChainId.length == 0) {
            showError("Internal error: chainId undefined", errStripDiv, errTextDiv);
            return false;
        }
    }

    var groupName = null;
    if (val == 3 || val == 4) {
        groupName = $("#groupName").attr("value");
        groupName = $.trim(groupName);
        if (groupName.length == 0) {
            showError("Please specify group name", errStripDiv, errTextDiv);
            return false;
        }

        if (_selectedGroupId.length == 0) {
            showError("Internal error: groupId undefined", errStripDiv, errTextDiv);
            return false;
        }
    }

    // get basic info inputs
    var formDataArray = new Array();
    var form = $("#venueInfoForm");
    if (!checkInputs(form, errStripDiv, errTextDiv, formDataArray)) {
        // error is already shown. no need to re-shown. just return
        return false;
    }

    // get lat long
    var lat = $("#lat").text();
    lat = $.trim(lat);
    if (lat.length == 0) {
        showError("Latitude is undefined", errStripDiv, errTextDiv);
        return false;
    }

    var long = $("#long").text();
    long = $.trim(long);
    if (long.length == 0) {
        showError("Longtitude is undefined", errStripDiv, errTextDiv);
        return false;
    }

    formDataArray["latitude"] = lat;
    formDataArray["longtitude"] = long;

    if (val == 2 || val == 4) {
        formDataArray["chainName"] = chainName;
        formDataArray["chainId"] = _selectedChainId;
    }
    else {
        formDataArray["chainName"] = formDataArray["venueName"];
        formDataArray["chainId"] = "00000000-0000-0000-0000-000000000000";
    }

    if (val == 3 || val == 4) {
        formDataArray["groupName"] = groupName;
        formDataArray["groupId"] = _selectedGroupId;
    }
    else {
        formDataArray["groupName"] = formDataArray["venueName"];
        formDataArray["groupId"] = "00000000-0000-0000-0000-000000000000";
    }

    // post inputs
    var options = {
        url: "OrgHandlers/AddVenue.ashx",
        type: 'POST',
        success: saveAllSucceeded,   // post-submit callback
        error: saveAllFailed,
        clearForm: false,    // clear all form fields after successful submit 
        resetForm: false,    // reset the form after successful submit 
        dataType: null,
        data: formDataArray,
        timeout: 30 * 1000
    };

    // submit the form
    form.ajaxSubmit(options);

    // avoid a postback to the server
    return false;
}

// call back on success
function saveAllSucceeded(responseText, statusText, xhr, $form) {

    // process the result
    if (statusText == "success") {
        var regexp = new RegExp("^([0-9a-fA-F]){32}$");
        if (regexp.test(responseText)) {
            window.location = "Default.aspx?redirect=" + responseText;
        }
        else if (responseText.indexOf("Authenticate.ashx") >= 0) {
            window.location = "Login.aspx";
        }               
        else {
            var errStripDiv = $(".errorStrip2");
            var errTextDiv = errStripDiv.find(".errorText2");
            showError(responseText, errStripDiv, errTextDiv);
        }
    }
    else {
        saveAllFailed();
    }
}

function saveAllFailed() {
    var errStripDiv = $(".errorStrip2");
    var errTextDiv = errStripDiv.find(".errorText2");
    showError("Unexpected failure! Please try again.", errStripDiv, errTextDiv);
}

//---------------------------------------------------//
//                 check inputs
//---------------------------------------------------// 

// check text input
function checkMandatoryText(name, value, errorStripDiv, errorTextDiv) {
    value = $.trim(value);
    if (value.length <= 0) {
        showError("Please specify " + name, errorStripDiv, errorTextDiv);
        return false;
    }

    return true;
}

// check all inputs
function checkInputs(form, errorStripDiv, errorTextDiv, formDataArray) {

    var venueName = form.find("#venueName").attr("value");
    if (!checkMandatoryText("Restaurant Name", venueName, errorStripDiv, errorTextDiv)) {
        return false;
    }

    if (venueName.indexOf(";") >= 0) {
        showError("Symbol ';' is not allowed in Venue Name", errorStripDiv, errorTextDiv);
        return false;
    }

    var venueUrl = form.find("#venueUrl").attr("value");

    var venueAddressLine1 = form.find("#venueAddressLine1").attr("value");
    if (!checkMandatoryText("restaurant address", venueAddressLine1, errorStripDiv, errorTextDiv)) {
        return false;
    }

    var venueAddressLine2 = form.find("#venueAddressLine2").attr("value");

    var venueAddressCity = form.find("#venueAddressCity").attr("value");
    if (!checkMandatoryText("City", venueAddressCity, errorStripDiv, errorTextDiv)) {
        return false;
    }

    if (!isValidName(venueAddressCity)) {
        showError("No symbol is allowed in City", errorStripDiv, errorTextDiv);
        return false;
    }

    var venueAddressState = form.find("#venueAddressState").attr("value");
    if (!checkMandatoryText("State", venueAddressState, errorStripDiv, errorTextDiv)) {
        return false;
    }

    venueAddressState = venueAddressState.toUpperCase();
    if (!isValidState(venueAddressState)) {
        showError("Invalid State", errorStripDiv, errorTextDiv);
        return false;
    }

    var venueAddressZip = form.find("#venueAddressZip").attr("value");
    if (!checkMandatoryText("Zip Code", venueAddressZip, errorStripDiv, errorTextDiv)) {
        return false;
    }

    venueAddressZip = $.trim(venueAddressZip);
    if (!isValidZipCode(venueAddressZip)) {
        showError("Invalid Zip Code format", errorStripDiv, errorTextDiv);
        return false;
    }

    var timeZoneIndex = form.find(".timeZoneCombo").val();

    formDataArray["venueName"] = venueName;
    formDataArray["venueUrl"] = venueUrl;
    formDataArray["venueAddressLine1"] = venueAddressLine1;
    formDataArray["venueAddressLine2"] = venueAddressLine2;
    formDataArray["venueAddressCity"] = venueAddressCity;
    formDataArray["venueAddressState"] = venueAddressState;
    formDataArray["venueAddressZip"] = venueAddressZip;
    formDataArray["venueTimeZoneIndex"] = timeZoneIndex;

    return true;
}

//---------------------------------------------------//
//                 show hide errors
//---------------------------------------------------//

// animation on error
function showError(message, errorStripDiv, errorTextDiv) {
    errorTextDiv.html(message);
    errorStripDiv.slideDown();
}

// hide error animation
function hideError(errorStripClsSel, errorTextClsSel) {
    var errorStripDiv = $(errorStripClsSel);
    var errorTextDiv = errorStripDiv.find(errorTextClsSel);
    errorTextDiv.html("");
    errorStripDiv.slideUp(100);
}

// wiring event to hide error on input focus
$(".secInput").live('focus', function () {
    hideError(".errorStrip", ".errorText");
});

// wiring event to hide error on input focus
$(".secInput2").live('focus', function () {
    hideError(".errorStrip2", ".errorText2");
});
    
//---------------------------------------------------//
//                navigation
//---------------------------------------------------//

function addNewLinkClicked() {
    // clear inputs
    $(".venueBasicInfo :input[type=text]").each(function () {
        $(this).attr("value", "");
    });
    // set the combo-box value to the default one
    $(".defaultTimeZoneOption").attr("selected", "selected");
    // clear errors
    $(".errorText").html("");
    // set the venue name by copying the value in the search input
    var vname = $(".searchInput").attr("value");
    vname = $.trim(vname);
    $("#venueName").attr("value", vname);
    $(".searchArea").slideUp(100, function(){
        $(".venueArea").slideDown();
    });

    return false;
}

function cancelAndBackToSearchClicked() {
    $(".venueArea").slideUp(100, function () {
        $(".searchArea").slideDown();
    });

    return false;
}

function nextToMapClicked() {
    var venueAreaDiv = $(".venueArea:first");

    // beginning from scratch: clear error                
    var errorStripDiv = venueAreaDiv.find(".errorStrip");
    var errorTextDiv = venueAreaDiv.find(".errorText");
    errorTextDiv.html("");
    errorStripDiv.hide();

    // get the form
    var form = venueAreaDiv.find("#venueInfoForm");

    // check inputs
    var formDataArray = new Array();
    if (!checkInputs(form, errorStripDiv, errorTextDiv, formDataArray)) {
        // error is already shown. no need to re-shown. just return
        return false;
    }

    var address = formDataArray["venueAddressLine1"];
    if (formDataArray["venueAddressLine2"].length > 0) {
        address += " ";
        address += formDataArray["venueAddressLine2"];
    }

    address += " ";
    address += formDataArray["venueAddressCity"];
    address += ", ";
    address += formDataArray["venueAddressState"];
    address += " ";
    address += formDataArray["venueAddressZip"];

    if (_gmap == null) {
        initMap();
    }

    if (!_geocodeInvoked) {
        _geocodeInvoked = true;
        _geocoder.geocode({ 'address': address }, geocodeComplete);
    }

    venueAreaDiv.hide();
    $(".mapArea").show();
}

function backToInfoClicked() {
    $(".venueArea").show();
    $(".mapArea").hide();
    _geocodeInvoked = false;
}

function nextToOrgClicked() {

    if (_gmap.getZoom() < 17) {
        alert("zoom level cannot be less than 17");
        return;
    }

    $(".orgTypeRadio").attr("checked", false);
    $(".chainAndVenue").hide();
    $("#chainName").attr("value", "");
    $("#groupName").attr("value", "");
    hideError(".errorStrip2", ".errorText2");
    $(".mapArea").slideUp(100, function () {
        $(".orgAreaWrp").slideDown();
    });

    _selectedChainId = "00000000-0000-0000-0000-000000000000";
    _selectedGroupId = "00000000-0000-0000-0000-000000000000";
    showHideExistingDesc(true, false);
    showHideExistingDesc(false, false);

    return false;
}

function backToMapClicked() {
    $(".orgAreaWrp").slideUp(100, function () {
        $(".mapArea").slideDown();
    });

    return false;
}

//---------------------------------------------------//
//                organization chart UI tweaks
//---------------------------------------------------//

function copyVenueNameClicked(id) {
    hideError(".errorStrip2", ".errorText2");
    var vname = $("#venueName").attr("value");
    vname = $.trim(vname);
    $(id).attr("value", vname);

    if (id == "#chainName") {
        _selectedChainId = "00000000-0000-0000-0000-000000000000";
        showHideExistingDesc(true, false);
    }
    else if (id == "#groupName") {
        _selectedGroupId = "00000000-0000-0000-0000-000000000000";
        showHideExistingDesc(false, false);
    }

    return false;
}

function adjustViewBasedOnOrgTypeRadio(val) {
    $(".chainAndVenue").slideUp(100, function () {
        if (val == 2 || val == 3 || val == 4) {
            if (val == 2 || val == 4) {
                $(".chainLine").show();
            }
            else {
                $(".chainLine").hide();
            }
            if (val == 3 || val == 4) {
                $(".groupLine").show();
            }
            else {
                $(".groupLine").hide();
            }

            $(".chainAndVenue").slideDown();
        }
    });
}

$(document).ready(function () {
    $(".orgTypeRadio").change(function () {
        hideError(".errorStrip2", ".errorText2");
        var val = $(this).attr("value");
        adjustViewBasedOnOrgTypeRadio(val);
    })
});

//---------------------------------------------------//
//          autocomplete chain
//---------------------------------------------------//

var _selectedChainId = "00000000-0000-0000-0000-000000000000";
var _lastChainSearchQuery = null;

function showHideExistingDesc(isChain, show) {
    if (isChain) {
        if (show) {
            $(".descChainExisting").show();
        }
        else {
            $(".descChainExisting").hide();
        }
    }
    else {
        if (show) {
            $(".descGroupExisting").show();
        }
        else {
            $(".descGroupExisting").hide();
        }
    }
}

function autoCompleteSearchChain(request, response) {
    // reset
    _selectedChainId = "00000000-0000-0000-0000-000000000000";
    showHideExistingDesc(true, false);
    if (_lastChainSearchQuery != null) {
        _lastChainSearchQuery.abort();
    }

    // get input
    var searchTerm = request.term;
    searchTerm = $.trim(searchTerm);
    if (searchTerm.length <= 2) {
        return false;
    }

    // prepare the url that needs to be called
    var randomId = guidGenerator();
    var url = "OrgHandlers/SearchBiz.ashx?t=chain&q=" + searchTerm + "&rid=" + randomId;
    url = encodeURI(url);
    _lastChainSearchQuery = $.getJSON(url, function (searchResponse) {
        // process the response
        if (searchResponse.ErrorCode == "0") {
            // insert fdbks to the html
            if (searchResponse.Items.length > 0) {
                var dataArray = new Array();
                $.each(searchResponse.Items, function (itemIndex, entity) {
                    dataArray[itemIndex] = { label: entity.Name, value: entity.Name, objectId: entity.ObjectId };
                });
                // callback
                response(dataArray);
            }
            else {
                response([]); 
            }
        }
        else {
            response([]);
        }
    });

    _lastChainSearchQuery.error(function (jqXHR, textStatus, errorThrown) {
        if (errorThrown.indexOf("Authenticate.ashx") >= 0) {
            window.location = "Login.aspx";
        }
    });
}

function autoCompleteItemSelectedChain(event, ui) {
    if (ui.item != null) {
        _selectedChainId = ui.item.objectId;
        showHideExistingDesc(true, true);
        // search to see if there is a known group for this chain?
        getGroupForSelectedChain(_selectedChainId);
    }
}

$(document).ready(function () {
    var options = { select: autoCompleteItemSelectedChain, source: autoCompleteSearchChain };
    $("#chainName").autocomplete(options);
    $("#chainName").live("focusout", function () {
        var txt = $(this).attr("value");
        if (txt.length == 0) {
            _selectedChainId = "00000000-0000-0000-0000-000000000000";
            showHideExistingDesc(true, false);
        }
    });
});


//---------------------------------------------------//
//          autocomplete group
//---------------------------------------------------//
var _lastGroupSearchQuery = null;
var _selectedGroupId = "00000000-0000-0000-0000-000000000000";

function autoCompleteSearchGroup(request, response) {
    // reset
    _selectedGroupId = "00000000-0000-0000-0000-000000000000";
    showHideExistingDesc(false, false);
    if (_lastGroupSearchQuery != null) {
        _lastGroupSearchQuery.abort();
    }

    // get input
    var searchTerm = request.term;
    searchTerm = $.trim(searchTerm);
    if (searchTerm.length <= 2) {
        return false;
    }

    // prepare the url that needs to be called
    var randomId = guidGenerator();
    var url = "OrgHandlers/SearchBiz.ashx?t=group&q=" + searchTerm + "&rid=" + randomId;
    url = encodeURI(url);
    _lastGroupSearchQuery = $.getJSON(url, function (searchResponse) {
        // process the response
        if (searchResponse.ErrorCode == "0") {
            // insert fdbks to the html
            if (searchResponse.Items.length > 0) {
                var dataArray = new Array();
                $.each(searchResponse.Items, function (itemIndex, entity) {
                    dataArray[itemIndex] = { label: entity.Name, value: entity.Name, objectId: entity.ObjectId };
                });
                // callback
                response(dataArray);
            }
            else {
                response([]);
            }
        }
        else {
            response([]);
        }
    });

    _lastGroupSearchQuery.error(function (jqXHR, textStatus, errorThrown) {
        if (errorThrown.indexOf("Authenticate.ashx") >= 0) {
            window.location = "Login.aspx";
        }
    });
}

function autoCompleteItemSelectedGroup(event, ui) {
    if (ui.item != null) {
        _selectedGroupId = ui.item.objectId;
        showHideExistingDesc(false, true);
    }
}

$(document).ready(function () {
    var options = { select: autoCompleteItemSelectedGroup, source: autoCompleteSearchGroup };
    $("#groupName").autocomplete(options);
    $("#groupName").live("focusout", function () {
        var txt = $(this).attr("value");
        if (txt.length == 0) {
            _selectedGroupId = "00000000-0000-0000-0000-000000000000";
            showHideExistingDesc(false, false);
        }
    });
});


//---------------------------------------------------//
//       get gorup for autoCompleted chain
//---------------------------------------------------//

var _lastGetGroupQuery = null;

function getGroupForSelectedChain(chainId) {

    if (_lastGetGroupQuery != null) {
        _lastGetGroupQuery.abort();
    }

    // prepare the url that needs to be called
    var randomId = guidGenerator();
    var url = "OrgHandlers/GetParent.ashx?pt=group&chid=" + chainId.toString() + "&rid=" + randomId;
    url = encodeURI(url);
    _lastGetGroupQuery = $.getJSON(url, function (searchResponse) {
        // process the response
        if (searchResponse.ErrorCode == "0") {
            // success
            if (searchResponse.Items.length == 1) {
                // update group info
                var entity = searchResponse.Items[0];
                // set name and id
                $("#groupName").attr("value", entity.Name);
                _selectedGroupId = entity.ObjectId;
                showHideExistingDesc(false, true);
                // change the radio if necessary
                /* this needs to be ':[checked=checked]' in 1.7.1 version */
                var val = $(".orgTypeRadio:[checked=true]").attr("value");
                if (val != 4) {
                    $(".orgTypeRadio:[value=4]").click();
                    $(".groupLine").show();
                }
            }
            else {
                // do nothing intentionally
            }
        }
        else {
            // do nothing intentionally
        }
    });

    _lastGetGroupQuery.error(function (jqXHR, textStatus, errorThrown) {
        if (errorThrown.indexOf("Authenticate.ashx") >= 0) {
            window.location = "Login.aspx";
        }
    });
}