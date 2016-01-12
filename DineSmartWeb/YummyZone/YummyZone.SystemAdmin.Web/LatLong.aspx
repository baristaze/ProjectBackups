<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LatLong.aspx.cs" Inherits="YummyZone.SystemAdmin.Web.LatLong" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">

<head >
    
    <title>Dine Smart 365 - Geocoding</title>
    
    <link href="Styles/Site.css?version=1.04" rel="stylesheet" type="text/css" />

    <script type="text/javascript" src="Scripts/jquery-1.5.1.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery.form.js"></script>
    <script type="text/javascript" src="Scripts/common.js?version=1.04"></script>

    <script type="text/javascript" src="http://maps.googleapis.com/maps/api/js?key=AIzaSyCbAI153_r8vfKa_4ooXR0cVk_DiIopLmI&sensor=true"></script> 

    <style type="text/css" media="screen">
        .main{margin-top:50px;}
        .mapWrp{position:relative;}
        .gmap{width:600px; height:450px;}
        .cross{position: absolute;top:217px;height:19px;width:19px;left: 50%;margin-left:-8px;display: block;background: url(Images/cross.gif);background-position:center center;background-repeat:no-repeat;}
        .outputAndAction{margin-top:8px;width:600px;}
        .outputWrp{float:left;width:540px;text-align:left;margin-top:3px;}
        .saveActionWrp{float:right;width:60px;text-align:right;}        
    </style>

    <script type="text/javascript">

        var _gmap = null;
        var _geocoder = null;
        var _geocodeInvoked = false;

        function geocodeComplete(results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                _gmap.setCenter(results[0].geometry.location);
                _gmap.setZoom(17);
            }
            else{
                alert("Geocode was not successful for the following reason: " + status);
            }
        }

        function initMap(streetAddress) {

            var zoomCtrlOpt = { style: google.maps.ZoomControlStyle.SMALL };
            
            var mapOptions =
            {
                center: new google.maps.LatLng(35.69299463209881, -101.513671875),
                zoom: 4,
                mapTypeControl:false,
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

                if (!_geocodeInvoked) {
                    _geocodeInvoked = true;
                    _geocoder.geocode({ 'address': streetAddress }, geocodeComplete);
                }
            });
        }
        
        $(document).ready(function () {
            initMap('17464 NE 38th St Redmond, WA 98052');
        });

    </script>

</head>

<body>
    <div class="page">
        <div class="main">  
            <center>
                <p>Please locate the restaurant precisely by panning and zooming in. Target zoom level should be >= 19-20</p>
                <div class="mapWrp">
                    <div class="gmap" id="gmap"></div>
                    <div class="cross" id="cross"></div>
                </div>
                <div class="outputAndAction">
                    <div class="outputWrp">
                        <div class="inline"><strong>Latitude :</strong> <span id="lat"></span></div>
                        <div class="inline"><strong>Longitude :</strong> <span id="long"></span></div>
                        <div class="inline"><strong>Zoom Level :</strong> <span id="zoom"></span></div>
                    </div>
                    <div class="saveActionWrp">
                        <input class="saveInfoButton" type="button" value="save" onclick="saveInfoClicked()" />
                    </div>
                </div>
            </center>
        </div>
        <div class="clear"></div>
    </div>
</body>
</html>
