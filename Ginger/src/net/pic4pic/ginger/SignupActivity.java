package net.pic4pic.ginger;

import com.facebook.Session;

import net.pic4pic.ginger.MainActivity.DummySectionFragment;
import net.pic4pic.ginger.utils.GingerHelpers;
import net.pic4pic.ginger.utils.ImageActivity;
import net.pic4pic.ginger.utils.ImageStorageHelper;
import net.pic4pic.ginger.utils.NonSwipeableViewPager;
import net.pic4pic.ginger.utils.PageAdvancer;

import android.content.Intent;
import android.graphics.Bitmap;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentActivity;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentPagerAdapter;
import android.util.Log;
import android.view.KeyEvent;
import android.view.Menu;

public class SignupActivity extends FragmentActivity implements PageAdvancer {
	
	public static final int CaptureCameraCode = 501;
	public static final int PickFromGalleryCode = 502;
	
	private static final int FRAG_COUNT = 5;
	private static final int FRAG_INDEX_SIGN_UP = 0;
	private static final int FRAG_INDEX_PHOTO_INFO = 1;
	private static final int FRAG_INDEX_FACE_DETECT = 2;
	private static final int FRAG_INDEX_FBOOK_INFO = 3;
	private static final int FRAG_INDEX_PERSONAL_DETAILS = 4;
	
	private NonSwipeableViewPager fragmentPager;
	private SignupPagerAdapter fragmentPagerAdapter;
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_signup);
		
		this.fragmentPagerAdapter = new SignupPagerAdapter(this.getSupportFragmentManager());		
		this.fragmentPager = (NonSwipeableViewPager) findViewById(R.id.fragmentPager);
		this.fragmentPager.setAdapter(this.fragmentPagerAdapter);
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
			/*if(currentFragment == FRAG_INDEX_FACE_DETECT){ // face-detection
				this.startCamera();
				return true;
			}
			else */if(currentFragment > 0){
				this.fragmentPager.setCurrentItem(currentFragment-1, true);
		        return true;
			}
		}
		
		return super.onKeyDown(keyCode, event);
	}
	
	public void moveToPreviousPage(){
		int currentFragment = this.fragmentPager.getCurrentItem();
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
	
	public Fragment getFragment(int index) {		
		int fragmentId = this.fragmentPager.getId();
		String fragmentTag = "android:switcher:" + fragmentId + ":" + index;		
		return this.getSupportFragmentManager().findFragmentByTag(fragmentTag);
	}
	
	private Bitmap processPhotoActivityResult(int resultCode, Intent data){
		ImageActivity.Result result = ImageActivity.getProcessedResult(this, resultCode, data);
		if(result.getBitmap() != null){
			String fileName = this.getString(R.string.lastCapturedPhoto_filename_key);
			if(ImageStorageHelper.saveToInternalStorage(this, result.getBitmap(), fileName, true)){
				Log.v("Storage", "Bitmap has been saved to the internal storage");
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
			Log.v("ActivityResult", "CaptureCameraActivity has returned");
			this.processPhotoActivityResult(resultCode, data);
	    }
		else if (requestCode == SignupActivity.PickFromGalleryCode) {
			Log.v("ActivityResult", "PickFromGalleryActivity has returned");
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