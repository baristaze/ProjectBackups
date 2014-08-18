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
import net.pic4pic.ginger.entities.IntegerEnum;
import net.pic4pic.ginger.entities.MarkingRequest;
import net.pic4pic.ginger.entities.MarkingType;
import net.pic4pic.ginger.entities.MatchedCandidate;
import net.pic4pic.ginger.entities.MatchedCandidateResponse;
import net.pic4pic.ginger.entities.ObjectType;
import net.pic4pic.ginger.entities.StartingPic4PicRequest;
import net.pic4pic.ginger.entities.UserResponse;
import net.pic4pic.ginger.service.Service;
import net.pic4pic.ginger.tasks.AcceptPic4PicTask;
import net.pic4pic.ginger.tasks.AcceptPic4PicTask.AcceptPic4PicListener;
import net.pic4pic.ginger.tasks.ITask;
import net.pic4pic.ginger.tasks.ImageDownloadTask;
import net.pic4pic.ginger.tasks.NonBlockedTask;
import net.pic4pic.ginger.tasks.RequestPic4PicTask;
import net.pic4pic.ginger.tasks.RequestPic4PicTask.RequestPic4PicListener;
import net.pic4pic.ginger.utils.GingerHelpers;
import net.pic4pic.ginger.utils.ImageClickListener;
import net.pic4pic.ginger.utils.ImageGalleryView;
import net.pic4pic.ginger.utils.MyLog;

public class PersonActivity extends Activity implements AcceptPic4PicListener, RequestPic4PicListener {

	public enum ForwardAction implements IntegerEnum {
		None(0),
		ShowMessages(1);
		
		private final int value;
		
		private ForwardAction(int value) {
			this.value = value;
		}

		public int getIntValue() {
			return this.value;
		}
	}
	
	public static final String PersonType = "net.pic4pic.ginger.Person";
	public static final String ForwardActionType = "net.pic4pic.ginger.ForwardAction";
	public static final int PersonActivityCode = 201;
	
	private UserResponse me;
	private MatchedCandidate person;
	private Date lastDetailsRetrieveTime = null;
	
	private ImageGalleryView galleryView;
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		
		MyLog.bag().v("onCreate");
		super.onCreate(savedInstanceState);
		
		if(this.recreatePropertiesFromSavedBundle(savedInstanceState)){
			MyLog.bag().i("At least one property is restored successfully");
		}
		
		setContentView(R.layout.activity_person);
		
		Intent intent = getIntent();
		UserResponse meX = (UserResponse)intent.getSerializableExtra(MainActivity.AuthenticatedUserBundleType);
		if(meX != null){
			// we only set it if the data is valid
			this.me = meX;
		}
		else {
			if(this.me == null){
				MyLog.bag().e("'me' couldn't be retrieved from intent in onCreate(). It is null.");
			}
			else{
				MyLog.bag().i("'me' couldn't be retrieved from intent in onCreate()");
			}			
		}
		
		MatchedCandidate personX = (MatchedCandidate)intent.getSerializableExtra(PersonType);
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
		this.setTitle("match");
		
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
				PersonActivity.this.openMessageThread(true);
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
		
		int forwardAction = intent.getIntExtra(ForwardActionType, 0);
		if(ForwardAction.ShowMessages.getIntValue() == forwardAction){
			MyLog.bag().v("Launching conversation thread");
			this.openMessageThread(false);
		}
	}
	
	@Override
	public void onSaveInstanceState(Bundle outState) {
		
		super.onSaveInstanceState(outState);

		if(outState == null){
			return;
		}

		MyLog.bag().v("onSaveInstanceState");
		
		if(this.me != null){
			outState.putSerializable("me", this.me);
		}
		
		if(this.person != null){
			outState.putSerializable("person", this.person);
		}
		
		if(this.lastDetailsRetrieveTime != null){
			outState.putSerializable("lastDetailsRetrieveTime", this.lastDetailsRetrieveTime);
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
		
		if(state.containsKey("me")){
			this.me = (UserResponse)state.getSerializable("me");
			restored = true;
		}
		
		if(state.containsKey("person")){
			this.person = (MatchedCandidate)state.getSerializable("person");
			restored = true;
		}
		
		if(state.containsKey("lastDetailsRetrieveTime")){
			this.lastDetailsRetrieveTime = (Date)state.getSerializable("lastDetailsRetrieveTime");
			restored = true;
		}
		
		return restored;
	}
	
	public boolean isNeedOfRequestingMoreDetails(){
		
		if(this.lastDetailsRetrieveTime == null){
			
			MyLog.bag().v("Last details retrieve time is null. We need to request the details again");
			return true;
		}
		
		Date now = new Date();
		long diffAsMilliSeconds = now.getTime() - this.lastDetailsRetrieveTime.getTime();
		long diffAsSeconds = diffAsMilliSeconds / 60000;
		if(diffAsSeconds > 60){
			MyLog.bag().v("Last history retrieve time was 60 seconds ago. We better request history again");
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
						
						MyLog.bag().i("CandidateDetails retrieved");
						
						// Only the original thread that created a view hierarchy can touch its views.
						runOnUiThread(new Runnable() {
						     @Override
						     public void run() {
						    	 onCandidateDetailsSuccessfullyRetrieved(response);
						    }
						});
					}
					else {
						MyLog.bag().e("CandidateDetails request failed: " + response.getErrorMessage());
					}
				}
				catch(GingerException e) {
					
					MyLog.bag().add(e).e("CandidateDetails request failed");
				}
			}
		});
	}
	
	protected void onCandidateDetailsSuccessfullyRetrieved(CandidateDetailsResponse response){
		
		// no error if we are here
		MyLog.bag().v("More details on candidate has been retrieved.");
		
		this.lastDetailsRetrieveTime = new Date();
		
		Familiarity fam1 = this.person.getCandidateProfile().getFamiliarity();
		Familiarity fam2 = response.getCandidate().getCandidateProfile().getFamiliarity();			
		
		UUID pending1 = this.person.getLastPendingPic4PicId();
		UUID pending2 = response.getCandidate().getLastPendingPic4PicId();
		
		this.person = response.getCandidate();
		
		if(fam1.getIntValue() != fam2.getIntValue()){			
			MyLog.bag().i("Familiarity has changed.");
			this.adjustAll(false);
		}
		else if(!pending1.equals(pending2)){			
			MyLog.bag().i("Pending pic4pic ID has changed.");
			this.adjustActionButtons(false);
		}
		else{
			MyLog.bag().v("Familiarity or Pending pic4pic IDs haven't changed.");
		}
	}
		
	private void adjustAll(boolean initialCreate){
		MyLog.bag().v("Adjusting all...");
		this.adjustAvatarImage(initialCreate);		
		this.adjustMainImage(initialCreate);		
		this.adjustActionButtons(initialCreate);		
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
		
		MyLog.bag().v("Adjusting action buttons...");
		
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
			
			Button likeButton = (Button)this.findViewById(R.id.candidateLikeButton);
			if(this.person.isLiked()) {
				String newText = this.getString(R.string.candidate_liked);					
				likeButton.setText(newText);
			}
			else{
				String newText = this.getString(R.string.candidate_like);					
				likeButton.setText(newText);
			}
		}
		
		MyLog.bag().v("Last Pending p4p ID: " + this.person.getLastPendingPic4PicId());
		if(this.person.hasPic4PicPending()){
			pic4picButton.setText(acceptText);
		}
	}
	
	private Button getPic4PicButton(){
		LinearLayout buttonGroupFamiliar = (LinearLayout)this.findViewById(R.id.candidateActions);
		if(buttonGroupFamiliar.getVisibility() == View.VISIBLE){
			return (Button)buttonGroupFamiliar.getChildAt(1);
		}
		
		LinearLayout buttonGroupStranger = (LinearLayout)this.findViewById(R.id.candidateAnonymousActions);
		if(buttonGroupStranger.getVisibility() == View.VISIBLE){
			return (Button)buttonGroupStranger.getChildAt(1);
		}
		
		return null;
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
	
	private void removeWatermark(){
		
		MyLog.bag().v("Adjusting water mark (remove)...");
		
		FrameLayout imageContainer = (FrameLayout)this.findViewById(R.id.candidateMainPhotoContainer);
		if(imageContainer == null){
			MyLog.bag().e("The view for MainPhotoContainer is null in removeWatermark");
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
		
		MyLog.bag().v("Setting result intent");
		
		Intent resultIntent = new Intent();
		resultIntent.putExtra(MainActivity.UpdatedMatchCandidate, this.person);
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
			return;
		}

		AcceptingPic4PicRequest request = new AcceptingPic4PicRequest();
		request.setPic4PicRequestId(pic4picId);
		request.setPictureIdToExchange(this.me.getProfilePictures().getFullSizeClear().getGroupingId());
		AcceptPic4PicTask task = new AcceptPic4PicTask(this, this, button, request);
		task.execute();
	}
	
	public void onPic4PicAccepted(MatchedCandidateResponse response, AcceptingPic4PicRequest request){
		
		if(response.getErrorCode() == 0){	
			
			MyLog.bag().v("Accepting pic4pic call has returned.");
			
			MatchedCandidate candidate = response.getData();
			
			Familiarity fam1 = this.person.getCandidateProfile().getFamiliarity();
			Familiarity fam2 = candidate.getCandidateProfile().getFamiliarity();			
			
			UUID pending1 = this.person.getLastPendingPic4PicId();
			UUID pending2 = candidate.getLastPendingPic4PicId();
			
			// set this first
			this.person = candidate;
			
			if(fam1.getIntValue() != fam2.getIntValue()){			
				MyLog.bag().i("Familiarity has changed after accepting pic4pic.");
				this.adjustAll(false);
			}
			else {
				MyLog.bag().i("Familiarity has not changed after accepting pic4pic. Adjusting image gallery.");
				this.adjustPhotoGallery(false);
			
				if(!pending1.equals(pending2)){			
					MyLog.bag().i("Pending pic4pic ID has changed after accepting pic4pic.");
					this.adjustActionButtons(false);
				}
			}
			
			GingerHelpers.toastShort(this, "Accepted successfully \u2713");
		}
		else{
			GingerHelpers.showErrorMessage(this, response.getErrorMessage());
		}
	}
	
	private void sendPic4PicRequest(final Button button){
		
		final UUID pic4picId = this.person.getLastPendingPic4PicId();
		if(pic4picId != null && !pic4picId.equals(new UUID(0,0))){
			GingerHelpers.showErrorMessage(this, "It seems like you have received a pic4pic request from this person already.");
			this.adjustActionButtons(false);
			return;
		}	
				
		String moreText = PersonActivity.this.getString(R.string.candidate_sendMoreP4P);
		if(button.getText().equals(moreText)){
			GingerHelpers.toast(PersonActivity.this, "Coming soon!..");
			return;
		}
		
		// prepare request
		final StartingPic4PicRequest request = new StartingPic4PicRequest();
		request.setUserIdToInteract(this.person.getUserId());
		// do not define this at the first time
		// request.setPictureIdToExchange(this.person.getProfilePics().getFullSize().getGroupingId());
		
		RequestPic4PicTask task = new RequestPic4PicTask(this, this, button, request);
		task.execute();
	}
	
	public void onPic4PicRequestSent(MatchedCandidateResponse response, StartingPic4PicRequest request){
		
		// check result
		if(response.getErrorCode() == 0){
			
			// log
			MyLog.bag().v("pic4pic request has been sent to: " + request.getUserIdToInteract());
		
			MatchedCandidate candidate = response.getData();
			
			Familiarity fam1 = this.person.getCandidateProfile().getFamiliarity();
			Familiarity fam2 = candidate.getCandidateProfile().getFamiliarity();			
			
			UUID pending1 = this.person.getLastPendingPic4PicId();
			UUID pending2 = candidate.getLastPendingPic4PicId();
			
			// set this first
			this.person = candidate;
			
			if(fam1.getIntValue() != fam2.getIntValue()){			
				MyLog.bag().i("Familiarity has changed after sending pic4pic.");
				this.adjustAll(false);
			}
			else {
				MyLog.bag().i("Familiarity has not changed after sending pic4pic. Adjusting image gallery.");
				this.adjustPhotoGallery(false);
			
				if(!pending1.equals(pending2)){			
					MyLog.bag().i("Pending pic4pic ID has changed after sending pic4pic.");
					this.adjustActionButtons(false);
				}
			}
						
			// change text
			Button button = getPic4PicButton();
			String temp = PersonActivity.this.getString(R.string.candidate_sendP4P);
			if(button.getText().equals(temp)){
				String newText = PersonActivity.this.getString(R.string.candidate_sendMoreP4P);					
				button.setText(newText);
			}
			
			// toast
			GingerHelpers.toastShort(PersonActivity.this, "Sent successfully \u2713");
		}
		else{
			// log error
			GingerHelpers.showErrorMessage(this, response.getErrorMessage());
			MyLog.bag().e("Requesting pic4pic from candidate(" + request.getUserIdToInteract() + ") failed: " + response.getErrorMessage());
		}
	}
	
	private void openMessageThread(boolean startTyping){
		
		Intent intent = new Intent(this, ConversationActivity.class);
		intent.putExtra(MainActivity.AuthenticatedUserBundleType, this.me);
		intent.putExtra(PersonActivity.PersonType, this.person);
		
		if(startTyping){
			intent.putExtra(ConversationActivity.ConversationModeType, ConversationActivity.ConversationMode.StartTyping.getIntValue());
		}
		else{
			intent.putExtra(ConversationActivity.ConversationModeType, ConversationActivity.ConversationMode.ReadFirst.getIntValue());
		}
		
		// calling a child activity for a result keeps the parent activity alive.
		// by that way, we don't have to keep track of active tab when child activity is closed.
		this.startActivityForResult(intent, ConversationActivity.ConversationActivityCode);
	}
	
	@Override
	public void onActivityResult(int requestCode, int resultCode, Intent data) {
		
		MyLog.bag().v("onActivityResult");
		
		if(requestCode == ConversationActivity.ConversationActivityCode){
			MyLog.bag().v("ConversationActivity has returned");			
			if(resultCode == Activity.RESULT_OK && data != null){				
				Bundle bundle = data.getExtras();
				UserResponse meX = (UserResponse)bundle.getSerializable(MainActivity.AuthenticatedUserBundleType);
				if(meX != null){
					this.me = meX;
				}
				
				MatchedCandidate personX = (MatchedCandidate)bundle.getSerializable(PersonActivity.PersonType);
				if(personX != null){
					this.person = personX;
				}
			}
		}
		else{
			MyLog.bag().v("Unknown Activity  has been received by PersonActivity: " + requestCode);
		}
	}
	
	private void sendLikeAction(final Button candidateLikeButton){
	
		if(this.person.isLiked()){
			GingerHelpers.showErrorMessage(this, "You have already liked this person recently");
			return;	
		}
		
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
						MyLog.bag().v("Candidate has been marked as LIKED: " + candidateId);
					
						// we have viewed this profile if we have liked it
						person.setLastViewTimeUTC(new Date());
						person.setLastLikeTimeUTC(new Date());
						
						// change the button text
						PersonActivity.this.runOnUiThread(new Runnable(){
							@Override
							public void run() {
								
								// change text
								String newText = PersonActivity.this.getString(R.string.candidate_liked);					
								candidateLikeButton.setText(newText);								
								candidateLikeButton.setEnabled(true);								
								// toast
								GingerHelpers.toastShort(PersonActivity.this, "Liked successfully \u2713");
							}
						});
					}
					else{
						// log error
						MyLog.bag().e("Liking candidate(" + candidateId + ") failed: " + response.getErrorMessage());
					}
				}
				catch(GingerException ge){
					
					// log error
					MyLog.bag().e("Liking candidate(" + candidateId + ") failed: " + ge.getMessage());
				}
				catch(Exception e){
					
					// log error
					MyLog.bag().e("Liking candidate(" + candidateId + ") failed: " + e.toString());
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
			
			MyLog.bag().v("Marking as viewed: " + candidateId);
			
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
							MyLog.bag().v("Candidate has been marked as viewed: " + candidateId);
						}
						else{
							
							// log error
							MyLog.bag().e("Marking candidate as viewed failed: " + response.getErrorMessage());
						}
					}
					catch(GingerException ge){
						
						// log error
						MyLog.bag().add(ge).e("Marking candidate as viewed failed");
					}
					catch(Exception e){
						
						// log error
						MyLog.bag().add(e).e("Marking candidate as viewed failed");
					}
				}
			});
		}
	}
}
