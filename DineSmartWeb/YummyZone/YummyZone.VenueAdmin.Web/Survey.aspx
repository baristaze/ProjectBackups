<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Survey.aspx.cs" Inherits="YummyZone.VenueAdmin.Web.Survey" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Dine Smart 365 - Survey</title>
    
    <link href="Styles/Site_0_Common.css?version=1.05" rel="stylesheet" type="text/css" />
    <link href="Styles/Survey.css?version=1.04" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.12.custom.css" rel="stylesheet" type="text/css" />

    <script type="text/javascript" src="Scripts/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui-1.8.12.custom.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery.form.js"></script>
    <script type="text/javascript" src="Scripts/jquery.scrollTo-min.js"></script>    
    <script type="text/javascript" src="Scripts/common2.js?version=1.04"></script>
    <script type="text/javascript" src="Scripts/Survey.js?version=1.04"></script>
    <script type="text/javascript" src="Scripts/MultiBranch.js?version=1.00"></script>

    <script type="text/javascript">

        var _gaq = _gaq || [];
        _gaq.push(['_setAccount', 'UA-35375215-1']);
        _gaq.push(['_trackPageview']);

        (function () {
            var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
            ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
            var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
        })();

    </script>

</head>

<body>
    <div class="page">
        
        <!-- below line is NOT a comment but a real CODE -->
        <!--#include file="header.html"-->
        <!-- DO NOT DELETE ABOVE LINE -->
        
        <div class="main">
        
            <div class="hiddenParts hidden">
                
                <div id="confirmDeletingQuestionDlg" >
                    <p>Are you sure you want<br/>to delete this item?</p>
                </div>

                <!-- below line is NOT a comment but a real CODE -->
                <!--#include file="mBrnch.html"-->
                <!-- DO NOT DELETE ABOVE LINE -->

                <div class="surveyQuestionEditable">
                    <input class="questionWording inputText" type="text" />
                    <a href="javascript:void(0);" class="questionSave">save</a> 
                    <a href="javascript:void(0);" class="questionCancel">cancel</a>
                    <div class="clear"></div>
                </div>

                <div class="surveyQuestionWrapper">
                    <div class="surveyQuestionReadOnly">
                        <div class="surveyQuestionId hidden"></div>
                        <div class="surveyQuestionText"></div>
                        <div class="questionActions hidden">
                            <a href="javascript:void(0);" class="editAction">edit</a>
                            <a href="javascript:void(0);" class="deleteAction">delete</a>
                        </div>
                        <div class="clear"></div>
                    </div>
                </div>


                <div class="surveyQuestionEditable surveyQuestionEditableMC">
                    <span>Q: </span>
                    <input class="questionWording inputText" type="text" />
                    <a href="javascript:void(0);" class="questionSave">save</a> 
                    <a href="javascript:void(0);" class="questionCancel">cancel</a>
                    <span class="choicesDesc">(please enter at least three choices below)</span>
                    <div class="clear"></div>                    
                    <div class="choiceList">
                        <div class="choiceInputWrapper">
                            <div class="choiceIdEdit hidden"></div>
                            <span class="choiceInputLabel">choice:</span>
                            <input class="choiceInput inputText" type="text" />
                        </div>
                        <div class="choiceInputWrapper">
                            <div class="choiceIdEdit hidden"></div>
                            <span class="choiceInputLabel">choice:</span>
                            <input class="choiceInput inputText" type="text" />
                        </div>
                        <div class="choiceInputWrapper">
                            <div class="choiceIdEdit hidden"></div>
                            <span class="choiceInputLabel">choice:</span>
                            <input class="choiceInput inputText" type="text" />
                        </div>
                        <div class="choiceInputWrapper">
                            <div class="choiceIdEdit hidden"></div>
                            <span class="choiceInputLabel">choice:</span>
                            <input class="choiceInput inputText" type="text" />
                        </div>
                        <div class="choiceInputWrapper">
                            <div class="choiceIdEdit hidden"></div>
                            <span class="choiceInputLabel">choice:</span>
                            <input class="choiceInput inputText" type="text" />
                        </div>
                    </div>                    
                </div>
                                
                <div class="surveyQuestionWrapper surveyQuestionWrapperMC">
                    <div class="surveyQuestionReadOnly">
                        <div class="surveyQuestionId hidden"></div>
                        <div class="surveyQuestionText"></div>
                        <div class="questionActions hidden">
                            <a href="javascript:void(0);" class="editAction">edit</a>
                            <a href="javascript:void(0);" class="deleteAction">delete</a>
                        </div>
                        <div class="clear"></div>
                    </div>
                    <div class="choiceList">
                    </div>
                </div>                
            </div>
            
            <div class="surveyExplanation">
                Feel free to edit the following survey for your customers.
            </div>

            <!-- rate questions -->
            <div class="surveySectionWrapper roundedCorners4">
                <div class="surveyQuestionType hidden">1</div>
                <div class="surveySectionHeader">
                    <div class="surveySectionTitle">Feedback Items for Diners to Rate?<span class="titleDetail">(other than menu items)</span></div>
                    <div class="addNewWrapper">
                        <a href="javascript:void(0);" class="addNewAction">add new</a>    
                    </div>
                </div>
                <div class="surveyQuestions">
                    <asp:Repeater id="rateQuestionsRepeater" runat="server">
                        <ItemTemplate>
                            <div class="surveyQuestionWrapper">
                                <div class="surveyQuestionReadOnly">
                                    <div class="surveyQuestionId hidden"><%#DataBinder.Eval(Container.DataItem, "Id")%></div>
                                    <div class="surveyQuestionText"><%#DataBinder.Eval(Container.DataItem, "Wording")%></div>
                                    <div class="questionActions hidden">
                                        <a href="javascript:void(0);" class="editAction">edit</a>
                                        <a href="javascript:void(0);" class="deleteAction">delete</a>
                                    </div>
                                    <div class="clear"></div>
                                </div>
                            </div>  
                        </ItemTemplate>
                    </asp:Repeater>
                </div>                
            </div>

            <!-- yes / no -->
            <div class="surveySectionWrapper roundedCorners4">
                <div class="surveyQuestionType hidden">2</div>
                <div class="surveySectionHeader">
                    <div class="surveySectionTitle">Simple Yes/No Questions</div>
                    <div class="addNewWrapper">
                        <a href="javascript:void(0);" class="addNewAction">add new</a>    
                    </div>
                </div>
                <div class="surveyQuestions">
                    <asp:Repeater id="yesNoQuestionsRepeater" runat="server">
                        <ItemTemplate>
                            <div class="surveyQuestionWrapper">
                                <div class="surveyQuestionReadOnly">
                                    <div class="surveyQuestionId hidden"><%#DataBinder.Eval(Container.DataItem, "Id")%></div>
                                    <div class="surveyQuestionText surveyQuestionTextLonger"><%#DataBinder.Eval(Container.DataItem, "Wording")%></div>
                                    <div class="questionActions hidden">
                                        <a href="javascript:void(0);" class="editAction">edit</a>
                                        <a href="javascript:void(0);" class="deleteAction">delete</a>
                                    </div>
                                    <div class="clear"></div>
                                </div>
                            </div>  
                        </ItemTemplate>
                    </asp:Repeater>
                </div>                
            </div>

            <!-- multiple option -->
            <div class="surveySectionWrapper roundedCorners4">
                <div class="surveyQuestionType hidden">3</div>
                <div class="surveySectionHeader">
                    <div class="surveySectionTitle">Multiple Choice Questions</div>
                    <div class="addNewWrapper">
                        <a href="javascript:void(0);" class="addNewAction">add new</a>    
                    </div>
                </div>
                <div class="surveyQuestions">
                    <asp:Repeater id="multiChoiceRepeater" runat="server">
                        <ItemTemplate>
                            <div class="surveyQuestionWrapper surveyQuestionWrapperMC">
                                <div class="surveyQuestionReadOnly">
                                    <div class="surveyQuestionId hidden"><%#DataBinder.Eval(Container.DataItem, "Id")%></div>
                                    <div class="surveyQuestionText surveyQuestionTextLonger"><%#DataBinder.Eval(Container.DataItem, "Wording")%></div>
                                    <div class="questionActions hidden">
                                        <a href="javascript:void(0);" class="editAction">edit</a>
                                        <a href="javascript:void(0);" class="deleteAction">delete</a>
                                    </div>
                                    <div class="clear"></div>
                                </div>
                                <div class="choiceList">
                                    <asp:Repeater id="choiceRepeater" DataSource='<%#DataBinder.Eval(Container.DataItem, "Choices")%>' runat="server">
                                        <ItemTemplate>
                                            <div class="choiceOuter">
                                                <div class="choiceId hidden"><%#DataBinder.Eval(Container.DataItem, "Id")%></div>
                                                <div class="choice"><%#DataBinder.Eval(Container.DataItem, "Wording")%></div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>  
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>

            <!-- open ended -->
            <div class="surveySectionWrapper roundedCorners4">
                <div class="surveyQuestionType hidden">4</div>
                <div class="surveySectionHeader">
                    <div class="surveySectionTitle">Open Ended Questions</div>
                    <div class="addNewWrapper">
                        <a href="javascript:void(0);" class="addNewAction">add new</a>    
                    </div>
                </div>
                <div class="surveyQuestions">
                    <asp:Repeater id="openEndedQuestionsRepeater" runat="server">
                        <ItemTemplate>
                            <div class="surveyQuestionWrapper">
                                <div class="surveyQuestionReadOnly">
                                    <div class="surveyQuestionId hidden"><%#DataBinder.Eval(Container.DataItem, "Id")%></div>
                                    <div class="surveyQuestionText surveyQuestionTextLonger"><%#DataBinder.Eval(Container.DataItem, "Wording")%></div>
                                    <div class="questionActions hidden">
                                        <a href="javascript:void(0);" class="editAction">edit</a>
                                        <a href="javascript:void(0);" class="deleteAction">delete</a>
                                    </div>
                                    <div class="clear"></div>
                                </div>
                            </div>  
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>

        <div class="clear">
        </div>
    </div>

</body>
</html>
