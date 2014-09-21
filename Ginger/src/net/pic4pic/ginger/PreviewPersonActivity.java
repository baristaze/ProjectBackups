package net.pic4pic.ginger;

import net.pic4pic.ginger.entities.Gender;
import net.pic4pic.ginger.entities.ImageFile;
import net.pic4pic.ginger.entities.MatchedCandidate;
import net.pic4pic.ginger.tasks.ImageDownloadTask;
import net.pic4pic.ginger.utils.ImageGalleryView;
import net.pic4pic.ginger.utils.MyLog;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.os.Bundle;
import android.view.ContextThemeWrapper;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.FrameLayout;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

public class PreviewPersonActivity extends Activity {

	public static final String PreviewPersonType = "net.pic4pic.ginger.PreviewPerson";	
	public static final int PreviewPersonActivityCode = 1201;
	
	private MatchedCandidate person;	
	private ImageGalleryView galleryView;
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		
		MyLog.bag().v("onCreate");
		super.onCreate(savedInstanceState);
		
		if(this.recreatePropertiesFromSavedBundle(savedInstanceState)){
			MyLog.bag().i("At least one property is restored successfully");
		}
				
		setContentView(R.layout.activity_preview_person);
		
		Intent intent = getIntent();
		MatchedCandidate personX = (MatchedCandidate)intent.getSerializableExtra(PreviewPersonType);
		if(personX != null){
			this.person = personX;
		}
		else{
			if(this.person == null){
				MyLog.bag().e("'person' couldn't be retrieved from intent in onCreate(). It is null.");
			}
			else{
				MyLog.bag().i("'person' couldn't be retrieved from intent in onCreate()");
			}	
		}
		
		// this.setTitle(this.person.getCandidateProfile().getUsername());
		// this.setTitle("pic4pic");
		this.setTitle(" pic4pic - preview - candidate");
		
		// Show the Up button in the action bar.
		setupActionBar();
		
		// set user-name
		TextView usernameText = (TextView)this.findViewById(R.id.candidateUsername);
		usernameText.setText(this.person.getCandidateProfile().getDisplayName());
		
		// set short bio
		TextView shortBioText = (TextView)this.findViewById(R.id.candidateShortBio);
		shortBioText.setText(this.person.getCandidateProfile().getShortBio());
		
		// set description
		String descr = this.person.getCandidateProfile().getDescription();
		TextView descrText = (TextView)this.findViewById(R.id.candidateDescription);
		if(descr == null || descr.trim().length() <= 0){
			descrText.setVisibility(View.GONE);
		}
		else {
			descrText.setText(descr);
		}
		
		// create gallery view
		LinearLayout photoGalleryParent = (LinearLayout)this.findViewById(R.id.candidateView);	
		this.galleryView = new ImageGalleryView(this, photoGalleryParent, this.person.getOtherPictures());
		
		// set dynamic content
		this.adjustAll(true);
		
		// send P4P
		final Button candidateSendP4PButton = (Button)this.findViewById(R.id.candidateSendP4PButton);
		candidateSendP4PButton.setOnClickListener(new OnClickListener(){
			@Override
			public void onClick(View v) {				
				PreviewPersonActivity.this.suggestSignUp(candidateSendP4PButton);
			}});
		
		// set like button candidateLikeButton
		final Button candidateLikeButton = (Button)this.findViewById(R.id.candidateLikeButton);
		candidateLikeButton.setOnClickListener(new OnClickListener(){
			@Override
			public void onClick(View v) {				
				PreviewPersonActivity.this.suggestSignUp(candidateLikeButton);
			}});
	}
	
	@Override
	public void onSaveInstanceState(Bundle outState) {
		
		super.onSaveInstanceState(outState);

		if(outState == null){
			return;
		}

		MyLog.bag().v("onSaveInstanceState");
		
		if(this.person != null){
			outState.putSerializable("person", this.person);
		}		
	}
	
	@Override
	public void onRestoreInstanceState(Bundle savedInstanceState) {
		
		MyLog.bag().v("onRestoreInstanceState");
		
		super.onRestoreInstanceState(savedInstanceState);
		
		if(this.recreatePropertiesFromSavedBundle(savedInstanceState)){
			MyLog.bag().i("At least one property is restored successfully");
		}
	}
	
	private boolean recreatePropertiesFromSavedBundle(Bundle state){
		
		if(state == null){
			return false;
		}
		
		boolean restored = false;
		
		if(state.containsKey("person")){
			this.person = (MatchedCandidate)state.getSerializable("person");
			restored = true;
		}
				
		return restored;
	}
	
	private void adjustAll(boolean initialCreate){
		MyLog.bag().v("Adjusting all...");
		this.adjustAvatarImage(initialCreate);		
		this.adjustMainImage(initialCreate);		
		this.adjustPhotoGallery(initialCreate);
	}
	
	private void adjustAvatarImage(boolean initialCreate){
		
		MyLog.bag().v("Adjusting avatar image...");
		
		// get image view
		ImageView avatarView = (ImageView)this.findViewById(R.id.candidateAvatar);	
		if(avatarView == null){
			MyLog.bag().e("The  avatar image view is null");
			return;	
		}
		
		// set something default
		if(initialCreate){
			
			// set the default image
			/*
			if(person.getCandidateProfile().getGender().getIntValue() == Gender.Male.getIntValue()){
				avatarView.setImageResource(R.drawable.man_downloading_small);
			}
			else if(person.getCandidateProfile().getGender().getIntValue() == Gender.Female.getIntValue()){
				avatarView.setImageResource(R.drawable.woman_downloading_small);
			}
			else{
				avatarView.setImageResource(android.R.drawable.ic_menu_gallery);
			}
			*/
			avatarView.setImageResource(R.drawable.downloading_small);
		}
		
		// download thumb-nail photo and show
		ImageFile imageToDownload = person.getProfilePics().getThumbnail();
		ImageDownloadTask avatarDownloadTask = new ImageDownloadTask(imageToDownload.getId(), avatarView);
		avatarDownloadTask.execute(imageToDownload.getCloudUrl());		
	}
	
	private void adjustMainImage(boolean initialCreate){
		
		MyLog.bag().v("Adjusting main image...");
		
		// get image view
		ImageView mainPhotoView = (ImageView)this.findViewById(R.id.candidateMainPhoto);
		if(mainPhotoView == null){
			MyLog.bag().e("The  main image view is null");
			return;	
		}
		
		// set something default
		if(initialCreate){			
			// set the default image
			/*
			if(person.getCandidateProfile().getGender().getIntValue() == Gender.Male.getIntValue()){
				mainPhotoView.setImageResource(R.drawable.man_downloading_big);
			}
			else if(person.getCandidateProfile().getGender().getIntValue() == Gender.Female.getIntValue()){
				mainPhotoView.setImageResource(R.drawable.woman_downloading_big);
			}
			else{
				mainPhotoView.setImageResource(android.R.drawable.ic_menu_gallery);
			}
			*/
			mainPhotoView.setImageResource(R.drawable.downloading_big);
		}
		
		// download full profile photo and show
		ImageFile imageToDownload = person.getProfilePics().getFullSize();
		ImageDownloadTask mainPhotoDownloadTask = new ImageDownloadTask(imageToDownload.getId(), mainPhotoView, true);
		mainPhotoDownloadTask.execute(imageToDownload.getCloudUrl());
		
		// add water-mark, which is safe if water-mark is added already
		this.addWatermark();
	}
	
	private void adjustPhotoGallery(boolean initialCreate){
		
		MyLog.bag().v("Adjusting photo gallery...");
		this.galleryView.fillPhotos(this.person.getOtherPictures());	
	}
	
	private void addWatermark(){
		
		MyLog.bag().v("Adjusting water mark (add)...");
		
		FrameLayout imageContainer = (FrameLayout)this.findViewById(R.id.candidateMainPhotoContainer);
		if(imageContainer == null){
			MyLog.bag().e("The view for MainPhotoContainer is null in addWatermark");
			return;
		}
		
		if(imageContainer.getChildCount() > 1){
			return;
		}
		
		String readyText = this.getString(R.string.candidate_ready_for_p4p);
		if(this.person.getCandidateProfile().getGender() == Gender.Female){
			readyText = this.getString(R.string.candidate_ready_for_p4p_she);
		}
		else if(this.person.getCandidateProfile().getGender() == Gender.Male){
			readyText = this.getString(R.string.candidate_ready_for_p4p_he);
		}
		
		View watermarkView = this.getLayoutInflater().inflate(R.layout.watermark, null);
		TextView watermark = (TextView)watermarkView.findViewById(R.id.watermarkText);
		watermark.setText(readyText);
		
		imageContainer.addView(watermark);
	}
	
	private void setupActionBar() {
		getActionBar().setDisplayHomeAsUpEnabled(true);
	}
	
	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.preview, menu);
		return true;
	}

	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
		switch (item.getItemId()) {
		case android.R.id.home:
			//NavUtils.navigateUpFromSameTask(this);	
			finish();
			return true;
		}
		return super.onOptionsItemSelected(item);
	}
	
	@Override
	public void onBackPressed() {
		super.onBackPressed();
	}
	
	private void suggestSignUp(final Button button){

		new AlertDialog.Builder(new ContextThemeWrapper(this, android.R.style.Theme_Holo_Dialog))
	    .setTitle(this.getString(R.string.register_suggest_dlg_title))
	    .setMessage(this.getString(R.string.register_suggest_dlg_desc))		    
	    .setIcon(android.R.drawable.ic_dialog_info)
	    .setCancelable(true)
	    .setNegativeButton(this.getString(R.string.general_Cancel), new DialogInterface.OnClickListener() {
	        public void onClick(DialogInterface dialog, int which) {
	        	
	        }})
	    .setPositiveButton(this.getString(R.string.register_suggest_dlg_register), new DialogInterface.OnClickListener() {
	        public void onClick(DialogInterface dialog, int which) {
	        	PreviewPersonActivity.this.startSignUp();
	        }})
	    .show();
	}
	
	private void startSignUp(){
		
		MyLog.bag().v("Setting result intent");
		
		Intent resultIntent = new Intent();
		resultIntent.putExtra(PreviewActivity.JumpToSignupCode, true);
		this.setResult(Activity.RESULT_OK, resultIntent);
		this.finish();
	}
}
