package net.pic4pic.ginger.utils;

import net.pic4pic.ginger.R;
import android.app.Activity;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.text.InputFilter;
import android.view.View;
import android.widget.EditText;

public class TextInputDialog {
	
	public interface TextInputListener{
		public void onNewText(String text);
	}
	
	private TextInputListener listener;
	private Activity activity;
	private String textValue;
	private int maxLength;
	private String title;
	
	private EditText editText;

	public TextInputDialog(TextInputListener listener, Activity activity, String textValue, String title, int maxLength){
		this.listener = listener;
		this.activity = activity;
		this.textValue = textValue;
		this.title = title;
		this.maxLength = maxLength;		
	}
	
	private String getTextFromWidget(){
		String s = "";
		if(this.editText != null){
			s = this.editText.getText().toString();			
		}
		
		s = s.trim().replace('\n', ' ').replace('\t', ' ');
		while(s.contains("  ")){
			s = s.replace("  ", " ");
		}
		
		return s;
	}
	
	public void show(){
		
		AlertDialog.Builder dialog = new AlertDialog.Builder(this.activity, AlertDialog.THEME_HOLO_LIGHT);
		dialog.setCancelable(false);
		dialog.setTitle(this.title);
		
		View contentView = this.activity.getLayoutInflater().inflate(R.layout.text_input_dialog, null);
    	this.editText = (EditText)contentView.findViewById(R.id.editTextMain);
    	this.editText.setText(this.textValue);
    	this.editText.setSelection(this.textValue.length());
    	
    	if(this.maxLength > 0){
			InputFilter[] fArray = new InputFilter[1];
			fArray[0] = new InputFilter.LengthFilter(this.maxLength);
			this.editText.setFilters(fArray);
		}
    	    	
    	dialog.setView(contentView);
    	
    	dialog.setPositiveButton("OK", new DialogInterface.OnClickListener(){
			@Override
			public void onClick(DialogInterface dialog, int whichButton) {
				TextInputDialog.this.textValue = TextInputDialog.this.getTextFromWidget();
				TextInputDialog.this.listener.onNewText(TextInputDialog.this.textValue);
			}});
    	
    	dialog.setNegativeButton("Cancel", new DialogInterface.OnClickListener(){
			@Override
			public void onClick(DialogInterface dialog, int whichButton) {
				// do nothing
			}});
    	
    	dialog.show();
	}
}
