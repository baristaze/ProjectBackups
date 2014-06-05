package net.pic4pic.ginger.entities;

import java.util.UUID;

import com.google.gson.annotations.SerializedName;

public class ConversationRequest extends BaseRequest
{
	@SerializedName("UserIdToInteract")
    protected UUID userIdToInteract;	
	
	@SerializedName("LastExchangedMessageId")
    protected UUID lastExchangedMessageId;

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
	 * @return the lastExchangedMessageId
	 */
	public UUID getLastExchangedMessageId() {
		return lastExchangedMessageId;
	}

	/**
	 * @param lastExchangedMessageId the lastExchangedMessageId to set
	 */
	public void setLastExchangedMessageId(UUID lastExchangedMessageId) {
		this.lastExchangedMessageId = lastExchangedMessageId;
	}
}
