package net.pic4pic.ginger.entities;

import java.util.UUID;

import com.google.gson.annotations.SerializedName;

public class NotificationRequest extends BaseRequest {
	
	@SerializedName("LastNotificationId")
    protected UUID lastNotificationId;

	/**
	 * @return the lastNotificationId
	 */
	public UUID getLastNotificationId() {
		return lastNotificationId;
	}

	/**
	 * @param lastNotificationId the lastNotificationId to set
	 */
	public void setLastNotificationId(UUID lastNotificationId) {
		this.lastNotificationId = lastNotificationId;
	}
}
