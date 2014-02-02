package net.pic4pic.ginger.utils;

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
}
