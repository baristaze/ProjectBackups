//---------------------------------------------------------//
//              Simple Initializations
//---------------------------------------------------------//

    $(document).ready(function () {
        $("#mi_srvy").addClass("selectedLink");
    });

//---------------------------------------------------------//
//              Mouse Over Highlights
//---------------------------------------------------------//   

function OnMouseOverQuestion() {
    var topSection = $(this).closest(".surveySectionWrapper");
    if (topSection.hasClass("editingQuestionFlag")) { return false; }

    //$(".highlightedRow").removeClass("highlightedRow");
    //$(this).addClass("highlightedRow");

    $(this).find(".questionActions:first").show();
}

function OnMouseLeaveQuestion() {
    //$(this).removeClass("highlightedRow");
    $(this).find(".questionActions:first").hide();
}
        
$(document).ready(function () {
    $(".surveyQuestionWrapper").live('mouseover', OnMouseOverQuestion);
    $(".surveyQuestionWrapper").live('mouseleave', OnMouseLeaveQuestion);
});

//---------------------------------------------------------//
//              Add a new Question
//---------------------------------------------------------//

$(document).ready(function () {
    $(".addNewAction").live('click', function () {
        var topSection = $(this).closest(".surveySectionWrapper");
        if (topSection.hasClass("editingQuestionFlag")) {
            return false;
        }

        var newQWrapper = null;
        var type = topSection.find(".surveyQuestionType:first").text();                
        if (type == 3) {
            newQWrapper = $(".hiddenParts").find(".surveyQuestionWrapperMC:first");
        }
        else {
            newQWrapper = $(".hiddenParts").find(".surveyQuestionWrapper:first");
        }

        newQWrapper = newQWrapper.clone();
                            
        var questionList = topSection.find(".surveyQuestions:first");
        questionList.append(newQWrapper);

        var editLink = newQWrapper.find(".editAction:first");
        editLink.click();

        return false;
    });
});

//---------------------------------------------------------//
//              Edit a Question
//---------------------------------------------------------//

$(document).ready(function () {

    $(".editAction").live("click", function () {

        // switch the page's status to edit mode
        var topSection = $(this).closest(".surveySectionWrapper");
        topSection.addClass("editingQuestionFlag");

        var questWrapperRO = $(this).closest(".surveyQuestionReadOnly");
        var itemText = questWrapperRO.find(".surveyQuestionText:first").text();
        itemText = $.trim(itemText.toString());

        // get editable view
        var type = topSection.find(".surveyQuestionType:first").text();
        var editableQuestion = $(".hiddenParts").find(".surveyQuestionEditable:first");
        if (type == 3) {
            editableQuestion = $(".hiddenParts").find(".surveyQuestionEditableMC:first");
        }

        editableQuestion = editableQuestion.clone();
        MakeChoicesSortable(editableQuestion.find(".choiceList:first"));
        var inputCtrl = editableQuestion.find(".questionWording:first");

        var maxLen = "120";
        if (type == 1) {
            maxLen = "40";
        }
        else {
            inputCtrl.addClass("questionWordingLonger");
        }

        inputCtrl.attr("maxlength", maxLen);

        // append the editable view
        questWrapperRO.before(editableQuestion);

        // set the value to the input
        var input = editableQuestion.find(".questionWording:first");
        input.attr("value", itemText);
        input.focus();
        input.select();

        // set the choices if this is a muliple-choice question
        if (type == 3) {
            var idIndex = 0;
            var choiceOuters = questWrapperRO.next().find(".choiceOuter");
            $(choiceOuters).each(function () {
                var choiceWord = $(this).find(".choice:first").text();
                if (choiceWord) {
                    var choiceId = $(this).find(".choiceId:first").text();
                    var choiceIdEdit = editableQuestion.find(".choiceIdEdit").eq(idIndex);
                    var choiceWordInput = editableQuestion.find(".choiceInput").eq(idIndex);
                    choiceIdEdit.text(choiceId);
                    choiceWordInput.attr("value", choiceWord);
                    idIndex++;
                }
            });
        }

        // hide the read-only view
        questWrapperRO.hide();
        questWrapperRO.next(".choiceList:first").hide();

        return false;
    });
});

//---------------------------------------------------------//
//              Cancel a Question
//---------------------------------------------------------//

$(document).ready(function () {

    $(".questionCancel").live('click', function () {

        // get the row
        var wrapper = $(this).closest(".surveyQuestionWrapper");
        var readOnly = wrapper.find(".surveyQuestionReadOnly:first");
        var editable = wrapper.find(".surveyQuestionEditable:first");
        var questId = wrapper.find(".surveyQuestionId:first").text();

        editable.remove();
        readOnly.show();
        readOnly.next(".choiceList:first").show();

        var topSection = wrapper.closest(".surveySectionWrapper");
        topSection.removeClass("editingQuestionFlag");

        if (!questId) {
            wrapper.remove();
        }

        return false;
    });
});

//---------------------------------------------------------//
//              Save a Question
//---------------------------------------------------------//

$(document).ready(function () {

    $(".questionWording").live('keydown', function (e) {

        var key = e.charCode || e.keyCode || 0;
        if (key == 13) {
            $(this).closest(".surveyQuestionEditable").find(".questionSave:first").click();
            return false;
        }
    });

    $(".questionSave").live('click', function () {

        // get the row
        var wrapper = $(this).closest(".surveyQuestionWrapper");
        var readOnly = wrapper.find(".surveyQuestionReadOnly:first");
        var editable = wrapper.find(".surveyQuestionEditable:first");
        var topSection = wrapper.closest(".surveySectionWrapper");

        // get the name
        var input = editable.find(".questionWording:first");
        var name = input.attr("value");
        name = $.trim(name.toString());
        if (name.length <= 0) {
            return false;
        }

        // escape the name
        var escapedName = encodeURI(name);
        while (escapedName.indexOf("&") >= 0) {
            escapedName = escapedName.replace("&", "%26");
        }

        // get the type
        var type = topSection.find(".surveyQuestionType:first").text();

        // get choices if this is a multiple choice
        var choiceUrl = "";
        var validChoiceIndex = 0;
        var validChoiceWords = new Array();
        if (type == 3) {
            var choiceList = editable.find(".choiceInputWrapper"); //not just first but all
            var indexer = 0;
            var choiceWords = new Array();
            var choiceIds = new Array();
            choiceList.each(function () {
                var choiceVal = $(this).find(".choiceInput:first").attr("value");
                choiceVal = $.trim(choiceVal);
                if (choiceVal) {
                    validChoiceWords[validChoiceIndex++] = choiceVal;
                    choiceVal = encodeURI(choiceVal);                            
                    while (choiceVal.indexOf("&") >= 0) {
                        choiceVal = choiceVal.replace("&", "%26");                                
                    }
                }
                else {
                    choiceVal = "";
                }

                choiceWords[indexer] = choiceVal;
                choiceIds[indexer] = $(this).find(".choiceIdEdit:first").text();
                indexer++;
            });

            for (var ch = 0; ch < indexer; ch++) {
                choiceUrl += "chwrd" + ch + "=" + choiceWords[ch];
                choiceUrl += "&";
                choiceUrl += "chid" + ch + "=" + choiceIds[ch];
                if (ch != indexer - 1) {
                    choiceUrl += "&";
                }
            }
        }

        // prepare the url that needs to be called
        var id = wrapper.find(".surveyQuestionId:first").text();
        var url = "SurveyHandlers/QuestionAddUpdate.ashx?qtype=" + type + "&qid=" + id + "&qtext=" + escapedName;
        if (type == 3) {
            url += "&" + choiceUrl;
        }

        var randomId = guidGenerator();
        url += "&rid=" + randomId;

        // make the call to update the item
        var jqxhr = $.get(url, function (data) {
            var regexp = new RegExp("^([0-9a-fA-F]){32}([;]([0-9a-fA-F]){32}){0,5}$");
            if (regexp.test(data)) {
                // split the data
                var guidList = data.split(";");
                var returnedQuestionId = guidList[0];

                // set the name
                readOnly.find(".surveyQuestionText:first").text(name);

                // set the Id
                wrapper.find(".surveyQuestionId:first").text(returnedQuestionId);

                // handle choices
                if (type == 3) {
                    // a simple check 
                    if (guidList.length != validChoiceWords.length + 1) {
                        alert("unexpected error; please refresh page");
                    }

                    // prepare html for the readonly view
                    var choiceHtml = "";
                    var itemIndex = 0;
                    $(validChoiceWords).each(function () {
                        // prepare the div to insert
                        var choiceDiv = [];
                        choiceDiv.push(
	                        '<div class="choiceOuter">',
	                        '<div class="choiceId hidden">',
	                        guidList[1 + itemIndex],
	                        '</div><div class="choice">',
	                        validChoiceWords[itemIndex],
	                        '</div></div>'
                        );
                        // append
                        choiceHtml += choiceDiv.join("");
                        itemIndex++;
                    });

                    // set the html to the choice section
                    var choiceListSection = readOnly.next(".choiceList:first");
                    choiceListSection.html(choiceHtml);
                }

                // update the UI for question
                editable.remove();
                readOnly.show();
                readOnly.next(".choiceList:first").show();

                // enable editablitiy on other rows        
                topSection.removeClass("editingQuestionFlag");
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
            alert("operation to save the question failed");
        });

        return false;
    });
});

//---------------------------------------------------------//
//              Delete a Question
//---------------------------------------------------------//

$(document).ready(function () {

    $(".deleteAction").live('click', function (e) {

        var dlgW = 240;
        var dlgH = 180;

        var x = e.pageX - (dlgW / 2) + 5;
        var y = e.pageY - (dlgH / 2) - 70;

        var scrollTop = $(window).scrollTop();
        if (y < scrollTop) {
            y = scrollTop;
        }

        var wrapper = $(this).closest(".surveyQuestionWrapper");

        // ask confirmation
        $("#confirmDeletingQuestionDlg").dialog({

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

                    // get Id
                    var id = wrapper.find(".surveyQuestionId:first").text();

                    // prepare the url that needs to be called
                    var randomId = guidGenerator();
                    var url = "SurveyHandlers/QuestionDeleteDisable.ashx?qid=" + id + "&act=delete" + "&rid=" + randomId;

                    // make the call to delete the item
                    var jqxhr = $.get(url, function (data) {

                        if (data == "1") {

                            // fade out first and then remove completely
                            wrapper.fadeOut(null, function () {

                                // remove the catW completely
                                wrapper.remove();
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
//              Sort Questions and Choices
//---------------------------------------------------------//

function QuestionPositionChanged(event, ui) {

    var topSection = $(ui.item).closest(".surveyQuestions");
    var questionIdCtrls = topSection.find(".surveyQuestionId"); // not just first one

    var concatQuestionIds = "";
    questionIdCtrls.each(function () {
        var questionId = $(this).text();
        if (questionId) {
            concatQuestionIds += questionId + ",";
        }
    });

    // save the new order
    // prepare the url that needs to be called
    var randomId = guidGenerator();
    var url = "SurveyHandlers/QuestionsReorder.ashx?qids=" + concatQuestionIds + "&rid=" + randomId; ;
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

function ChoicePositionChanged(event, ui) {
    var topSection = $(ui.item).closest(".choiceList");
    var questionWrapper = topSection.closest(".surveyQuestionWrapper");
    if (questionWrapper.find(".surveyQuestionEditable").length > 0) {
        // we don't send query if this is an editable view.
        // this is because we have a 'Save' | 'Cancel' button in that scenario
        return;
    }

    var questionId = questionWrapper.find(".surveyQuestionId:first").text();
    var choiceIdCtrls = topSection.find(".choiceId"); // not just first one
    if (choiceIdCtrls.length <= 0) {
        return;
    }

    var concatChoiceIds = "";
    choiceIdCtrls.each(function () {
        var choiceId = $(this).text();
        if (choiceId) {
            concatChoiceIds += choiceId + ",";
        }
    });

    // save the new order
    // prepare the url that needs to be called
    var randomId = guidGenerator();
    var url = "SurveyHandlers/ChoicesReorder.ashx?qid=" + questionId + "&chids=" + concatChoiceIds + "&rid=" + randomId; ;
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

var choiceSortOpt = { update: ChoicePositionChanged };

function MakeChoicesSortable($choiceListSelector) {
    $choiceListSelector.sortable(choiceSortOpt);
}
        
var questionSortOpt = { update: QuestionPositionChanged, handle: ".surveyQuestionReadOnly" };

$(document).ready(function () {
    $(".surveyQuestions").sortable(questionSortOpt);
    MakeChoicesSortable($(".choiceList"));
});