package net.pic4pic.ginger;

import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.List;

import com.facebook.Session;
import com.facebook.SessionState;

import android.annotation.SuppressLint;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.graphics.Color;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.TextView;

import net.pic4pic.ginger.entities.BaseRequest;
import net.pic4pic.ginger.entities.BaseResponse;
import net.pic4pic.ginger.entities.EducationLevel;
import net.pic4pic.ginger.entities.ImageFile;
import net.pic4pic.ginger.entities.MaritalStatus;
import net.pic4pic.ginger.entities.UserProfile;
import net.pic4pic.ginger.entities.UserResponse;
import net.pic4pic.ginger.entities.VerifyBioRequest;
import net.pic4pic.ginger.tasks.ActivateUserTask;
import net.pic4pic.ginger.tasks.ActivateUserTask.ActivateUserListener;
import net.pic4pic.ginger.tasks.ImageDownloadTask;
import net.pic4pic.ginger.tasks.VerifyBioTask;
import net.pic4pic.ginger.tasks.VerifyBioTask.VerifyBioListener;
import net.pic4pic.ginger.utils.GingerHelpers;
import net.pic4pic.ginger.utils.MyLog;

@SuppressLint("ResourceAsColor")
public class PersonalDetailsFragment extends Fragment implements VerifyBioListener, ActivateUserListener{

	private UserResponse userInfo = null;
	private Button finishButton = null;
	
	// constructor cannot have any parameters!!!
	public PersonalDetailsFragment(/*no parameter here please*/){		 
	}	
	
	public void setUserResponse(UserResponse userInfo){
		this.userInfo = userInfo;
	}
	
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
		
		View rootView = inflater.inflate(R.layout.fragment_personal_details, container, false);
		this._applyData(rootView);
		
		final ImageButton buttonAgeEdit = (ImageButton)(rootView.findViewById(R.id.buttonAgeEdit));
		buttonAgeEdit.setOnClickListener(new OnClickListener(){
			@Override
			public void onClick(View v) {
				
				PersonalDetailsFragment.this.startFacebookGet(
						"user_birthday", "birthday", R.string.pref_user_birthday_key, R.id.ageText);
			}});
		
		ImageButton buttonHomeTownEdit = (ImageButton)(rootView.findViewById(R.id.buttonHomeTownEdit));
		buttonHomeTownEdit.setOnClickListener(new OnClickListener(){
			@Override
			public void onClick(View v) {
				PersonalDetailsFragment.this.startFacebookGet(
						"user_hometown", "hometown", R.string.pref_user_hometown_city_key, R.id.homeTownText);								
			}});
		
		ImageButton buttonRelationStatusEdit = (ImageButton)(rootView.findViewById(R.id.buttonRelationStatusEdit));
		buttonRelationStatusEdit.setOnClickListener(new OnClickListener(){
			@Override
			public void onClick(View v) {				
				PersonalDetailsFragment.this.startFacebookGet(
						"user_relationships", "relationship_status", R.string.pref_user_relation_status, R.id.relationStatusText);
			}});
		
		ImageButton buttonProfessionEdit = (ImageButton)(rootView.findViewById(R.id.buttonProfessionEdit));
		buttonProfessionEdit.setOnClickListener(new OnClickListener(){
			@Override
			public void onClick(View v) {				
				PersonalDetailsFragment.this.startFacebookGet(
						"user_work_history", "work", R.string.pref_user_profession_key, R.id.professionText);
			}});
		
		ImageButton buttonEducationLevelEdit = (ImageButton)(rootView.findViewById(R.id.buttonEducationLevelEdit));
		buttonEducationLevelEdit.setOnClickListener(new OnClickListener(){
			@Override
			public void onClick(View v) {				
				PersonalDetailsFragment.this.startFacebookGet(
						"user_education_history", "education", R.string.pref_user_education_key, R.id.educationLevelText);					
			}});
		
		this.finishButton = (Button)(rootView.findViewById(R.id.finishButton));
		this.finishButton.setOnClickListener(new OnClickListener(){
			@Override
			public void onClick(View v) {
				
				if(!checkRequiredFields()){
					
					// error cases are written into the metrics in above function already
					return;
				}
				
				MyLog.bag()
	    		.add("funnel", "signup")
	    		.add("step", "8")
	    		.add("page", "profile")
	    		.add("action", "click finish")
	    		.add("success", "1")
	    		.m();
				
				PersonalDetailsFragment.this.activateUser();
				
			}});
		
		return rootView;
	}	
	
	public void activateUser(){
	
		ActivateUserTask task = new ActivateUserTask(this, this.getActivity(), this.finishButton, new BaseRequest());
		task.execute();
	}
	
	@Override
	public void onUserActivated(BaseResponse response, BaseRequest request){
		
		if(response.getErrorCode() == 0){
			// flag that signup is done
			SharedPreferences prefs = this.getActivity().getSharedPreferences(
					getString(R.string.pref_filename_key), Context.MODE_PRIVATE);
				
			SharedPreferences.Editor editor = prefs.edit();		
			editor.putInt(this.getString(R.string.pref_signupComplete_key), 1);
			editor.commit();
			
			this.userInfo.getUserProfile().setActive(true);
		}
		
		MyLog.bag()
		.add("funnel", "signup")
		.add("step", "8")
		.add("page", "profile")
		.add("action", "post activation")
		.add("success", "1")
		.m();
		
		// launch the main activity
		SignupActivity activity = (SignupActivity)this.getActivity();
		Intent intent = new Intent(activity, MainActivity.class);
		
		intent.putExtra(MainActivity.AuthenticatedUserBundleType, this.userInfo); 
		intent.putExtra("PreSelectedTabIndexOnMainActivity", activity.getPreSelectedTabIndexOnMainActivity()); // pass through
		this.startActivity(intent);
		this.getActivity().finish();			
	}
	
	private boolean checkRequiredFields(){
		
		CharSequence text = "";
		CharSequence required = "required"; 
		View rootView = this.getView();
		
		TextView genderText = (TextView)(rootView.findViewById(R.id.genderText));
		text = genderText.getText();
		if(required.equals(text)){
			
			GingerHelpers.showErrorMessage(getActivity(), "Your gender info is a must");
			
			MyLog.bag()
    		.add("funnel", "signup")
    		.add("step", "8")
    		.add("page", "profile")
    		.add("action", "click finish")
    		.add("success", "0")
    		.add("error", "gender required")
    		.m();
			
			return false;
		}
		
		TextView ageText = (TextView)(rootView.findViewById(R.id.ageText));
		text = ageText.getText();
		if(required.equals(text)){
			
			GingerHelpers.showErrorMessage(getActivity(), "Your age info is a must");
			
			MyLog.bag()
    		.add("funnel", "signup")
    		.add("step", "8")
    		.add("page", "profile")
    		.add("action", "click finish")
    		.add("success", "0")
    		.add("error", "age required")
    		.m();
			
			return false;
		}
		
		TextView homeTownText = (TextView)(rootView.findViewById(R.id.homeTownText));
		text = homeTownText.getText();
		if(required.equals(text)){
			
			GingerHelpers.showErrorMessage(getActivity(), "Your hometown info is a must");
			
			MyLog.bag()
    		.add("funnel", "signup")
    		.add("step", "8")
    		.add("page", "profile")
    		.add("action", "click finish")
    		.add("success", "0")
    		.add("error", "hometown required")
    		.m();
			
			return false;
		}		

		TextView relationStatusText = (TextView)(rootView.findViewById(R.id.relationStatusText));
		text = relationStatusText.getText();
		if(required.equals(text)){
			
			GingerHelpers.showErrorMessage(getActivity(), "Your marriage status is a must");
			
			MyLog.bag()
    		.add("funnel", "signup")
    		.add("step", "8")
    		.add("page", "profile")
    		.add("action", "click finish")
    		.add("success", "0")
    		.add("error", "marriage status required")
    		.m();
			
			return false;
		}
		
		TextView professionText = (TextView)(rootView.findViewById(R.id.professionText));
		text = professionText.getText();
		if(required.equals(text)){
			
			GingerHelpers.showErrorMessage(getActivity(), "Your profession info is a must");
			
			MyLog.bag()
    		.add("funnel", "signup")
    		.add("step", "8")
    		.add("page", "profile")
    		.add("action", "click finish")
    		.add("success", "0")
    		.add("error", "profession required")
    		.m();
			
			return false;
		}
		
		TextView educationLevelText = (TextView)(rootView.findViewById(R.id.educationLevelText));
		text = educationLevelText.getText();
		if(required.equals(text)){
			
			GingerHelpers.showErrorMessage(getActivity(), "Your education level is a must");
			
			MyLog.bag()
    		.add("funnel", "signup")
    		.add("step", "8")
    		.add("page", "profile")
    		.add("action", "click finish")
    		.add("success", "0")
    		.add("error", "education level required")
    		.m();
			
			return false;
		}
		
		return true;
	}
	
	public void applyData(){
		
		View rootView = this.getView();
		if(rootView != null){
			_applyData(rootView);
		}	
		else{
			MyLog.bag().e("applyData() has been called before onCreateView() of PersonalDetailsFragment");
		}
	}
	
	private void _applyData(View rootView){
		
		SharedPreferences prefs = this.getActivity().getSharedPreferences(
				getString(R.string.pref_filename_key), Context.MODE_PRIVATE);
		
		String username = prefs.getString(this.getString(R.string.pref_username_key), "");	
		//String thumbnailUrl = prefs.getString(this.getString(R.string.pref_user_thumbnail_blurred_key), "");
		String gender = prefs.getString(this.getString(R.string.pref_user_gender_key), "Unspecified");
		String homeTownCity = prefs.getString(this.getString(R.string.pref_user_hometown_city_key), "required");
		String relationStatus = prefs.getString(this.getString(R.string.pref_user_relation_status), "required");
		
		String professionRequired = "required";
		String educationLevelRequired = "optional";
		if(gender.toLowerCase().equals("female")){
			professionRequired = "optional";
			educationLevelRequired = "optional";
		}
		
		String profession = prefs.getString(this.getString(R.string.pref_user_profession_key), professionRequired);		
		String educationLevel = prefs.getString(this.getString(R.string.pref_user_education_key), educationLevelRequired);
		
		String age = "required";
		String birthday = prefs.getString(this.getString(R.string.pref_user_birthday_key), null); // "required"
		if(birthday != null && !birthday.isEmpty()){
			try{
				DateFormat format = new SimpleDateFormat("yyyy-MM-dd");
				Date birthDate =  format.parse(birthday);
				int ageInt = this.calculateAge(birthDate);
				if(ageInt > 0 && ageInt < 120){
					age = Integer.toString(ageInt);
				}
			}
			catch(Throwable ex){
				MyLog.bag().add(ex).e();
			}
		}
		
		TextView usernameText = (TextView)(rootView.findViewById(R.id.usernameText));
		usernameText.setText(username);
		
		TextView genderText = (TextView)(rootView.findViewById(R.id.genderText));
		genderText.setText(gender);
		
		TextView ageText = (TextView)(rootView.findViewById(R.id.ageText));
		ageText.setText(age);
		if(this.isRealValue(age)){
			this.setActiveColor(ageText);
		}		
		
		TextView homeTownText = (TextView)(rootView.findViewById(R.id.homeTownText));
		homeTownText.setText(homeTownCity);
		if(this.isRealValue(homeTownCity)){
			this.setActiveColor(homeTownText);
		}
		
		TextView professionText = (TextView)(rootView.findViewById(R.id.professionText));
		professionText.setText(profession);
		if(this.isRealValue(profession)){
			this.setActiveColor(professionText);
		}
		
		TextView relationStatusText = (TextView)(rootView.findViewById(R.id.relationStatusText));
		relationStatusText.setText(relationStatus);
		if(this.isRealValue(relationStatus)){
			this.setActiveColor(relationStatusText);
		}
		
		TextView educationLevelText = (TextView)(rootView.findViewById(R.id.educationLevelText));
		educationLevelText.setText(educationLevel);
		if(this.isRealValue(educationLevel)){
			this.setActiveColor(educationLevelText);
		}
		
		// override thumbnailUrl
		if(this.userInfo != null){
			ImageFile imageToDownload = this.userInfo.getProfilePictures().getThumbnailBlurred();
			ImageView imageView = (ImageView)(rootView.findViewById(R.id.thumbnailImage));
			ImageDownloadTask asyncTask = new ImageDownloadTask(imageToDownload.getId(), imageView);
			
			MyLog.bag().v("Thumbnail Url = " + imageToDownload.getCloudUrl());
			asyncTask.execute(imageToDownload.getCloudUrl());
		}
		else{
			MyLog.bag().v("No Thumbnail Url since there is no UserInfo yet.");
		}
			
		/*
		if(thumbnailUrl != null && !thumbnailUrl.isEmpty()){
			ImageView imageView = (ImageView)(rootView.findViewById(R.id.thumbnailImage));
			ImageDownloadTask asyncTask = new ImageDownloadTask(imageView);
			asyncTask.execute(thumbnailUrl);
		}
		*/		
	}
	
	private boolean isRealValue(String val){
		if(val == null || val.isEmpty()){
			return false;
		}
		if(val.compareToIgnoreCase("optional") == 0){
			return false;
		}
		if(val.compareToIgnoreCase("required") == 0){
			return false;
		}
		if(val.compareToIgnoreCase("unknown") == 0){
			return false;
		}
		if(val.compareToIgnoreCase("unspecified") == 0){
			return false;
		}
		return true;
	}
	
	private void startFacebookGet(final String requiredPermission, final String fieldName, final int userSettingsKeyForField, final int matchingTextViewId){
		
		MyLog.bag()
		.add("funnel", "signup")
		.add("step", "8")
		.add("page", "profile")
		.add("action", "edit")
		.add("field", fieldName)
		.m();
		
		// start Facebook Login
		List<String> permissionList = new ArrayList<String>();
		permissionList.add(requiredPermission);		
		Session.openActiveSession(this.getActivity(), true, permissionList, new Session.StatusCallback() {
			// callback when session changes state
			@Override
			public void call(Session session, SessionState state, Exception exception) {
				// session is never null indeed...
				if (session != null && session.isOpened()) {
					if(session.getPermissions().contains(requiredPermission)){
						// PersonalDetailsFragment.this.getFacebookUser(session, fieldName, userSettingsKeyForField, matchingTextViewId);
						PersonalDetailsFragment.this.startVerifyBio(session, fieldName, userSettingsKeyForField, matchingTextViewId);
					}					
				}
			}
		});
	}
	
	private void startVerifyBio(Session session, final String fieldName, final int userSettingsKeyForField, final int matchingTextViewId){
		
		SharedPreferences prefs = this.getActivity().getSharedPreferences(
				getString(R.string.pref_filename_key), Context.MODE_PRIVATE);
		
		String facebookAccessToken = prefs.getString(this.getString(R.string.pref_user_facebookid_key), "");
		long facebookUserId = prefs.getLong(this.getString(R.string.pref_user_facebooktoken_key), 0);
		
		// prepare verification request		
		VerifyBioRequest request = new VerifyBioRequest();
		request.setFacebookUserId(facebookUserId);
		request.setFacebookAccessToken(facebookAccessToken);
		request.setUserFields(fieldName); // comma separated
		
		// start verifying... 
		VerifyBioTask task = new VerifyBioTask(this, this.getActivity(), this.finishButton, request, matchingTextViewId);
		task.execute();
	}
		
	public void onVerifyBio(UserResponse response, VerifyBioRequest request, int matchingTextViewId){
		
		if(response.getErrorCode() != 0){
			
			// error cases were written to the metrics in the task already
			
			//  "We couldn't verify your data with Facebook"
			MyLog.bag().e(response.getErrorMessage());
			GingerHelpers.showErrorMessage(this.getActivity(), response.getErrorMessage());
			this.updatePageField("optional", matchingTextViewId, false);
		}
		else{
			
			MyLog.bag()
    		.add("funnel", "signup")
    		.add("step", "8")
    		.add("page", "profile")
    		.add("action", "post edit")
    		.add("field", request.getUserFields())
    		.add("success", "1")
    		.m();
			
			this.userInfo = response;
			UserProfile user = response.getUserProfile();
						
			SharedPreferences prefs = this.getActivity().getSharedPreferences(
					getString(R.string.pref_filename_key), Context.MODE_PRIVATE);
				
			SharedPreferences.Editor editor = prefs.edit();		
			editor.putString(this.getString(R.string.pref_user_gender_key), user.getGender().toString());
			
			if(user.getHometownCity() != null && user.getHometownCity().trim().length() > 0){
				String data = user.getHometownCity().trim();
				editor.putString(this.getString(R.string.pref_user_hometown_city_key), data);
				this.updatePageField(data, R.id.homeTownText);
			}
			
			if(user.getMaritalStatus() != MaritalStatus.Unknown){
				String data = user.getMaritalStatus().toString();
				editor.putString(this.getString(R.string.pref_user_relation_status), data);
				this.updatePageField(data, R.id.relationStatusText);
			}
			
			if(user.getProfession() != null && user.getProfession().trim().length() > 0){
				String data = user.getProfession().trim();
				editor.putString(this.getString(R.string.pref_user_profession_key), data);
				this.updatePageField(data, R.id.professionText);
			}
			
			if(user.getEducationLevel() != EducationLevel.Unknown){
				String data = user.getEducationLevelAsString();
				editor.putString(this.getString(R.string.pref_user_education_key), data);
				this.updatePageField(data, R.id.educationLevelText);
			}
			
			if(user.getBirthDay() != null){
				
				DateFormat format = new SimpleDateFormat("yyyy-MM-dd");
				String data = format.format(user.getBirthDay());				
				editor.putString(this.getString(R.string.pref_user_birthday_key), data);
				
				int ageInt = this.calculateAge(user.getBirthDay());
				if(ageInt > 0 && ageInt < 120){
					String age = Integer.toString(ageInt);
					this.updatePageField(age, R.id.ageText);
				}
			}
			
			editor.commit();
		}
	}
	
	private void updatePageField(String userFriendlyValue, int matchingTextViewId){
		this.updatePageField(userFriendlyValue, matchingTextViewId, true);
	}
	
	private void updatePageField(String userFriendlyValue, int matchingTextViewId, boolean setActiveColor){
		View rootView = this.getView();
		TextView textView = (TextView)(rootView.findViewById(matchingTextViewId));
		textView.setText(userFriendlyValue);
		if(setActiveColor){
			this.setActiveColor(textView);
		}
	}
	
	private void setActiveColor(TextView textView){		
		// int color = this.getResources().getColor(android.R.color.holo_blue_light);
		// textView.setTextColor(color);
		textView.setTextColor(Color.BLACK);
		// android.R.color.
	}
	
	private int calculateAge(Date birthDate){
		Calendar dob = Calendar.getInstance();  
		dob.setTime(birthDate);  
		Calendar today = Calendar.getInstance();  
		int age = today.get(Calendar.YEAR) - dob.get(Calendar.YEAR);  
		if (today.get(Calendar.MONTH) < dob.get(Calendar.MONTH)) {
		  age--;  
		} 
		else if (today.get(Calendar.MONTH) == dob.get(Calendar.MONTH)
		    && today.get(Calendar.DAY_OF_MONTH) < dob.get(Calendar.DAY_OF_MONTH)) {
		  age--;  
		}
		
		return age;
	}
	
	/*
	private void getFacebookUser(Session session, final String fieldName, final int userSettingsKeyForField, final int matchingTextViewId){	
		//make request to the /me API
		Log.v("PersonalDetailsFragment", "Retrieving Facebook user info...");
		Request request = Request.newMeRequest(session, new Request.GraphUserCallback() {
			// callback after Graph API response with user object
			@Override
			public void onCompleted(GraphUser user, Response response) {				
				if(response != null && response.getError() != null){
					GingerHelpers.showErrorMessage(PersonalDetailsFragment.this.getActivity(), response.getError().getErrorMessage());
				}
				else if (user == null) {
					Log.e("PersonalDetailsFragment", "Facebook user retrieved from 'Me' request is null");
					GingerHelpers.showErrorMessage(PersonalDetailsFragment.this.getActivity(), "We couldn't retrieve your data from Facebook");					
				}
				else {
					
					// log
					Log.v("PersonalDetailsFragment", "Facebook user: " + user.getName());
					
					// Create the GraphObject from the response
		            GraphObject responseGraphObject = response.getGraphObject();

		            // Create the JSON object
		            JSONObject json = responseGraphObject.getInnerJSONObject();					
					
		            // get details
					PersonalDetailsFragment.this.onFacebookUserRetrieval(json, fieldName, userSettingsKeyForField, matchingTextViewId);
				}
			}
		});
		
		Request.executeBatchAsync(request);
	}
	
	private void onFacebookUserRetrieval(JSONObject userJSON, String fieldName, int userSettingsKeyForField, int matchingTextViewId){
		
		String value = this.getFieldValueFromJSON(fieldName, userJSON);	
		String userFriendlyValue = this.getUserFriendlyFieldValue(fieldName, value);
		if(value == null || value.isEmpty() || userFriendlyValue == null || userFriendlyValue.isEmpty()){
			return;
		}
		
		SharedPreferences prefs = this.getActivity().getSharedPreferences(
			getString(R.string.pref_filename_key), Context.MODE_PRIVATE);
		
		SharedPreferences.Editor editor = prefs.edit();		
		editor.putString(this.getString(userSettingsKeyForField), value);
		editor.commit();
		
		View rootView = this.getView();
		TextView textView = (TextView)(rootView.findViewById(matchingTextViewId));
		textView.setText(userFriendlyValue);
		this.setActiveColor(textView);
	}
	
	private String getUserFriendlyFieldValue(String fieldName, String fieldValue){
		
		if(fieldValue == null || fieldValue.isEmpty()){
			return null;
		}
		else if(fieldName.compareToIgnoreCase("birthday") == 0){
			try{
				Log.v("PersonalDetailsFragment", "birthday: " + fieldValue);
				DateFormat format = new SimpleDateFormat("yyyy-MM-dd");
				Date birthDate =  format.parse(fieldValue);
				int age = this.calculateAge(birthDate);
				String ageText = Integer.toString(age);
				Log.v("PersonalDetailsFragment", "Age: " + ageText);
				return ageText;
			}
			catch(Throwable ex){
				Log.e("PersonalDetailsFragment", ex.toString());
				return null;
			}
		}
		else{
			return fieldValue;
		}
	}
	
	
	private String getFieldValueFromJSON(String fieldName, JSONObject userJSON){
		
		if(userJSON == null){
			return null;
		}		 
		else if(fieldName.compareToIgnoreCase("birthday") == 0){
			try{
				String birthday = userJSON.getString("birthday");
				DateFormat format = new SimpleDateFormat("MM/dd/yyyy");
				Date birthDate =  format.parse(birthday);
				
				DateFormat format2 = new SimpleDateFormat("yyyy-MM-dd");
				return format2.format(birthDate);
			}
			catch(Throwable ex){
				Log.e("PersonalDetailsFragment", ex.toString());
				return null;
			}
		}
		else if(fieldName.compareToIgnoreCase("hometown") == 0){
			try{
				String hometown = userJSON.getJSONObject("hometown").getString("name");
				if(hometown.contains(",")){
					hometown = hometown.split(",")[0];
				}
				return hometown;
			}
			catch(Throwable ex){
				Log.e("PersonalDetailsFragment", ex.toString());
				return null;
			}
		}
		else if(fieldName.compareToIgnoreCase("relationship_status") == 0){
			try{
				return userJSON.getString("relationship_status");
			}
			catch(Throwable ex){
				Log.e("PersonalDetailsFragment", ex.toString());
				return null;
			}
		}
		else if(fieldName.compareToIgnoreCase("work") == 0){
			try{
				JSONArray workHistory = userJSON.getJSONArray("work");
				String profession = null;
				for(int i=0; i<workHistory.length(); i++){
					try {
						profession = workHistory.getJSONObject(i).getJSONObject("position").getString("name");
						if(profession != null && !profession.isEmpty()){
							break;
						}
					}
					catch(Throwable e){}
				}
				return profession;
			}
			catch(Throwable ex){
				Log.e("PersonalDetailsFragment", ex.toString());
				return null;
			}
		}
		else if(fieldName.compareToIgnoreCase("education") == 0){
			try{
				JSONArray educationHistory = userJSON.getJSONArray("education");
				String educationlevel = null;
				for(int i=educationHistory.length()-1; i>=0; i--){
					try {
						educationlevel = educationHistory.getJSONObject(i).getString("type");
						if(educationlevel != null && !educationlevel.isEmpty()){
							if(educationlevel.compareToIgnoreCase("Graduate School") == 0){
								// quick fix
								educationlevel = "Master";
							}
							break;
						}
					}
					catch(Throwable e){}
				}
				
				return educationlevel;
			}
			catch(Throwable ex){
				Log.e("PersonalDetailsFragment", ex.toString());
				return null;
			}
		}
		
		return null;
	}
	*/	
}
