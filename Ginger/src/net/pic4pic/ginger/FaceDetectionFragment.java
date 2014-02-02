package net.pic4pic.ginger;

import net.pic4pic.ginger.entities.ImageUploadRequest;
import net.pic4pic.ginger.entities.ImageUploadResponse;
import net.pic4pic.ginger.tasks.FaceDetectionTask;
import net.pic4pic.ginger.tasks.ImageUploadTask;
import net.pic4pic.ginger.utils.DrawView;
import net.pic4pic.ginger.utils.ImageStorageHelper;
import net.pic4pic.ginger.utils.PageAdvancer;

import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.SharedPreferences;
import android.graphics.Bitmap;
import android.media.FaceDetector;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.util.Log;
import android.view.ContextThemeWrapper;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.FrameLayout;
import android.widget.ImageView;
import android.widget.ProgressBar;
import android.widget.TextView;

public class FaceDetectionFragment extends Fragment 
	implements FaceDetectionTask.FaceDetectionListener,	ImageUploadTask.ImageUploadListener {
	
	private static final int MAX_FACE_COUNT = 5;
	
	private Bitmap faceDetectedBitmap;
	private View uploadControls;
	
	// constructor cannot have any parameters!!!
	public FaceDetectionFragment(/*no parameter here please*/){
	}
	
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {		
		
		View rootView = inflater.inflate(R.layout.fragment_face_detection, container, false);		
		this.uploadControls = inflater.inflate(R.layout.fragment_face_detection_upload, container, false);		
		Button cancelUploadBtn = (Button)(this.uploadControls.findViewById(R.id.cancelUploadButton));
		cancelUploadBtn.setOnClickListener(new View.OnClickListener() {
			@Override
			public void onClick(View v) {
				((PageAdvancer)FaceDetectionFragment.this.getActivity()).moveToPreviousPage();
			}
		});
		
		Button uploadBtn = (Button)(this.uploadControls.findViewById(R.id.uploadButton));
		uploadBtn.setOnClickListener(new View.OnClickListener() {
			@Override
			public void onClick(View v) {
				FaceDetectionFragment.this.startUpload();
			}
		});
		
		this.readAndShowImageBitmap(rootView);
		return rootView;
	}
	
	private Bitmap readAndShowImageBitmap(View view){
		
		Bitmap bitmap = null;
		String fileName = this.getString(R.string.lastCapturedPhoto_filename_key);
		bitmap = ImageStorageHelper.readBitmapForFaceDetection(this.getActivity(), fileName, false);		
		if(bitmap != null){
			Log.v("Bitmap", "Bitmap WxH = " + bitmap.getWidth() + "x" + bitmap.getHeight());
			ImageView imageView = (ImageView) view.findViewById(R.id.imageView);
			imageView.setImageBitmap(bitmap);
		}
		
		return bitmap;
	}
	
	public void applyData(){		
		
		View rootView = this.getView();
		if(rootView != null){
			FrameLayout frameLayout = ((FrameLayout)rootView);
			while(frameLayout.getChildCount() > 1){
				// remove last draw view
				frameLayout.removeViewAt(frameLayout.getChildCount()-1);
			}
			
			ProgressBar spinnerProgressBar = (ProgressBar)(rootView.findViewById(R.id.spinnerProgressBar));
			spinnerProgressBar.setVisibility(View.VISIBLE);
			
			TextView feedbackTextView = (TextView)(rootView.findViewById(R.id.feedbackTextView));
			feedbackTextView.setVisibility(View.VISIBLE);
			
			// temporarily draw this until we detect the faces
			Bitmap bitmap = this.readAndShowImageBitmap(rootView);
			if(bitmap != null){
				this.faceDetectedBitmap = null;
				FaceDetectionTask detectTask = new FaceDetectionTask(this, this.getActivity(), bitmap, MAX_FACE_COUNT);
				detectTask.execute();
			}
		}
		else{
			Log.e("FaceDetectionFragment", "applyData() has been called before onCreateView() of FaceDetectionFragment");
		}	
	}
	
	@Override
	public void onDetect(Bitmap bitmap, FaceDetector.Face[] detectedFaces){
		
		final ProgressBar spinnerProgressBar = (ProgressBar)this.getView().findViewById(R.id.spinnerProgressBar);
		spinnerProgressBar.setVisibility(View.INVISIBLE);
		
		final TextView feedbackTextView = (TextView)this.getView().findViewById(R.id.feedbackTextView);
		feedbackTextView.setVisibility(View.INVISIBLE);
		
		if(detectedFaces.length == 0){
			Log.v("FaceDetection", "Face couldn't be detected");			
			new AlertDialog.Builder(new ContextThemeWrapper(this.getActivity(), android.R.style.Theme_Holo_Dialog))
		    .setTitle(this.getString(R.string.general_error_title))
		    .setMessage(this.getString(R.string.err_no_face_detected))		    
		    .setIcon(android.R.drawable.ic_dialog_alert)
		    .setCancelable(false)
		    .setNeutralButton(this.getString(R.string.general_retry), new DialogInterface.OnClickListener() {
		        public void onClick(DialogInterface dialog, int which) {
		        	((PageAdvancer)FaceDetectionFragment.this.getActivity()).moveToPreviousPage();
		        }})
		    .show();
		}
		else{
			Log.v("Detection", "Detected Face Count: " + detectedFaces.length);
			this.faceDetectedBitmap = bitmap;
			drawGreenStrokesOnCanvas(bitmap, detectedFaces);	
			showUploadControls();
		}
	}
	
	private void drawGreenStrokesOnCanvas(Bitmap bitmap, FaceDetector.Face[] detectedFaces){		
		DrawView drawView = new DrawView(this.getActivity(), bitmap, detectedFaces);		
		drawView.setLayoutParams(new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.MATCH_PARENT));		
		((FrameLayout)this.getView()).addView(drawView);
	}
	
	private void showUploadControls(){
		((FrameLayout)this.getView()).addView(this.uploadControls);
	}
	
	private void startUpload(){
		String fileName = this.getString(R.string.lastCapturedPhoto_filename_key);
		String absoluteFilePath = ImageStorageHelper.getAbsolutePath(this.getActivity(), fileName);
		ImageUploadRequest request = new ImageUploadRequest();
		request.setFullLocalPath(absoluteFilePath);
		request.setWidth(this.faceDetectedBitmap.getWidth());
		request.setHeight(this.faceDetectedBitmap.getHeight());
		ImageUploadTask task = new ImageUploadTask(this, this.getActivity(), request);
		String url = "http://p4p-web-svc.azurewebsites.net/file/upload";
		task.execute(url);
	}

	@Override
	public void onUpload(ImageUploadRequest request, ImageUploadResponse response) {
		
		String error = null;
		if(response == null){
			error = "Photo couldn't be uploaded. Please try again later!";
		}
		else if(response.getErrorCode() != 0){
			error = response.getErrorMessage();
		}
			
		if(error != null){
			new AlertDialog.Builder(new ContextThemeWrapper(this.getActivity(), android.R.style.Theme_Holo_Dialog))
		    .setTitle(this.getString(R.string.general_error_title))
		    .setMessage(error)		    
		    .setIcon(android.R.drawable.ic_dialog_alert)
		    .setCancelable(false)
		    .setNegativeButton(this.getString(R.string.general_Cancel), new DialogInterface.OnClickListener() {
		        public void onClick(DialogInterface dialog, int which) {
		        	// ((PageAdvancer)FaceDetectionFragment.this.getActivity()).moveToPreviousPage();
		        }})
		    .setPositiveButton(this.getString(R.string.general_retry), new DialogInterface.OnClickListener() {
		        public void onClick(DialogInterface dialog, int which) {
		        	FaceDetectionFragment.this.startUpload();
		        }})
		    .show();	
		}
		else{
			/*
			GingerHelpers.toast(this.getActivity(),
					"Id = " + response.getFullImage().getId() +
					" & CloudUrl = " + response.getFullImage().getCloudUrl() + 
					" & ContentLength: " + response.getFullImage().getContentLength() + 
					" & CreateTime: " + response.getFullImage().getCreateTimeUTC()
					);*/
			
			Log.v("FaceDetectionFragment", "Full Image Url = " + response.getFullImage().getCloudUrl());
			Log.v("FaceDetectionFragment", "Thumbnail Image Url = " + response.getThumbnail().getCloudUrl());
			
			SharedPreferences prefs = this.getActivity().getSharedPreferences(
					getString(R.string.pref_filename_key), Context.MODE_PRIVATE);
			
			SharedPreferences.Editor editor = prefs.edit();
			editor.putString(this.getString(R.string.pref_user_bighoto_plain_key), response.getFullImage().getCloudUrl());
			editor.putString(this.getString(R.string.pref_user_thumbnail_plain_key), response.getThumbnail().getCloudUrl());
			editor.commit();
			
			((PageAdvancer)this.getActivity()).moveToNextPage(0);
		}
	}
}
