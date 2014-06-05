package net.pic4pic.ginger.entities;

import java.util.UUID;

import com.google.gson.annotations.SerializedName;

public class InstantMessageRequest extends BaseRequest {

	@SerializedName("UserIdToInteract")
    protected UUID userIdToInteract;
	
	@SerializedName("Content")
    protected String content;

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
	 * @return the content
	 */
	public String getContent() {
		return content;
	}

	/**
	 * @param content the content to set
	 */
	public void setContent(String content) {
		this.content = content;
	}
}
