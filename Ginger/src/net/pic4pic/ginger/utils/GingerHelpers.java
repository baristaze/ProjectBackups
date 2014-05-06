package net.pic4pic.ginger.utils;

import java.util.Date;

import net.pic4pic.ginger.R;
import android.app.AlertDialog;
import android.content.Context;
import android.view.ContextThemeWrapper;
import android.view.View;
import android.view.inputmethod.InputMethodManager;
import android.widget.Toast;

public class GingerHelpers {
	
	public static void hideKeyboard(Context context, View focusedItem){
		InputMethodManager imm = (InputMethodManager)context.getSystemService(Context.INPUT_METHOD_SERVICE);
        imm.hideSoftInputFromWindow(focusedItem.getWindowToken(), 0);
	}
	
	public static void showErrorMessage(Context context, String errorMessage){
		new AlertDialog.Builder(new ContextThemeWrapper(context, android.R.style.Theme_Holo_Dialog))
	    .setTitle(context.getString(R.string.general_error_title))
	    .setMessage(errorMessage)		    
	    .setIcon(android.R.drawable.ic_dialog_alert)
	    .setNeutralButton(context.getString(R.string.general_OK), null)
	    .show();	
	}
	
	public static void toast(Context context, String message){
		Toast t = Toast.makeText(context, message, Toast.LENGTH_LONG);
		t.show();
	}
	
	public static String getTimeDiffHumanReadable(Date timeUTC){
	
		Date utcNow = new Date();
		long seconds = (utcNow.getTime() - timeUTC.getTime()) / 1000;
		long minutes = seconds / 60;
		long hours = minutes / 60;
		long days = hours / 24;
		
		if(days > 7){
			return "more than a week ago";
		}
		else if (days == 7){
			return "a week ago";
		}
		else if(days > 1){
			// 6 days ago
			// 2 days ago
			return Long.toString(days) + " days ago";
		}
		else if(hours > 1){
			// 41 hours ago
			// 27 hours ago
			// 3 hours ago
			return Long.toString(hours) + " hours ago";
		}
		else if(minutes > 1){
			// 110 minutes ago
			// 90 minutes ago
			// 40 minutes ago
			// 3 minutes ago
			return Long.toString(minutes) + " minutes ago";
		}
		else{
			return "just now";
		}
	}
}
