package net.pic4pic.ginger;

import android.content.Intent;
import android.graphics.Bitmap;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentActivity;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentPagerAdapter;
import android.view.KeyEvent;
import android.view.Menu;

import com.facebook.Session;

import net.pic4pic.ginger.MainActivity.DummySectionFragment;
import net.pic4pic.ginger.entities.GeoLocation;
import net.pic4pic.ginger.entities.Locality;
import net.pic4pic.ginger.entities.Location;
import net.pic4pic.ginger.entities.UserResponse;
import net.pic4pic.ginger.tasks.SendLocationTask;
import net.pic4pic.ginger.tasks.SendLocationTask.LocationType;
import net.pic4pic.ginger.tasks.SendLocationTask.SendLocationListener;
import net.pic4pic.ginger.utils.BitmapHelpers;
import net.pic4pic.ginger.utils.GingerHelpers;
import net.pic4pic.ginger.utils.ImageActivity;
import net.pic4pic.ginger.utils.ImageStorageHelper;
import net.pic4pic.ginger.utils.LocationManagerUtil;
import net.pic4pic.ginger.utils.LocationManagerUtil.LocationListener;
import net.pic4pic.ginger.utils.MyLog;
import net.pic4pic.ginger.utils.NonSwipeableViewPager;
import net.pic4pic.ginger.utils.PageAdvancer;

public class SignupActivity extends FragmentActivity implements PageAdvancer, LocationListener, SendLocationListener {
	
	public static final int CaptureCameraCode = 501;
	public static final int PickFromGalleryCode = 502;
	
	private static final int FRAG_COUNT = 5;
	private static final int FRAG_INDEX_SIGN_UP = 0;
	private static final int FRAG_INDEX_PHOTO_INFO = 1;
	private static final int FRAG_INDEX_FACE_DETECT = 2;
	private static final int FRAG_INDEX_FBOOK_INFO = 3;
	public static final int FRAG_INDEX_PERSONAL_DETAILS = 4;
	
	private NonSwipeableViewPager fragmentPager;
	private SignupPagerAdapter fragmentPagerAdapter;
	
	private LocationManagerUtil locationManager;
	private GeoLocation lastGeoLocation;
	private Locality lastLocality;
	
	private boolean isBackEnabled = true;
	
	private int preSelectedTabIndexOnMainActivity = 0;	
	public int getPreSelectedTabIndexOnMainActivity(){
		return this.preSelectedTabIndexOnMainActivity;
	}
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		
		super.onCreate(savedInstanceState);		
		this.recreatePropertiesFromSavedBundle(savedInstanceState);
		
		Intent intent = getIntent();
		this.preSelectedTabIndexOnMainActivity = intent.getIntExtra("PreSelectedTabIndexOnMainActivity", this.preSelectedTabIndexOnMainActivity);
		
		setContentView(R.layout.activity_signup);
		
		this.fragmentPagerAdapter = new SignupPagerAdapter(this.getSupportFragmentManager());		
		this.fragmentPager = (NonSwipeableViewPager) findViewById(R.id.fragmentPager);
		this.fragmentPager.setAdapter(this.fragmentPagerAdapter);
		
		this.locationManager = new LocationManagerUtil(this, this);
		if(!this.locationManager.init()){
			MyLog.e("SignupActivity", "Location Manager couldn't be initialized.");
		}
	}
	
	@Override
	public void onSaveInstanceState(Bundle outState) {
		
		super.onSaveInstanceState(outState);

		if(outState == null){
			return;
		}
		
		outState.putBoolean("isBackEnabled", this.isBackEnabled);
		outState.putInt("PreSelectedTabIndexOnMainActivity", this.preSelectedTabIndexOnMainActivity);
		
		if(this.lastGeoLocation != null){
			outState.putSerializable("lastGeoLocation", this.lastGeoLocation);
		}
		
		if(this.lastLocality != null){
			outState.putSerializable("lastLocality", this.lastLocality);
		}
	}
	
	@Override
	public void onRestoreInstanceState(Bundle savedInstanceState) {
		
		super.onRestoreInstanceState(savedInstanceState);		
		this.recreatePropertiesFromSavedBundle(savedInstanceState);
	}
	
	private boolean recreatePropertiesFromSavedBundle(Bundle state){
		
		if(state == null){
			return false;
		}
		
		boolean restored = false;
		
		if(state.containsKey("isBackEnabled")){
			this.isBackEnabled = state.getBoolean("isBackEnabled", true);
			restored = true;
		}
		
		if(state.containsKey("PreSelectedTabIndexOnMainActivity")){
			this.preSelectedTabIndexOnMainActivity = state.getInt("PreSelectedTabIndexOnMainActivity", 0);
			restored = true;
		}
		
		if(state.containsKey("lastGeoLocation")){
			this.lastGeoLocation = (GeoLocation)state.getSerializable("lastGeoLocation");
			restored = true;
		}
		
		if(state.containsKey("lastLocality")){
			this.lastLocality = (Locality)state.getSerializable("lastLocality");
			restored = true;
		}
		
		return restored;
	}
	
	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.signup, menu);
		return true;
	}
	
	@Override
	public boolean onKeyDown(int keyCode, KeyEvent event) {		
		if (keyCode == KeyEvent.KEYCODE_BACK){
			
			int currentFragment = this.fragmentPager.getCurrentItem();
			
			if(this.isBackEnabled){
				/*if(currentFragment == FRAG_INDEX_FACE_DETECT){ // face-detection
					this.startCamera();
					return true;
				}
				else */if(currentFragment > 0){
					this.fragmentPager.setCurrentItem(currentFragment-1, true);
			        return true;
				}
			}
			else{
				if(currentFragment > FRAG_INDEX_SIGN_UP){
					this.fragmentPager.setCurrentItem(FRAG_INDEX_SIGN_UP); // go to beginning
					return true;
				}
			}
		}
		
		return super.onKeyDown(keyCode, event);
	}
	
	@Override
	public void onLocationChanged(GeoLocation geoLocation, Locality locality) {
		
		boolean needsToSend = true;
		if(this.lastLocality != null && this.lastLocality.getCity().equalsIgnoreCase(locality.getCity())){
			needsToSend = false;
		}
		
		this.lastGeoLocation = geoLocation;
		this.lastLocality = locality;
		
		MyLog.v("SignupActivity", "Location Changed. Geo = [" + this.lastGeoLocation.toString() + "], Locality = [" + this.lastLocality.toString() + "]");
		
		Location location = new Location();
		location.setGeoLocation(geoLocation);
		location.setLocality(locality);
		
		if(needsToSend){
			SendLocationTask task = new SendLocationTask(this, this, location, SendLocationTask.LocationType.HintForSupport);
			task.execute();
		}
		else{
			MyLog.v("SignupActivity", "No need to re-send location data");
		}
	}
	
	@Override
	public void onLocationInfoSent(Location location, LocationType locationType) {
		MyLog.v("SignupActivity", "Location info is sent to server.");
	}
	
	@Override
	public void moveToPreviousPage(){
		
		int currentFragment = this.fragmentPager.getCurrentItem();
		
		if(!this.isBackEnabled){
			if(currentFragment > FRAG_INDEX_SIGN_UP){
				this.fragmentPager.setCurrentItem(FRAG_INDEX_SIGN_UP); // go to beginning
				return;
			}
		}
		
		if(currentFragment == FRAG_INDEX_FACE_DETECT){ // face-detect
			this.fragmentPager.setCurrentItem(currentFragment-1);
						
			// start camera after 0.5 seconds to let the animation finishes. 
			// Edit: camera or gallery? do nothing!
			/*
			final ScheduledExecutorService exec = Executors.newScheduledThreadPool(1);
			exec.schedule(new Runnable(){
			    @Override
			    public void run(){
			    	SignupActivity.this.startCamera();
			    }
			}, 500, TimeUnit.MILLISECONDS);
			*/
		}
		/*
		else if(currentFragment == FRAG_INDEX_FBOOK_INFO){
			this.fragmentPagerAdapter.getFaceDetectionFragment().applyData(); // NO!!!
		}*/
	}
	
	@Override
	public void moveToNextPage(int data){
		/*
		FRAG_INDEX_SIGN_UP = 0;
		FRAG_INDEX_PHOTO_INFO = 1;
		FRAG_INDEX_FACE_DETECT = 2;
		FRAG_INDEX_FBOOK_INFO = 3;
		FRAG_INDEX_PERSONAL_DETAILS = 4;
		*/
		int currentFragment = this.fragmentPager.getCurrentItem();
		if(currentFragment == FRAG_INDEX_PHOTO_INFO) { // photo-info		
			ImageActivity.Source source = (data == 0) ? ImageActivity.Source.Gallery : ImageActivity.Source.Camera;
			int requestCode = (data == 0) ? SignupActivity.PickFromGalleryCode : SignupActivity.CaptureCameraCode; 
			ImageActivity.start(source, this, requestCode);
		}		
		else if(currentFragment < this.fragmentPagerAdapter.getCount()){
			
			if(currentFragment == FRAG_INDEX_SIGN_UP){
				// next is photo info. it needs to refresh its UI since we have a 'username' to show
				this.fragmentPagerAdapter.getPhotoInfoFragment().applyData();
			}
			else if(currentFragment == FRAG_INDEX_FACE_DETECT){
				// next is facebook info. it needs to refresh its UI since we have a 'username' and 'thumbnail' to show
				this.fragmentPagerAdapter.getFacebookInfoFragment().applyData();
			}
			else if(currentFragment == FRAG_INDEX_FBOOK_INFO){
				// next is personal details. it needs to refresh its UI since we have a 'username' and 'thumbnail' to show
				this.fragmentPagerAdapter.getPersonalDetailsFragment().applyData();
			}
			
			this.fragmentPager.setCurrentItem(currentFragment+1);
		}
	}
	
	@Override
	public void moveToLastPage(UserResponse response, boolean backEnabled){
		this.isBackEnabled = backEnabled;
		// next is personal details. it needs to refresh its UI since we have a 'username' and 'thumbnail' to show
		this.fragmentPagerAdapter.getPersonalDetailsFragment().setUserResponse(response);
		this.fragmentPagerAdapter.getPersonalDetailsFragment().applyData();	
		this.fragmentPager.setCurrentItem(FRAG_INDEX_FBOOK_INFO + 1);
	}
	
	public Fragment getFragment(int index) {		
		int fragmentId = this.fragmentPager.getId();
		String fragmentTag = "android:switcher:" + fragmentId + ":" + index;		
		return this.getSupportFragmentManager().findFragmentByTag(fragmentTag);
	}
	
	private Bitmap processPhotoActivityResult(int resultCode, Intent data){
		ImageActivity.Result result = ImageActivity.getProcessedResult(this, resultCode, data);
		Bitmap photo = result.getBitmap();
		if(photo != null){
			if(photo.getWidth() > BitmapHelpers.MAX_SIZE || photo.getHeight() > BitmapHelpers.MAX_SIZE){
				
				int newWidth = photo.getWidth();
				int newHeight = photo.getHeight();
				if(photo.getWidth() >= photo.getHeight()){
					newWidth = BitmapHelpers.MAX_SIZE;
					newHeight = (int)((float)newWidth * ((float)photo.getHeight() / (float)photo.getWidth())); 
				}
				else {
					newHeight = BitmapHelpers.MAX_SIZE;
					newWidth = (int)((float)newHeight * ((float)photo.getWidth() / (float)photo.getHeight()));
				}
				
				System.gc();
				try{
					String sizeInfoOld = "Old size: " + photo.getWidth() + "x" + photo.getHeight() + ". Required byte: " + photo.getByteCount();
					photo = Bitmap.createScaledBitmap(photo, newWidth, newHeight, true);
					String sizeInfoNew = "New size: " + photo.getWidth() + "x" + photo.getHeight() + ". Required byte: " + photo.getByteCount();
					MyLog.i("SignupActivity", "Photo size has been reduced: " + sizeInfoOld + " => " + sizeInfoNew);
				}
				catch(OutOfMemoryError exception){
					String sizeInfo = "Source size: " + photo.getWidth() + "x" + photo.getHeight() + ". Required byte: " + photo.getByteCount();
			    	MyLog.e("SignupActivity", "Out of memory exception when creating scaling Bitmap in 'processPhotoActivityResult' method. " + sizeInfo);
			    	// no need to re-throw it here. just swallow.
				}
			}
			
			String fileName = this.getString(R.string.lastCapturedPhoto_filename_key);
			if(ImageStorageHelper.saveToInternalStorage(this, photo, fileName, true)){
				MyLog.v("Storage", "Bitmap has been saved to the internal storage");
				int currentFragment = this.fragmentPager.getCurrentItem();
				this.fragmentPager.setCurrentItem(currentFragment+1);				
				FaceDetectionFragment theFrag = (FaceDetectionFragment)this.getFragment(FRAG_INDEX_FACE_DETECT);
				theFrag.applyData();				
			}
			else{
				GingerHelpers.toast(this, "A private copy of the selected photo couldn't be saved to your phone.");
			}
		}
		else{
			GingerHelpers.toast(this, result.getErrorMessage());
		}
		
		return result.getBitmap();
	}
	
	@Override
	public void onActivityResult(int requestCode, int resultCode, Intent data) {
		if (requestCode == SignupActivity.CaptureCameraCode) {
			MyLog.v("ActivityResult", "CaptureCameraActivity has returned");
			this.processPhotoActivityResult(resultCode, data);
	    }
		else if (requestCode == SignupActivity.PickFromGalleryCode) {
			MyLog.v("ActivityResult", "PickFromGalleryActivity has returned");
			this.processPhotoActivityResult(resultCode, data);
		}
		else{
			// this might be facebook session
			//Log.v("ActivityResult", "Unknown Activity has been received by SignupActivity: " + requestCode);
		}
		
		// make sure that facebook doesn't starve
		Session facebookSession = Session.getActiveSession();
		if(facebookSession != null){
			facebookSession.onActivityResult(this, requestCode, resultCode, data);
		}
	}
	
	public static class SignupPagerAdapter extends FragmentPagerAdapter{
		
		private CheckUsernameFragment checkUsernameFragment;
		private PhotoInfoFragment photoInfoFragment;
		private FaceDetectionFragment faceDetectionFragment;
		private FacebookInfoFragment facebookInfoFragment;
		private PersonalDetailsFragment personalDetailsFragment;
		
		private synchronized CheckUsernameFragment getCheckUsernameFragment(){
			if(this.checkUsernameFragment == null){
				this.checkUsernameFragment = new CheckUsernameFragment();
			}
			
			return this.checkUsernameFragment;
		}
		
		private synchronized PhotoInfoFragment getPhotoInfoFragment(){
			if(this.photoInfoFragment == null){
				this.photoInfoFragment = new PhotoInfoFragment();
			}
			
			return this.photoInfoFragment;
		}
		
		private synchronized FaceDetectionFragment getFaceDetectionFragment(){
			if(this.faceDetectionFragment == null){
				this.faceDetectionFragment = new FaceDetectionFragment();
			}
			
			return this.faceDetectionFragment;
		}
		
		private synchronized FacebookInfoFragment getFacebookInfoFragment(){
			if(this.facebookInfoFragment == null){
				this.facebookInfoFragment = new FacebookInfoFragment();
			}
			
			return this.facebookInfoFragment;
		}
		
		private synchronized PersonalDetailsFragment getPersonalDetailsFragment(){
			if(this.personalDetailsFragment == null){
				this.personalDetailsFragment = new PersonalDetailsFragment();
			}
			
			return this.personalDetailsFragment;
		}
		
		public SignupPagerAdapter(FragmentManager fm) {
			super(fm);
		}
	
		@Override
		public Fragment getItem(int fragmentIndex) {
			if(fragmentIndex == FRAG_INDEX_SIGN_UP){
				return this.getCheckUsernameFragment();
				// return new CheckUsernameFragment();
			}
			else if(fragmentIndex == FRAG_INDEX_PHOTO_INFO){
				return this.getPhotoInfoFragment();
				//return new PhotoInfoFragment();
			}
			else if(fragmentIndex == FRAG_INDEX_FACE_DETECT){
				return this.getFaceDetectionFragment();
				//return new FaceDetectionFragment();
			}
			else if(fragmentIndex == FRAG_INDEX_FBOOK_INFO){
				return this.getFacebookInfoFragment();
				// return new FacebookInfoFragment();
			}
			else if(fragmentIndex == FRAG_INDEX_PERSONAL_DETAILS){
				return this.getPersonalDetailsFragment();
				// return new PersonalDetailsFragment();
			}
			else{
				Fragment fragment = new DummySectionFragment();
				Bundle args = new Bundle();
				args.putInt(DummySectionFragment.ARG_SECTION_NUMBER, fragmentIndex + 1);
				fragment.setArguments(args);
				return fragment;
			}
		}

		@Override
		public int getCount() {
			return FRAG_COUNT;
		}
	}	 
}