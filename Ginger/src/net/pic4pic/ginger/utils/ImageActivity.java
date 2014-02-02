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
import android.util.Log;

public class ImageActivity {
	
	public enum ErrorCode{
		None,
		UnknownError,
		Canceled,
		InvalidUri,
		InvalidAbsolutePath,
		StoredRemotely,
		OrientationFailure,
		RotationFailure,
		DecodingFailure,
		NullAfterDecoding,
	}
	
	public static enum Source{
		Camera,
		Gallery
	}
	
	public static class Result {

		private Bitmap resultBitmap;
		private ErrorCode errorCode;
		
		public Bitmap getBitmap(){
			return this.resultBitmap;
		}
		
		public ErrorCode getErrorCode(){
			return this.errorCode;
		}
		
		public String getErrorMessage(){
			
			if(this.errorCode == ErrorCode.UnknownError){
				return "Unknown error";
			}
			
			if(this.errorCode == ErrorCode.Canceled){
				return "Camera or gallery activity has cancelled";
			}
			
			if(this.errorCode == ErrorCode.InvalidUri){
				return "Camera or gallery activity has failed";
			}
						
			if(this.errorCode == ErrorCode.InvalidAbsolutePath){
				return "Image path is invalid";
			}
			
			if(this.errorCode == ErrorCode.StoredRemotely){
				return "Image is not stored on this phone";
			}
			
			if(this.errorCode == ErrorCode.OrientationFailure){
				return "Couldn't get the image orientation";
			}
			
			if(this.errorCode == ErrorCode.RotationFailure){
				return "Couldn't fix the image orientation";
			}
			
			if(this.errorCode == ErrorCode.DecodingFailure){
				return "Image couldn't be decoded";
			}
			
			if(this.errorCode == ErrorCode.NullAfterDecoding){
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
		catch(Exception ex){
			Log.e("ImageActivity", ex.toString());
			return null;
		}
    }
	
	public static Result getProcessedResult(Context context, int resultCode, Intent data){
		
		Result result = new Result();			
		if(resultCode != Activity.RESULT_OK || data == null){
			result.errorCode = ErrorCode.Canceled;
			return result;
		}
		
		Uri imageUri = data.getData();
		if(imageUri == null){
			result.errorCode = ErrorCode.InvalidUri;
			return result;
		}
		
		Log.v("ImageActivity", "URI from Gallery or Camera result = " + imageUri.toString());
		String imagePath = getAbsoluteImagePath(context, imageUri);
		if(imagePath == null){
			result.errorCode = ErrorCode.InvalidAbsolutePath;
			return result;
		}
		
		Log.v("ImageActivity", "Absolute Path of Image = " + imagePath);
		if(imagePath.startsWith("http")){
			result.errorCode = ErrorCode.StoredRemotely;
			return result;
		}
		
		int rotate = 0;
		try {
			rotate = BitmapHelpers.getOrientationFromExif(imagePath);
			Log.v("ImageActivity", "Rotation of Image = " + rotate);
		} 
		catch (IOException e) {
			result.errorCode = ErrorCode.OrientationFailure;
			return result;
		}
		
		Bitmap bitmap = null;
		try{					
			bitmap = BitmapFactory.decodeFile(imagePath);
			if(bitmap == null){
				result.errorCode = ErrorCode.NullAfterDecoding;
				return result;
			}
		}
		catch(Exception ex){
			result.errorCode = ErrorCode.DecodingFailure;
			return result;
		}
		
		if(rotate != 0){
			try {
				Log.v("ImageActivity", "Fixing rotation of the image...");
				bitmap = BitmapHelpers.rotateImage(bitmap, rotate);
			}
			catch(Exception ex){
				result.errorCode = ErrorCode.RotationFailure;
				return result;	
			}
		}
		
		if(bitmap == null){
			result.errorCode = ErrorCode.UnknownError;
			return result;	
		}
		
		result.resultBitmap = bitmap;
		Log.v("ImageActivity", "Dimension of Image (WxH) = " + bitmap.getWidth() + "x" + bitmap.getHeight());
		return result;	
	}	
}
