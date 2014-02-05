package net.pic4pic.ginger.entities;

import net.pic4pic.ginger.R;
import android.content.Context;

public class Notification {
	
	protected boolean read;
	protected NotificationType type;
	protected String title;
	protected String sentTime;
	protected Person sender;
	protected NotificationAction recommendedAction;

	public boolean isRead(){
		return this.read;
	}
	
	public void setRead(boolean read){
		this.read = read;
	}
	
	public NotificationType getType(){
		return this.type;
	}
	
	public void setType(NotificationType type){
		this.type = type;
	}
	
	public String getTitle(){
		return this.title;
	}
	
	public void setTitle(String title){
		this.title = title;
	}
	
	public String getSentTime(){
		return this.sentTime;
	}
	
	public void setSentTime(String sentTime){
		this.sentTime = sentTime;
	}
	
	public Person getSender(){
		return this.sender;
	}
	
	public void setSender(Person sender){
		this.sender = sender;
	}
	
	public NotificationAction getRecommendedAction(){
		return this.recommendedAction;
	}
	
	public void setRecommendedAction(NotificationAction recommendedAction){
		this.recommendedAction = recommendedAction;
	}
	
	@Override
	public String toString(){
		return this.title;
	}
	
	public static String GetActionText(Context c, NotificationAction action){		
		
		switch(action){
		
			case None:
				return null;
			
			case ViewProfile:
				return c.getString(R.string.action_view_profile);
			
			case ViewMessage:
				return c.getString(R.string.action_view_message);
				
			case RequestP4P:
				return c.getString(R.string.action_request_pic4pic);
			
			case AcceptP4P:
				return c.getResources().getString(R.string.action_accept_pic4pic);
			
			default:
				return null;		
		}
	}
}
