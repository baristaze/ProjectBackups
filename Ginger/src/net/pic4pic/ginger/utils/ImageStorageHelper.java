package net.pic4pic.ginger.utils;

import java.io.ByteArrayOutputStream;
import java.io.FileOutputStream;

import net.pic4pic.ginger.R;
import net.pic4pic.ginger.entities.GingerException;
import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;

public class ImageStorageHelper {
	
	public static boolean saveToInternalStorage(Context context, Bitmap bitmap, String fileName, boolean alertOnError){
		byte[] byteArray = null;		
		try{
			MyLog.bag().v("Bitmap Width before Compress: " + bitmap.getWidth());
			MyLog.bag().v("Bitmap Height before Compress: " + bitmap.getHeight());
			ByteArrayOutputStream stream = new ByteArrayOutputStream();
			bitmap.compress(Bitmap.CompressFormat.JPEG, 100, stream);
			byteArray = stream.toByteArray();
			MyLog.bag().v("Bitmap has been compressed successfully");
		}
		catch(Throwable ex){
			MyLog.bag().v("Bitmap couldn't be compressed");
			MyLog.bag().e(ex.toString());
			if(alertOnError){
				GingerHelpers.showErrorMessage(context, context.getString(R.string.err_image_compress_failed));
			}
		}
		
		if(byteArray != null){
			try{
				FileOutputStream fos = context.openFileOutput(fileName, Context.MODE_PRIVATE);
				fos.write(byteArray);
				fos.close();
				MyLog.bag().v("Bitmap has been saved successfully");
				return true;
			}
			catch(Throwable e){
				MyLog.bag().v("Bitmap couldn't be saved");
				MyLog.bag().add(e).e();
				if(alertOnError){
					GingerHelpers.showErrorMessage(context, context.getString(R.string.err_image_save_failed));
				}
			}
		}
		
		return false;
	}
	
	public static Bitmap readBitmapForFaceDetection(Context context, String fileName, boolean alertOnError){		
		BitmapFactory.Options options = new BitmapFactory.Options();
		options.inPreferredConfig = Bitmap.Config.RGB_565;
		Bitmap bitmap = readFromInternalStorage(context,fileName, alertOnError, options);
		if(bitmap != null){
			try {
				bitmap = BitmapHelpers.trimOddDimensions(bitmap);
			} 
			catch (GingerException e) {
				GingerHelpers.toast(context, "Out of memory exception when drawing image");
			}
		}
		
		return bitmap;
	}
	
	public static Bitmap readFromInternalStorage(Context context, String fileName, boolean alertOnError){
		return readFromInternalStorage(context,fileName, alertOnError, null);
	}
	
	public static Bitmap readFromInternalStorage(
			Context context, String fileName, boolean alertOnError, BitmapFactory.Options options){
		
		try{
			String absolutePath = getAbsolutePath(context, fileName);
			MyLog.bag().v("Absolute Path: " + absolutePath);
			
			System.gc();
			Bitmap bitmap = null;
			if(options == null){
				bitmap = BitmapFactory.decodeFile(absolutePath);
			}
			else{
				bitmap = BitmapFactory.decodeFile(absolutePath, options);
			}
			
			if(bitmap == null){
				MyLog.bag().v("Bitmap couldn't be read from the storage (null).");
			}
			else{
				MyLog.bag().v("Bitmap has been read from storage successfully");
			}
			return bitmap;
		}
		catch(Throwable e){
			MyLog.bag().v("Bitmap couldn't be read from the storage (exception).");
			if(alertOnError){
				GingerHelpers.showErrorMessage(context, context.getString(R.string.err_image_read_failed));
			}
		}
		
		return null;
	}
	
	public static String getAbsolutePath(Context context, String fileName){
		return context.getFilesDir().getAbsolutePath() + "/" + fileName;
	}
}
