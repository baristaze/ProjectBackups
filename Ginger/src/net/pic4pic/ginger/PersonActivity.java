package net.pic4pic.ginger;

import java.util.Date;
import java.util.UUID;

import android.os.Bundle;
import android.app.Activity;
import android.content.Intent;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.FrameLayout;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import net.pic4pic.ginger.entities.AcceptingPic4PicRequest;
import net.pic4pic.ginger.entities.BaseResponse;
import net.pic4pic.ginger.entities.CandidateDetailsRequest;
import net.pic4pic.ginger.entities.CandidateDetailsResponse;
import net.pic4pic.ginger.entities.Familiarity;
import net.pic4pic.ginger.entities.Gender;
import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.entities.ImageFile;
import net.pic4pic.ginger.entities.MarkingRequest;
import net.pic4pic.ginger.entities.MarkingType;
import net.pic4pic.ginger.entities.MatchedCandidate;
import net.pic4pic.ginger.entities.MatchedCandidateResponse;
import net.pic4pic.ginger.entities.ObjectType;
import net.pic4pic.ginger.entities.SimpleResponseGuid;
import net.pic4pic.ginger.entities.StartingPic4PicRequest;
import net.pic4pic.ginger.entities.UserResponse;
import net.pic4pic.ginger.service.Service;
import net.pic4pic.ginger.tasks.AcceptPic4PicTask;
import net.pic4pic.ginger.tasks.AcceptPic4PicTask.AcceptPic4PicListener;
import net.pic4pic.ginger.tasks.ITask;
import net.pic4pic.ginger.tasks.ImageDownloadTask;
import net.pic4pic.ginger.tasks.NonBlockedTask;
import net.pic4pic.ginger.utils.GingerHelpers;
import net.pic4pic.ginger.utils.ImageClickListener;
import net.pic4pic.ginger.utils.ImageGalleryView;
import net.pic4pic.ginger.utils.MyLog;

public class PersonActivity extends Activity implements AcceptPic4PicListener {

	public static final String PersonType = "net.pic4pic.ginger.Person"; 
	public static final String ParentCallerClassName = "net.pic4pic.ginger.ParentCallerClassName";
	
	public static final int PersonActivityCode = 201;
	
	private UserResponse me;
	private MatchedCandidate person;
	private Date lastDetailsRetrieveTime = null;
	private String parentCallerClassName;
	
	private ImageGalleryView galleryView;
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		
		MyLog.v("PersonActivity", "onCreate");
		super.onCreate(savedInstanceState);
		if(this.recreatePropertiesFromSavedBundle(savedInstanceState)){
			MyLog.i("PersonActivity", "At least one property is restored successfully");
		}
		
		setContentView(R.layout.activity_person);
		
		Intent intent = getIntent();
		this.me = (UserResponse)intent.getSerializableExtra(MainActivity.AuthenticatedUserBundleType);
		this.person = (MatchedCandidate)intent.getSerializableExtra(PersonType);	
		this.parentCallerClassName = intent.getStringExtra(ParentCallerClassName);
				
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
		else {
			descrText.setText(descr);
		}
		
		// create gallery view
		LinearLayout photoGalleryParent = (LinearLayout)this.findViewById(R.id.candidateView);	
		this.galleryView = new ImageGalleryView(this, photoGalleryParent, this.person.getOtherPictures());
		
		// set dynamic content
		this.adjustAll(true);
		
		// send or accept P4P
		final Button candidateSendP4PButton = (Button)this.findViewById(R.id.candidateSendP4PButton);
		candidateSendP4PButton.setOnClickListener(new OnClickListener(){
			@Override
			public void onClick(View v) {				
				if(PersonActivity.this.person.hasPic4PicPending()){
					PersonActivity.this.acceptLastPic4PicRequest(candidateSendP4PButton);
				}
				else{
					PersonActivity.this.sendPic4PicRequest(candidateSendP4PButton);
				}	
			}});
		
		// send more or accept P4P
		final Button candidateSendMoreP4PButton = (Button)this.findViewById(R.id.candidateSendMoreP4PButton);
		candidateSendMoreP4PButton.setOnClickListener(new OnClickListener(){
			@Override
			public void onClick(View v) {
				if(PersonActivity.this.person.hasPic4PicPending()){
					PersonActivity.this.acceptLastPic4PicRequest(candidateSendMoreP4PButton);
				}
				else{
					PersonActivity.this.sendPic4PicRequest(candidateSendMoreP4PButton);
				}				
			}});
		
		// set message button candidateMessageButton
		final Button candidateMessageButton = (Button)this.findViewById(R.id.candidateMessageButton);
		candidateMessageButton.setOnClickListener(new OnClickListener(){
			@Override
			public void onClick(View v) {				
				PersonActivity.this.openMessageThread();
			}});
		
		// set like button candidateLikeButton
		final Button candidateLikeButton = (Button)this.findViewById(R.id.candidateLikeButton);
		candidateLikeButton.setOnClickListener(new OnClickListener(){
			@Override
			public void onClick(View v) {				
				PersonActivity.this.sendLikeAction(candidateLikeButton);
			}});
		
		// mark as read
		this.markAsViewed();
		
		// initiate a refresh from server
		if(this.isNeedOfRequestingMoreDetails()){
			// get candidate details again
			this.startRetrievingMoreDetails();
		}
	}
	
	@Override
	public void onSaveInstanceState(Bundle outState) {
		
		super.onSaveInstanceState(outState);

		if(outState == null){
			return;
		}

		MyLog.v("PersonActivity", "onSaveInstanceState");
		
		if(this.lastDetailsRetrieveTime != null){
			outState.putSerializable("lastDetailsRetrieveTime", this.lastDetailsRetrieveTime);
		}
	}
	
	@Override
	public void onRestoreInstanceState(Bundle savedInstanceState) {
		MyLog.v("PersonActivity", "onRestoreInstanceState");
		super.onRestoreInstanceState(savedInstanceState);
		if(this.recreatePropertiesFromSavedBundle(savedInstanceState)){
			MyLog.i("PersonActivity", "At least one property is restored successfully");
		}
	}
	
	private boolean recreatePropertiesFromSavedBundle(Bundle state){
		
		if(state == null){
			return false;
		}
		
		boolean restored = false;
				
		if(state.containsKey("lastDetailsRetrieveTime")){
			this.lastDetailsRetrieveTime = (Date)state.getSerializable("lastDetailsRetrieveTime");
			restored = true;
		}
		
		return restored;
	}
	
	public boolean isNeedOfRequestingMoreDetails(){
		
		if(this.lastDetailsRetrieveTime == null){
			
			MyLog.v("PersonActivity", "Last details retrieve time is null. We need to request the details again");
			return true;
		}
		
		Date now = new Date();
		long diffAsMilliSeconds = now.getTime() - this.lastDetailsRetrieveTime.getTime();
		long diffAsSeconds = diffAsMilliSeconds / 60000;
		if(diffAsSeconds > 60){
			MyLog.v("PersonActivity", "Last history retrieve time was 60 seconds ago. We better request history again");
			return true;
		}
		
		return false;
	}
	
	public void startRetrievingMoreDetails(){
		
		final CandidateDetailsRequest request = new CandidateDetailsRequest();
		request.setUserId(this.person.getUserId());
		
		NonBlockedTask.SafeRun(new ITask(){
			@Override
			public void perform() {
				try
				{
					final CandidateDetailsResponse response = Service.getInstance().getCandidateDetails(PersonActivity.this, request);
					if(response.getErrorCode() == 0){
						
						MyLog.i("PersonActivity", "CandidateDetails retrieved");
						
						// Only the original thread that created a view hierarchy can touch its views.
						runOnUiThread(new Runnable() {
						     @Override
						     public void run() {
						    	 onCandidateDetailsSuccessfullyRetrieved(response);
						    }
						});
					}
					else {
						MyLog.e("PersonActivity", "CandidateDetails request failed: " + response.getErrorMessage());
					}
				}
				catch(GingerException e) {
					
					MyLog.e("PersonActivity", "CandidateDetails request failed: " + e.getMessage());
				}
			}
		});
	}
	
	protected void onCandidateDetailsSuccessfullyRetrieved(CandidateDetailsResponse response){
		
		// no error if we are here
		MyLog.v("PersonActivity", "More details on candidate has been retrieved.");
		
		this.lastDetailsRetrieveTime = new Date();
		
		Familiarity fam1 = this.person.getCandidateProfile().getFamiliarity();
		Familiarity fam2 = response.getCandidate().getCandidateProfile().getFamiliarity();			
		
		UUID pending1 = this.person.getLastPendingPic4PicId();
		UUID pending2 = response.getCandidate().getLastPendingPic4PicId();
		
		this.person = response.getCandidate();
		
		if(fam1.getIntValue() != fam2.getIntValue()){			
			MyLog.i("PersonActivity", "Familiarity has changed.");
			this.adjustAll(false);
		}
		else if(!pending1.equals(pending2)){			
			MyLog.i("PersonActivity", "Pending pic4pic ID has changed.");
			this.adjustActionButtons(false);
		}
		else{
			MyLog.v("PersonActivity", "Familiarity or Pending pic4pic IDs haven't changed.");
		}
	}
		
	private void adjustAll(boolean initialCreate){
		MyLog.v("PersonActivity", "Adjusting all...");
		this.adjustAvatarImage(initialCreate);		
		this.adjustMainImage(initialCreate);		
		this.adjustActionButtons(initialCreate);		
		this.adjustPhotoGallery(initialCreate);
	}
	
	private void adjustAvatarImage(boolean initialCreate){
		
		MyLog.v("PersonActivity", "Adjusting avatar image...");
		
		// get image view
		ImageView avatarView = (ImageView)this.findViewById(R.id.candidateAvatar);	
		if(avatarView == null){
			MyLog.e("PersonActivity", "The  avatar image view is null");
			return;	
		}
		
		// set something default
		if(initialCreate){
			avatarView.setImageResource(android.R.drawable.ic_menu_gallery);
		}
		
		// download thumb-nail photo and show
		ImageFile imageToDownload = person.getProfilePics().getThumbnail();
		ImageDownloadTask avatarDownloadTask = new ImageDownloadTask(imageToDownload.getId(), avatarView);
		avatarDownloadTask.execute(imageToDownload.getCloudUrl());		
	}
	
	private void adjustMainImage(boolean initialCreate){
		
		MyLog.v("PersonActivity", "Adjusting main image...");
		
		// get image view
		ImageView mainPhotoView = (ImageView)this.findViewById(R.id.candidateMainPhoto);
		if(mainPhotoView == null){
			MyLog.e("PersonActivity", "The  main image view is null");
			return;	
		}
		
		// set something default
		if(initialCreate){
			mainPhotoView.setImageResource(android.R.drawable.ic_menu_gallery);
		}
		
		// download full profile photo and show
		ImageFile imageToDownload = person.getProfilePics().getFullSize();
		ImageDownloadTask mainPhotoDownloadTask = new ImageDownloadTask(imageToDownload.getId(), mainPhotoView, true);
		mainPhotoDownloadTask.execute(imageToDownload.getCloudUrl());
		
		// set click event again since
		if(person.getCandidateProfile().getFamiliarity() == Familiarity.Familiar){
			
			// remove water-mark, which is safe if water-mark is not there
			this.removeWatermark(); 
			
			// set click listener
			mainPhotoView.setOnClickListener(new ImageClickListener(this, R.id.candidateMainPhoto));			
		}
		else{		
			
			// add water-mark, which is safe if water-mark is added already
			this.addWatermark(); 
			
			// reset click listener
			mainPhotoView.setOnClickListener(null);
		}
	}
	
	private void adjustActionButtons(boolean initialCreate){
		
		MyLog.v("PersonActivity", "Adjusting action buttons...");
		
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
		
		MyLog.v("PersonActivity", "Last Pending p4p ID: " + this.person.getLastPendingPic4PicId());
		if(this.person.hasPic4PicPending()){
			pic4picButton.setText(acceptText);
		}
	}
	
	private void adjustPhotoGallery(boolean initialCreate){
		
		MyLog.v("PersonActivity", "Adjusting photo gallery...");
		this.galleryView.fillPhotos(this.person.getOtherPictures());	
	}
	
	private void addWatermark(){
		
		MyLog.v("PersonActivity", "Adjusting water mark (add)...");
		
		FrameLayout imageContainer = (FrameLayout)this.findViewById(R.id.candidateMainPhotoContainer);
		if(imageContainer == null){
			MyLog.e("PersonActivity", "The view for MainPhotoContainer is null in addWatermark");
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
	
	private void removeWatermark(){
		
		MyLog.v("PersonActivity", "Adjusting water mark (remove)...");
		
		FrameLayout imageContainer = (FrameLayout)this.findViewById(R.id.candidateMainPhotoContainer);
		if(imageContainer == null){
			MyLog.e("PersonActivity", "The view for MainPhotoContainer is null in removeWatermark");
			return;
		}
		
		while(imageContainer.getChildCount() > 1){
			imageContainer.removeViewAt(imageContainer.getChildCount()-1);
		}
	} 
	
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
			this.setResultIntent();
			finish();
			return true;
		}
		return super.onOptionsItemSelected(item);
	}
	
	@Override
	public void onBackPressed() {
		this.setResultIntent();
		super.onBackPressed();
	}

	private void setResultIntent(){
		
		MyLog.v("PersonActivity", "Setting result intent");
		
		Intent resultIntent = new Intent();
		resultIntent.putExtra(MainActivity.UpdatedMatchCandidate, this.person);
		resultIntent.putExtra(ParentCallerClassName, this.parentCallerClassName);
		
		this.setResult(Activity.RESULT_OK, resultIntent);
		/*
		if (this.getParent() == null) {
		    this.setResult(Activity.RESULT_OK, resultIntent);
		} 
		else {
			this.getParent().setResult(Activity.RESULT_OK, resultIntent);
		}*/	
	}
	
	private void acceptLastPic4PicRequest(final Button button){
	
		UUID pic4picId = this.person.getLastPendingPic4PicId();
		if(pic4picId == null || pic4picId.equals(new UUID(0,0))){
			GingerHelpers.showErrorMessage(this, "It seems like you don't have any more pic4pic request");
			this.adjustActionButtons(false);
		}

		AcceptingPic4PicRequest request = new AcceptingPic4PicRequest();
		request.setPic4PicRequestId(pic4picId);
		request.setPictureIdToExchange(this.me.getProfilePictures().getFullSizeClear().getGroupingId());
		AcceptPic4PicTask task = new AcceptPic4PicTask(this, this, button, request);
		task.execute();
	}
	
	public void onPic4PicAccepted(MatchedCandidateResponse response, AcceptingPic4PicRequest request){
		
		if(response.getErrorCode() == 0){	
			
			MyLog.v("PersonActivity", "Accepting pic4pic call has returned.");
			
			MatchedCandidate candidate = response.getData();
			
			Familiarity fam1 = this.person.getCandidateProfile().getFamiliarity();
			Familiarity fam2 = candidate.getCandidateProfile().getFamiliarity();			
			
			UUID pending1 = this.person.getLastPendingPic4PicId();
			UUID pending2 = candidate.getLastPendingPic4PicId();
			
			// set this first
			this.person = candidate;
			
			if(fam1.getIntValue() != fam2.getIntValue()){			
				MyLog.i("PersonActivity", "Familiarity has changed after pic4pic.");
				this.adjustAll(false);
			}
			else {
				MyLog.i("PersonActivity", "Familiarity has not changed after pic4pic. Adjusting image gallery.");
				this.adjustPhotoGallery(false);
			
				if(!pending1.equals(pending2)){			
					MyLog.i("PersonActivity", "Pending pic4pic ID has changed.");
					this.adjustActionButtons(false);
				}
			}
			
			GingerHelpers.toast(this, "Accepted successfully \u2713");
		}
		else{
			GingerHelpers.showErrorMessage(this, response.getErrorMessage());
		}
	}
	
	private void sendPic4PicRequest(final Button button){
		
		final UUID candidateId = this.person.getUserId();
		final UUID pic4picId = this.person.getLastPendingPic4PicId();
		if(pic4picId != null && !pic4picId.equals(new UUID(0,0))){
			GingerHelpers.showErrorMessage(this, "It seems like you have received a pic4pic request from this person already.");
			this.adjustActionButtons(false);
		}	
		
		// prepare request
		final StartingPic4PicRequest request = new StartingPic4PicRequest();
		request.setUserIdToInteract(this.person.getUserId());
		// do not define this at the first time
		// request.setPictureIdToExchange(this.person.getProfilePics().getFullSize().getGroupingId());
		
		// disable button at the beginning
		button.setEnabled(false);
		
		// send like action
		NonBlockedTask.SafeRun(new ITask(){
			
			@Override
			public void perform() {
				
				boolean success = false;
				
				try{
					// mark as liked
					SimpleResponseGuid response = Service.getInstance().requestPic4Pic(PersonActivity.this, request);
					
					// check result
					if(response.getErrorCode() == 0){
						
						success = true;
						
						// log
						MyLog.v("PersonActivity", "pic4pic request has been sent to: " + candidateId + " -> request ID: " + response.getData());
					
						// we have viewed this profile if we have sent a pic4pic request it
						person.setLastViewTimeUTC(new Date());
						
						// change the button text
						PersonActivity.this.runOnUiThread(new Runnable(){
							@Override
							public void run() {
								
								// change text
								String newText = PersonActivity.this.getString(R.string.candidate_sendMoreP4P);					
								button.setText(newText);
								
								// enable for better UI but remove listener
								button.setEnabled(true);
								button.setOnClickListener(null);
								
								// toast
								GingerHelpers.toast(PersonActivity.this, "Sent successfully \u2713");
							}
						});
					}
					else{
						// log error
						MyLog.e("PersonActivity", "Requesting pic4pic from candidate(" + candidateId + ") failed: " + response.getErrorMessage());
					}
				}
				catch(GingerException ge){
					
					// log error
					MyLog.e("PersonActivity", "Requesting pic4pic from candidate(" + candidateId + ") failed: " + ge.getMessage());
				}
				catch(Exception e){
					
					// log error
					MyLog.e("PersonActivity", "Liking candidate(" + candidateId + ") failed: " + e.toString());
				}
				
				if(!success){
					
					PersonActivity.this.runOnUiThread(new Runnable(){
						@Override
						public void run() {
							// enable back since we failed
							button.setEnabled(true);
						}
					});
				}
			}
		});
	}
	
	private void openMessageThread(){
	
		GingerHelpers.toast(PersonActivity.this, "Coming soon!..");
	}
	
	private void sendLikeAction(final Button candidateLikeButton){
	
		// mark as liked
		// prepare request
		final UUID candidateId = person.getUserId();
		final MarkingRequest marking = new MarkingRequest();
		marking.setObjectType(ObjectType.Profile);
		marking.setMarkingType(MarkingType.Liked);
		marking.setObjectId(candidateId);
		
		// disable button at the beginning
		candidateLikeButton.setEnabled(false);
		
		// send like action
		NonBlockedTask.SafeRun(new ITask(){
			
			@Override
			public void perform() {
				
				boolean success = false;
				
				try{
					// mark as liked
					BaseResponse response = Service.getInstance().mark(PersonActivity.this, marking);
					
					// check result
					if(response.getErrorCode() == 0){
						
						success = true;
						
						// log
						MyLog.v("PersonActivity", "Candidate has been marked as LIKED: " + candidateId);
					
						// we have viewed this profile if we have liked it
						person.setLastViewTimeUTC(new Date());
						
						// change the button text
						PersonActivity.this.runOnUiThread(new Runnable(){
							@Override
							public void run() {
								
								// change text
								String newText = PersonActivity.this.getString(R.string.candidate_liked);					
								candidateLikeButton.setText(newText);
								
								// enable for better UI but remove listener
								candidateLikeButton.setEnabled(true);
								candidateLikeButton.setOnClickListener(null);
								
								// toast
								GingerHelpers.toast(PersonActivity.this, "Liked successfully \u2713");
							}
						});
					}
					else{
						// log error
						MyLog.e("PersonActivity", "Liking candidate(" + candidateId + ") failed: " + response.getErrorMessage());
					}
				}
				catch(GingerException ge){
					
					// log error
					MyLog.e("PersonActivity", "Liking candidate(" + candidateId + ") failed: " + ge.getMessage());
				}
				catch(Exception e){
					
					// log error
					MyLog.e("PersonActivity", "Liking candidate(" + candidateId + ") failed: " + e.toString());
				}
				
				if(!success){
					PersonActivity.this.runOnUiThread(new Runnable(){
						@Override
						public void run() {
							// enable back since we failed
							candidateLikeButton.setEnabled(true);
						}
					});
				}
			}
		});
	}
	
	private void markAsViewed(){	
		
		// mark as read
		if(!person.isViewed()){			
			
			// prepare request
			final UUID candidateId = person.getUserId();
			final MarkingRequest marking = new MarkingRequest();
			marking.setObjectType(ObjectType.Profile);
			marking.setMarkingType(MarkingType.Viewed);
			marking.setObjectId(candidateId);
			
			MyLog.v("PersonActivity", "Marking as viewed: " + candidateId);
			
			NonBlockedTask.SafeRun(new ITask(){
				@Override
				public void perform() {					
					try{
						// mark as viewed
						BaseResponse response = Service.getInstance().mark(PersonActivity.this, marking);
						if(response.getErrorCode() == 0){
							
							// set the view time to avoid excessive posts to service
							person.setLastViewTimeUTC(new Date());
							
							// log
							MyLog.v("PersonActivity", "Candidate has been marked as viewed: " + candidateId);
						}
						else{
							
							// log error
							MyLog.e("PersonActivity", "Marking candidate as viewed failed: " + response.getErrorMessage());
						}
					}
					catch(GingerException ge){
						
						// log error
						MyLog.e("PersonActivity", "Marking candidate as viewed failed: " + ge.getMessage());
					}
					catch(Exception e){
						
						// log error
						MyLog.e("PersonActivity", "Marking candidate as viewed failed: " + e.toString());
					}
				}
			});
		}
	}
}
