package net.pic4pic.ginger.entities;

import java.util.UUID;
import com.google.gson.annotations.SerializedName;

public class StartingPic4PicRequest extends BaseRequest {

	@SerializedName("UserIdToInteract")
    protected UUID userIdToInteract;
	
	@SerializedName("PictureIdToExchange")
    protected UUID pictureIdToExchange;

	/**
	 * @return the userIdToInteract
	 */
	public UUID getUserIdToInteract() {
		return userIdToInteract;
	}

	/**
	 * @param userIdToInteract the userIdToInteract to set
	 */
	public void setUserIdToInteract(UUID userIdToInteract) {
		this.userIdToInteract = userIdToInteract;
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
