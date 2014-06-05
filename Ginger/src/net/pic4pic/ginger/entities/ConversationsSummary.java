package net.pic4pic.ginger.entities;

import java.io.Serializable;
import java.util.Date;
import java.util.UUID;

import com.google.gson.annotations.SerializedName;

public class ConversationsSummary implements Serializable {
	
	private static final long serialVersionUID = 1;
	
	@SerializedName("UserId")
    protected UUID userId;

	@SerializedName("UnreadMessageCount")
	protected int unreadMessageCount;

	@SerializedName("LastUpdateUTC")
	protected Date lastUpdateUTC;

	/**
	 * @return the userId
	 */
	public UUID getUserId() {
		return userId;
	}

	/**
	 * @param userId the userId to set
	 */
	public void setUserId(UUID userId) {
		this.userId = userId;
	}

	/**
	 * @return the unreadMessageCount
	 */
	public int getUnreadMessageCount() {
		return unreadMessageCount;
	}

	/**
	 * @param unreadMessageCount the unreadMessageCount to set
	 */
	public void setUnreadMessageCount(int unreadMessageCount) {
		this.unreadMessageCount = unreadMessageCount;
	}

	/**
	 * @return the lastUpdateUTC
	 */
	public Date getLastUpdateUTC() {
		return lastUpdateUTC;
	}

	/**
	 * @param lastUpdateUTC the lastUpdateUTC to set
	 */
	public void setLastUpdateUTC(Date lastUpdateUTC) {
		this.lastUpdateUTC = lastUpdateUTC;
	}
}
