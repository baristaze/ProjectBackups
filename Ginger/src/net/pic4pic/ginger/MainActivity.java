package net.pic4pic.ginger;

import java.util.ArrayList;
import java.util.Date;
import java.util.UUID;

import android.app.ActionBar;
import android.app.Activity;
import android.app.FragmentTransaction;
import android.content.Intent;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentActivity;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentPagerAdapter;
import android.support.v4.view.ViewPager;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import net.pic4pic.ginger.entities.BaseRequest;
import net.pic4pic.ginger.entities.Familiarity;
import net.pic4pic.ginger.entities.MatchedCandidate;
import net.pic4pic.ginger.entities.MatchedCandidateListResponse;
import net.pic4pic.ginger.entities.Notification;
import net.pic4pic.ginger.entities.NotificationListResponse;
import net.pic4pic.ginger.entities.NotificationRequest;
import net.pic4pic.ginger.entities.UserResponse;
import net.pic4pic.ginger.tasks.MatchedCandidatesTask;
import net.pic4pic.ginger.tasks.MatchedCandidatesTask.MatchedCandidatesListener;
import net.pic4pic.ginger.tasks.NotificationsTask;
import net.pic4pic.ginger.tasks.NotificationsTask.NotificationsListener;
import net.pic4pic.ginger.utils.GingerHelpers;
import net.pic4pic.ginger.utils.MyLog;

public class MainActivity extends FragmentActivity implements ActionBar.TabListener, MatchedCandidatesListener, NotificationsListener {

	public static final String AuthenticatedUserBundleType = "net.pic4pic.ginger.AuthenticatedUser";
	public static final String UpdatedMatchCandidate = "net.pic4pic.ginger.UpdatedMatchCandidate";
	public static final int CaptureCameraCode = 102;
	public static final int PickFromGalleryCode = 103;
	
	private UserResponse me;
	public UserResponse getCurrentUser(){
		return this.me;
	}
	
	private Date lastMatchRetrieveTime = null;
	private Date lastNotificationRetrieveTime = null;
	private ArrayList<MatchedCandidate> matches = new ArrayList<MatchedCandidate>();
	private ArrayList<Notification> notifications = new ArrayList<Notification>();
	
	private SectionsPagerAdapter mSectionsPagerAdapter;	
	private ViewPager mViewPager;
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		
		MyLog.v("MainActivity", "onCreate");
		
		super.onCreate(savedInstanceState);
		if(this.recreatePropertiesFromSavedBundle(savedInstanceState)){
			MyLog.i("MainActivity", "At least one property is restored successfully");
		}
		
		setContentView(R.layout.activity_main);
		
		Intent intent = getIntent();
		this.me = (UserResponse)intent.getSerializableExtra(AuthenticatedUserBundleType);
		MyLog.v("MainActivity", "Current user is: " + this.me.getUserProfile().getUsername());
		
		// Set up the action bar.
		final ActionBar actionBar = getActionBar();
		actionBar.setNavigationMode(ActionBar.NAVIGATION_MODE_TABS);

		// Create the adapter that will return a fragment for each of the three
		// primary sections of the application.
		mSectionsPagerAdapter = new SectionsPagerAdapter(getSupportFragmentManager());

		// Set up the ViewPager with the sections adapter.
		mViewPager = (ViewPager) findViewById(R.id.pager);
		mViewPager.setAdapter(mSectionsPagerAdapter);

		// When swiping between different sections, select the corresponding
		// tab. We can also use ActionBar.Tab#select() to do this if we have
		// a reference to the Tab.
		mViewPager.setOnPageChangeListener(new ViewPager.SimpleOnPageChangeListener() {
					@Override
					public void onPageSelected(int position) {
						actionBar.setSelectedNavigationItem(position);
					}
				});

		// For each of the sections in the app, add a tab to the action bar.
		for (int i = 0; i < mSectionsPagerAdapter.getCount(); i++) {
			// Create a tab with text corresponding to the page title defined by
			// the adapter. Also specify this Activity object, which implements
			// the TabListener interface, as the callback (listener) for when
			// this tab is selected.
			actionBar.addTab(actionBar.newTab()
					.setText(mSectionsPagerAdapter.getPageTitle(i))
					.setTabListener(this));
		}
	}
	
	@Override
	public void onSaveInstanceState(Bundle outState) {
		
		super.onSaveInstanceState(outState);

		if(outState == null){
			return;
		}
		
		MyLog.v("MainActivity", "onSaveInstanceState");
		
		if(this.me != null){
			outState.putSerializable("me", this.me);
		}

		if(this.lastMatchRetrieveTime != null){
			outState.putSerializable("lastMatchRetrieveTime", this.lastMatchRetrieveTime);
		}
		
		if(this.lastNotificationRetrieveTime != null){
			outState.putSerializable("lastNotificationRetrieveTime", this.lastNotificationRetrieveTime);
		}
	   
		if(this.matches != null){
			outState.putSerializable("matches", this.matches);
		}
		
		if(this.notifications != null){
			outState.putSerializable("notifications", this.notifications);
		}
	}
	
	@Override
	public void onRestoreInstanceState(Bundle savedInstanceState) {
		
		super.onRestoreInstanceState(savedInstanceState);
		
		MyLog.v("MainActivity", "onRestoreInstanceState");
		if(this.recreatePropertiesFromSavedBundle(savedInstanceState)){
			MyLog.i("MainActivity", "At least one property is restored successfully");
		}
	}
	
	@SuppressWarnings("unchecked")
	private boolean recreatePropertiesFromSavedBundle(Bundle state){
		
		if(state == null){
			return false;
		}
		
		boolean restored = false;
		if(state.containsKey("me")){
			this.me = (UserResponse)state.getSerializable("me");
			restored = true;
		}
		
		if(state.containsKey("lastMatchRetrieveTime")){
			this.lastMatchRetrieveTime = (Date)state.getSerializable("lastMatchRetrieveTime");
			restored = true;
		}
		
		if(state.containsKey("lastNotificationRetrieveTime")){
			this.lastNotificationRetrieveTime = (Date)state.getSerializable("lastNotificationRetrieveTime");
			restored = true;
		}
		
		if(state.containsKey("matches")){
			this.matches = (ArrayList<MatchedCandidate>)state.getSerializable("matches");
			restored = true;
		}
		
		if(state.containsKey("notifications")){
			this.notifications = (ArrayList<Notification>)state.getSerializable("notifications");
			restored = true;
		}
		
		return restored;
	}
	
	public ArrayList<MatchedCandidate> getMatchedCandidates(){
		return this.matches;
	}
	
	public ArrayList<Notification> getNotifications(){
		return this.notifications;
	}
	
	public boolean isNeedOfRequestingMatches(){
		
		if(this.lastMatchRetrieveTime == null){
			
			MyLog.v("MainActivity", "Last match retrieve time is null. We need to request matches again");
			return true;
		}
		
		Date now = new Date();
		long diffAsMilliSeconds = now.getTime() - this.lastMatchRetrieveTime.getTime();
		long diffAsMinutes = diffAsMilliSeconds / 60000;
		if(diffAsMinutes > 30){
			MyLog.v("MainActivity", "Last match retrieve time was 30 minutes ago. We need to request matches again");
			return true;
		}
		
		if(this.matches.size() == 0){
			MyLog.v("MainActivity", "View doesn't have any match on it. We need to request matches again");
			return true;
		}
		
		return false;
	}
	
	public boolean isNeedOfRequestingNotifications(){
		
		if(this.lastNotificationRetrieveTime == null){
			
			MyLog.v("MainActivity", "Last notification retrieve time is null. We need to request notifications again");
			return true;
		}
		
		Date now = new Date();
		long diffAsMilliSeconds = now.getTime() - this.lastNotificationRetrieveTime.getTime();
		long diffAsMinutes = diffAsMilliSeconds / 60000;
		if(diffAsMinutes > 30){
			MyLog.v("MainActivity", "Last notification retrieve time was 30 minutes ago. We need to request notifications again");
			return true;
		}
		
		if(this.notifications.size() == 0){
			MyLog.v("MainActivity", "View doesn't have any notification on it. We need to request notifications again");
			return true;
		}
		
		return false;
	}
	
	public void startRetrievingMatches(){
		BaseRequest request = new BaseRequest();
		MatchedCandidatesTask task = new MatchedCandidatesTask(this, this, request);
		task.execute();
	}
	
	public void startRetrievingNotifications(){		
		NotificationRequest request = new NotificationRequest();
		request.setLastNotificationId(new UUID(0,0));
		/*
		if(this.notifications.size() > 0){
			request.setLastNotificationId(this.notifications.get(0).getId());
		}
		*/
		NotificationsTask task = new NotificationsTask(this, this, request);
		task.execute();
	}
	
	@Override
	public void onMatchComplete(MatchedCandidateListResponse response, BaseRequest request){
		
		if(response.getErrorCode() == 0){
			
			this.lastMatchRetrieveTime = new Date();
			
			// update cache
			ArrayList<MatchedCandidate> candidates = response.getItems();
			if(candidates != null && candidates.size() > 0){
				this.matches.clear();
				this.matches.addAll(candidates);
			}
			
			// update UI
			this.mSectionsPagerAdapter.getMatchListFragment().onMatchComplete(this.matches);			
		}
		else{
			// show error
			GingerHelpers.showErrorMessage(this, response.getErrorMessage());
			
			// update UI
			this.mSectionsPagerAdapter.getMatchListFragment().onMatchComplete(this.matches);
		}	
	}
	
	@Override
	public void onNotificationsComplete(NotificationListResponse response, NotificationRequest request){
		
		if(response.getErrorCode() == 0){
			
			this.lastNotificationRetrieveTime = new Date();
			
			// update cache
			ArrayList<Notification> notifs = response.getItems();
			if(notifs != null && notifs.size() > 0){
				this.notifications.clear();
				this.notifications.addAll(notifs);
			}
			
			// update UI
			this.mSectionsPagerAdapter.getNotificationListFragment().onNotificationsComplete(this.notifications);			
		}
		else{
			// show error
			GingerHelpers.showErrorMessage(this, response.getErrorMessage());
			
			// update UI
			this.mSectionsPagerAdapter.getNotificationListFragment().onNotificationsComplete(this.notifications);
		}	
	}
	
	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.main, menu);
		return true;
	}

	@Override
	public void onTabSelected(ActionBar.Tab tab, FragmentTransaction fragmentTransaction) {
		// When the given tab is selected, switch to the corresponding page in
		// the ViewPager.
		mViewPager.setCurrentItem(tab.getPosition());
	}

	@Override
	public void onTabUnselected(ActionBar.Tab tab, FragmentTransaction fragmentTransaction) {
	}

	@Override
	public void onTabReselected(ActionBar.Tab tab, FragmentTransaction fragmentTransaction) {
	}
	
	@Override
	public void onActivityResult(int requestCode, int resultCode, Intent data) {
		
		if (requestCode == MainActivity.CaptureCameraCode) {
			MyLog.v("MainActivity", "CaptureCameraActivity has returned");
			this.mSectionsPagerAdapter.getProfileFragment().processCameraActivityResult(resultCode, data);			
	    }
		else if (requestCode == MainActivity.PickFromGalleryCode) {
			MyLog.v("MainActivity", "PickFromGalleryActivity has returned");
			this.mSectionsPagerAdapter.getProfileFragment().processGalleryActivityResult(resultCode, data);
		}
		else if (requestCode == TextInputActivity.TextInputCode) {
			MyLog.v("MainActivity", "TextInputActivity has returned");
			this.mSectionsPagerAdapter.getProfileFragment().processTextInputActivityResult(resultCode, data);
		}
		else if(requestCode == PersonActivity.PersonActivityCode){
			MyLog.v("MainActivity", "PersonActivity has returned");			
			if(resultCode == Activity.RESULT_OK && data != null){				
				Bundle bundle = data.getExtras();
				MatchedCandidate candidate = (MatchedCandidate)bundle.getSerializable(MainActivity.UpdatedMatchCandidate);
				String initialCallerClass = bundle.getString(PersonActivity.ParentCallerClassName);
				MyLog.i("MainActivity", "Candidate: " + candidate.getUserId() + " viewed: " + candidate.isViewed());				
				this.updateCandidate(candidate, initialCallerClass);				
				this.updateNotification(candidate, initialCallerClass);
			}
		}
		else{
			MyLog.v("MainActivity", "Unknown Activity  has been received by MainActivity: " + requestCode);
		}
	}
	
	public void updateCandidate(MatchedCandidate candidate, String initialCallerClass){
		
		if(candidate == null){
			return;
		}
		
		for(int x=0; x<this.matches.size(); x++){
			MatchedCandidate initial = this.matches.get(x);
			UUID userId1 = initial.getUserId();
			UUID userId2 = candidate.getUserId();
			if(userId1.equals(userId2)){
				
				// prepare flag
				Familiarity f1 = initial.getCandidateProfile().getFamiliarity();
				Familiarity f2 = candidate.getCandidateProfile().getFamiliarity();
				boolean hasFamiliarityChanged = (f1.getIntValue() != f2.getIntValue());
				
				// update person
				this.matches.set(x, candidate);
				
				// update view... having this within the for loop is OK.
				this.mSectionsPagerAdapter.getMatchListFragment().updateCandidateView(candidate, initialCallerClass, hasFamiliarityChanged);
				
				// break is fine here.
				break;
			}
		}
	}
	
	public void updateNotification(MatchedCandidate candidate, String initialCallerClass){
		
		if(candidate == null){
			return;
		}
		
		boolean hasFamiliarityChanged = false;
		for(int x=0; x<this.notifications.size(); x++){
			MatchedCandidate initial = this.notifications.get(x).getSender();
			UUID userId1 = initial.getUserId();
			UUID userId2 = candidate.getUserId();
			if(userId1.equals(userId2)){
				
				// prepare flag
				Familiarity f1 = initial.getCandidateProfile().getFamiliarity();
				Familiarity f2 = candidate.getCandidateProfile().getFamiliarity();
				if(f1.getIntValue() != f2.getIntValue()){
					hasFamiliarityChanged = true;
				}
				
				// set
				this.notifications.get(x).setSender(candidate);
				
				// don't break since we might have multiple notifications from a candidate
				// break;
			}
		}
		
		// keep this outside of for loop
		// below method updates the avatar only
		this.mSectionsPagerAdapter.getNotificationListFragment().updateCandidateView(candidate, initialCallerClass, hasFamiliarityChanged);
	}
	
	/**
	 * A {@link FragmentPagerAdapter} that returns a fragment corresponding to
	 * one of the sections/tabs/pages.
	 */
	public class SectionsPagerAdapter extends FragmentPagerAdapter {
		
		private MatchListFragment matchesFragment;
		private NotificationListFragment notificationListFragment;
		private ProfileFragment profileFragment;
		
		public synchronized MatchListFragment getMatchListFragment(){
			if(this.matchesFragment == null){
				this.matchesFragment = new MatchListFragment();
			}
			
			return this.matchesFragment;
		}
		
		public synchronized NotificationListFragment getNotificationListFragment(){
			if(this.notificationListFragment == null){
				this.notificationListFragment = new NotificationListFragment();
			}
			
			return this.notificationListFragment;
		}
		
		public synchronized ProfileFragment getProfileFragment(){
			if(this.profileFragment == null){
				this.profileFragment = new ProfileFragment();
			}
			
			return this.profileFragment;
		}
		
		public SectionsPagerAdapter(FragmentManager fm) {
			super(fm);
		}

		@Override
		public Fragment getItem(int position) {			
			// getItem is called to instantiate the fragment for the given page.
			// Return a DummySectionFragment (defined as a static inner class
			// below) with the page number as its lone argument.
			if(position == 0){
				return this.getMatchListFragment();
			}
			else if(position == 1){
				return this.getNotificationListFragment();
			}
			else if (position == 2){
				return this.getProfileFragment();
			}
			else{
				Fragment fragment = new DummySectionFragment();
				Bundle args = new Bundle();
				args.putInt(DummySectionFragment.ARG_SECTION_NUMBER, position + 1);
				fragment.setArguments(args);
				return fragment;
			}
		}

		@Override
		public int getCount() {
			// Show 3 total pages.
			return 3;
		}

		@Override
		public CharSequence getPageTitle(int position) {			
			switch (position) {			
				case 0:
					return getString(R.string.title_section_matches);
				case 1:
					return getString(R.string.title_section_notif);
				case 2:
					return getString(R.string.title_section_me);
			}
			
			return null;
		}
	}

	/**
	 * A dummy fragment representing a section of the app, but that simply
	 * displays dummy text.
	 */
	public static class DummySectionFragment extends Fragment {
		/**
		 * The fragment argument representing the section number for this
		 * fragment.
		 */
		public static final String ARG_SECTION_NUMBER = "section_number";

		public DummySectionFragment() {
		}

		@Override
		public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			View rootView = inflater.inflate(R.layout.fragment_main_dummy, container, false);
			TextView dummyTextView = (TextView) rootView.findViewById(R.id.section_label);
			dummyTextView.setText(Integer.toString(getArguments().getInt(ARG_SECTION_NUMBER)));
			return rootView;
		}
	}
}
