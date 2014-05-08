package net.pic4pic.ginger;

import java.util.Date;

import android.os.Bundle;
import android.app.Activity;
import android.content.Intent;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.FrameLayout;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import net.pic4pic.ginger.entities.Familiarity;
import net.pic4pic.ginger.entities.Gender;
import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.entities.ImageFile;
import net.pic4pic.ginger.entities.MatchedCandidate;
import net.pic4pic.ginger.entities.Pic4PicHistory;
import net.pic4pic.ginger.entities.Pic4PicHistoryRequest;
import net.pic4pic.ginger.service.Service;
import net.pic4pic.ginger.tasks.ITask;
import net.pic4pic.ginger.tasks.ImageDownloadTask;
import net.pic4pic.ginger.tasks.NonBlockedTask;
import net.pic4pic.ginger.utils.ImageClickListener;
import net.pic4pic.ginger.utils.ImageGalleryView;
import net.pic4pic.ginger.utils.MyLog;

public class PersonActivity extends Activity {

	public static final String PersonType = "net.pic4pic.ginger.Person"; 
	public static final int PersonActivityCode = 201;
	
	private MatchedCandidate person;
	private Pic4PicHistory pic4picHistory;
	private Date lastHistoryRetrieveTime = null;
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		
		MyLog.v("PersonActivity", "onCreate");
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_person);
		
		Intent intent = getIntent();
		this.person = (MatchedCandidate)intent.getSerializableExtra(PersonType);
		this.setTitle(this.person.getCandidateProfile().getUsername());
		
		// Show the Up button in the action bar.
		setupActionBar();
		
		// set user-name
		TextView usernameText = (TextView)this.findViewById(R.id.candidateUsername);
		usernameText.setText(this.person.getCandidateProfile().getUsername());
		
		// set short bio
		TextView shortBioText = (TextView)this.findViewById(R.id.candidateShortBio);
		shortBioText.setText(this.person.getCandidateProfile().getShortBio());
		
		// set description
		String descr = this.person.getCandidateProfile().getDescription();
		TextView descrText = (TextView)this.findViewById(R.id.candidateDescription);
		if(descr == null || descr.trim().length() <= 0){
			descrText.setVisibility(View.GONE);
		}
		else{
			descrText.setText(descr);
		}
		
		// set dynamic content
		this.adjustAll();
		
		// initiate a refresh from server
		if(this.isNeedOfRequestingPic4PicHistory()){
			// get pic4picHistory again
			this.startRetrievingPic4PicHistory();
		}
	}
	
	@Override
	public void onSaveInstanceState(Bundle outState) {
		
		MyLog.v("PersonActivity", "onSaveInstanceState");
		super.onSaveInstanceState(outState);

		if(outState == null){
			return;
		}

		if(this.pic4picHistory != null){
			outState.putSerializable("pic4picHistory", this.pic4picHistory);
		}
		
		if(this.lastHistoryRetrieveTime != null){
			outState.putSerializable("lastHistoryRetrieveTime", this.lastHistoryRetrieveTime);
		}
	}
	
	@Override
	public void onRestoreInstanceState(Bundle savedInstanceState) {
		MyLog.v("PersonActivity", "onRestoreInstanceState");
		super.onRestoreInstanceState(savedInstanceState);
		this.recreatePropertiesFromSavedBundle(savedInstanceState);
	}
	
	private void recreatePropertiesFromSavedBundle(Bundle state){
		
		if(state == null){
			return;
		}
		
		if(state.containsKey("pic4picHistory")){
			this.pic4picHistory = (Pic4PicHistory)state.getSerializable("pic4picHistory");
		}
		
		if(state.containsKey("lastHistoryRetrieveTime")){
			this.lastHistoryRetrieveTime = (Date)state.getSerializable("lastHistoryRetrieveTime");
		}
	}
	
	public boolean isNeedOfRequestingPic4PicHistory(){
		
		if(this.lastHistoryRetrieveTime == null){
			
			MyLog.v("PersonActivity", "Last history retrieve time is null. We need to request history again");
			return true;
		}
		
		Date now = new Date();
		long diffAsMilliSeconds = now.getTime() - this.lastHistoryRetrieveTime.getTime();
		long diffAsSeconds = diffAsMilliSeconds / 60000;
		if(diffAsSeconds > 60){
			MyLog.v("PersonActivity", "Last history retrieve time was 60 seconds ago. We better request history again");
			return true;
		}
		
		return false;
	}
	
	public void startRetrievingPic4PicHistory(){
		
		final Pic4PicHistoryRequest request = new Pic4PicHistoryRequest();
		request.setUserIdToInteract(this.person.getCandidateProfile().getUserId());
		
		NonBlockedTask.SafeRun(new ITask(){
			@Override
			public void perform() {
				try
				{
					final Pic4PicHistory response = Service.getInstance().getPic4PicHistory(PersonActivity.this, request);
					if(response.getErrorCode() == 0){
						MyLog.i("PersonActivity", "Pic4PicHistory retrieved");
						
						// Only the original thread that created a view hierarchy can touch its views.
						runOnUiThread(new Runnable() {
						     @Override
						     public void run() {
						    	 onPic4PicHistorySuccessfullyRetrieved(response);
						    }
						});
					}
					else {
						MyLog.e("PersonActivity", "Pic4PicHistory request failed: " + response.getErrorMessage());
					}
				}
				catch(GingerException e) {
					
					MyLog.e("PersonActivity", "Pic4PicHistory request failed: " + e.getMessage());
				}
			}
		});
	}
	
	protected void onPic4PicHistorySuccessfullyRetrieved(Pic4PicHistory response){
		
		this.pic4picHistory = response;
		this.lastHistoryRetrieveTime = new Date();
		this.person.getCandidateProfile().setFamiliarity(this.pic4picHistory.getFamiliarity());
		
		this.adjustActionButtons();	
	}
	
	private void adjustAll(){
		this.adjustAvatarImage();		
		this.adjustMainImage();		
		this.adjustActionButtons();		
		this.adjustPhotoGallery();
	}
	
	private void adjustAvatarImage(){
		
		ImageView avatarView = (ImageView)this.findViewById(R.id.candidateAvatar);
		avatarView.setImageResource(android.R.drawable.ic_menu_gallery);
		ImageFile imageToDownload = person.getProfilePics().getThumbnail();
		ImageDownloadTask avatarDownloadTask = new ImageDownloadTask(imageToDownload.getId(), avatarView);
		avatarDownloadTask.execute(imageToDownload.getCloudUrl());		
	}
	
	private void adjustMainImage(){
		
		ImageView mainPhotoView = (ImageView)this.findViewById(R.id.candidateMainPhoto);
		mainPhotoView.setImageResource(android.R.drawable.ic_menu_gallery);
		ImageFile imageToDownload = person.getProfilePics().getFullSize();
		ImageDownloadTask mainPhotoDownloadTask = new ImageDownloadTask(imageToDownload.getId(), mainPhotoView, true);
		mainPhotoDownloadTask.execute(imageToDownload.getCloudUrl());
		
		if(person.getCandidateProfile().getFamiliarity() == Familiarity.Familiar){
			mainPhotoView.setOnClickListener(new ImageClickListener(this, R.id.candidateMainPhoto));			
		}
		else{					
			this.addWatermark();
		}
	}
	
	private void adjustActionButtons(){
		
		Button pic4picButton = null;
		String acceptText = "accept pic4pic";
		LinearLayout buttonGroupFamiliar = (LinearLayout)this.findViewById(R.id.candidateActions);
		LinearLayout buttonGroupStranger = (LinearLayout)this.findViewById(R.id.candidateAnonymousActions);		
		
		if(this.person.getCandidateProfile().getFamiliarity() == Familiarity.Familiar){
			
			buttonGroupStranger.setVisibility(View.INVISIBLE);
			buttonGroupFamiliar.setVisibility(View.VISIBLE);
			
			pic4picButton = (Button)this.findViewById(R.id.candidateSendMoreP4PButton);
			acceptText = this.getString(R.string.candidate_acceptP4P);
			
			if(this.person.getCandidateProfile().getGender() == Gender.Male){
				acceptText = this.getString(R.string.candidate_acceptP4P_he);
			}
			else if(this.person.getCandidateProfile().getGender() == Gender.Female){
				acceptText = this.getString(R.string.candidate_acceptP4P_she);
			}
		}
		else{
			
			buttonGroupFamiliar.setVisibility(View.INVISIBLE);
			buttonGroupStranger.setVisibility(View.VISIBLE);
			
			pic4picButton = (Button)this.findViewById(R.id.candidateSendP4PButton);
			acceptText = this.getString(R.string.candidate_acceptP4P);
		}
		
		if(this.pic4picHistory != null && this.pic4picHistory.getLastPendingPic4PicRequest() != null){
			
			pic4picButton.setText(acceptText);
		}
	}
	
	private void adjustPhotoGallery(){
		
		LinearLayout photoGalleryParent = (LinearLayout)this.findViewById(R.id.candidateView);
		ImageGalleryView gallery = new ImageGalleryView(this, photoGalleryParent, this.person.getOtherPictures()); 
		gallery.fillPhotos();	
	}
	
	private void addWatermark(){
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
		
		FrameLayout imageContainer = (FrameLayout)this.findViewById(R.id.candidateMainPhotoContainer);
		imageContainer.addView(watermark);
	}
	
	/**
	 * Set up the {@link android.app.ActionBar}.
	 */
	private void setupActionBar() {
		getActionBar().setDisplayHomeAsUpEnabled(true);
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.person, menu);
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
}
