package net.pic4pic.ginger;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import net.pic4pic.ginger.entities.BaseResponse;
import net.pic4pic.ginger.entities.PicturePair;
import net.pic4pic.ginger.entities.SimpleRequest;
import net.pic4pic.ginger.entities.UserResponse;
import net.pic4pic.ginger.tasks.ImageDownloadTask;
import net.pic4pic.ginger.tasks.UpdateUserDetailsTask;
import net.pic4pic.ginger.tasks.UpdateUserDetailsTask.UserDetailsUpdateListener;
import net.pic4pic.ginger.utils.ImageClickListener;
import net.pic4pic.ginger.utils.ImageGalleryView;
import net.pic4pic.ginger.utils.MyLog;
import net.pic4pic.ginger.utils.TextInputDialog;

public class ProfileFragment extends Fragment implements TextInputDialog.TextInputListener, UserDetailsUpdateListener{

	private static final String defaultDescr = "tell something about yourself here (tap to edit)";
	
	private TextView meDescriptionView;
	private ImageGalleryView gallery;
	
	public ProfileFragment(){		
	}
	
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
		
		final View rootView = inflater.inflate(R.layout.me_profile, container, false);		
		
		ImageView meAvatarView = (ImageView) rootView.findViewById(R.id.meAvatar);
		TextView meUsernameView = (TextView) rootView.findViewById(R.id.meUsername);
		TextView meShortBioView = (TextView) rootView.findViewById(R.id.meShortBio);
		this.meDescriptionView = (TextView) rootView.findViewById(R.id.meDescription);
		ImageView meMainPhotoView = (ImageView) rootView.findViewById(R.id.meMainPhoto);
		
		final UserResponse me = this.getMe();
		meUsernameView.setText(me.getUserProfile().getDisplayName());
		meShortBioView.setText(me.getUserProfile().getShortBio());
		
		String descr = me.getUserProfile().getDescription();
		if(descr == null || descr.trim().length() == 0){
			descr = defaultDescr;
		}
		
		this.meDescriptionView.setText(descr);
		
		this.meDescriptionView.setOnClickListener(new OnClickListener(){
			
			@Override
			public void onClick(View v) {
				/*
				Activity activity = ProfileFragment.this.getActivity();
				String description = ProfileFragment.this.me.getDescription();
				TextInputDialog dlg = new TextInputDialog(ProfileFragment.this, activity, description, "Enter Input", 3000);
				dlg.show();
				*/
								
				Intent intent = new Intent(ProfileFragment.this.getActivity(), TextInputActivity.class);
				intent.putExtra(TextInputActivity.TextInputType, me.getUserProfile().getDescription());

				// calling a child activity for a result keeps the parent activity alive.
				// by that way, we don't have to keep track of active tab when child activity is closed.

				// if you call this from the Fragment, the result will be delivered to the fragment.
				ProfileFragment.this.getActivity().startActivityForResult(intent, TextInputActivity.TextInputCode);
			}});
		
		// set the default image for thumb-nail
		/*
		if(this.getMe().getUserProfile().getGender().getIntValue() == Gender.Male.getIntValue()){
			meAvatarView.setImageResource(R.drawable.man_downloading_small);
		}
		else if(this.getMe().getUserProfile().getGender().getIntValue() == Gender.Female.getIntValue()){
			meAvatarView.setImageResource(R.drawable.woman_downloading_small);
		}
		else{
			meAvatarView.setImageResource(android.R.drawable.ic_menu_gallery);
		}
		*/
		meAvatarView.setImageResource(R.drawable.downloading_small);
		
		// set the real image with an asynchronous download operation
		ImageDownloadTask downloadTask = new ImageDownloadTask(me.getProfilePictures().getThumbnailBlurred().getId(), meAvatarView);
		downloadTask.execute(me.getProfilePictures().getThumbnailBlurred().getCloudUrl());
		
		// set the default image for mainPhoto		
		/*
		if(this.getMe().getUserProfile().getGender().getIntValue() == Gender.Male.getIntValue()){
			meMainPhotoView.setImageResource(R.drawable.man_downloading_big);
		}
		else if(this.getMe().getUserProfile().getGender().getIntValue() == Gender.Female.getIntValue()){
			meMainPhotoView.setImageResource(R.drawable.woman_downloading_big);
		}
		else{
			meMainPhotoView.setImageResource(android.R.drawable.ic_menu_gallery);
		}
		*/
		meMainPhotoView.setImageResource(R.drawable.downloading_big);
				
		meMainPhotoView.setOnClickListener(new ImageClickListener(this.getActivity(), meMainPhotoView));
		
		// set the real image with an asynchronous download operation
		ImageDownloadTask downloadTask2 = new ImageDownloadTask(me.getProfilePictures().getFullSizeClear().getId(), meMainPhotoView);
		downloadTask2.execute(me.getProfilePictures().getFullSizeClear().getCloudUrl());
		
		LinearLayout photoGalleryParent = (LinearLayout)rootView.findViewById(R.id.thumbnailPlaceholder);
		ImageGalleryView.Margin margin = new ImageGalleryView.Margin(6);   
		this.gallery = new ImageGalleryView(
				this.getActivity(), 
				photoGalleryParent, 
				me.getOtherPictures(), 
				true, 
				margin, 
				6, 
				true,
				MainActivity.CaptureCameraCode,
				MainActivity.PickFromGalleryCode);
		
		gallery.fillPhotos();
		
		return rootView;
	}	
	
	public void onNewImageAdded(PicturePair pair){
		
		if(this.gallery != null){
			this.gallery.onNewImageAdded(pair);
		}
	}
	
	@Override
	public void onNewText(String text) {
		this.updateDescription(text);
	}
	
	public void updateDescription(String descr){
		
		UserResponse me = this.getMe();
		
		if(me != null && this.meDescriptionView != null){
			
			if (descr == null || descr.trim().length() == 0 || descr.trim().equalsIgnoreCase(defaultDescr)){
				this.meDescriptionView.setText(defaultDescr);
			}
			else{
				this.meDescriptionView.setText(descr);
			}
			
			descr = ((descr == null) ? "" : descr.trim());
			if(!descr.equals(me.getUserProfile().getDescription())){
				SimpleRequest<String> request = new SimpleRequest<String>();
				request.setData(descr);
				UpdateUserDetailsTask task = new UpdateUserDetailsTask(this.getActivity(), this, request);
				task.execute();
			}
		}
	}
	
	public void onUserDetailsUpdated(BaseResponse response, SimpleRequest<String> request){
		if(response.getErrorCode() == 0){
			UserResponse me = this.getMe();
			if(me != null){
				me.getUserProfile().setDescription(request.getData());
			}
		}
	}
	
	private UserResponse getMe(){		
		MainActivity parent = (MainActivity)this.getActivity();
		return parent.getCurrentUser();
	}
	
	@Override
	public void onActivityResult(int requestCode, int resultCode, Intent data) {
		/*
		if (requestCode == MainActivity.CaptureCameraCode) {
			Log.v("ActivityResult", "CaptureCameraActivity has returned");
			this.processCameraActivityResult(resultCode, data);			
	    }
		else if (requestCode == MainActivity.PickFromGalleryCode) {
			Log.v("ActivityResult", "PickFromGalleryActivity has returned");
			this.processGalleryActivityResult(resultCode, data);
		}
		*/ 
		if (requestCode == TextInputActivity.TextInputCode) {
			MyLog.bag().v("TextInputActivity has returned");
			this.processTextInputActivityResult(resultCode, data);
		}
		else{
			MyLog.bag().v("Unknown Activity has returned: " + requestCode);
		}
	}
	
	public void processTextInputActivityResult(int resultCode, Intent data){
		if(resultCode == Activity.RESULT_OK && data != null){
			String descr = data.getExtras().getString(TextInputActivity.TextInputType, "");
			this.updateDescription(descr);
		}
	}
}
