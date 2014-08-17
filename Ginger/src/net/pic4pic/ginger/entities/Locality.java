package net.pic4pic.ginger.entities;

import java.io.Serializable;

import com.google.gson.annotations.SerializedName;

public class Locality implements Serializable {

	private static final long serialVersionUID = 1L;

	@SerializedName("City")
	protected String city;
	
	@SerializedName("Region")
	protected String region;
	
	@SerializedName("Country")
	protected String country;
	
	@SerializedName("ZipCode")
	protected String zipCode;
	
	@Override
	public String toString(){
		
		String address = "";
		
		if(this.city != null && this.city.length() > 0){
			
			if(address.length() > 0){
				address += ", ";
			}
			
			address += this.city;
		}
		
		if(this.region != null && this.region.length() > 0){
			
			if(address.length() > 0){
				address += ", ";
			}
			
			address += this.region;
		}
		
		if(this.country != null && this.country.length() > 0){
			
			if(address.length() > 0){
				address += ", ";
			}
			
			address += this.country;
		}
		
		if(this.zipCode != null && this.zipCode.length() > 0){
			
			if(address.length() > 0){
				address += ", ";
			}
			
			address += this.zipCode;
		}
		
		return address;
	}
	
	/**
	 * @return the city
	 */
	public String getCity() {
		return city;
	}

	/**
	 * @param city the city to set
	 */
	public void setCity(String city) {
		this.city = city;
	}

	/**
	 * @return the region
	 */
	public String getRegion() {
		return region;
	}

	/**
	 * @param region the region to set
	 */
	public void setRegion(String region) {
		this.region = region;
	}

	/**
	 * @return the country
	 */
	public String getCountry() {
		return country;
	}

	/**
	 * @param country the country to set
	 */
	public void setCountry(String country) {
		this.country = country;
	}

	/**
	 * @return the zipCode
	 */
	public String getZipCode() {
		return zipCode;
	}

	/**
	 * @param zipCode the zipCode to set
	 */
	public void setZipCode(String zipCode) {
		this.zipCode = zipCode;
	}
}
