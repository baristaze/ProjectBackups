package net.pic4pic.ginger.entities;

import java.io.Serializable;

import com.google.gson.annotations.SerializedName;

public class GeoLocation implements Serializable {

	private static final long serialVersionUID = 1L;

	@SerializedName("Latitude")
	protected double latitude;
	
	@SerializedName("Longitude")
	protected double longitude;
	
	@Override
	public String toString(){
		return "Lat=" + this.latitude + ", Long=" + this.longitude;
	}

	/**
	 * @return the latitude
	 */
	public double getLatitude() {
		return latitude;
	}

	/**
	 * @param latitude the latitude to set
	 */
	public void setLatitude(double latitude) {
		this.latitude = latitude;
	}

	/**
	 * @return the longitude
	 */
	public double getLongitude() {
		return longitude;
	}

	/**
	 * @param longitude the longitude to set
	 */
	public void setLongitude(double longitude) {
		this.longitude = longitude;
	}
}
