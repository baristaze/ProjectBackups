package net.pic4pic.ginger.utils;

import java.util.StringTokenizer;

import net.pic4pic.ginger.service.Service;
import android.util.Log;

public class MyLog {
	
	public static final String PublishedAppVersion = "1.0";
	
	public static LogBag bag(){
		
		LogBag bag = new LogBag();
		addAppData(bag);
		addUserData(bag);
		addCallerStackTrace(bag);
		return bag;
	}
	
	protected static void addAppData(LogBag bag){		
		bag.add(LogBag.TagAppType, "Android");
		bag.add(LogBag.TagAppVersion, PublishedAppVersion);
	}
	
	protected static void addUserData(LogBag bag){		
		bag.addClientId(Service.getInstance().getCachedClientId());		
	}
	
	protected static void addCallerStackTrace(LogBag bag){
		
		boolean isNextElement = false;
		StackTraceElement[] stackTrace = Thread.currentThread().getStackTrace();
		for(StackTraceElement elem : stackTrace){
			
			if(isNextElement){
				bag.add(LogBag.TagFileName, elem.getFileName());				
				String fullClassName = elem.getClassName();
				String className = fullClassName; 
				StringTokenizer tokenizer = new StringTokenizer(fullClassName, ".");
				while(tokenizer.hasMoreTokens()){
					className = tokenizer.nextToken();
				}
				
				bag.add(LogBag.TagClassName, className);
				bag.add(LogBag.TagMethodName, elem.getMethodName());
				bag.add(LogBag.TagLineNumber, Integer.toString(elem.getLineNumber()));
				break;
			}
			
			if(elem.getClassName().equals(MyLog.class.getName()) && elem.getMethodName().equals("bag")){
				isNextElement = true;
			}
		}
	}		
	
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
