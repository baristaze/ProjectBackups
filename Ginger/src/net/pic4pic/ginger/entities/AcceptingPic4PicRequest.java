package net.pic4pic.ginger.entities;

import java.util.UUID;
import com.google.gson.annotations.SerializedName;

public class AcceptingPic4PicRequest extends BaseRequest {
	
	@SerializedName("Pic4PicRequestId")
    protected UUID pic4PicRequestId;
	
	@SerializedName("PictureIdToExchange")
    protected UUID pictureIdToExchange;
	
	/**
	 * @return the pic4PicRequestId
	 */
	public UUID getPic4PicRequestId() {
		return pic4PicRequestId;
	}

	/**
	 * @param pic4PicRequestId the pic4PicRequestId to set
	 */
	public void setPic4PicRequestId(UUID pic4PicRequestId) {
		this.pic4PicRequestId = pic4PicRequestId;
	}

	/**
	 * @return the pictureIdToExchange
	 */
	public UUID getPictureIdToExchange() {
		return pictureIdToExchange;
	}

	/**
	 * @param pictureIdToExchange the pictureIdToExchange to set
	 */
	public void setPictureIdToExchange(UUID pictureIdToExchange) {
		this.pictureIdToExchange = pictureIdToExchange;
	}
}
