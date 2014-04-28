package net.pic4pic.ginger.utils;

import java.io.ByteArrayOutputStream;
import java.io.FileOutputStream;

import net.pic4pic.ginger.R;
import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;

public class ImageStorageHelper {
	
	public static boolean saveToInternalStorage(Context context, Bitmap bitmap, String fileName, boolean alertOnError){
		byte[] byteArray = null;		
		try{
			MyLog.v("Bitmap", "Bitmap Width before Compress: " + bitmap.getWidth());
			MyLog.v("Bitmap", "Bitmap Height before Compress: " + bitmap.getHeight());
			ByteArrayOutputStream stream = new ByteArrayOutputStream();
			bitmap.compress(Bitmap.CompressFormat.JPEG, 100, stream);
			byteArray = stream.toByteArray();
			MyLog.v("Storage", "Bitmap has been compressed successfully");
		}
		catch(Exception ex){
			MyLog.v("Storage", "Bitmap couldn't be compressed");
			MyLog.e("Storage", ex.toString());
			if(alertOnError){
				GingerHelpers.showErrorMessage(context, context.getString(R.string.err_image_compress_failed));
			}
		}
		
		if(byteArray != null){
			try{
				FileOutputStream fos = context.openFileOutput(fileName, Context.MODE_PRIVATE);
				fos.write(byteArray);
				fos.close();
				MyLog.v("Storage", "Bitmap has been saved successfully");
				return true;
			}
			catch(Exception e){
				MyLog.v("Storage", "Bitmap couldn't be saved");
				MyLog.e("File", e.toString());
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
			bitmap = BitmapHelpers.trimOddDimensions(bitmap);
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
			MyLog.v("Storage", "Absolute Path: " + absolutePath);
			
			Bitmap bitmap = null;
			if(options == null){
				bitmap = BitmapFactory.decodeFile(absolutePath);
			}
			else{
				bitmap = BitmapFactory.decodeFile(absolutePath, options);
			}
			
			if(bitmap == null){
				MyLog.v("Storage", "Bitmap couldn't be read from the storage (null).");
			}
			else{
				MyLog.v("Storage", "Bitmap has been read from storage successfully");
			}
			return bitmap;
		}
		catch(Exception e){
			MyLog.v("Storage", "Bitmap couldn't be read from the storage (exception).");
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
