package net.pic4pic.ginger.utils;

import java.io.IOException;

import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.Matrix;
import android.graphics.Paint;
import android.graphics.PointF;
import android.graphics.RectF;
import android.media.ExifInterface;
import android.media.FaceDetector;
import android.util.Log;

public class BitmapHelpers {
	
	/*
	private static enum Orientation {
		Unknown,
		NormalPortrait,
		NormalLandscpage,
		UpsideDownPortrait,
		UpsideDownLandscpage,
	}
	*/
	
	public static enum ScaleType{
		Unknown,
		CenterCrop,
		CenterFit
	}
	
	public static float calculateScale(int sourceWidth, int sourceHeight, int newWidth, int newHeight, ScaleType scaleType){
		// Compute the scaling factors to fit the new height and width, respectively.
	    // To cover the final image, the final scaling will be the bigger 
	    // of these two.
	    float xScale = (float) newWidth / sourceWidth;
	    float yScale = (float) newHeight / sourceHeight;
	    float scale = Math.max(xScale, yScale); // this is for crop
	    if(scaleType == ScaleType.CenterFit){
	    	scale = Math.min(xScale, yScale); // this is for fit
	    }
	    
	    return scale;
	}
	
	public static Bitmap scale(Bitmap source, int newWidth, int newHeight, ScaleType scaleType){
	    // Compute the scaling factors to fit the new height and width, respectively.
		int sourceWidth = source.getWidth();
	    int sourceHeight = source.getHeight();
	    float scale = calculateScale(sourceWidth, sourceHeight, newWidth, newHeight, scaleType);
	    float scaledWidth = scale * sourceWidth;
	    float scaledHeight = scale * sourceHeight;
	    
	    Bitmap scaledImage = Bitmap.createBitmap((int)scaledWidth, (int)scaledHeight, source.getConfig());
		Canvas canvas = new Canvas(scaledImage); // rectangulatedImage is mutable... notice that it is empty
		RectF targetRect = new RectF(0, 0, scaledWidth, scaledHeight);
		canvas.drawBitmap(source, null, targetRect, null);
	    return scaledImage;
	}
	
	public static Bitmap scaleWithFaces(Bitmap source, int newWidth, int newHeight, FaceDetector.Face[] detectedFaces, ScaleType scaleType){
		// scale image
		float scale = calculateScale(source.getWidth(), source.getHeight(), newWidth, newHeight, scaleType);	    
		Bitmap scaledImage = scale(source, newWidth, newHeight, scaleType);
		Canvas canvas = new Canvas(scaledImage); // scaledImage is mutable... notice that it is empty
		
	    // draw the detected faces
	    PointF midPoint = new PointF();
	    Paint paint = new Paint();
        paint.setColor(Color.GREEN);
        paint.setStyle(Paint.Style.STROKE);
        paint.setStrokeWidth(3);        
        
	    for(int i=0; i < detectedFaces.length; i++) {
			FaceDetector.Face face = detectedFaces[i];
			face.getMidPoint(midPoint);
			float eyesDistance = face.eyesDistance();
			canvas.drawRect(
				(int)(scale * (midPoint.x - eyesDistance)),
				(int)(scale * (midPoint.y - eyesDistance)),
				(int)(scale * (midPoint.x + eyesDistance)), 
				(int)(scale * (midPoint.y + eyesDistance)), 
				paint);
		}
	    
	    return scaledImage;
	}
		
	public static Bitmap crop(Bitmap source, int newWidth, int newHeight){
	    int sourceWidth = source.getWidth();
	    int sourceHeight = source.getHeight();

	    // Let's find out the upper left coordinates if the scaled bitmap
	    // should be centered in the new size give by the parameters
	    float left = (newWidth - sourceWidth) / 2;
	    float top = (newHeight - sourceHeight) / 2;

	    // The target rectangle for the new, cropped version of the source bitmap will now be
	    RectF targetRect = new RectF(left, top, left + sourceWidth, top + sourceHeight);
	    
	    // Finally, we create a new bitmap of the specified size and draw our new,
	    // scaled bitmap onto it.
	    Bitmap croppedImage = Bitmap.createBitmap(newWidth, newHeight, source.getConfig());
	    Canvas canvas = new Canvas(croppedImage); // croppedImage is mutable... notice that it is empty
	    canvas.drawBitmap(source, null, targetRect, null);	    
	    return croppedImage;
	}
	
	public static Bitmap scaleAndCrop(Bitmap source, int newWidth, int newHeight, ScaleType scaleType){
	    Bitmap scaledImage = scale(source, newWidth, newHeight, scaleType);
	    return crop(scaledImage, newWidth, newHeight);
	}
	
	public static Bitmap scaleAndCropWithFaces(Bitmap source, int newWidth, int newHeight, FaceDetector.Face[] detectedFaces, ScaleType scaleType) {		
		
		// scale image with faces
		Bitmap scaledImage = scaleWithFaces(source, newWidth, newHeight, detectedFaces, scaleType);

	    // crop image
	    return crop(scaledImage, newWidth, newHeight);
	}
	
	public static Bitmap scaleCenterCrop(Bitmap source, int newWidth, int newHeight) {
		return scaleAndCrop(source, newWidth, newHeight, ScaleType.CenterCrop);
	}
	
	public static Bitmap scaleCenterFit(Bitmap source, int newWidth, int newHeight) {
		return scaleAndCrop(source, newWidth, newHeight, ScaleType.CenterFit);
	}
	
	public static Bitmap scaleCenterCrop(Bitmap source, int newWidth, int newHeight, FaceDetector.Face[] detectedFaces) {
		return scaleAndCropWithFaces(source, newWidth, newHeight, detectedFaces, ScaleType.CenterCrop);
	}
	
	public static Bitmap scaleCenterFit(Bitmap source, int newWidth, int newHeight, FaceDetector.Face[] detectedFaces) {
		return scaleAndCropWithFaces(source, newWidth, newHeight, detectedFaces, ScaleType.CenterFit);
	}
	
	/*
	private static Orientation getOrientation(Context context, String absoluteFilePath){
		Orientation orientation = Orientation.Unknown;
		Uri uri = Uri.parse(absoluteFilePath);
		String[] params = new String[] { MediaStore.Images.ImageColumns.ORIENTATION };		
		Cursor cursor = context.getContentResolver().query(uri, params, null, null, null);		
		try {
			if (cursor.moveToFirst()) {
				int angle = cursor.getInt(0);
				if(angle == 0){
					orientation = Orientation.NormalLandscpage;
				}
				else if(angle == 90){
					orientation = Orientation.NormalPortrait;
				}
				else if(angle == 180){
					orientation = Orientation.UpsideDownLandscpage;
				}
				else if(angle == 270){
					orientation = Orientation.UpsideDownPortrait;
				}
			}
		}
		finally {
			cursor.close();
		}
		
		return orientation;
	}
	*/
	
	public static Bitmap trimOddDimensions(Bitmap bitmap){
		int width = (bitmap.getWidth() % 2) == 0 ? bitmap.getWidth() : bitmap.getWidth()-1;
		int height = (bitmap.getHeight() % 2) == 0 ? bitmap.getHeight() : bitmap.getHeight()-1;
		if(width != bitmap.getWidth() || height != bitmap.getHeight()){
			Log.v("FaceDetection", "Cropping the image by 1 pixel since it must be even for face detection.");
			Bitmap temp = Bitmap.createBitmap(bitmap, 0, 0, width, height, null, true);			
			bitmap = temp;			
		}
		
		return bitmap;
	}
	
	public static int getOrientationFromExif(String absoluteFilePath) throws IOException{
		
		int exifOrientation = ExifInterface.ORIENTATION_NORMAL; 
		
		try {
			ExifInterface exif = new ExifInterface(absoluteFilePath);
			exifOrientation = exif.getAttributeInt(ExifInterface.TAG_ORIENTATION, ExifInterface.ORIENTATION_NORMAL);
		} 
		catch (IOException e) {
			Log.e("Rotation", "Rotation information couldn't be retrieved");
			Log.e("Rotation", e.toString());
			throw e;
		}
		
		int rotate = 0;		
		switch (exifOrientation) {
		    case ExifInterface.ORIENTATION_ROTATE_90:
		        rotate = 90;
		        break; 
		
		   case ExifInterface.ORIENTATION_ROTATE_180:
		       rotate = 180;
		       break;
		
		   case ExifInterface.ORIENTATION_ROTATE_270:
		       rotate = 270;
		       break;
		}
	
		Log.v("BitmapHelpers", "Rotation of the image = " + rotate + ". Image = " + absoluteFilePath);
		return rotate;
	}
	
	public static Bitmap rotateImage(Bitmap bitmap, int rotate){		
		
		if(rotate == 0){
			return bitmap;
		}
		
		int width = bitmap.getWidth();
		int height = bitmap.getHeight();
		
		// Setting pre rotate
		Matrix matrix = new Matrix();
		matrix.preRotate(rotate);
		
		// Rotating Bitmap
		bitmap = Bitmap.createBitmap(bitmap, 0, 0, width, height, matrix, false);
		
		return bitmap;
	}
}
