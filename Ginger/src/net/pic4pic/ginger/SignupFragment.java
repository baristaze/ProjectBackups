package net.pic4pic.ginger;

import net.pic4pic.ginger.tasks.SignupTask;
import net.pic4pic.ginger.utils.GingerHelpers;
import net.pic4pic.ginger.utils.PageAdvancer;
import android.content.Context;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.EditText;

public class SignupFragment extends Fragment implements SignupTask.SignupListener {
	
	// constructor cannot have any parameters!!!
	public SignupFragment(/*no parameter here please*/){
	}
		
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
		
		// other fragments might have hidden the bar
		this.getActivity().getActionBar().show();
		
		View rootView = inflater.inflate(R.layout.fragment_signup, container, false);
		this._applyData(rootView);
		
		final EditText usernameEditText = (EditText)(rootView.findViewById(R.id.usernameEditText));
		final EditText passwordEditText = (EditText)(rootView.findViewById(R.id.passwordEditText));
		
		Button continueButton = (Button)(rootView.findViewById(R.id.continueButton));
		continueButton.setOnClickListener(new OnClickListener(){
			@Override
			public void onClick(View v) {				
				GingerHelpers.hideKeyboard(SignupFragment.this.getActivity(), usernameEditText);
				GingerHelpers.hideKeyboard(SignupFragment.this.getActivity(), passwordEditText);
				signUp();
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
			Log.e("SignupFragment", "applyData() has been called before onCreateView() of SignupFragment");
		}
	}
	
	private void signUp(){
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
		
		SignupTask asyncTask = new SignupTask(this, this.getActivity(), continueButton, username, password);
		asyncTask.execute(new String[]{});
	}
	
	public void onSignup(String errorMessage, String username, String password){
		if(errorMessage == null || errorMessage.trim().length() == 0){
			
			SharedPreferences prefs = this.getActivity().getSharedPreferences(
					getString(R.string.pref_filename_key), Context.MODE_PRIVATE);
			
			SharedPreferences.Editor editor = prefs.edit();
			editor.putString(this.getString(R.string.pref_username_key), username);
			editor.putString(this.getString(R.string.pref_password_key), password);
			editor.commit();
			
			((PageAdvancer)this.getActivity()).moveToNextPage(0);
		}
		else{
			GingerHelpers.showErrorMessage(this.getActivity(), errorMessage);
		}
	}
}
