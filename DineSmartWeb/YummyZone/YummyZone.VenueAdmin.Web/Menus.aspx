<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Menus.aspx.cs" Inherits="YummyZone.VenueAdmin.Web.Menus" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Dine Smart 365 - Menu</title>
    
    <link href="Styles/Site_0_Common.css?version=1.05" rel="stylesheet" type="text/css" />
    <link href="Styles/Menus.css?version=1.05" rel="stylesheet" type="text/css" />
    <link href="Styles/jquery-ui-1.8.12.custom.css" rel="stylesheet" type="text/css" />

    <script type="text/javascript" src="Scripts/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui-1.8.12.custom.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery.form.js"></script>
    <script type="text/javascript" src="Scripts/jquery.scrollTo-min.js"></script>
    <script type="text/javascript" src="Scripts/common2.js?version=1.04"></script>
    <script type="text/javascript" src="Scripts/RestMenusLoadSelMenu.js?version=1.04"></script>
    <script type="text/javascript" src="Scripts/RestMenusPlates.js?version=1.04"></script>
    <script type="text/javascript" src="Scripts/RestMenusCategories.js?version=1.04"></script>
    <script type="text/javascript" src="Scripts/RestMenusMenus.js?version=1.04"></script>
    <script type="text/javascript" src="Scripts/RestMenusHighlightAndSort.js?version=1.04"></script>
    <script type="text/javascript" src="Scripts/RestMenusBulkInsert.js?version=1.00"></script>
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
                <div id="confirmDeletingMenuDlg" >
                    <p>Are you sure you want<br/>to delete this menu?</p>
                </div>
                
                <div id="confirmDeletingMenuCategoryDlg" >
                    <p>Are you sure you want to<br/>delete this menu category?</p>
                </div>
                
                <div id="confirmDeletingMenuItemDlg" >
                    <p>Are you sure you want<br/>to delete this plate?</p>
                </div>

                <div class="allCategoryNames" >
                </div>

                <!-- below line is NOT a comment but a real CODE -->
                <!--#include file="mBrnch.html"-->
                <!-- DO NOT DELETE ABOVE LINE -->

                <div class="categoryNameAndId">
                    <div class="categoryId hidden"></div>
                    <input class="categoryNameCB" type="checkbox" name="categoryName"/>
                    <span class="categoryName"></span>
                </div>

                <div class="menuEditableRow">
                    <input class="menuEditable inputText" type="text" maxlength="25" />
                    <a href="javascript:void(0);" class="menuSave">save</a> 
                    <a href="javascript:void(0);" class="menuCancel">cancel</a>
                    <div class="serviceTimes">
                        <span class="serveTimeLabel">Served between:</span>
                        <div class="serviceTimeOptions">
                            <select class="serviceTime" name="serviceStartTime">
                               <option value="360" selected="selected">06:00 AM</option>
                               <option value="390">06:30 AM</option>
                               <option value="420">07:00 AM</option>
                               <option value="450">07:30 AM</option>
                               <option value="480">08:00 AM</option>
                               <option value="510">08:30 AM</option>
                               <option value="540">09:00 AM</option>
                               <option value="570">09:30 AM</option>
                               <option value="600">10:00 AM</option>
                               <option value="630">10:30 AM</option>
                               <option value="660">11:00 AM</option>
                               <option value="690">11:30 AM</option>
                               <option value="720">12:00 PM</option>
                               <option value="750">12:30 PM</option>
                               <option value="780">01:00 PM</option>
                               <option value="810">01:30 PM</option>
                               <option value="840">02:00 PM</option>
                               <option value="870">02:30 PM</option>
                               <option value="900">03:00 PM</option>
                               <option value="930">03:30 PM</option>
                               <option value="960">04:00 PM</option>
                               <option value="990">04:30 PM</option>
                               <option value="1020">05:00 PM</option>
                               <option value="1050">05:30 PM</option>
                               <option value="1080">06:00 PM</option>
                               <option value="1110">06:30 PM</option>
                               <option value="1140">07:00 PM</option>
                               <option value="1170">07:30 PM</option>
                               <option value="1200">08:00 PM</option>
                               <option value="1230">08:30 PM</option>
                               <option value="1260">09:00 PM</option>
                               <option value="1290">09:30 PM</option>
                               <option value="1320">10:00 PM</option>
                               <option value="1350">10:30 PM</option>
                               <option value="1380">11:00 PM</option>
                               <option value="1410">11:30 PM</option>
                               <option value="1440">00:00 AM</option>
                               <option value="1470">00:30 AM</option>
                               <option value="1500">01:00 AM</option>
                               <option value="1530">01:30 AM</option>
                               <option value="1560">02:00 AM</option>
                            </select>
                            <select class="serviceTime" name="serviceEndTime">
                               <option value="360">06:00 AM</option>
                               <option value="390">06:30 AM</option>
                               <option value="420">07:00 AM</option>
                               <option value="450">07:30 AM</option>
                               <option value="480">08:00 AM</option>
                               <option value="510">08:30 AM</option>
                               <option value="540">09:00 AM</option>
                               <option value="570">09:30 AM</option>
                               <option value="600">10:00 AM</option>
                               <option value="630">10:30 AM</option>
                               <option value="660">11:00 AM</option>
                               <option value="690">11:30 AM</option>
                               <option value="720">12:00 PM</option>
                               <option value="750">12:30 PM</option>
                               <option value="780">01:00 PM</option>
                               <option value="810">01:30 PM</option>
                               <option value="840">02:00 PM</option>
                               <option value="870">02:30 PM</option>
                               <option value="900">03:00 PM</option>
                               <option value="930">03:30 PM</option>
                               <option value="960">04:00 PM</option>
                               <option value="990">04:30 PM</option>
                               <option value="1020">05:00 PM</option>
                               <option value="1050">05:30 PM</option>
                               <option value="1080">06:00 PM</option>
                               <option value="1110">06:30 PM</option>
                               <option value="1140">07:00 PM</option>
                               <option value="1170">07:30 PM</option>
                               <option value="1200">08:00 PM</option>
                               <option value="1230">08:30 PM</option>
                               <option value="1260">09:00 PM</option>
                               <option value="1290">09:30 PM</option>
                               <option value="1320">10:00 PM</option>
                               <option value="1350">10:30 PM</option>
                               <option value="1380">11:00 PM</option>
                               <option value="1410" selected="selected" >11:30 PM</option>
                               <option value="1440">00:00 AM</option>
                               <option value="1470">00:30 AM</option>
                               <option value="1500">01:00 AM</option>
                               <option value="1530">01:30 AM</option>
                               <option value="1560">02:00 AM</option>
                            </select>
                        </div>
                    </div>
                </div>

                <div class="menuRowWrapper nonSelectedMenuRow roundedCorners2">
                    <div class="menuId hidden"></div>
                    <div class="serviceStartTime hidden"></div>
                    <div class="serviceEndTime hidden"></div>
                    <div class="menuRow">
                        <span class="menuName"></span>
                        <span class="staticMenuLiteral">menu</span>
                    </div>
                    <span class="menuRowSelectHint">(click to select)</span>
                    <span class="menuRowHoverActions">
                        <a href="javascript:void(0);" class="menuRowEditLink">edit</a>
                        <a href="javascript:void(0);" class="menuRowDeleteLink">delete</a>
                    </span>
                    <div class="menuRowActions">
                        <div class="addCategoryIntoMenuLinkWrapper">
                            <a href="javascript:void(0);" class="addCategoryIntoMenuLink" >add a new category</a>
                        </div>                        
                        <div class="importCategoryIntoMenuLinkWrapper">
                            <a href="javascript:void(0);" class="importCategoryIntoMenuLink">import a category from</a>
                        </div>
                    </div>
                </div>

                <div class="menuCategoryNameEditableRow">
                    <input class="menuCategoryNameEditable inputText" type="text" maxlength="40" />
                    <a href="javascript:void(0);" class="menuCategoryNameSave">save</a> 
                    <a href="javascript:void(0);" class="menuCategoryNameCancel">cancel</a> 
                </div>
                
                <div class="menuCategoryWrapper roundedCorners2">
                    <div class="menuCategoryHeaderWrapper">
                        <div class="menuCategoryHeader">
                            <div class="menuCategoryNameWrapper">
                                <div class="menuCategoryNameRow">
                                    <span class="menuCategoryId hidden"></span>
                                    <div class="menuCategoryNameWithToggle">
                                        <img class="treeCollapsed hidden" alt="" src="Images/TreeCollapsed.png" />
                                        <img class="treeExpanded" alt="" src="Images/TreeExpanded.png" />
                                        <span class="menuCategoryNameReadOnly"></span></div>
                                        <span class="menuCategoryStaticLabel">category</span>
                                    <div class="menuCategoryHooverActions">                                        
                                        <a class="menuCategoryNameEdit" href="javascript:void(0);">rename</a>
                                        <a class="menuCategoryDelete" href="javascript:void(0);">delete</a></div>
                                </div>
                            </div>
                            <div class="menuCategoryActionsWrapper">
                                <div class="menuCategoryActions">                                        
                                    <a class="addNewMenuItemButton" href="javascript:void(0);">add a new plate</a>
                                </div>                                
                            </div>  
                            <div class="hidden">
                                <form class="hiddenReorderForm">
                                </form>
                            </div>
                            <div class="clearLeft"></div>
                        </div>
                    </div>
                    <div class="menuCategoryContent">
                    </div>
                    <div class="menuCategoryBottomActions hidden">
                        <a class="addNewMenuItemButton addMenuItemLinkExtra hidden" href="javascript:void(0);">[ + ]</a>
                    </div>    
                </div>

                <div class="editableRow" >
                    <form action="MenuHandlers/MenuItemAddUpdate.ashx">
                        <div class="editNameAndPriceWrapper">
                            <div class="editNameWrapper">
                                <div class="namePair">
                                    <span class="nameLabel">Item Name:</span>
                                    <input class="nameInput inputText" type="text" maxlength="50" />
                                </div>
                            </div>
                            <div class="editPriceWrapper">
                                <div class="pricePair">
                                    <span class="priceLabel">Price:</span>
                                    <input class="priceInput inputText" type="text" maxlength="10"/>
                                </div>
                            </div>
                        </div>
                        <div class="editDescriptionWrapper">
                            <div class="descriptionPair">
                                <span class="descriptionLabel">Description:</span>
                                <textarea class="descriptionInput inputText" rows="3" cols="100%" ></textarea>
                            </div>
                        </div>
                        <div class="editImageWrapper">
                            <div class="imagePair">
                                <span class="imageLabel">Item Picture:</span>
                                <span class="noChange">
                                    <input class="imageSourceRadio" type="radio" name="imageSource" value="0" /><span class="imageSourceRadioText">keep current picture</span>
                                </span>
                                <input class="imageSourceRadio" type="radio" name="imageSource" value="1" /><span class="imageSourceRadioText">upload from a web link</span>
                                <input class="imageSourceRadio" type="radio" name="imageSource" value="2" /><span class="imageSourceRadioText">upload from a local file</span>
                                <span class="noImage">
                                    <input class="imageSourceRadio" type="radio" name="imageSource" value="0" /><span class="imageSourceRadioText">no picture</span>
                                </span>
                                <br/>
                                <span class="imageLabel"></span>
                                <input class="imageInput inputText fromWebLink" type="text" maxlength="500"/>
                                <input class="imageInput inputText fromLocalFile" type="file" name="imageFileFromLocal" />
                            </div>
                        </div>
                        <div class="actionsWrapper">
                            <button class="saveButton roundedCorners2">+ save this plate</button>
                            <span>or</span>
                            <a href="javascript:void(0);" class="cancelLink" >cancel</a>
                        </div>
                    </form>
                </div>

                <div class="rowWrapper">
                    <div class="rowId hidden"></div>
                    <div class="row">
                        <div class="imageWrapper">
                            <img class="image" alt="downloading..." src="" />
                        </div> 
                        <div class="menuItemContentWrapper">
                            <div class="menuItemContent">
                                <div class="menuItemFirstLine">
                                    <div class="menuItemActionsWrapper">
                                        <div class="menuItemActions hidden">
                                            <a href="javascript:void(0);" class="editLink" >edit this plate</a>
                                            <span class="linkSeparator"> | </span>
                                            <a href="javascript:void(0);" class="deleteLink" >delete this plate</a>
                                        </div>
                                    </div>
                                    <div class="price"></div>
                                    <div class="plateName"></div>
                                </div>
                                <div class="plateDescription"></div>
                            </div>
                        </div>
                    </div>
                    <div class="clearLeft">
                    </div>
                    <div class="rowSeparator">              
                    </div>
                </div>

            </div>

            <div class="menuListPageContainer">

                <div class="rightWrapper">
                    <div class="right">
                        <div class="menuRowList">
                            <asp:Repeater id="menuRepeater" runat="server">
                                <ItemTemplate>
                                    <div class="menuRowWrapper nonSelectedMenuRow roundedCorners2">
                                        <div class="menuId hidden"><%#DataBinder.Eval(Container.DataItem, "Id")%></div>
                                        <div class="serviceStartTime hidden"><%#DataBinder.Eval(Container.DataItem, "ServiceStartTime")%></div>
                                        <div class="serviceEndTime hidden"><%#DataBinder.Eval(Container.DataItem, "ServiceEndTime")%></div>
                                        <div class="menuRow">
                                            <span class="menuName"><%#DataBinder.Eval(Container.DataItem, "Name")%></span>
                                            <span class="staticMenuLiteral">menu</span>
                                        </div>
                                        <span class="menuRowSelectHint">(click to select)</span>
                                        <span class="menuRowHoverActions">
                                            <a href="javascript:void(0);" class="menuRowEditLink">edit</a>
                                            <a href="javascript:void(0);" class="menuRowDeleteLink">delete</a>
                                        </span>
                                        <div class="menuRowActions">
                                            <div class="addCategoryIntoMenuLinkWrapper">
                                                <a href="javascript:void(0);" class="addCategoryIntoMenuLink" >add a new category</a>
                                            </div>                                            
                                            <div class="importCategoryIntoMenuLinkWrapper">
                                                <a href="javascript:void(0);" class="importCategoryIntoMenuLink">import a category</a>
                                            </div>
                                        </div>
                                    </div>                            
                                </ItemTemplate>              
                            </asp:Repeater>
                        </div>
                        <button class="addNewMenuButton roundedCorners2">+ add a new menu</button>
                    </div>                    
                </div>
                <div class="leftWrapper">                    
                </div>

                <!-- do no add a clear here-->
                <div class="bulkInsertArea">
                    <div class="bulkInsertLinkWrapper">
                        <a class="bulkInsertStartLink" href="javascript:void(0);">Bulk Insert</a>
                    </div>
                    <div class="bulkInsertDataArea hidden">
                        <div class="bulkInsertInstructionsArea">
                            Any new line without a tag or $ symbol will be interpreted as a new plate (for example: Calamari).
                            You can add a [N] tag before the <span class="bulkInsertTag">plate name</span> to mark it explicitly
                            (for example: <span class="bulkInsertTag">[N]</span>Calamari).<br/>
                            <br/>
                            Any <span class="bulkInsertTag">description</span> must be preceded by a <span class="bulkInsertTag">[D]</span> tag. 
                            (for example: [D]This is a description). Descriptions can be on the next line. Add tag [D] before any description line 
                            if you have multiple of them; otherwise they will be interpreted as plate name since they are on a new line.<br/>
                            <br/>
                            Any text starting with a <span class="bulkInsertTag">$</span> sign will be interpreted as price (for example: $8).
                            Price info, too, can be on the next line.<br/>
                            <br/>
                            Multiple <span class="bulkInsertTag">categories</span> can be added at the same time. 
                            Categories must be separated by a <span class="bulkInsertTag">[C]</span> tag. For example: [C]Desserts...
                            If you haven't specified any category, then, the last category in this menu will be assumed.
                            If there is not any category in this menu, then, you are assumed to provide a category in the first line.<br/>
                            <br/>
                            <span class="validExamples">Valid Examples:</span><br/>

                            <div class="bulkInsertInstLeftWrp">
                                <div class="bulkInsertInstLeft">
                                    Cheese Platter<br/>
                                    [N]Caesar Salad [N]Green Salad<br/>
                                    Grilled Salmon $8 [N] Grilled Chicken $12<br/>
                                    [N] Calamari [D]served with parmesan sauce $ 12<br/>
                                    <br/>
                                    [C]Red Wine<br/>
                                    Merlot $13<br/>
                                    Cabernet Sauvignon 2008 $13<br/>
                                    Chateâu Rauzan Despagne [D] Blanc France 2009
                                </div>
                            </div>
                            <div class="bulkInsertInstRightWrp">
                                <div class="bulkInsertInstRight">
                                    [C]White Wine<br/>
                                    Salmon Creek, White Zinfandel $9<br/>
                                    [D]California 2006<br/>
                                    Hirschbach &amp; Sohne, Riesling<br/>
                                    [D] Germany 2003<br/>$7.95<br/>
                                    Raymond, Chardonnay [D]Napa Valley<br/>
                                    [D]Extra description continued from previous line.<br/>
                                    [D]This item has a very long description.
                                </div>
                            </div>
                            <div class="clear">
                            </div>                            
                        </div>
                        <form class="bulkInsertForm" id="bulkInsertForm" action="">
                            <textarea class="bulkInsertData inputText" rows="20" cols="100%" ></textarea>
                        </form>
                        <div class="bulkInsertActionArea">
                            <div class="bulkInsertErrorLeft">
                                <div class="errorStrip hidden">
                                    <div class="errorText"></div>
                                </div>
                            </div>
                            <div class="bulkInsertActionRight">
                                <button class="bulkInsertButton roundedCorners2">add them all</button>
                                <a href="javascript:void(0);" class="bulkInsertCancel">cancel</a>
                            </div>
                        </div>
                        <div class="clear">
                        </div>
                    </div>               
                </div>

                <div class="bottomMarker"><br/></div>
            </div>

        </div>

        <div class="clear">
        </div>

    </div>

</body>
</html>
