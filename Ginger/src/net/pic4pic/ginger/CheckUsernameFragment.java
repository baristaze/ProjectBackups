package net.pic4pic.ginger;

import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.net.Uri;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;

import net.pic4pic.ginger.entities.UserCredentials;
import net.pic4pic.ginger.entities.UserResponse;
import net.pic4pic.ginger.tasks.CheckUsernameTask;
import net.pic4pic.ginger.utils.FacebookHelpers;
import net.pic4pic.ginger.utils.FacebookHelpers.FacebookLoginListener;
import net.pic4pic.ginger.utils.GingerHelpers;
import net.pic4pic.ginger.utils.MyLog;
import net.pic4pic.ginger.utils.PageAdvancer;
import net.pic4pic.ginger.utils.UserHelpers;

public class CheckUsernameFragment extends Fragment implements CheckUsernameTask.CheckUsernameListener, FacebookLoginListener {
	
	// constructor cannot have any parameters!!!
	public CheckUsernameFragment(/*no parameter here please*/){
	}
		
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
		
		// other fragments might have hidden the bar
		this.getActivity().getActionBar().show();
		
		View rootView = inflater.inflate(R.layout.fragment_checkusername, container, false);
		this._applyData(rootView);
		
		TextView termsLink = (TextView)rootView.findViewById(R.id.termsLink);
		//termsLink.setMovementMethod (LinkMovementMethod.getInstance());
		//String linkHtml = this.getResources().getString(R.string.terms);		
		//termsLink.setText(Html.fromHtml(linkHtml));
		
		termsLink.setOnClickListener(new OnClickListener(){
			@Override
			public void onClick(View v) {				
				Uri uri = Uri.parse( getResources().getString(R.string.termsUrl));
				startActivity(new Intent(Intent.ACTION_VIEW, uri));
			}});
		
		final EditText usernameEditText = (EditText)(rootView.findViewById(R.id.usernameEditText));
		final EditText passwordEditText = (EditText)(rootView.findViewById(R.id.passwordEditText));
		
		Button continueButton = (Button)(rootView.findViewById(R.id.continueButton));
		continueButton.setOnClickListener(new OnClickListener(){
			@Override
			public void onClick(View v) {				
				GingerHelpers.hideKeyboard(CheckUsernameFragment.this.getActivity(), usernameEditText);
				GingerHelpers.hideKeyboard(CheckUsernameFragment.this.getActivity(), passwordEditText);
				checkUsername();
			}});
		
		return rootView;
	}
	
	private void _applyData(View rootView){
		
		SharedPreferences prefs = this.getActivity().getSharedPreferences(
				getString(R.string.pref_filename_key), Context.MODE_PRIVATE);
		
		String username = prefs.getString(this.getString(R.string.pref_username_key), "");
		String password = prefs.getString(this.getString(R.string.pref_password_key), "");
		
		EditText usernameEditText = (EditText)(rootView.findViewById(R.id.usernameEditText));
		EditText passwordEditText = (EditText)(rootView.findViewById(R.id.passwordEditText));
		usernameEditText.setText(username);
		passwordEditText.setText(password);	
	}
	
	public void applyData(){
		
		View rootView = this.getView();
		if(rootView != null){
			_applyData(rootView);
		}	
		else{
			MyLog.e("CheckUsernameFragment", "applyData() has been called before onCreateView() of CheckUsernameFragment");
		}
	}
	
	private void checkUsername(){
		View rootView = this.getView();
		EditText usernameEditText = (EditText)(rootView.findViewById(R.id.usernameEditText));
		EditText passwordEditText = (EditText)(rootView.findViewById(R.id.passwordEditText));
		String username = usernameEditText.getText().toString().trim();
		String password = passwordEditText.getText().toString().trim();
		
		if(username.length() == 0){
			GingerHelpers.showErrorMessage(this.getActivity(), this.getString(R.string.signup_err_enter_username));
			return;
		}
		
		if(username.length() < 6){
			GingerHelpers.showErrorMessage(this.getActivity(), this.getString(R.string.signup_err_short_username));
			return;
		}
		
		if(password.length() == 0){
			GingerHelpers.showErrorMessage(this.getActivity(), this.getString(R.string.signup_err_enter_password));
			return;
		}
		
		if(password.length() < 6){
			GingerHelpers.showErrorMessage(this.getActivity(), this.getString(R.string.signup_err_short_password));
			return;
		}
		
		Button continueButton = (Button)(rootView.findViewById(R.id.continueButton));
		
		// prepare input
		UserCredentials credentials = new UserCredentials();
		credentials.setUsername(username);
		credentials.setPassword(password);
		
		// call the task asynchronously... it will make a web service call. see results onSignup method
		CheckUsernameTask asyncTask = new CheckUsernameTask(this, this.getActivity(), continueButton,credentials);
		asyncTask.execute(new String[]{});
	}
	
	@Override
	public void onCheckUser(UserResponse response, UserCredentials credentials){
		/**
         * ErrorCode != 0 : UserName is already in use
         * ErrorCode == 0 & AuthToken == null => UserName is available
         * ErrorCode == 0 & AuthToken != null => User is already signed up
         */
		if(response.getErrorCode() != 0){
			// ErrorCode != 0 : UserName is already in use
			MyLog.i("CheckUserName", response.getErrorMessage());			
			GingerHelpers.showErrorMessage(this.getActivity(), response.getErrorMessage());
		}
		else{
			
			if (response.getAuthToken() == null || response.getAuthToken().trim().length() == 0){
				// ErrorCode == 0 & AuthToken == null => UserName is available
				UserHelpers.saveUserCredentialsToFile(this.getActivity(), credentials, false);
				((PageAdvancer)this.getActivity()).moveToNextPage(0);
			}
			else {
				// ErrorCode == 0 & AuthToken != null => User is already signed up (full or partial)
				if(response.getUserProfile().isActive()){
					
					// flag that signup was done previously (full signup)
					UserHelpers.saveUserCredentialsToFile(this.getActivity(), credentials, true);
					
					// launch the view for the signed-in use
					this.startMainActivity(response);
				}
				else {
					// user has already signed up but partially...
					// we need to go to the last page but before that we need to make sure that there is a Facebook session
					
					FacebookHelpers helpers = new FacebookHelpers();
					helpers.startFacebook(getActivity(), CheckUsernameFragment.this, new Object[]{credentials, response});
					
					/*
					 * below code is postponed to 
					UserHelpers.saveUserCredentialsToFile(this.getActivity(), credentials, prefs, false);
					UserHelpers.saveUserPropertiesToFile(this.getActivity(), response.getUserProfile());
					((PageAdvancer)this.getActivity()).moveToLastPage(response, false);
					*/
				}
			}
		} 
	}
	
	@Override
	public void onFacebookUserRetrieval(String facebookSessionToken, long facebookUserId, Object state) {
		Object[] array = (Object[]) state;
		 
		UserCredentials credentials = (UserCredentials)array[0];
		UserResponse response = (UserResponse)array[1];
		
		UserHelpers.saveUserCredentialsToFile(this.getActivity(), credentials, false);
		UserHelpers.saveUserPropertiesToFile(this.getActivity(), response.getUserProfile());
		((PageAdvancer)this.getActivity()).moveToLastPage(response, false);
	}
	
	private void startMainActivity(UserResponse response){
		// launch the view for the signed-in use
		SignupActivity activity = (SignupActivity)this.getActivity();
		Intent intent = new Intent(activity, MainActivity.class);
		intent.putExtra(MainActivity.AuthenticatedUserBundleType, response); 
		intent.putExtra("PreSelectedTabIndexOnMainActivity", activity.getPreSelectedTabIndexOnMainActivity()); // pass through
		this.startActivity(intent);
		this.getActivity().finish();
	}
}
