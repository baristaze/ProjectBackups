package net.pic4pic.ginger;

import java.util.ArrayList;

import android.content.Intent;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.FrameLayout;
import android.widget.ListView;

import net.pic4pic.ginger.entities.BaseResponse;
import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.entities.MarkingRequest;
import net.pic4pic.ginger.entities.MarkingType;
import net.pic4pic.ginger.entities.MatchedCandidate;
import net.pic4pic.ginger.entities.Notification;
import net.pic4pic.ginger.entities.ObjectType;
import net.pic4pic.ginger.service.Service;
import net.pic4pic.ginger.tasks.ITask;
import net.pic4pic.ginger.tasks.NonBlockedTask;
import net.pic4pic.ginger.utils.GingerHelpers;
import net.pic4pic.ginger.utils.MyLog;

public class NotificationListFragment extends Fragment {

	// a public empty constructor is a must in fragment. 
	// Do not add any parameter to this constructor.
	public NotificationListFragment(/*no parameter here please*/) {
	}
	
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
		
		View rootView = inflater.inflate(R.layout.notif_listview, container, false);		
		return rootView;
	}
	
	@Override
	public void onViewCreated(View view, Bundle savedInstanceState){
		
		MainActivity activity = (MainActivity)this.getActivity();
		if(activity.isNeedOfRequestingNotifications()){		
			
			// log
			MyLog.i("NotificationListFragment", "Starting to retrieve notifications");
			
			// below asynchronous process will call our 'onNotificationsComplete' method
			activity.startRetrievingNotifications();
		}
		else {
			
			// log
			MyLog.i("NotificationListFragment", "Cached notifications are being used");
			
			// get notifications
			ArrayList<Notification> notifications = activity.getNotifications();
						
			// update UI
			this.updateUI(view, notifications);
		}
	}
	
	public void onNotificationsComplete(ArrayList<Notification> notifications){

		int notificationCount = (notifications == null ? 0 : notifications.size());
		MyLog.i("NotificationListFragment", "onNotificationsComplete signal retrieved. Notification Count: " + notificationCount);
		
		// update UI
		View rootView = this.getView();
		if(rootView != null){
			this.updateUI(this.getView(), notifications);
		}
		else{
			MyLog.e("NotificationListFragment", "Retrieved notifications before rendering the root view.");
		}	
	}
	
	private void updateUI(View rootView, ArrayList<Notification> notifications){
		
		MyLog.i("NotificationListFragment", "Updating UI based on notifications...");
		
		// remove spinner block
		this.removeTheFrontestView(rootView);
		
		// update UI
		if(notifications != null && notifications.size() > 0){
		
			// fill up list view
			ListView listview = (ListView) rootView.findViewById(R.id.notifList);
			NotificationListItemAdapter adapter = new NotificationListItemAdapter((MainActivity)this.getActivity(), notifications);
			listview.setAdapter(adapter);
			
			// bind click actions
			listview.setOnItemClickListener(new AdapterView.OnItemClickListener() {
				@Override
				public void onItemClick(AdapterView<?> parent, final View view, int position, long id) {
					Notification item = (Notification) parent.getItemAtPosition(position);
					onShowPersonDetails(item, view);
				}
			});	
						
			// remove info-for-empty-content block 
			this.removeTheFrontestView(rootView);
		}
		else{
			// Sorry, no notification at this time.
			// Tap and pull down to refresh!
			// Nothing to so since the current view is already the one that we want
		}	
	}
	
	private boolean removeTheFrontestView(View rootView){
		
		// get the frame view
		FrameLayout frameLayout = (FrameLayout)rootView.findViewById(R.id.notifListParentFrame);
		
		if(frameLayout.getChildCount() > 1){
			// remove last view
			frameLayout.removeViewAt(frameLayout.getChildCount()-1);
			return true;
		}
		else{
			MyLog.e("NotificationListFragment", "We cannot remove the latest view since we have only 1");
			return false;
		}
	}
	
	public void onShowPersonDetails(final Notification notification, final View listItemView){
		
		// Toast.makeText(this.getActivity(), "Showing " + person, Toast.LENGTH_LONG).show();
		Intent intent = new Intent(this.getActivity(), PersonActivity.class);
		intent.putExtra(MainActivity.AuthenticatedUserBundleType, ((MainActivity)this.getActivity()).getCurrentUser());
		intent.putExtra(PersonActivity.PersonType, notification.getSender());
		intent.putExtra(PersonActivity.ParentCallerClassName, this.getClass().getName());

		// calling a child activity for a result keeps the parent activity alive.
		// by that way, we don't have to keep track of active tab when child activity is closed.
		this.getActivity().startActivityForResult(intent, PersonActivity.PersonActivityCode);
		
		// also mark the notification as read
		this.markNotificationAsRead(listItemView, notification);
		
		// marking the person as read will be handled by the PersonActivity. No need to take action here
		// do nothing...
	}
	
	private void markNotificationAsRead(final View listItemView, final Notification notification){
		
		// mark as read
		if(!notification.isRead()){
			
			// prepare request
			final MarkingRequest marking = new MarkingRequest();
			marking.setObjectType(ObjectType.Notification);
			marking.setMarkingType(MarkingType.Viewed);
			marking.setObjectId(notification.getId());
			
			// prepare to run
			final MainActivity activity = (MainActivity)this.getActivity();
			
			NonBlockedTask.SafeRun(new ITask(){
				@Override
				public void perform() {
					
					BaseResponse response = null;
					try{
						response = Service.getInstance().mark(activity, marking);
						if(response.getErrorCode() == 0){
							notification.setRead(true);
							MyLog.v("NotificationListFragment", "Notification has been marked as read: " + notification.getId());
						}
						else{
							MyLog.e("NotificationListFragment", "Marking notification failed: " + response.getErrorMessage());
						}
					}
					catch(GingerException ge){
						MyLog.e("NotificationListFragment", "Marking notification failed: " + ge.getMessage());
					}
					catch(Exception e){
						MyLog.e("NotificationListFragment", "Marking notification as read failed: " + e.toString());
					}					
					
					// update UI
					if(response != null && response.getErrorCode() == 0){
						// Only the original thread that created a view hierarchy can touch its views.
						activity.runOnUiThread(new Runnable() {
						     @Override
						     public void run() {
						    	 listItemView.setBackground(GingerHelpers.getListItemBackgroundDrawable(activity, true));
						    	 // listItemView.refreshDrawableState();
						    }
						});
					}
				}
			});
		}			
	}
	
	/**
	 * Candidate has changed. Maybe he/she is not a stranger anymore...
	 * This has nothing to do with marking a notification as read !!!
	 * We wont mark any notification here... It is mostly about updating thumb-nail image.
	 * @param person
	 */
	public void updateCandidateView(final MatchedCandidate person, String initialCallerClass, final boolean hasFamiliarityChanged){
		
		/*
		 * This method has nothing to do with 'marking as read'... 
		 * 
		String c1 = this.getClass().getName();		
		String c2 = NotificationListItemAdapter.class.getName();
		if(!initialCallerClass.equals(c1) && !initialCallerClass.equals(c2)){
			// do we need to update the avatar
			return;
		}
		*/
		
		View rootView = this.getView(); 
		if(rootView == null){
			MyLog.w("NotificationListFragment", "Root view is null? Hah!");
			return;
		}
		
		final ListView listView = (ListView)rootView.findViewById(R.id.notifList);
		
		// we don't have a wrapper adapter since we are not using a footer.
		final NotificationListItemAdapter adapter = (NotificationListItemAdapter)listView.getAdapter();

		// below iteration goes through only VISIBLE elements
		int found = 0;
		int start = listView.getFirstVisiblePosition();
		for (int i = start; i < listView.getLastVisiblePosition(); i++) {
			Notification temp = (Notification)listView.getItemAtPosition(i);
			if(person.getUserId().equals(temp.getSender().getUserId())){
				final View listItemView = listView.getChildAt(i-start);
				final int position = i;
				NonBlockedTask.SafeSleepAndRunOnUI(400, new ITask(){
					@Override
					public void perform() {
						
						if(hasFamiliarityChanged){
							
							// refresh whole row (list item view)
							// ...
							// getting view for the target child refreshes it automatically
							adapter.getView(position, listItemView, listView);
							
							// log
							MyLog.i("MatchListFragment", "Refreshing avatar image for user: " + person.getUserId());	
						}
						
						// do not 'mark as read'...
					}
				});
		    	
		    	MyLog.v("NotificationListFragment", "ListItemView is found(" + (++found) + "). Avatar will be changed shortly for: " + person.getUserId());
		    	
		    	// DO NOT break since we might have received multiple notification from a candidate
		    	// break;
			}
		}
	}
}
