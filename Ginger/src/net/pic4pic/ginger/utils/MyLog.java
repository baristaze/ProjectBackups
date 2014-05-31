package net.pic4pic.ginger.utils;

import android.util.Log;

public class MyLog {

	public static void e (String tag, String message){
		
		if(message == null){
			message = "(null)";
		}
		
		Log.e("Ginger-" + tag, message);
	}
	
	public static void w (String tag, String message){
		
		if(message == null){
			message = "(null)";
		}
		
		Log.w("Ginger-" + tag, message);
	}
	
	public static void i (String tag, String message){
		
		if(message == null){
			message = "(null)";
		}
		
		Log.i("Ginger-" + tag, message);
	}
	
	public static void v (String tag, String message){
		
		if(message == null){
			message = "(null)";
		}
		
		Log.v("Ginger-" + tag, message);
	}
	
	public static void d (String tag, String message){
		
		if(message == null){
			message = "(null)";
		}
		
		Log.d("Ginger-" + tag, message);
	}	
}
