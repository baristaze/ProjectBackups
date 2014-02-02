package net.pic4pic.ginger;

import net.pic4pic.ginger.tasks.SigninTask;

import android.os.Bundle;
import android.app.Activity;
import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.SharedPreferences;
import android.view.ContextThemeWrapper;
import android.view.Menu;
import android.view.View;
import android.widget.ProgressBar;
import android.widget.TextView;

public class LaunchActivity extends Activity {

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_launch);
		
		this.signinOrSignup();
	}
	
	private void signinOrSignup(){
		
		SharedPreferences prefs = this.getSharedPreferences(getString(R.string.pref_filename_key), Context.MODE_PRIVATE);
		String username = prefs.getString(this.getString(R.string.pref_username_key), null);
		String password = prefs.getString(this.getString(R.string.pref_password_key), null);
		int signupCompleted = prefs.getInt(this.getString(R.string.pref_signupComplete_key), 0);
		/*
		username = "foobar";
		password = "1234";
		signupCompleted = 1;
		*/
		if(username != null && password != null && signupCompleted == 1){
			// sign in
			this.signIn(username, password);			
		}
		else{
			// sign up
			this.startSignUp(username, password);
		}
	}
	
	private void signIn(String username, String password){
		SigninTask asyncTask = new SigninTask(this, username, password);
		asyncTask.execute(new String[]{});
	}
	
	private void startSignUp(String username, String password){
		Intent intent = new Intent(this, SignupActivity.class);
		this.startActivity(intent);
		this.finish();
	}
	
	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.launch, menu);
		return true;
	}
	
	public void onSignin(Person person, final String username, final String password){
		if(person != null){
			Intent intent = new Intent(this, MainActivity.class);
			intent.putExtra(MainActivity.SignedInPersonType, person); 
			this.startActivity(intent);
			this.finish();
		}
		else{
			final ProgressBar spinnerProgressBar = (ProgressBar)this.findViewById(R.id.spinnerProgressBar);
			spinnerProgressBar.setVisibility(View.INVISIBLE);
			
			final TextView feedbackTextView = (TextView)this.findViewById(R.id.feedbackTextView);
			feedbackTextView.setVisibility(View.INVISIBLE);
			
			new AlertDialog.Builder(new ContextThemeWrapper(this, android.R.style.Theme_Holo_Dialog))
		    .setTitle(this.getString(R.string.general_error_title))
		    .setMessage(this.getString(R.string.launch_signin_error_msg))
		    .setPositiveButton(this.getString(R.string.general_retry), new DialogInterface.OnClickListener() {
		        public void onClick(DialogInterface dialog, int which) {
		        	spinnerProgressBar.setVisibility(View.VISIBLE);
		        	feedbackTextView.setVisibility(View.VISIBLE);
		        	LaunchActivity.this.signIn(username, password);
		        }
		     })
		    .setNegativeButton(this.getString(R.string.general_close), new DialogInterface.OnClickListener() {
		        public void onClick(DialogInterface dialog, int which) { 
		            LaunchActivity.this.finish();
		        }
		     })
		     .setCancelable(false)
		     .setIcon(android.R.drawable.ic_dialog_alert)
		     .show();
		}
	}
}
