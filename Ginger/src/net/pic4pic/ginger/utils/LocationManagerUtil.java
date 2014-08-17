package net.pic4pic.ginger.utils;

import java.io.IOException;
import java.util.List;
import java.util.Locale;

import net.pic4pic.ginger.entities.GeoLocation;
import net.pic4pic.ginger.entities.Locality;

import android.app.Activity;
import android.content.Context;
import android.location.Address;
import android.location.Criteria;
import android.location.Geocoder;
import android.location.Location;
import android.location.LocationManager;
import android.os.Bundle;

public class LocationManagerUtil {
	
	private static final int TIME_INTERVAL_MSEC = 2000;
	private static final int DISTANCE_INTERVAL_METER = 500;
	
	public interface LocationListener{
		void onLocationChanged(GeoLocation geoLocation, Locality locality);
	}
	
	private Activity activity;
	private LocationListener listener;
	
	private GeoLocation lastGeoLocation;
	private Locality lastLocality;
	
	public GeoLocation getLastGeoLocation(){
		return this.lastGeoLocation;
	}
	
	public Locality getLastLocality(){
		return this.lastLocality;
	}
	
	public LocationManagerUtil(Activity activity){
		this(activity, null);
	}
	
	public LocationManagerUtil(Activity activity, LocationListener listener){
		this.activity = activity; 
		this.listener = listener;
	}
	
	public boolean init(){
		
		LocationManager locationManager = (LocationManager) this.activity.getSystemService(Context.LOCATION_SERVICE);
		if(locationManager == null){
			MyLog.e("LocationManagerUtil", "LocationManager is null");
			return false;
		}
		
		Criteria criteria = new Criteria();
		criteria.setAccuracy(Criteria.ACCURACY_FINE);
		String locationProvider = locationManager.getBestProvider(criteria, true);
		if(locationProvider == null || locationProvider.trim().length() <= 0){
			MyLog.e("LocationManagerUtil", "LocationProvider is null");
			return false;
		}
		
		MyLog.i("LocationManagerUtil", "LocationProvider=" + locationProvider);
		Location location = locationManager.getLastKnownLocation(locationProvider);
		if(location != null){
			location.getLatitude();
			location.getLongitude();
			
			this.onLocationChanged(location.getLatitude(), location.getLongitude());
		}
		
		android.location.LocationListener locationListener = new android.location.LocationListener() {
			
			@Override
		    public void onLocationChanged(Location location) {
		    	LocationManagerUtil.this.onLocationChanged(location.getLatitude(), location.getLongitude());
		    }

			@Override
			public void onProviderEnabled(String provider) {
			}
			
			@Override
			public void onStatusChanged(String provider, int status, Bundle extras) {
			}

			@Override
			public void onProviderDisabled(String provider) {	
			}
		};
		
		locationManager.requestLocationUpdates(locationProvider, TIME_INTERVAL_MSEC, DISTANCE_INTERVAL_METER, locationListener);
		return true;
	}
	
	protected void onLocationChanged(double latitude, double longitude){
		
		GeoLocation geoLocation = new GeoLocation();
		geoLocation.setLatitude(latitude);
		geoLocation.setLongitude(longitude);
		
		Locality locality = this.convertToLocality(geoLocation);
		if(locality != null){
			this.lastGeoLocation = geoLocation;
			this.lastLocality = locality;
			
			if(this.listener != null){
				this.listener.onLocationChanged(this.lastGeoLocation, this.lastLocality);
			}
		}
	}
	
	public Locality convertToLocality(GeoLocation geoLocation){
	
		Geocoder geoCoder = new Geocoder(this.activity, Locale.getDefault());
		
		try {
			List<Address> addresses = geoCoder.getFromLocation(geoLocation.getLatitude(), geoLocation.getLongitude(), 1);
            if (addresses.size() > 0){
            	
            	Address address = addresses.get(0);
            	
            	Locality locality = new Locality();
            	locality.setCountry(address.getCountryName());
            	locality.setRegion(address.getAdminArea());
            	locality.setCity(address.getLocality());
            	locality.setZipCode(address.getPostalCode());
            	//address.getCountryCode();		// country
            	//address.getSubAdminArea(); 	// county
            	//address.getSubLocality();		// neighborhood
            	
            	return locality;
            }
            else {
            	MyLog.e("LocationManagerUtil", "No Address info from GeoCode for lat = " + geoLocation.getLatitude() + ", long = " + geoLocation.getLongitude());	
            }
        } 
		catch (IOException e) {
			MyLog.e("LocationManagerUtil", "Geocoder failed: " + e.getMessage());
        }
		
		return null;
	}
}
