package net.pic4pic.ginger;

import java.util.ArrayList;

import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ExpandableListView;
import android.widget.FrameLayout;
import android.widget.ListView;

import net.pic4pic.ginger.entities.Notification;
import net.pic4pic.ginger.utils.MyLog;

public class NotificationListFragment extends Fragment {

	// a public empty constructor is a must in fragment. 
	// Do not add any parameter to this constructor.
	public NotificationListFragment(/*no parameter here please*/) {
	}
	
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
		
		View rootView = inflater.inflate(R.layout.notif_listview, container, false);		
		final ExpandableListView listview = (ExpandableListView) rootView.findViewById(R.id.notifList);		
		listview.setChoiceMode(ListView.CHOICE_MODE_SINGLE);
		
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
			this.updateUI(rootView, notifications);
		}
		
		return rootView;
	}
	
	public void onNotificationsComplete(ArrayList<Notification> notifications){

		MyLog.i("NotificationListFragment", "onNotificationsComplete signal retrieved");
		
		// update UI
		View rootView = this.getView();
		if(rootView != null){
			this.updateUI(this.getView(), notifications);
		}
		else{
			MyLog.e("NotificationListFragment", "Something is too fast: Retrieved notifications before rendering the root view");
		}	
	}
	
	private void updateUI(View rootView, ArrayList<Notification> notifications){
		
		MyLog.i("NotificationListFragment", "Updating UI based on notifications...");
		
		// remove spinner block
		this.removeTheFrontestView(rootView);
		
		// update UI
		if(notifications != null && notifications.size() > 0){
		
			// fill up list view
			ExpandableListView listview = (ExpandableListView) rootView.findViewById(R.id.notifList);
			NotificationListItemAdapter adapter = new NotificationListItemAdapter(this.getActivity(), notifications);
			listview.setAdapter(adapter);
						
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
}
