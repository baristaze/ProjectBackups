package net.pic4pic.ginger;

import android.app.ActionBar;
import android.app.FragmentTransaction;
import android.content.Intent;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentActivity;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentPagerAdapter;
import android.support.v4.view.ViewPager;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

public class MainActivity extends FragmentActivity implements ActionBar.TabListener {

	public static final String SignedInPersonType = "net.pic4pic.ginger.SignedInPerson"; 
	public static final int CaptureCameraCode = 102;
	public static final int PickFromGalleryCode = 103;
	
	private Person me;
	
	private SectionsPagerAdapter mSectionsPagerAdapter;	
	private ViewPager mViewPager;
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_main);
		
		Intent intent = getIntent();
		this.me = (Person)intent.getSerializableExtra(SignedInPersonType);
		Log.v("CurrentUser", "current user is: " + this.me.getUsername());
		
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
			Log.v("ActivityResult", "CaptureCameraActivity has returned");
			this.mSectionsPagerAdapter.getProfileFragment().processCameraActivityResult(resultCode, data);			
	    }
		else if (requestCode == MainActivity.PickFromGalleryCode) {
			Log.v("ActivityResult", "PickFromGalleryActivity has returned");
			this.mSectionsPagerAdapter.getProfileFragment().processGalleryActivityResult(resultCode, data);
		}
		else if (requestCode == TextInputActivity.TextInputCode) {
			Log.v("ActivityResult", "TextInputActivity has returned");
			this.mSectionsPagerAdapter.getProfileFragment().processTextInputActivityResult(resultCode, data);
		}		
		else{
			Log.v("ActivityResult", "Unknown Activity  has been received by MainActivity: " + requestCode);
		}
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
