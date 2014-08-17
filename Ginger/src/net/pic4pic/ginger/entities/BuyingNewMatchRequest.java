package net.pic4pic.ginger.entities;

import com.google.gson.annotations.SerializedName;

public class BuyingNewMatchRequest extends BaseRequest {

	@SerializedName("MaxCount")
    protected int maxCount;
	
	@SerializedName("Location")
    protected Location location;

	/**
	 * @return the maxCount
	 */
	public int getMaxCount() {
		return maxCount;
	}

	/**
	 * @param maxCount the maxCount to set
	 */
	public void setMaxCount(int maxCount) {
		this.maxCount = maxCount;
	}

	/**
	 * @return the location
	 */
	public Location getLocation() {
		return location;
	}

	/**
	 * @param location the location to set
	 */
	public void setLocation(Location location) {
		this.location = location;
	}
}
