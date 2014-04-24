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

import net.pic4pic.ginger.entities.Notification;
import net.pic4pic.ginger.tasks.ImageDownloadTask;

public class NotificationListItemAdapter extends BaseExpandableListAdapter {

	private Activity activity;
	private ArrayList<Notification> notifications;
	private LayoutInflater inflater;
	
	public NotificationListItemAdapter(Activity activity, List<Notification> notifications){
		this.activity = activity;
		this.inflater = activity.getLayoutInflater();
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
			convertView = inflater.inflate(R.layout.notif_list_item_detail, null);
		}
		
		Notification group = (Notification) getGroup(groupPosition);
		((TextView) convertView.findViewById(R.id.senderShortBio)).setText(group.getSender().getCandidateProfile().getShortBio());
		((TextView) convertView.findViewById(R.id.senderDescription)).setText(group.getSender().getCandidateProfile().getDescription());
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
			convertView = inflater.inflate(R.layout.notif_list_item, null);
		}
		
		final Notification group = (Notification) getGroup(groupPosition);
		
		TextView usernameText = (TextView)convertView.findViewById(R.id.senderUsername);
		usernameText.setText(group.getSender().getCandidateProfile().getUsername());
		
		TextView titleText = (TextView)convertView.findViewById(R.id.notifTitle); 
		titleText.setText(group.getTitle());
		
		TextView timeText = (TextView)convertView.findViewById(R.id.sentTime); 
		timeText.setText(group.getSentTime());
		
		// set the default image
		ImageView imageView = (ImageView) convertView.findViewById(R.id.senderAvatar);
		imageView.setImageResource(android.R.drawable.ic_menu_gallery);
		
		// set the real image with an asynchronous download operation
		ImageDownloadTask asyncTask = new ImageDownloadTask(imageView);
		asyncTask.execute(group.getSender().getProfilePics().getThumbnail().getCloudUrl());
		
		String actionName = Notification.GetActionText(this.activity, group.getRecommendedAction());
		Button actionButton = ((Button)convertView.findViewById(R.id.notifActionButton));
		actionButton.setText(actionName);		
		actionButton.setFocusable(false);
		actionButton.setOnClickListener(new OnClickListener() {
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
