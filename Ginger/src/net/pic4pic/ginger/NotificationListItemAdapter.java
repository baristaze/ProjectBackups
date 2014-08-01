package net.pic4pic.ginger;

import java.util.ArrayList;
import java.util.UUID;

import android.content.Intent;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.View.OnClickListener;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.TextView;

import net.pic4pic.ginger.entities.AcceptingPic4PicRequest;
import net.pic4pic.ginger.entities.BaseResponse;
import net.pic4pic.ginger.entities.Familiarity;
import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.entities.ImageFile;
import net.pic4pic.ginger.entities.MarkingRequest;
import net.pic4pic.ginger.entities.MarkingType;
import net.pic4pic.ginger.entities.MatchedCandidate;
import net.pic4pic.ginger.entities.MatchedCandidateResponse;
import net.pic4pic.ginger.entities.Notification;
import net.pic4pic.ginger.entities.NotificationAction;
import net.pic4pic.ginger.entities.NotificationType;
import net.pic4pic.ginger.entities.ObjectType;
import net.pic4pic.ginger.entities.StartingPic4PicRequest;
import net.pic4pic.ginger.entities.UserResponse;
import net.pic4pic.ginger.service.Service;
import net.pic4pic.ginger.tasks.AcceptPic4PicTask;
import net.pic4pic.ginger.tasks.ITask;
import net.pic4pic.ginger.tasks.ImageDownloadTask;
import net.pic4pic.ginger.tasks.NonBlockedTask;
import net.pic4pic.ginger.tasks.RequestPic4PicTask;
import net.pic4pic.ginger.tasks.AcceptPic4PicTask.AcceptPic4PicListener;
import net.pic4pic.ginger.tasks.RequestPic4PicTask.RequestPic4PicListener;
import net.pic4pic.ginger.utils.GingerHelpers;
import net.pic4pic.ginger.utils.MyLog;

public class NotificationListItemAdapter extends ArrayAdapter<Notification> implements AcceptPic4PicListener, RequestPic4PicListener {

	private MainActivity activity;
	private ArrayList<Notification> notifications;
	
	private class ViewCache {
		public TextView usernameTextView;
		public TextView titleTextView;
		public TextView timeTextView;		
		public ImageView avatarImageView;	    
	    public Button actionButton;
	}
	
	public NotificationListItemAdapter(MainActivity activity, ArrayList<Notification> notifications){
		super(activity, R.layout.notif_list_item, notifications);
		this.activity = activity;
	    this.notifications = notifications;
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
		/*
		if(notification.getSender().getCandidateProfile().getGender().getIntValue() == Gender.Male.getIntValue()){
			cachedView.avatarImageView.setImageResource(R.drawable.man_downloading_small);
		}
		else if(notification.getSender().getCandidateProfile().getGender().getIntValue() == Gender.Female.getIntValue()){
			cachedView.avatarImageView.setImageResource(R.drawable.woman_downloading_small);
		}
		else{
			cachedView.avatarImageView.setImageResource(android.R.drawable.ic_menu_gallery);
		}
		*/
		cachedView.avatarImageView.setImageResource(R.drawable.downloading_small);
		
		// set type face: bold vs. regular
		/*
		if(notification.isRead()){
			cachedView.usernameTextView.setTypeface(null, Typeface.NORMAL);
			cachedView.titleTextView.setTypeface(null, Typeface.NORMAL);
			cachedView.timeTextView.setTypeface(null, Typeface.NORMAL);
		}
		else{
			cachedView.usernameTextView.setTypeface(null, Typeface.BOLD);
			cachedView.titleTextView.setTypeface(null, Typeface.BOLD);
			cachedView.timeTextView.setTypeface(null, Typeface.BOLD);
		}
		*/
		// set background color.
		convertView.setBackground(GingerHelpers.getListItemBackgroundDrawable(activity, notification.isRead()));
		
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
		
		// enable/disable -or- show-hide button
		//cachedView.actionButton.setEnabled(true);
		cachedView.actionButton.setVisibility(View.INVISIBLE);
		/*
		if(notification.getRecommendedAction() == NotificationAction.AcceptP4P){
			UUID lastPendingPic4PicId = notification.getSender().getLastPendingPic4PicId(); 
			if(lastPendingPic4PicId == null || lastPendingPic4PicId.equals(new UUID(0,0))){
				cachedView.actionButton.setEnabled(false);
				//cachedView.actionButton.setVisibility(View.INVISIBLE);
			}
		}		
		else if(notification.getRecommendedAction() == NotificationAction.RequestP4P){
			UUID lastPendingPic4PicId = notification.getSender().getLastPendingPic4PicId(); 
			if(lastPendingPic4PicId != null && !lastPendingPic4PicId.equals(new UUID(0,0))){
				cachedView.actionButton.setEnabled(false);
				//cachedView.actionButton.setVisibility(View.INVISIBLE);
			}
			else if(notification.getSender().getCandidateProfile().getFamiliarity() == Familiarity.Familiar){
				cachedView.actionButton.setEnabled(false);
				//cachedView.actionButton.setVisibility(View.INVISIBLE);
			}
		}
		*/
		// return
		return convertView;
	}
	
	private void onNotificationAction(final View actionButton, final Notification notification){
		
		// android.widget.Toast.makeText(this.activity, "Action", android.widget.Toast.LENGTH_LONG).show();		
		
		boolean markAsRead = false;
		if(notification.getRecommendedAction() == NotificationAction.AcceptP4P){
			markAsRead = this.acceptLastPic4PicRequest(actionButton, notification);
		}
		else if(notification.getRecommendedAction() == NotificationAction.RequestP4P){
			markAsRead = this.sendPic4PicRequest(actionButton, notification);
		}
		else {
			this.launchPersonActivity(notification);
			markAsRead = false; // person activity will send it already.
		}

		// mark notification as read
		if(markAsRead){
			this.markNotificationAsRead(actionButton, notification);
		}
	}
	
	private void launchPersonActivity(final Notification notification){
		
		Intent intent = new Intent(this.activity, PersonActivity.class);
		intent.putExtra(MainActivity.AuthenticatedUserBundleType, ((MainActivity)this.activity).getCurrentUser());
		intent.putExtra(PersonActivity.PersonType, notification.getSender());
		
		if(notification.getType().getIntValue() == NotificationType.SentText.getIntValue()){
			MyLog.v("NotificationListItemAdapter", "Launching conversation activity is desired as a forward action");
			intent.putExtra(PersonActivity.ForwardActionType, PersonActivity.ForwardAction.ShowMessages.getIntValue());
		}

		// calling a child activity for a result keeps the parent activity alive.
		// by that way, we don't have to keep track of active tab when child activity is closed. 
		this.activity.startActivityForResult(intent, PersonActivity.PersonActivityCode);	
	}
	
	private boolean acceptLastPic4PicRequest(final View button, final Notification notification){
		
		UUID pic4picId = notification.getSender().getLastPendingPic4PicId();
		if(pic4picId == null || pic4picId.equals(new UUID(0,0))){
			GingerHelpers.showErrorMessage(this.activity, "It seems like you don't have a pic4pic request anymore");
			button.setEnabled(false);
			return false;
		}

		UserResponse me = ((MainActivity)this.activity).getCurrentUser();
		AcceptingPic4PicRequest request = new AcceptingPic4PicRequest();
		request.setPic4PicRequestId(pic4picId);
		request.setPictureIdToExchange(me.getProfilePictures().getFullSizeClear().getGroupingId());
		AcceptPic4PicTask task = new AcceptPic4PicTask(this, this.activity, (Button)button, request);
		task.execute();
		return true;
	}
	
	public void onPic4PicAccepted(MatchedCandidateResponse response, AcceptingPic4PicRequest request){
		
		if(response.getErrorCode() == 0){	
			
			MyLog.v("NotificationListItemAdapter", "Accepting pic4pic call has returned.");			
			
			// get recent version from response
			MatchedCandidate candidate = response.getData();
			
			this.activity.updateCandidate(candidate);				
			this.activity.updateNotification(candidate);
			
			GingerHelpers.toastShort(this.activity, "Accepted successfully \u2713");
		}
		else{
			GingerHelpers.showErrorMessage(this.activity, response.getErrorMessage());
		}
	}
	
	private boolean sendPic4PicRequest(final View button, final Notification notification){
		
		final UUID pic4picId = notification.getSender().getLastPendingPic4PicId();
		if(pic4picId != null && !pic4picId.equals(new UUID(0,0))){
			GingerHelpers.showErrorMessage(this.activity, "It seems like you have received a pic4pic request from this person already.");
			button.setEnabled(false);
			return false;
		}	
				
		if(notification.getSender().getCandidateProfile().getFamiliarity() == Familiarity.Familiar){
			GingerHelpers.toast(this.activity, "You have exchanged profile pictures already. More is coming soon!..");
			button.setEnabled(false);
			return false;
		}
		
		// prepare request
		final StartingPic4PicRequest request = new StartingPic4PicRequest();
		request.setUserIdToInteract(notification.getSender().getUserId());
		// do not define this at the first time
		// request.setPictureIdToExchange(this.person.getProfilePics().getFullSize().getGroupingId());
		
		RequestPic4PicTask task = new RequestPic4PicTask(this, this.activity, (Button)button, request);
		task.execute();
		return true;
	}
	
	public void onPic4PicRequestSent(MatchedCandidateResponse response, StartingPic4PicRequest request){
		
		// check result
		if(response.getErrorCode() == 0){
			
			// log
			MyLog.v("NotificationListItemAdapter", "pic4pic request has been sent to: " + request.getUserIdToInteract());
		
			// get recent version from response
			MatchedCandidate candidate = response.getData();
			this.activity.updateCandidate(candidate);				
			this.activity.updateNotification(candidate);
			
			// toast
			GingerHelpers.toastShort(this.activity, "Sent successfully \u2713");
		}
		else{
			// log error
			GingerHelpers.showErrorMessage(this.activity, response.getErrorMessage());
			MyLog.e("NotificationListItemAdapter", "Requesting pic4pic from candidate(" + request.getUserIdToInteract() + ") failed: " + response.getErrorMessage());
		}
	}
	
	private void markNotificationAsRead(final View actionButton, final Notification notification){
		
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
						
						if(response.getErrorCode() == 0){
							notification.setRead(true);
							MyLog.v("NotificationListItemAdapter", "Notification has been marked as read: " + notification.getId());
						}
						else{
							MyLog.e("NotificationListItemAdapter", "Marking notification failed: " + response.getErrorMessage());
						}
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
						    	 listItemView.setBackground(GingerHelpers.getListItemBackgroundDrawable(activity, true));
						    	 // listItemView.refreshDrawableState();
						    }
						});
					}
				}
			});
		}			
	}
}
