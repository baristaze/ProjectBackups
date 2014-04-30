package net.pic4pic.ginger.utils;

import java.util.List;

import android.app.Activity;
import android.content.Intent;
import android.content.res.Configuration;
import android.graphics.Bitmap;
import android.graphics.Point;
import android.view.Display;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup.LayoutParams;
import android.widget.ImageView;
import android.widget.LinearLayout;

import net.pic4pic.ginger.MainActivity;
import net.pic4pic.ginger.R;
import net.pic4pic.ginger.entities.ImageFile;
import net.pic4pic.ginger.entities.PicturePair;
import net.pic4pic.ginger.tasks.ImageDownloadTask;

public class ImageGalleryView {
	
	public static class Margin{
		
		private static final int DEFAULT = 6; // dp
		
		private int left;
		private int right;
		private int top;
		private int bottom;
		
		public Margin(){
			this(DEFAULT);
		}
		
		public Margin(int margin){
			this(margin, margin);
		}
		
		public Margin(int horizontal, int vertical){
			this(horizontal, horizontal, vertical, vertical);
		}
		
		public Margin(int left, int right, int top, int bottom){
			this.left = left;
			this.right = right;
			this.top = top;
			this.bottom = bottom;
		}
	}
	
	private static final int MAX_IMAGE_COUNT_PER_ROW_PORTRAIT = 4;
	private static final int MAX_IMAGE_COUNT_PER_ROW_LANDSCAPE = 6;
	
	private Activity activity;
	private LinearLayout parentView;
	private List<PicturePair> images;
	private boolean enableImageClick;
	private Margin marginOuter;
	private int gapBetweenImages;
	private boolean insertAddImgIcon;
	private int populatedImageCount;
	
	public ImageGalleryView(Activity activity, LinearLayout parentView, List<PicturePair> images){
		this(activity, parentView, images, true, null, 6, false);
	}
	
	public ImageGalleryView(
			Activity activity, 
			LinearLayout parentView, 
			List<PicturePair> images,
			boolean enableImageClick,
			Margin marginOuter, 
			int gapBetweenImages,
			boolean insertAddImgIcon){
		
		this.activity = activity;
		this.parentView = parentView;
		this.images = images;
		this.enableImageClick = enableImageClick;
		this.marginOuter = marginOuter;
		this.gapBetweenImages = gapBetweenImages;
		this.insertAddImgIcon = insertAddImgIcon;
		this.populatedImageCount = 0;
		
		if(this.marginOuter == null){
			this.marginOuter = new Margin(12, 12, 0, 6);
		}
	}
	
	protected int getMaxImageCountPerLine(){
		
		if(this.activity.getResources().getConfiguration().orientation == Configuration.ORIENTATION_PORTRAIT){
			// portrait
			return MAX_IMAGE_COUNT_PER_ROW_PORTRAIT;
		}
		else{
			// landscape
			return MAX_IMAGE_COUNT_PER_ROW_LANDSCAPE;
		}		
	}
	
	protected int getImageDimension(int stackFactor){
		Display display = this.activity.getWindowManager().getDefaultDisplay();
		Point size = new Point();
		display.getSize(size);
		
		MyLog.v("Screen Size", "Screen Size in DPI: " + size.x + " x " + size.y);
		
		int width = size.x - this.marginOuter.left - this.marginOuter.right;
		width -= this.gapBetweenImages * (stackFactor-1);
		
		if(this.activity.getResources().getConfiguration().orientation == Configuration.ORIENTATION_PORTRAIT){
			// portrait
			return Math.min(width, size.y) / stackFactor;
		}
		else{
			// landscape
			return Math.max(width, size.y) / stackFactor;
		}
	}
	
	protected int calculateImageCount(){
		return this.insertAddImgIcon ? (this.images.size() + 2) : this.images.size();   
	}
	
	protected boolean isRegularImage(int imageIndex){
		return imageIndex < this.images.size();
	}
	
	protected boolean isCameraImage(int imageIndex){
		return this.insertAddImgIcon && (imageIndex == this.images.size());
	}
	
	protected boolean isGalleryImage(int imageIndex){
		return this.insertAddImgIcon && (imageIndex == this.images.size() + 1);
	}
	
	protected int calculateStackFactor(){
		int min = this.insertAddImgIcon ? 3 : 2;  
		int stackFactor = Math.min(this.calculateImageCount(), this.getMaxImageCountPerLine());
		stackFactor = Math.max(min, stackFactor);
		return stackFactor; 
	}
	
	protected int calculateContainerCount(int stackFactor){
		return (this.calculateImageCount() + stackFactor - 1) / stackFactor;
	}
	
	protected LinearLayout createLayout(LayoutInflater inflater){
		LinearLayout container = (LinearLayout)inflater.inflate(R.layout.image_stack, null);			
		LayoutParams temp = new LayoutParams(LayoutParams.MATCH_PARENT, LayoutParams.WRAP_CONTENT);
		LinearLayout.LayoutParams outerLayout = new LinearLayout.LayoutParams(temp);
		outerLayout.leftMargin = this.marginOuter.left;
		outerLayout.rightMargin = this.marginOuter.right;
		outerLayout.topMargin = this.marginOuter.top;
		outerLayout.bottomMargin = this.marginOuter.bottom;		
		container.setLayoutParams(outerLayout);
		return container;
	}
	
	protected ImageView createImageView(LayoutInflater inflater, LinearLayout container, int dimension){
		ImageView imgView = (ImageView)inflater.inflate(R.layout.image_draft, null);				
		LinearLayout.LayoutParams imageLayout = new LinearLayout.LayoutParams(new LayoutParams(dimension, dimension));				
		if(this.populatedImageCount != this.calculateImageCount()-1){
			imageLayout.rightMargin = this.gapBetweenImages;
		}
		
		imgView.setLayoutParams(imageLayout);				
		container.addView(imgView);
		return imgView;
	}
	
	protected void assignImageToView(ImageView imgView){
		ImageFile imageToDownload = images.get(this.populatedImageCount).getThumbnail();
		imgView.setOnClickListener(new ImageClickListener(this.activity, imgView));
		ImageDownloadTask photoDownloadTask = new ImageDownloadTask(imageToDownload.getId(), imgView, this.enableImageClick);
		photoDownloadTask.execute(imageToDownload.getCloudUrl());	
	}
	
	protected void assignCameraIconToView(ImageView imgView){
		imgView.setImageResource(android.R.drawable.ic_menu_camera);
		imgView.setOnClickListener(new View.OnClickListener() {						
			@Override
			public void onClick(View v) {
				// start camera
				Intent cameraIntent = new Intent(android.provider.MediaStore.ACTION_IMAGE_CAPTURE);
	            ImageGalleryView.this.activity.startActivityForResult(
	            		cameraIntent, MainActivity.CaptureCameraCode);
	        }
		});
	}
	
	protected void assignGalleryIconToView(ImageView imgView){
		imgView.setImageResource(android.R.drawable.ic_menu_upload);
		imgView.setOnClickListener(new View.OnClickListener() {						
			@Override
			public void onClick(View v) {
				// in onCreate or any event where your want the user to
                // select a file
                Intent intent = new Intent();
                intent.setType("image/*");
                intent.setAction(Intent.ACTION_GET_CONTENT);
                ImageGalleryView.this.activity.startActivityForResult(
                		Intent.createChooser(intent, "Select Picture"), MainActivity.PickFromGalleryCode);
			}
		});
	}
	
	public void fillPhotos(){  
		
		this.populatedImageCount = 0;
		int stackFactor = this.calculateStackFactor();	
		int countOfContainers = this.calculateContainerCount(stackFactor);
		int dimension = this.getImageDimension(stackFactor);
		LayoutInflater inflater = this.activity.getLayoutInflater();
		
		for(int x=0; x<countOfContainers; x++){			
			
			LinearLayout container = this.createLayout(inflater);
			
			for(int y=0; y<stackFactor && this.populatedImageCount < this.calculateImageCount(); y++){				
				
				ImageView imgView = this.createImageView(inflater, container, dimension);
				
				if(this.isRegularImage(this.populatedImageCount)){
					this.assignImageToView(imgView);				
				}
				else if(this.isCameraImage(this.populatedImageCount)){
					this.assignCameraIconToView(imgView);
				}
				else if(this.isGalleryImage(this.populatedImageCount)){
					this.assignGalleryIconToView(imgView);
				}
				
				this.populatedImageCount++;
			}
			
			this.parentView.addView(container);
		}	
	}
	
	public void addNewImage(Bitmap bitmap, String persistedPath){
		this.parentView.removeAllViews();
		this.fillPhotos();
	}
	
	/*
	// BELOW function suffers from size issues. Keeping old photos are not good idea 
	// because their thumbnail size is relatively large 
	public void addNewImage(Bitmap bitmap, String persistedPath){
		
		// get last container
		int currentContainerCount = this.parentView.getChildCount();
		LinearLayout container = (LinearLayout)this.parentView.getChildAt(currentContainerCount-1);
		
		// remove gallery
		container.removeViewAt(container.getChildCount()-1);
		this.populatedImageCount--;
		
		// remove camera
		container.removeViewAt(container.getChildCount()-1);
		this.populatedImageCount--;
		
		// add new image
		// ImageInfo object has been added to the list already		
		LayoutInflater inflater = this.activity.getLayoutInflater();
		int stackFactor = this.calculateStackFactor();
		int dimension = this.getImageDimension(stackFactor);
		ImageView imgView = this.createImageView(inflater, container, dimension);
		//this.assignImageToView(imgView);
		imgView.setImageBitmap(bitmap);
		imgView.setClickable(true);
		imgView.setOnClickListener(new ImageClickListener(this.activity, imgView));
		this.populatedImageCount++;
		
		// add camera image back
		imgView = this.createImageView(inflater, container, dimension);
		this.assignCameraIconToView(imgView);
		this.populatedImageCount++;
		
		// check to see if we need a new container
		int requiredContainerCount = this.calculateContainerCount(stackFactor);
		if(requiredContainerCount > currentContainerCount){		
			// yes, we don't need a new container. create it			
			container = this.createLayout(inflater);
		}
		
		// add gallery image back
		imgView = this.createImageView(inflater, container, dimension);
		this.assignGalleryIconToView(imgView);
		this.populatedImageCount++;
		
		if(requiredContainerCount > currentContainerCount){
			// the container is a new one. add it to the root
			this.parentView.addView(container);
		}
	}*/
}
