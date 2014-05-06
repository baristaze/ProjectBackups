package net.pic4pic.ginger.entities;

import java.io.Serializable;
import java.util.Date;
import java.util.UUID;

import com.google.gson.annotations.SerializedName;
import android.content.Context;

import net.pic4pic.ginger.R;

public class Notification implements Serializable {
	
	private static final long serialVersionUID = 1;
	
	@SerializedName("Id")
	protected UUID id;

	@SerializedName("IsViewed")
	protected boolean read;
	
	@SerializedName("ActionType")
	protected NotificationType type;
	
	@SerializedName("Title")
	protected String title;
		
	@SerializedName("ActionTimeUTC")
	protected Date sentTimeUTC;
		
	@SerializedName("StartedBy")
	protected MatchedCandidate sender;
	
	@SerializedName("RecommendedAction")
	protected NotificationAction recommendedAction;

	public UUID getId() {
		return id;
	}

	public void setId(UUID id) {
		this.id = id;
	}
	
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
	
	public Date getSentTimeUTC(){
		return this.sentTimeUTC;
	}
		
	public void setSentTimeUTC(Date sentTime){
		this.sentTimeUTC = sentTime;
	}
	
	public MatchedCandidate getSender(){
		return this.sender;
	}
	
	public void setSender(MatchedCandidate sender){
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
