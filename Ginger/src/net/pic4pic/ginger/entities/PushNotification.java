package net.pic4pic.ginger.entities;

import java.io.Serializable;

import com.google.gson.annotations.SerializedName;

public class PushNotification implements Serializable {

	private static final long serialVersionUID = 1L;

	@SerializedName("NotificationType")
	protected int notificationType;
	
	@SerializedName("ActionType")
	protected int actionType;
	
	@SerializedName("ActionData")
	protected String actionData;
	
	@SerializedName("Title")
	protected String title;
	
	@SerializedName("Message")
	protected String message;
	
	@SerializedName("SmallIcon")
	protected int smallIcon;
	
	public static PushNotification createDefault(){
		
		PushNotification notification = new PushNotification();
		notification.notificationType = 0;
		notification.actionType = 0;
		notification.actionData = "";
		notification.title = "New Notification";
		notification.message = "You have a new notification from pic4pic";
		return notification;
	}

	/**
	 * @return the notificationType
	 */
	public int getNotificationType() {
		return notificationType;
	}

	/**
	 * @param notificationType the notificationType to set
	 */
	public void setNotificationType(int notificationType) {
		this.notificationType = notificationType;
	}

	/**
	 * @return the actionType
	 */
	public int getActionType() {
		return actionType;
	}

	/**
	 * @param actionType the actionType to set
	 */
	public void setActionType(int actionType) {
		this.actionType = actionType;
	}

	/**
	 * @return the actionData
	 */
	public String getActionData() {
		return actionData;
	}

	/**
	 * @param actionData the actionData to set
	 */
	public void setActionData(String actionData) {
		this.actionData = actionData;
	}

	/**
	 * @return the title
	 */
	public String getTitle() {
		return title;
	}

	/**
	 * @param title the title to set
	 */
	public void setTitle(String title) {
		this.title = title;
	}

	/**
	 * @return the message
	 */
	public String getMessage() {
		return message;
	}

	/**
	 * @param message the message to set
	 */
	public void setMessage(String message) {
		this.message = message;
	}

	/**
	 * @return the smallIcon
	 */
	public int getSmallIcon() {
		return smallIcon;
	}

	/**
	 * @param smallIcon the smallIcon to set
	 */
	public void setSmallIcon(int smallIcon) {
		this.smallIcon = smallIcon;
	}
}
