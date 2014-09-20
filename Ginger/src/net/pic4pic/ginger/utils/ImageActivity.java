package net.pic4pic.ginger.utils;

import java.io.IOException;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.database.Cursor;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.net.Uri;
import android.provider.MediaStore;

import net.pic4pic.ginger.entities.IntegerEnum;

public class ImageActivity {
	
	public enum ErrorCode implements IntegerEnum {
		
		None (0),
		UnknownError (1),
		Canceled (2),
		InvalidUri (3),
		InvalidAbsolutePath (4),
		StoredRemotely (5),
		OrientationFailure (6),
		RotationFailure (7),
		DecodingFailure (8),
		NullAfterDecoding (9);
		
		private final int value;
		
		private ErrorCode(int value) {
			this.value = value;
		}

		public int getIntValue() {
			return this.value;
		}
	}
	
	public enum Source implements IntegerEnum {
		Camera(0),
		Gallery(1);
		
		private final int value;
		
		private Source(int value) {
			this.value = value;
		}

		public int getIntValue() {
			return this.value;
		}
	}
	
	public static class Result {

		private Bitmap resultBitmap;
		private ErrorCode errorCode = ErrorCode.None;
		
		public Bitmap getBitmap(){
			return this.resultBitmap;
		}
		
		public ErrorCode getErrorCode(){
			return this.errorCode;
		}
		
		public String getErrorMessage(){
			
			if(this.errorCode.getIntValue() == ErrorCode.UnknownError.getIntValue()){
				return "Unknown error";
			}
			
			if(this.errorCode.getIntValue() == ErrorCode.Canceled.getIntValue()){
				return "Camera or gallery activity has cancelled";
			}
			
			if(this.errorCode.getIntValue() == ErrorCode.InvalidUri.getIntValue()){
				return "Camera or gallery activity has failed";
			}
						
			if(this.errorCode.getIntValue() == ErrorCode.InvalidAbsolutePath.getIntValue()){
				return "Image path is invalid";
			}
			
			if(this.errorCode.getIntValue() == ErrorCode.StoredRemotely.getIntValue()){
				return "Image is not stored on this phone";
			}
			
			if(this.errorCode.getIntValue() == ErrorCode.OrientationFailure.getIntValue()){
				return "Couldn't get the image orientation";
			}
			
			if(this.errorCode.getIntValue() == ErrorCode.RotationFailure.getIntValue()){
				return "Couldn't fix the image orientation";
			}
			
			if(this.errorCode.getIntValue() == ErrorCode.DecodingFailure.getIntValue()){
				return "Image couldn't be decoded";
			}
			
			if(this.errorCode.getIntValue() == ErrorCode.NullAfterDecoding.getIntValue()){
				return "Decoded image is invalid (null)";
			}
			
			return null;
		}
	}	
	
	public static void start(Source source, Activity context, int requestCode){
		if(source == Source.Camera){
			startCamera(context, requestCode);
		}
		else{
			startGallery(context, requestCode);
		}
	}
	
	public static void startCamera(Activity context, int requestCode){
		Intent intent = new Intent(android.provider.MediaStore.ACTION_IMAGE_CAPTURE);
		intent.putExtra("android.intent.extras.CAMERA_FACING", 1);
		//Uri fileUri = Uri.parse(targetFile);
		//intent.putExtra(MediaStore.EXTRA_OUTPUT, fileUri); 
		context.startActivityForResult(intent, requestCode);
	}
	
	public static void startGallery(Activity context, int requestCode){
		Intent intent = new Intent();
        intent.setType("image/*");
        intent.setAction(Intent.ACTION_GET_CONTENT);
        context.startActivityForResult(Intent.createChooser(intent, "Select Picture"), requestCode);
	}
	
	// returns the image path or NULL
	public static String getAbsoluteImagePath(Context context, Uri uri) {
		try{
	        String[] projection = { MediaStore.Images.Media.DATA };
	        Cursor cursor = context.getContentResolver().query(uri, projection, null, null, null);
	        int column_index = cursor.getColumnIndexOrThrow(MediaStore.Images.Media.DATA);
	        cursor.moveToFirst();
	        return cursor.getString(column_index);
		}
		catch(Throwable ex){
			MyLog.bag().e("ImageActivity: " + ex.toString());
			return null;
		}
    }
	
	public static Result getProcessedResult(Context context, int resultCode, Intent data){
		
		Result result = new Result();
		result.errorCode = ErrorCode.None;
		
		if(resultCode != Activity.RESULT_OK || data == null){
			result.errorCode = ErrorCode.Canceled;
			return result;
		}
		
		Uri imageUri = data.getData();
		if(imageUri == null){
			result.errorCode = ErrorCode.InvalidUri;
			return result;
		}
		
		MyLog.bag().v("ImageActivity: URI from Gallery or Camera result = " + imageUri.toString());
		String imagePath = getAbsoluteImagePath(context, imageUri);
		if(imagePath == null){
			result.errorCode = ErrorCode.InvalidAbsolutePath;
			return result;
		}
		
		MyLog.bag().v("ImageActivity: Absolute Path of Image = " + imagePath);
		if(imagePath.startsWith("http")){
			result.errorCode = ErrorCode.StoredRemotely;
			return result;
		}
		
		int rotate = 0;
		try {
			rotate = BitmapHelpers.getOrientationFromExif(imagePath);
			MyLog.bag().v("ImageActivity: Rotation of Image = " + rotate);
		} 
		catch (IOException e) {
			result.errorCode = ErrorCode.OrientationFailure;
			return result;
		}
		
		System.gc();
		Bitmap bitmap = null;
		try{					
			bitmap = BitmapFactory.decodeFile(imagePath);
			if(bitmap == null){
				result.errorCode = ErrorCode.NullAfterDecoding;
				return result;
			}
		}
		catch(Throwable ex){
			result.errorCode = ErrorCode.DecodingFailure;
			return result;
		}
		
		if(rotate != 0){
			try {
				MyLog.bag().v("ImageActivity: Fixing rotation of the image...");
				bitmap = BitmapHelpers.rotateImage(bitmap, rotate);
			}
			catch(Throwable ex){
				result.errorCode = ErrorCode.RotationFailure;
				return result;	
			}
		}
		
		if(bitmap == null){
			result.errorCode = ErrorCode.UnknownError;
			return result;	
		}
		
		result.resultBitmap = bitmap;
		MyLog.bag().v("ImageActivity: Dimension of Image (WxH) = " + bitmap.getWidth() + "x" + bitmap.getHeight());
		return result;	
	}	
	
	public static Bitmap trimSize(Bitmap photo){
		
		if(photo.getWidth() > BitmapHelpers.MAX_SIZE || photo.getHeight() > BitmapHelpers.MAX_SIZE){
			
			int newWidth = photo.getWidth();
			int newHeight = photo.getHeight();
			if(photo.getWidth() >= photo.getHeight()){
				newWidth = BitmapHelpers.MAX_SIZE;
				newHeight = (int)((float)newWidth * ((float)photo.getHeight() / (float)photo.getWidth())); 
			}
			else {
				newHeight = BitmapHelpers.MAX_SIZE;
				newWidth = (int)((float)newHeight * ((float)photo.getWidth() / (float)photo.getHeight()));
			}
			
			System.gc();
			try{
				String sizeInfoOld = "Old size: " + photo.getWidth() + "x" + photo.getHeight() + ". Required byte: " + photo.getByteCount();
				photo = Bitmap.createScaledBitmap(photo, newWidth, newHeight, true);
				String sizeInfoNew = "New size: " + photo.getWidth() + "x" + photo.getHeight() + ". Required byte: " + photo.getByteCount();
				MyLog.bag().i("Photo size has been reduced: " + sizeInfoOld + " => " + sizeInfoNew);
			}
			catch(OutOfMemoryError exception){
				String sizeInfo = "Source size: " + photo.getWidth() + "x" + photo.getHeight() + ". Required byte: " + photo.getByteCount();
		    	MyLog.bag().e("Out of memory exception when creating scaling Bitmap in 'processPhotoActivityResult' method. " + sizeInfo);
		    	// no need to re-throw it here. just swallow.
			}
		}
		
		return photo;
	}
}
