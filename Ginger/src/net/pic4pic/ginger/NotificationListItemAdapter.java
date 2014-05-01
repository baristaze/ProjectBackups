package net.pic4pic.ginger;

import java.util.ArrayList;
import java.util.List;

import android.app.Activity;
import android.content.Intent;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.View.OnClickListener;
import android.widget.BaseExpandableListAdapter;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.TextView;

import net.pic4pic.ginger.entities.ImageFile;
import net.pic4pic.ginger.entities.Notification;
import net.pic4pic.ginger.tasks.ImageDownloadTask;

public class NotificationListItemAdapter extends BaseExpandableListAdapter {

	private Activity activity;
	private ArrayList<Notification> notifications;
	
	private class GroupViewCache {		
		public TextView usernameTextView;
		public TextView titleTextView;
		public TextView timeTextView;		
		public ImageView avatarImageView;	    
	    public Button actionButton;
	}
	
	private class ChildViewCache {		
		public TextView shortBioTextView;
		public TextView descriptionTextView;
	}
	
	public NotificationListItemAdapter(Activity activity, List<Notification> notifications){
		this.activity = activity;
	    this.notifications = new ArrayList<Notification>(notifications);
	}
	
	@Override
	public Object getChild(int groupPosition, int childPosition) {
		return this.notifications.get(groupPosition).getSender();
	}

	@Override
	public long getChildId(int groupPosition, int childPosition) {
		return 0;
	}

	@Override
	public View getChildView(int groupPosition, int childPosition, boolean isLastChild, View convertView, ViewGroup parent) {
		
		if(convertView == null){
			LayoutInflater inflater = LayoutInflater.from(this.activity);
			convertView = inflater.inflate(R.layout.notif_list_item_detail, null);
			
			ChildViewCache viewCache = new ChildViewCache();
			viewCache.shortBioTextView = (TextView) convertView.findViewById(R.id.senderShortBio);
			viewCache.descriptionTextView = (TextView) convertView.findViewById(R.id.senderDescription);
			convertView.setTag(viewCache);
		}
		
		Notification group = (Notification) getGroup(groupPosition);
		ChildViewCache viewCache = (ChildViewCache)convertView.getTag();
		
		// set short biography
		viewCache.shortBioTextView.setText(group.getSender().getCandidateProfile().getShortBio());
				
		// set description
		viewCache.descriptionTextView.setText(group.getSender().getCandidateProfile().getDescription());
		
		return convertView;
	}

	@Override
	public int getChildrenCount(int groupPosition) {
		/*
		NotificationListItem group = (NotificationListItem) getGroup(groupPosition);
		NotificationType type = group.getType(); 
		if(type == NotificationType.LikedBio || 
		   type == NotificationType.ViewedProfile || 
		   type == NotificationType.RequestingP4P){
			return 1;
		}
		
		return 0;*/
		return 1;
	}

	@Override
	public Object getGroup(int groupPosition) {
		return this.notifications.get(groupPosition);
	}

	@Override
	public int getGroupCount() {
		return this.notifications.size();
	}

	@Override
	public long getGroupId(int groupPosition) {
		return 0;
	}

	@Override
	public View getGroupView(int groupPosition, boolean isExpanded, View convertView, ViewGroup parent) {
		
		if (convertView == null) {
			LayoutInflater inflater = LayoutInflater.from(this.activity);
			convertView = inflater.inflate(R.layout.notif_list_item, null);
			
			GroupViewCache cachedView = new GroupViewCache();
			cachedView.usernameTextView = (TextView)convertView.findViewById(R.id.senderUsername);
			cachedView.titleTextView = (TextView)convertView.findViewById(R.id.notifTitle);
			cachedView.timeTextView = (TextView)convertView.findViewById(R.id.sentTime);
			cachedView.avatarImageView = (ImageView) convertView.findViewById(R.id.senderAvatar);
			cachedView.actionButton = ((Button)convertView.findViewById(R.id.notifActionButton));			
			convertView.setTag(cachedView);
		}
		
		final Notification group = (Notification) getGroup(groupPosition);
		GroupViewCache cachedView = (GroupViewCache) convertView.getTag();
		
		// set user name
		cachedView.usernameTextView.setText(group.getSender().getCandidateProfile().getUsername());
		
		// set title
		cachedView.titleTextView.setText(group.getTitle());
		
		// set time info
		cachedView.timeTextView.setText(group.getSentTime());
		
		// set dummy image first...
		cachedView.avatarImageView.setImageResource(android.R.drawable.ic_menu_gallery);
		
		// now set the real image with an asynchronous download operation
		ImageFile imageToDownload = group.getSender().getProfilePics().getThumbnail();
		ImageDownloadTask asyncTask = new ImageDownloadTask(imageToDownload.getId(), cachedView.avatarImageView);
		asyncTask.execute(imageToDownload.getCloudUrl());
		
		// set button
		String actionName = Notification.GetActionText(this.activity, group.getRecommendedAction());		
		cachedView.actionButton.setText(actionName);		
		cachedView.actionButton.setFocusable(false);
		cachedView.actionButton.setOnClickListener(new OnClickListener() {
			@Override
		    public void onClick(View v) {
		    	onNotificationAction(v, group);
		    }
		 });
		
		return convertView;
	}

	@Override
	public boolean hasStableIds() {
		return false;
	}

	@Override
	public boolean isChildSelectable(int groupPosition, int childPosition) {
		return false;
	}
	
	public void onNotificationAction(View v, Notification group){
		// Toast.makeText(this.activity, "Action", Toast.LENGTH_LONG).show();		
		Intent intent = new Intent(this.activity, PersonActivity.class);
		intent.putExtra(PersonActivity.PersonType, group.getSender());

		// calling a child activity for a result keeps the parent activity alive.
		// by that way, we don't have to keep track of active tab when child activity is closed. 
		this.activity.startActivityForResult(intent, PersonActivity.PersonActivityCode);
	}
}
