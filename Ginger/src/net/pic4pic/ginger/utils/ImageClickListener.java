package net.pic4pic.ginger.utils;

import net.pic4pic.ginger.R;
import net.pic4pic.ginger.entities.ImageFile;
import net.pic4pic.ginger.tasks.ImageDownloadTask;
import android.app.Activity;
import android.app.Dialog;
import android.graphics.Bitmap;
import android.graphics.drawable.BitmapDrawable;
import android.view.View;
import android.view.Window;
import android.view.View.OnClickListener;
import android.view.ViewGroup.LayoutParams;
import android.widget.ImageView;

// OnLongClickListener
public class ImageClickListener implements OnClickListener {

	private Activity activity;
	private ImageView original;
	private ImageFile bigImage;
	
	public ImageClickListener(Activity activity, ImageView original){
		this(activity, original, null);
	}
	
	public ImageClickListener(Activity activity, ImageView original, ImageFile bigImage){
		this.activity = activity;
		this.original = original;
		this.bigImage = bigImage;
	}
	
	public ImageClickListener(Activity activity, int imageViewId){
		this(activity, imageViewId, null);
	}
	
	public ImageClickListener(Activity activity, int imageViewId, ImageFile bigImage){
		this.activity = activity;
		this.original = (ImageView)this.activity.findViewById(imageViewId);
		this.bigImage = bigImage;
	}
	
	@Override
	// onLongClick
	public void onClick(View v) {

		// passing a dialog-less theme (second parameter) into the dialog constructor makes it full-screen
    	final Dialog dialog = new Dialog(this.activity, android.R.style.Theme_Light_NoTitleBar_Fullscreen);
    	dialog.requestWindowFeature(Window.FEATURE_NO_TITLE); 
    	dialog.setContentView(R.layout.image_full);
    	dialog.getWindow().getAttributes().width = LayoutParams.MATCH_PARENT;    	
    	ImageView imageView = (ImageView) dialog.findViewById(R.id.fullImage);
    	
    	if(this.bigImage == null){
    		Bitmap bitmap = ((BitmapDrawable)this.original.getDrawable()).getBitmap();
    		imageView.setImageBitmap(bitmap);
    	}
    	else{
    		ImageDownloadTask mainPhotoDownloadTask = new ImageDownloadTask(this.bigImage.getId(), imageView, true);
			mainPhotoDownloadTask.execute(this.bigImage.getCloudUrl());	
    	}
    	
    	imageView.setOnClickListener(new OnClickListener(){
    		@Override
            public void onClick(View v) {
    			dialog.dismiss();
    		}
    	});
    	
    	dialog.show();	
	}
}
