package net.pic4pic.ginger;

import java.util.ArrayList;
import java.util.List;

import android.app.Activity;
import android.content.Intent;
import android.graphics.drawable.Drawable;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.View.OnClickListener;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.TextView;

import net.pic4pic.ginger.entities.BaseResponse;
import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.entities.ImageFile;
import net.pic4pic.ginger.entities.MarkingRequest;
import net.pic4pic.ginger.entities.MarkingType;
import net.pic4pic.ginger.entities.Notification;
import net.pic4pic.ginger.entities.ObjectType;
import net.pic4pic.ginger.service.Service;
import net.pic4pic.ginger.tasks.ITask;
import net.pic4pic.ginger.tasks.ImageDownloadTask;
import net.pic4pic.ginger.tasks.NonBlockedTask;
import net.pic4pic.ginger.utils.GingerHelpers;
import net.pic4pic.ginger.utils.MyLog;

public class NotificationListItemAdapter extends ArrayAdapter<Notification> {

	private Activity activity;
	private ArrayList<Notification> notifications;
	
	private static Drawable unreadBackground;
	private static Drawable readBackground;
	private static Object lockR = new Object();
	private static Object lockUR = new Object();
	
	private Drawable getBackgroundDrawable(boolean isRead){
		
		if(isRead){			
			if(readBackground == null){
				synchronized(lockR){
					if(readBackground == null){
						readBackground = this.activity.getResources().getDrawable(R.drawable.list_item_background_read);
					}
				}
			}
			
			return readBackground;
		}
		else{
			if(unreadBackground == null){
				synchronized(lockUR){
					if(unreadBackground == null){
						unreadBackground = this.activity.getResources().getDrawable(R.drawable.list_item_background_unread);
					}
				}
			}			
			return unreadBackground;	
		}
	}
	
	
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
		
		// set background color.
		convertView.setBackground(getBackgroundDrawable(notification.isRead()));
		
		// return
		return convertView;
	}
	
	public void onNotificationAction(final View actionButton, final Notification notification){
		
		// android.widget.Toast.makeText(this.activity, "Action", android.widget.Toast.LENGTH_LONG).show();		
		
		Intent intent = new Intent(this.activity, PersonActivity.class);
		intent.putExtra(PersonActivity.PersonType, notification.getSender());

		// calling a child activity for a result keeps the parent activity alive.
		// by that way, we don't have to keep track of active tab when child activity is closed. 
		this.activity.startActivityForResult(intent, PersonActivity.PersonActivityCode);

		// mark as read
		if(!notification.isRead()){
			
			// prepare request
			final MarkingRequest marking = new MarkingRequest();
			marking.setObjectType(ObjectType.Notification);
			marking.setMarkingType(MarkingType.Viewed);
			marking.setObjectId(notification.getId());
			
			NonBlockedTask.SafeRun(new ITask(){
				@Override
				public void perform() {
					
					BaseResponse response = null;
					try{
						response = Service.getInstance().mark(activity, marking);
						MyLog.v("NotificationListItemAdapter", "Notification has been marked as read: " + notification.getId());
					}
					catch(GingerException ge){
						MyLog.e("NotificationListItemAdapter", "Marking notification failed: " + ge.getMessage());
					}
					catch(Exception e){
						MyLog.e("NotificationListItemAdapter", "Marking notification as read failed: " + e.toString());
					}					
					
					// update UI
					if(response != null && response.getErrorCode() == 0){
						// Only the original thread that created a view hierarchy can touch its views.
						activity.runOnUiThread(new Runnable() {
						     @Override
						     public void run() {
						    	 View listItemView = (View)actionButton.getParent();
						    	 listItemView.setBackground(getBackgroundDrawable(true));
						    	 // listItemView.refreshDrawableState();
						    }
						});
					}
				}
			});
		}		
	}
}
