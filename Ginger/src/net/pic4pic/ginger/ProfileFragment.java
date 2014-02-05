package net.pic4pic.ginger;

import java.util.List;
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
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;
import android.widget.Toast;

import net.pic4pic.ginger.entities.Familiarity;
import net.pic4pic.ginger.entities.Gender;
import net.pic4pic.ginger.entities.ImageInfo;
import net.pic4pic.ginger.entities.Person;
import net.pic4pic.ginger.tasks.ImageDownloadTask;
import net.pic4pic.ginger.utils.ImageClickListener;
import net.pic4pic.ginger.utils.ImageGalleryView;
import net.pic4pic.ginger.utils.TextInputDialog;

public class ProfileFragment extends Fragment implements TextInputDialog.TextInputListener {

	private static final String defaultDescr = "tell something about yourself here (tap to edit)";
	
	private Person me;
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
		
		if(this.me == null){
			this.me = this.getMe();
		}
		
		meUsernameView.setText(this.me.getUsername());
		meShortBioView.setText(this.me.getShortBio());
		
		String descr = this.me.getDescription();
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
				intent.putExtra(TextInputActivity.TextInputType, ProfileFragment.this.me.getDescription());

				// calling a child activity for a result keeps the parent activity alive.
				// by that way, we don't have to keep track of active tab when child activity is closed.

				// if you call this from the Fragment, the result will be delivered to the fragment.
				ProfileFragment.this.getActivity().startActivityForResult(intent, TextInputActivity.TextInputCode);
			}});
		
		// set the default image for avatar
		meAvatarView.setImageResource(android.R.drawable.ic_menu_gallery);
		
		// set the real image with an asynchronous download operation
		ImageDownloadTask downloadTask = new ImageDownloadTask(meAvatarView);
		downloadTask.execute(this.me.getAvatarUri());
		
		// set the default image for mainPhoto
		meMainPhotoView.setImageResource(android.R.drawable.ic_menu_gallery);		
		meMainPhotoView.setOnClickListener(new ImageClickListener(this.getActivity(), meMainPhotoView));
		
		// set the real image with an asynchronous download operation
		ImageDownloadTask downloadTask2 = new ImageDownloadTask(meMainPhotoView);
		downloadTask2.execute(this.me.getMainPhoto());
		
		LinearLayout photoGalleryParent = (LinearLayout)rootView.findViewById(R.id.thumbnailPlaceholder);
		ImageGalleryView.Margin margin = new ImageGalleryView.Margin(6);   
		this.gallery = new ImageGalleryView(
				this.getActivity(), photoGalleryParent, this.me.getOtherPhotos(), true, margin, 6, true);
		
		gallery.fillPhotos();
		
		return rootView;
	}	
	
	public void addNewImage(ImageInfo image, Bitmap bitmap){
		if(this.me != null){
			this.me.getOtherPhotos().add(image);
			if(this.gallery != null){
				this.gallery.addNewImage(bitmap, image.getOriginal());
			}
		}
	}
	
	@Override
	public void onNewText(String text) {
		this.updateDescription(text);
	}
	
	public void updateDescription(String descr){
		if(this.me != null && this.meDescriptionView != null){			
			if (descr == null || descr.trim().length() == 0 || descr.trim().equalsIgnoreCase(defaultDescr)){
				this.me.setDescription("");
				this.meDescriptionView.setText(defaultDescr);
			}
			else{
				this.me.setDescription(descr);
				this.meDescriptionView.setText(descr);
			}
		}
	}
	
	private Person getMe(){
		
		String s = "Persius officiis eloquentiam ut sed,ius nostrud sensibus ea. Eu ullum inani posidonium quo, zzril quaestio intellegat in quo. Persius officiis eloquentiam ut sed,ius nostrud sensibus ea.";
		s += " " + s;
		
		Person p = new Person();
		p.setUsername("CuriousGeorge79");
		p.setAvatarUri("http://www.prosportstickers.com/product_images/h/curious_george_decal_head__26524.jpg");
		p.setShortBio("34 / M / Married / Redmond / Software Developer");
		p.setDescription(s);
		p.setFamiliarity(Familiarity.Familiar);
		p.setGender(Gender.Male);
		p.setMainPhoto("http://4.bp.blogspot.com/_dO5wi4i0JDs/TSYVpF2xp6I/AAAAAAAAAns/Khd0ETSNNvA/s1600/ad.jpg");
		
		String commonUrl = "http://tvmedia.ign.com/tv/image/article/805/805797/bionic-woman-2007-20070717053021720.jpg"; 
 	    String commonUrl2 = "http://tvreviews.files.wordpress.com/2007/10/michelle-ryan-bionic-woman.jpg";
 	     	    
 	    List<ImageInfo> photos = p.getOtherPhotos();
    	for(int y=0; y<11; y++){
    		ImageInfo imgInfo = new ImageInfo();
 	    	imgInfo.setThumbnail((y%2 == 0) ? commonUrl : commonUrl2);
 	    	photos.add(imgInfo);	
    	}
    	 	    
    	return p;
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
			Log.v("ActivityResult", "TextInputActivity has returned");
			this.processTextInputActivityResult(resultCode, data);
		}
		else{
			Log.v("ActivityResult", "Unknown Activity has returned: " + requestCode);
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
			
			Log.v("Camera", "Camera result seems OK");				
			
			Bitmap bitmapPhoto = (Bitmap) data.getExtras().get("data");
			Log.v("Camera", "Photo width: " + bitmapPhoto.getWidth());
			Log.v("Camera", "Photo height: " + bitmapPhoto.getHeight());
			
			String uri = "http://tvmedia.ign.com/tv/image/article/805/805797/bionic-woman-2007-20070717053021720.jpg";
			if((new Random()).nextInt(2) % 2 == 0){
				uri = "http://tvreviews.files.wordpress.com/2007/10/michelle-ryan-bionic-woman.jpg";
			}
			
			ImageInfo info = new ImageInfo();
			info.setThumbnail(uri);
			this.addNewImage(info, bitmapPhoto);
			
	        Toast toast = Toast.makeText(this.getActivity(), "Camera is a success", Toast.LENGTH_LONG);
			toast.show();
		}
		else{
			Log.e("Camera", "Camera result is not ok");
			
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
				Log.v("Gallery", "Gallery result seems OK");
				Log.v("Gallery", "Uri: " + selectedImageUri.toString());
                
				String selectedImagePath = this.getImagePath(selectedImageUri);
				if(selectedImagePath == null){
					errorMessage = "Selected image path couldn't be retrieved!";
				}
				else
				{
					Log.v("Gallery", "Path: " + selectedImagePath);
					
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
							Log.e("Gallery", ex.toString());
						}
						
						if(bitmapPhoto != null){
							
							Log.v("Gallery", "Photo width: " + bitmapPhoto.getWidth());
							Log.v("Gallery", "Photo height: " + bitmapPhoto.getHeight());
							
							String uri = "http://tvmedia.ign.com/tv/image/article/805/805797/bionic-woman-2007-20070717053021720.jpg";
							if((new Random()).nextInt(2) % 2 == 0){
								uri = "http://tvreviews.files.wordpress.com/2007/10/michelle-ryan-bionic-woman.jpg";
							}
							
							ImageInfo info = new ImageInfo();
							info.setThumbnail(uri);
							this.addNewImage(info, bitmapPhoto);
							
							success = true;
						}
					}
				}
			}
		}

		if(!success){
			Log.e("Gallery", "Gallery result is not ok");
			
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
			Log.e("Gallery", ex.toString());
			return null;
		}
    }
}
