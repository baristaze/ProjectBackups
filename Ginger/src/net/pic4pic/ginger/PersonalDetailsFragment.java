package net.pic4pic.ginger;

import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.List;

import org.json.JSONArray;
import org.json.JSONObject;

import com.facebook.Request;
import com.facebook.Response;
import com.facebook.Session;
import com.facebook.SessionState;
import com.facebook.model.GraphObject;
import com.facebook.model.GraphUser;

import android.annotation.SuppressLint;
import android.content.Context;
import android.content.SharedPreferences;
import android.graphics.Color;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.TextView;

import net.pic4pic.ginger.tasks.ImageDownloadTask;
import net.pic4pic.ginger.utils.GingerHelpers;

@SuppressLint("ResourceAsColor")
public class PersonalDetailsFragment extends Fragment {
	
	// constructor cannot have any parameters!!!
	public PersonalDetailsFragment(/*no parameter here please*/){
	}
	
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
		
		View rootView = inflater.inflate(R.layout.fragment_personal_details, container, false);
		this._applyData(rootView);
		
		ImageButton buttonAgeEdit = (ImageButton)(rootView.findViewById(R.id.buttonAgeEdit));
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
		
		Button finishButton = (Button)(rootView.findViewById(R.id.finishButton));
		finishButton.setOnClickListener(new OnClickListener(){
			@Override
			public void onClick(View v) {				
				// ((PageAdvancer)PersonalDetailsFragment.this.getActivity()).moveToNextPage(0);				
			}});
		
		return rootView;
	}
	
	public void applyData(){
		
		View rootView = this.getView();
		if(rootView != null){
			_applyData(rootView);
		}	
		else{
			Log.e("PersonalDetailsFragment", "applyData() has been called before onCreateView() of PersonalDetailsFragment");
		}
	}
	
	private void _applyData(View rootView){
		
		SharedPreferences prefs = this.getActivity().getSharedPreferences(
				getString(R.string.pref_filename_key), Context.MODE_PRIVATE);
		
		String username = prefs.getString(this.getString(R.string.pref_username_key), "");	
		String thumbnailUrl = prefs.getString(this.getString(R.string.pref_user_thumbnail_plain_key), "");
		String gender = prefs.getString(this.getString(R.string.pref_user_gender_key), "Unspecified");
		String homeTownCity = prefs.getString(this.getString(R.string.pref_user_hometown_city_key), "required");
		String relationStatus = prefs.getString(this.getString(R.string.pref_user_relation_status), "optional");
		String profession = prefs.getString(this.getString(R.string.pref_user_profession_key), "optional");
		String educationLevel = prefs.getString(this.getString(R.string.pref_user_education_key), "optional");
		
		String age = "required";
		String birthday = prefs.getString(this.getString(R.string.pref_user_birthday_key), "required");
		if(birthday != null && !birthday.isEmpty()){
			try{
				DateFormat format = new SimpleDateFormat("yyyy-MM-dd");
				Date birthDate =  format.parse(birthday);
				int ageInt = this.calculateAge(birthDate);
				age = Integer.toString(ageInt);
			}
			catch(Exception ex){
				Log.e("PersonalDetailsFragment", ex.toString());
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
		
		Log.v("PersonalDetailsFragment", "Thumbnail Url = " + thumbnailUrl);
		if(thumbnailUrl != null && !thumbnailUrl.isEmpty()){
			ImageView imageView = (ImageView)(rootView.findViewById(R.id.thumbnailImage));
			ImageDownloadTask asyncTask = new ImageDownloadTask(imageView);
			asyncTask.execute(thumbnailUrl);
		}		
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
						PersonalDetailsFragment.this.getFacebookUser(session, fieldName, userSettingsKeyForField, matchingTextViewId);
					}					
				}
			}
		});
	}
	
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
		
		/*
		LinearLayout parent = (LinearLayout)textView.getParent();
		if(parent.getChildCount() > 1){
			Log.v("PersonalDetailsFragment", "Removing button for " + fieldName);
			parent.removeViewAt(1);
			parent.invalidate();
		}
		*/
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
			catch(Exception ex){
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
			catch(Exception ex){
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
			catch(Exception ex){
				Log.e("PersonalDetailsFragment", ex.toString());
				return null;
			}
		}
		else if(fieldName.compareToIgnoreCase("relationship_status") == 0){
			try{
				return userJSON.getString("relationship_status");
			}
			catch(Exception ex){
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
					catch(Exception e){}
				}
				return profession;
			}
			catch(Exception ex){
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
					catch(Exception e){}
				}
				
				return educationlevel;
			}
			catch(Exception ex){
				Log.e("PersonalDetailsFragment", ex.toString());
				return null;
			}
		}
		
		return null;
	}
}
