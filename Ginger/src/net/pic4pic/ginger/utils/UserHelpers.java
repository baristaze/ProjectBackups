package net.pic4pic.ginger.utils;

import java.text.DateFormat;
import java.text.SimpleDateFormat;

import android.app.Activity;
import android.content.Context;
import android.content.SharedPreferences;

import net.pic4pic.ginger.R;
import net.pic4pic.ginger.entities.EducationLevel;
import net.pic4pic.ginger.entities.MaritalStatus;
import net.pic4pic.ginger.entities.UserCredentials;
import net.pic4pic.ginger.entities.UserProfile;

public class UserHelpers {

	public static void saveUserPropertiesToFile(Activity activity, UserProfile user){
		
		SharedPreferences prefs = activity.getSharedPreferences(activity.getString(R.string.pref_filename_key), Context.MODE_PRIVATE);

		SharedPreferences.Editor editor = prefs.edit();
		editor.putString(activity.getString(R.string.pref_user_gender_key), user.getGender().toString());

		if (user.getHometownCity() != null && user.getHometownCity().trim().length() > 0) {
			editor.putString(activity.getString(R.string.pref_user_hometown_city_key), user.getHometownCity().trim());
		}

		if (user.getMaritalStatus() != MaritalStatus.Unknown) {
			editor.putString(activity.getString(R.string.pref_user_relation_status), user.getMaritalStatus().toString());
		}

		if (user.getProfession() != null && user.getProfession().trim().length() > 0) {
			editor.putString(activity.getString(R.string.pref_user_profession_key), user.getProfession().trim());
		}

		if (user.getEducationLevel() != EducationLevel.Unknown) {
			editor.putString(activity.getString(R.string.pref_user_education_key), user.getEducationLevelAsString());
		}

		if (user.getBirthDay() != null) {

			DateFormat format = new SimpleDateFormat("yyyy-MM-dd");
			String birthDateAsText = format.format(user.getBirthDay());
			editor.putString(activity.getString(R.string.pref_user_birthday_key), birthDateAsText);
		}

		editor.commit();
	}
	
	public static void saveUserCredentialsToFile(Activity activity, UserCredentials credentials, boolean includeSignUpComplete){
		
		SharedPreferences prefs = activity.getSharedPreferences(activity.getString(R.string.pref_filename_key), Context.MODE_PRIVATE);
		
		// set local flag
		SharedPreferences.Editor editor = prefs.edit();		
		editor.putString(activity.getString(R.string.pref_username_key), credentials.getUsername());
		editor.putString(activity.getString(R.string.pref_password_key), credentials.getPassword());
		if(includeSignUpComplete){
			editor.putInt(activity.getString(R.string.pref_signupComplete_key), 1);
		}
		editor.commit();
	}
}
