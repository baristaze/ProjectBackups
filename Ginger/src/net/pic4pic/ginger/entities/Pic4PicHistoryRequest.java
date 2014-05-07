package net.pic4pic.ginger.entities;

import java.util.UUID;

import com.google.gson.annotations.SerializedName;

public class Pic4PicHistoryRequest extends BaseRequest {
	
	@SerializedName("UserIdToInteract")
    protected UUID userIdToInteract;
	
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
}
