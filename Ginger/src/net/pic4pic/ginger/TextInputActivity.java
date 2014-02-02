package net.pic4pic.ginger;

import android.os.Bundle;
import android.app.Activity;
import android.content.Intent;
import android.text.InputFilter;
import android.view.Menu;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.EditText;

public class TextInputActivity extends Activity {

	public static final String TextInputType = "net.pic4pic.ginger.TextInput";
	public static final String TextInputMaxLen = "net.pic4pic.ginger.TextInputMaxLen";
	public static final int TextInputCode = 300;
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.text_input_activity);
		
		Intent intent = getIntent();
		String descr = intent.getExtras().getString(TextInputActivity.TextInputType, "");
		int maxLen = intent.getExtras().getInt(TextInputActivity.TextInputMaxLen, 0);
		
		EditText editText = (EditText)this.findViewById(R.id.editTextMain);
		editText.setText(descr);
		editText.setSelection(descr.length());
		
		if(maxLen > 0){
			InputFilter[] fArray = new InputFilter[1];
			fArray[0] = new InputFilter.LengthFilter(maxLen);
			editText.setFilters(fArray);
		}
		
		Button cancelButton = (Button)this.findViewById(R.id.cancelButton);
		cancelButton.setOnClickListener(new OnClickListener(){
			@Override
			public void onClick(View v) {
				Intent resultIntent = new Intent();
				resultIntent.putExtra(TextInputActivity.TextInputType, "");
				setResult(Activity.RESULT_CANCELED, resultIntent);
				finish();	
			}
		});
		
		Button okButton = (Button)this.findViewById(R.id.okButton);
		okButton.setOnClickListener(new OnClickListener(){
			@Override
			public void onClick(View v) {
				String descr = TextInputActivity.this.getTextFromWidget();
				Intent resultIntent = new Intent();
				resultIntent.putExtra(TextInputActivity.TextInputType, descr);		
				setResult(Activity.RESULT_OK, resultIntent);
				finish();
			}
		});
		
	}
	
	// we don't need this
	/*
	@Override
	public void onPause(){
		super.onPause();		
		EditText editText = (EditText)this.findViewById(R.id.editTextMain);
		InputMethodManager imm = (InputMethodManager)getSystemService(Context.INPUT_METHOD_SERVICE);
        imm.hideSoftInputFromWindow(editText.getWindowToken(), 0);
	}
	*/
	
	private String getTextFromWidget(){
		String s = "";
		EditText editText = (EditText)this.findViewById(R.id.editTextMain);
		if(editText != null){
			s = editText.getText().toString();			
		}
		
		s = s.trim().replace('\n', ' ').replace('\t', ' ');
		while(s.contains("  ")){
			s = s.replace("  ", " ");
		}
		
		return s;
	}
	
	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		// getMenuInflater().inflate(R.menu.text_input, menu);
		return true;
	}
}
