<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ActionLogs.aspx.cs" Inherits="Crosspl.Web.ActionLogs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Admin Diagnostics</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.9.2/jquery-ui.min.js"></script>
        
    <link rel="stylesheet" type="text/css" href="<%=Request.ApplicationPath.TrimEnd('/') + "/css/bootstrap.min.css" %>" />
    <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/js/bootstrap.min.js" %>" ></script>
    <script type="text/javascript" src="<%=Request.ApplicationPath.TrimEnd('/') + "/Scripts/Common.debug.js?version=0.97" %>" ></script>

    <style type="text/css">
        .btn, .btn:active, .btn:focus{outline:none;}
        a,i,li,li:active,li:focus{outline:none;}
        
        .the-container, .filter-block, .act-log-table{margin-top:20px;}
        .user-block{margin-top:10px;}
        .error-message{margin-top:10px;}
        .error-message{text-align:center;}
        .table-block-outer{background-color:White;}
        .the-refresh-icon{margin-right:5px;}
        .act-log-user-photo{cursor:pointer;}
        .act-log-action-data{cursor:pointer;color:#19558f;}
        .act-log-action-data:hover{text-decoration:underline;}
        .act-log-user-img{width:30px;height:30px;}
        .current-user-name{padding-top:10px;}
        .current-user-img{width:50px;height:50px;border:1px solid #ccc;}
        .current-user-name-wrp{text-align:right;}
        /*.current-user-img-wrp{text-align:left;}*/
        .current-user-img-wrp{text-align:right;}
        table, td{vertical-align:middle;}
        .table-cell-wrapper{vertical-align:middle;}
        .user-block{text-align:right;}
                
        .hidden{display:none;}
    </style>

    <script type="text/javascript">

        function getLogFetchSizeFromUrl() {
            var url = window.location.href.toString();
            var index = url.lastIndexOf("/");
            var fetch = "";
            if (index < url.length - 1) {
                fetch = url.substring(index + 1);
                index = fetch.lastIndexOf("?");
                if (index >= 0) {
                    fetch = fetch.substring(0, index);
                }
            }

            var size = -1;
            if (isInteger(fetch)) {
                size = parseInt(fetch);
            }

            return size;
        }

        function updateFetchSizeButtons(size) {
            var className = ".log-fetch-size-" + size.toString();
            $(className).addClass("active");
        }

        function getSizeFromButtonOrDefault($btn) {
            var size = 20;
            if ($btn.hasClass("log-fetch-size-50")) {
                size = 50;
            }
            else if ($btn.hasClass("log-fetch-size-100")) {
                size = 100;
            }
            else if ($btn.hasClass("log-fetch-size-200")) {
                size = 200;
            }

            return size;
        }

        function initActivityLogPage() {
            var size = getLogFetchSizeFromUrl();
            if (size > 0) {
                updateFetchSizeButtons(size);
            }
            else {
                $(".log-fetch-size-20").addClass("active");
            }
        }

        function activityLogPage_DocumentReady() {
            $(".log-fetch-size").live('click', function () {
                if ($(this).hasClass("active")) {
                    return;
                }

                $(".log-fetch-size").removeClass("active");
                $(this).addClass("active");

                var size = getSizeFromButtonOrDefault($(this));
                var url = $("#applicationPath").text() + "/admin/logs/" + size.toString();
                window.location = url;
            });

            $(".button-refresh").live('click', function () {
                var size = getLogFetchSizeFromUrl();
                if (size <= 0) {
                    size = 20;
                }

                var rid = guidGeneratorNoDash();
                var url = $("#applicationPath").text() + "/admin/logs/" + size.toString();
                url += "?rid=" + rid;
                window.location = url;
            });

            $(".act-log-user-photo").live('click', function () {
                var url = $(this).prev("td.act-log-data").find(".act-log-fb-link").text();
                if (url) {
                    window.open(url, "_blank");
                    return false;
                }
            });

            $(".act-log-action-data").live('click', function () {
                var url = $(this).closest("tr").find("td.act-log-data").find(".act-log-data-link").text();
                if (url) {
                    window.open(url, "_blank");
                    return false;
                }
            });

            initActivityLogPage();
        }

        $(document).ready(function () {
            activityLogPage_DocumentReady();
        });

    </script>

</head>
<body>
    <div id="applicationPath" class="hidden" runat="server"></div>
    <div id="isAuthenticated" class="hidden" runat="server">0</div>
    <div id="splitResources" runat="server" class="splitResources hidden"></div>
    <div class="container the-container">
        
        <div class="row span10 well well-small">
          <div id="errorMessage" runat="server" class="row span7 alert alert-error error-message">Error</div>
          <div class="row span2 pull-right user-block">
                <!--
                <div class="span1 current-user-name-wrp">
                    <div id="currentUserName" class="current-user-name" runat="server"></div>
                </div>
                -->
                <div class="span1 current-user-img-wrp">
                    <img id="currentUserPhoto" class="current-user-img"  runat="server" alt="user" src="" />
                </div>                
            </div>     
        </div>

        <div id="interactionBlock" runat="server" class="row span10 well well-small">
          <div class="span9">
                <div class="row filter-block">
                    <div class="span7">
                        <ul class="nav nav-pills">
                            <li class="log-fetch-size log-fetch-size-20"><a href="javascript:void(0);">Last 20 Logs</a></li>
                            <li class="log-fetch-size log-fetch-size-50"><a href="javascript:void(0);">Last 50 Logs</a></li>
                            <li class="log-fetch-size log-fetch-size-100"><a href="javascript:void(0);">Last 100 Logs</a></li>
                            <li class="log-fetch-size log-fetch-size-200"><a href="javascript:void(0);">Last 200 Logs</a></li>
                        </ul>                        
                    </div>
                    <div class="span2">
                        <button class="btn btn-info pull-right button-refresh" type="button"><i class="icon-refresh icon-white the-refresh-icon"></i>Refresh</button>
                    </div>                    
                </div>
            </div>     
        </div>
        <div id="dataBlock" runat="server" class="row span10 well well-small table-block-outer">
            <div class="span9">
                <div class="row table-block">
                    <div class="span9">
                        <table class="table table-hover table-condensed act-log-table">
                            <!--<caption>User Activity Logs</caption>-->
                            <thead>
                                <tr>
                                    <th>Photo</th>
                                    <th>Name</th>
                                    <th>Action</th>
                                    <th>Time (PST)</th>
                                    <th>Data</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater id="activityLogTableRepeater" runat="server">
	                                <ItemTemplate>
		                            <tr class='<%#DataBinder.Eval(Container.DataItem, "ActionTypeColorClass")%>'>
                                        <td class="act-log-data hidden">
                                            <div class="act-log-user-id hidden"><%#DataBinder.Eval(Container.DataItem, "UserId")%></div>
                                            <div class="act-log-user-split hidden"><%#DataBinder.Eval(Container.DataItem, "UserSplit")%></div>
                                            <div class="act-log-fb-id hidden"><%#DataBinder.Eval(Container.DataItem, "FacebookId")%></div>
                                            <div class="act-log-fb-link hidden"><%#DataBinder.Eval(Container.DataItem, "FacebookLink")%></div>
                                            <div class="act-log-data-link hidden"><%#DataBinder.Eval(Container.DataItem, "LinkToData")%></div>
                                        </td>
                                        <td class="act-log-user-photo">
                                            <img src='<%#DataBinder.Eval(Container.DataItem, "PhotoUrl")%>' class="act-log-user-img" alt='...' title="Click to visit user's Facebook page" />
                                        </td>
                                        <td class="act-log-user-name">
                                            <span title='<%#DataBinder.Eval(Container.DataItem, "Profile")%>'><%#DataBinder.Eval(Container.DataItem, "Name")%></span>
                                        </td>
                                        <td class="act-log-action-name">
                                            <%#DataBinder.Eval(Container.DataItem, "Action")%>
                                        </td>
                                        <td class="act-log-action-time">
                                            <%#DataBinder.Eval(Container.DataItem, "ActionTime")%>
                                        </td>
                                        <td class="act-log-action-data">
                                            <%#DataBinder.Eval(Container.DataItem, "ActionData")%>
                                        </td>
                                    </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <!--
	                            <tr>
                                    <td class="act-log-data hidden">
                                        <div class="act-log-user-id hidden"></div>
                                        <div class="act-log-fb-id hidden"></div>
                                        <div class="act-log-fb-link hidden"></div>
                                    </td>
                                    <td class="act-log-user-photo">
                                        <img src="/Images/user.png" class="act-log-user-img img-rounded" alt='...' title="Click to visit user's Facebook page" />
                                    </td>
                                    <td class="act-log-user-name">
                                        <span title="Man, living in Seattle, Washington">Baris Taze</span>
                                    </td>
                                    <td class="act-log-action-name">
                                        Sign Up (New User)
                                    </td>
                                    <td class="act-log-action-time">
                                        July 4th, 1776
                                    </td>
                                    <td class="act-log-action-data">
                                        N/A
                                    </td>
                                </tr>
                                -->
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>            
        </div>
    </div>
</body>
</html>
