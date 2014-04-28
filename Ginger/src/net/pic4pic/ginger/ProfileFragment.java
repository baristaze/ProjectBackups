package net.pic4pic.ginger;

import java.util.Random;

import android.app.Activity;
import android.content.Intent;
import android.database.Cursor;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.net.Uri;
import android.os.Bundle;
import android.provider.MediaStore;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;
import android.widget.Toast;

import net.pic4pic.ginger.entities.ImageFile;
import net.pic4pic.ginger.entities.PicturePair;
import net.pic4pic.ginger.entities.UserResponse;
import net.pic4pic.ginger.tasks.ImageDownloadTask;
import net.pic4pic.ginger.utils.ImageClickListener;
import net.pic4pic.ginger.utils.ImageGalleryView;
import net.pic4pic.ginger.utils.MyLog;
import net.pic4pic.ginger.utils.TextInputDialog;

public class ProfileFragment extends Fragment implements TextInputDialog.TextInputListener {

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
		meUsernameView.setText(me.getUserProfile().getUsername());
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
		meAvatarView.setImageResource(android.R.drawable.ic_menu_gallery);
		
		// set the real image with an asynchronous download operation
		ImageDownloadTask downloadTask = new ImageDownloadTask(meAvatarView);
		downloadTask.execute(me.getProfilePictures().getThumbnailBlurred().getCloudUrl());
		
		// set the default image for mainPhoto
		meMainPhotoView.setImageResource(android.R.drawable.ic_menu_gallery);		
		meMainPhotoView.setOnClickListener(new ImageClickListener(this.getActivity(), meMainPhotoView));
		
		// set the real image with an asynchronous download operation
		ImageDownloadTask downloadTask2 = new ImageDownloadTask(meMainPhotoView);
		downloadTask2.execute(me.getProfilePictures().getFullSizeClear().getCloudUrl());
		
		LinearLayout photoGalleryParent = (LinearLayout)rootView.findViewById(R.id.thumbnailPlaceholder);
		ImageGalleryView.Margin margin = new ImageGalleryView.Margin(6);   
		this.gallery = new ImageGalleryView(
				this.getActivity(), photoGalleryParent, me.getOtherPictures(), true, margin, 6, true);
		
		gallery.fillPhotos();
		
		return rootView;
	}	
	
	public void addNewImage(PicturePair image, Bitmap bitmap){
		
		UserResponse me = this.getMe();
		if(me != null){
			me.getOtherPictures().add(image);
			if(this.gallery != null){
				this.gallery.addNewImage(bitmap, image.getFullSize().getCloudUrl());
			}
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
				me.getUserProfile().setDescription("");
				this.meDescriptionView.setText(defaultDescr);
			}
			else{
				me.getUserProfile().setDescription(descr);
				this.meDescriptionView.setText(descr);
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
			MyLog.v("ActivityResult", "TextInputActivity has returned");
			this.processTextInputActivityResult(resultCode, data);
		}
		else{
			MyLog.v("ActivityResult", "Unknown Activity has returned: " + requestCode);
		}
	}
	
	public void processTextInputActivityResult(int resultCode, Intent data){
		if(resultCode == Activity.RESULT_OK && data != null){
			String descr = data.getExtras().getString(TextInputActivity.TextInputType, "");
			this.updateDescription(descr);
		}
	}
	
	public void processCameraActivityResult(int resultCode, Intent data){
		
		if(resultCode == Activity.RESULT_OK && data != null){
			
			MyLog.v("Camera", "Camera result seems OK");				
			
			Bitmap bitmapPhoto = (Bitmap) data.getExtras().get("data");
			MyLog.v("Camera", "Photo width: " + bitmapPhoto.getWidth());
			MyLog.v("Camera", "Photo height: " + bitmapPhoto.getHeight());
			
			String uri = "http://tvmedia.ign.com/tv/image/article/805/805797/bionic-woman-2007-20070717053021720.jpg";
			if((new Random()).nextInt(2) % 2 == 0){
				uri = "http://tvreviews.files.wordpress.com/2007/10/michelle-ryan-bionic-woman.jpg";
			}
			
			ImageFile imgFile = new ImageFile();
			imgFile.setCloudUrl(uri);
			imgFile.setThumbnailed(true);
			
			PicturePair info = new PicturePair();
			info.setThumbnail(imgFile);
			
			this.addNewImage(info, bitmapPhoto);
			
	        Toast toast = Toast.makeText(this.getActivity(), "Camera is a success", Toast.LENGTH_LONG);
			toast.show();
		}
		else{
			MyLog.e("Camera", "Camera result is not ok");
			
			Toast toast = Toast.makeText(this.getActivity(), "Capturing photo is unsuccessfull", Toast.LENGTH_LONG);
			toast.show();
		}
	}
	
	public void processGalleryActivityResult(int resultCode, Intent data){
		boolean success = false;
		String errorMessage = "Picking photo from gallery is unsuccessfull";		
		if(resultCode == Activity.RESULT_OK && data != null){
			Uri selectedImageUri = data.getData();
			if(selectedImageUri != null){					
				MyLog.v("Gallery", "Gallery result seems OK");
				MyLog.v("Gallery", "Uri: " + selectedImageUri.toString());
                
				String selectedImagePath = this.getImagePath(selectedImageUri);
				if(selectedImagePath == null){
					errorMessage = "Selected image path couldn't be retrieved!";
				}
				else
				{
					MyLog.v("Gallery", "Path: " + selectedImagePath);
					
					if(selectedImagePath.startsWith("http")){
						errorMessage = "Selected image is not on this phone!";
					}
					else{
						Bitmap bitmapPhoto = null;						
						try{					
							bitmapPhoto = BitmapFactory.decodeFile(selectedImagePath);
						}
						catch(Exception ex){
							errorMessage = "Selected image couldn't be decoded!";
							MyLog.e("Gallery", ex.toString());
						}
						
						if(bitmapPhoto != null){
							
							MyLog.v("Gallery", "Photo width: " + bitmapPhoto.getWidth());
							MyLog.v("Gallery", "Photo height: " + bitmapPhoto.getHeight());
							
							String uri = "http://tvmedia.ign.com/tv/image/article/805/805797/bionic-woman-2007-20070717053021720.jpg";
							if((new Random()).nextInt(2) % 2 == 0){
								uri = "http://tvreviews.files.wordpress.com/2007/10/michelle-ryan-bionic-woman.jpg";
							}
							
							ImageFile imgFile = new ImageFile();
							imgFile.setCloudUrl(uri);
							imgFile.setThumbnailed(true);
							
							PicturePair info = new PicturePair();
							info.setThumbnail(imgFile);
							
							this.addNewImage(info, bitmapPhoto);
							
							success = true;
						}
					}
				}
			}
		}

		if(!success){
			MyLog.e("Gallery", "Gallery result is not ok");
			
			Toast toast = Toast.makeText(this.getActivity(), errorMessage, Toast.LENGTH_LONG);
			toast.show();
		}
	}
	
	private String getImagePath(Uri uri) {
		try{
	        String[] projection = { MediaStore.Images.Media.DATA };
	        Cursor cursor = this.getActivity().getContentResolver().query(uri, projection, null, null, null);
	        int column_index = cursor.getColumnIndexOrThrow(MediaStore.Images.Media.DATA);
	        cursor.moveToFirst();
	        return cursor.getString(column_index);
		}
		catch(Exception ex){
			MyLog.e("Gallery", ex.toString());
			return null;
		}
    }
}
