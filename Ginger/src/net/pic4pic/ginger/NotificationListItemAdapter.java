package net.pic4pic.ginger;

import java.util.ArrayList;
import java.util.List;

import android.app.Activity;
import android.content.Intent;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.View.OnClickListener;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.TextView;

import net.pic4pic.ginger.entities.ImageFile;
import net.pic4pic.ginger.entities.Notification;
import net.pic4pic.ginger.tasks.ImageDownloadTask;
import net.pic4pic.ginger.utils.GingerHelpers;

public class NotificationListItemAdapter extends ArrayAdapter<Notification> {

	private Activity activity;
	private ArrayList<Notification> notifications;
	
	private class ViewCache {		
		public TextView usernameTextView;
		public TextView titleTextView;
		public TextView timeTextView;		
		public ImageView avatarImageView;	    
	    public Button actionButton;
	}
	
	public NotificationListItemAdapter(Activity activity, List<Notification> notifications){
		super(activity, R.layout.notif_list_item, notifications);
		this.activity = activity;
	    this.notifications = new ArrayList<Notification>(notifications);
	}

	@Override
	public View getView(int position, View convertView, ViewGroup parent) {
		
		if (convertView == null) {
			LayoutInflater inflater = LayoutInflater.from(this.activity);
			convertView = inflater.inflate(R.layout.notif_list_item, null);
			
			ViewCache cachedView = new ViewCache();
			cachedView.usernameTextView = (TextView)convertView.findViewById(R.id.senderUsername);
			cachedView.titleTextView = (TextView)convertView.findViewById(R.id.notifTitle);
			cachedView.timeTextView = (TextView)convertView.findViewById(R.id.sentTime);
			cachedView.avatarImageView = (ImageView) convertView.findViewById(R.id.senderAvatar);
			cachedView.actionButton = ((Button)convertView.findViewById(R.id.notifActionButton));			
			convertView.setTag(cachedView);
		}
		
		final Notification notification = (Notification) this.notifications.get(position);
		ViewCache cachedView = (ViewCache) convertView.getTag();
		
		// set user name
		cachedView.usernameTextView.setText(notification.getSender().getCandidateProfile().getUsername());
		
		// set title
		cachedView.titleTextView.setText(notification.getTitle());
		
		// set time info
		cachedView.timeTextView.setText(GingerHelpers.getTimeDiffHumanReadable(notification.getSentTimeUTC()));
		
		// set dummy image first...
		cachedView.avatarImageView.setImageResource(android.R.drawable.ic_menu_gallery);
		
		// now set the real image with an asynchronous download operation
		ImageFile imageToDownload = notification.getSender().getProfilePics().getThumbnail();
		ImageDownloadTask asyncTask = new ImageDownloadTask(imageToDownload.getId(), cachedView.avatarImageView);
		asyncTask.execute(imageToDownload.getCloudUrl());
		
		// set button
		String actionName = Notification.GetActionText(this.activity, notification.getRecommendedAction());		
		cachedView.actionButton.setText(actionName);		
		cachedView.actionButton.setFocusable(false);
		cachedView.actionButton.setOnClickListener(new OnClickListener() {
			@Override
		    public void onClick(View v) {
		    	onNotificationAction(v, notification);
		    }
		 });
		
		return convertView;
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
