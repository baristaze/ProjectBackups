package net.pic4pic.ginger;

import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;

import android.os.Bundle;
import android.app.Activity;
import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.content.pm.PackageManager.NameNotFoundException;
import android.content.pm.Signature;
import android.util.Base64;
import android.view.ContextThemeWrapper;
import android.view.Menu;
import android.view.View;
import android.widget.ProgressBar;
import android.widget.TextView;

import net.pic4pic.ginger.entities.UserCredentials;
import net.pic4pic.ginger.entities.UserResponse;
import net.pic4pic.ginger.service.Service;
import net.pic4pic.ginger.tasks.SigninTask;
import net.pic4pic.ginger.tasks.TrackDeviceTask;
import net.pic4pic.ginger.utils.MyLog;

public class LaunchActivity extends Activity {

	private int preSelectedTabIndexOnMainActivity = 0;
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		
		super.onCreate(savedInstanceState);
		this.recreatePropertiesFromSavedBundle(savedInstanceState);
		
		Service.getInstance().init(this);
		MyLog.bag().v("Web Service Proxy is initialized");
		
		Intent intent = getIntent();
		this.preSelectedTabIndexOnMainActivity = intent.getIntExtra("PreSelectedTabIndexOnMainActivity", this.preSelectedTabIndexOnMainActivity);
		if(this.preSelectedTabIndexOnMainActivity != 0) {
			MyLog.bag().add("LaunchByNotif", "1").v();
		}
		
		setContentView(R.layout.activity_launch);		
		this.signinOrPreview();
	}
	
	private static void showHashKey(Context context) {
		
        try {
            PackageInfo info = context.getPackageManager().getPackageInfo("net.pic4pic.ginger", PackageManager.GET_SIGNATURES);
            for (Signature signature : info.signatures) {
                MessageDigest md = MessageDigest.getInstance("SHA");
                md.update(signature.toByteArray());
                MyLog.bag().add("FacebookHashKey", Base64.encodeToString(md.digest(), Base64.DEFAULT)).i("Hashkey for FB is calculated.");                
            }
        } 
        catch (NameNotFoundException e){
        	MyLog.bag().add(e).e("Retrieving Hashkey for FB is failed.");
        } 
        catch (NoSuchAlgorithmException e) {
        	MyLog.bag().add(e).e("Retrieving Hashkey for FB is failed.");
        }
	}
	
	@Override
	public void onSaveInstanceState(Bundle outState) {
		
		super.onSaveInstanceState(outState);

		if(outState == null){
			return;
		}
		
		outState.putInt("PreSelectedTabIndexOnMainActivity", this.preSelectedTabIndexOnMainActivity);
	}
	
	@Override
	public void onRestoreInstanceState(Bundle savedInstanceState) {
		
		super.onRestoreInstanceState(savedInstanceState);		
		this.recreatePropertiesFromSavedBundle(savedInstanceState);
	}
	
	private boolean recreatePropertiesFromSavedBundle(Bundle state){
		
		if(state == null){
			return false;
		}
		
		boolean restored = false;
		if(state.containsKey("PreSelectedTabIndexOnMainActivity")){
			this.preSelectedTabIndexOnMainActivity = state.getInt("PreSelectedTabIndexOnMainActivity", 0);
			restored = true;
		}
		
		return restored;
	}
	
	private void signinOrPreview(){
		
		SharedPreferences prefs = this.getSharedPreferences(getString(R.string.pref_filename_key), Context.MODE_PRIVATE);
		String username = prefs.getString(this.getString(R.string.pref_username_key), null);
		String password = prefs.getString(this.getString(R.string.pref_password_key), null);
		int signupCompleted = prefs.getInt(this.getString(R.string.pref_signupComplete_key), 0);
		
		if(username != null && password != null && signupCompleted == 1){
			// sign in
			UserCredentials credentials = new UserCredentials();
			credentials.setUsername(username);
			credentials.setPassword(password);
			this.signIn(credentials);			
		}
		else{
			
			// show hash key
			showHashKey(this);
			
			// preview
			this.startPreview();
		}
	}
	
	private void signIn(UserCredentials credentials){
		SigninTask asyncTask = new SigninTask(this, credentials);
		asyncTask.execute(new String[]{});
	}
	
	private void startPreview(){
		
		// track device...
		TrackDeviceTask trackingTask = new TrackDeviceTask(this);
		trackingTask.execute();
		
		// start preview
		Intent intent = new Intent(this, PreviewActivity.class);
		intent.putExtra("PreSelectedTabIndexOnMainActivity", this.preSelectedTabIndexOnMainActivity); // pass through
		this.startActivity(intent);
		this.finish();
	}
	
	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.launch, menu);
		return true;
	}
	
	public void onSignin(UserResponse response, final UserCredentials credentials){
		
		// track device...
		TrackDeviceTask trackingTask = new TrackDeviceTask(this);
		trackingTask.execute();
		
		if(response.getErrorCode() == 0){
			// successfully signed in
			Intent intent = new Intent(this, MainActivity.class);
			intent.putExtra(MainActivity.AuthenticatedUserBundleType, response); 
			intent.putExtra("PreSelectedTabIndexOnMainActivity", this.preSelectedTabIndexOnMainActivity); // pass through
			this.startActivity(intent);
			this.finish();
		}
		else{
			
			MyLog.bag().e(response.getErrorMessage());
			
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
		        	LaunchActivity.this.signIn(credentials);
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
