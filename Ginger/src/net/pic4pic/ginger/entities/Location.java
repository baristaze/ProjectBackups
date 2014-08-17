package net.pic4pic.ginger.entities;

import java.io.Serializable;

import com.google.gson.annotations.SerializedName;

public class Location implements Serializable {
	
	private static final long serialVersionUID = 1L;

	@SerializedName("GeoLocation")
	protected GeoLocation geoLocation;
	
	@SerializedName("Locality")
	protected Locality locality;

	/**
	 * @return the geoLocation
	 */
	public GeoLocation getGeoLocation() {
		return geoLocation;
	}

	/**
	 * @param geoLocation the geoLocation to set
	 */
	public void setGeoLocation(GeoLocation geoLocation) {
		this.geoLocation = geoLocation;
	}

	/**
	 * @return the locality
	 */
	public Locality getLocality() {
		return locality;
	}

	/**
	 * @param locality the locality to set
	 */
	public void setLocality(Locality locality) {
		this.locality = locality;
	}
}
