package net.pic4pic.ginger.entities;

import com.google.gson.annotations.SerializedName;

public class BuyingNewMatchRequest extends BaseRequest {

	@SerializedName("MaxCount")
    protected int maxCount;

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
}
