package net.pic4pic.ginger;

import java.util.ArrayList;
import java.util.List;

import android.content.Context;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.text.Html;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.View.OnClickListener;
import android.widget.Button; 
import android.widget.ImageView;
import android.widget.TextView;

import com.facebook.*;
import com.facebook.model.GraphUser;

import net.pic4pic.ginger.tasks.ImageDownloadTask;
import net.pic4pic.ginger.utils.GingerHelpers;
import net.pic4pic.ginger.utils.PageAdvancer;

public class FacebookInfoFragment extends Fragment {
	
	// constructor cannot have any parameters!!!
	public FacebookInfoFragment(/*no parameter here please*/){
	}
	
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
		
		View rootView = inflater.inflate(R.layout.fragment_facebook_info, container, false);
		this._applyData(rootView);
		
		Button continueButton = (Button)(rootView.findViewById(R.id.continueButton));
		continueButton.setOnClickListener(new OnClickListener(){
			@Override
			public void onClick(View v) {				
				// ((PageAdvancer)FacebookInfoFragment.this.getActivity()).moveToNextPage(0);
				FacebookInfoFragment.this.startFacebook();
			}});
		
		return rootView;
	}
	
	private void _applyData(View rootView){
		
		SharedPreferences prefs = this.getActivity().getSharedPreferences(
				getString(R.string.pref_filename_key), Context.MODE_PRIVATE);
		
		String username = prefs.getString(this.getString(R.string.pref_username_key), "");	
		String thumbnailUrl = prefs.getString(this.getString(R.string.pref_user_thumbnail_plain_key), "");
		String header = String.format(this.getHeader(), username);
		String info = String.format(this.getInfo(), username);
		
		TextView headerTextView = (TextView)(rootView.findViewById(R.id.headerTextView));
		headerTextView.setText(Html.fromHtml(header));		
		
		TextView infoTextView = (TextView)(rootView.findViewById(R.id.infoTextView));
		infoTextView.setText(Html.fromHtml(info));
		
		Log.v("FacebookInfoFragment", "Thumbnail Url = " + thumbnailUrl);
		if(thumbnailUrl != null && !thumbnailUrl.isEmpty()){
			ImageView imageView = (ImageView)(rootView.findViewById(R.id.thumbnailImage));
			ImageDownloadTask asyncTask = new ImageDownloadTask(imageView);
			asyncTask.execute(thumbnailUrl);
		}		
	}
	
	public void applyData(){
		
		View rootView = this.getView();
		if(rootView != null){
			_applyData(rootView);
		}	
		else{
			Log.e("FacebookInfoFragment", "applyData() has been called before onCreateView() of FacebookInfoFragment");
		}
	}
	
	private String getHeader(){
		return "Congrats <strong><font color=\"#FF4444\">%s</font></strong> !<br/>";
	}
	
	private String getInfo(){
		StringBuilder s = new StringBuilder();
		s.append("<br/>");
		s.append("You have verified your face picture.<br/>");
		s.append("<br/>");
		s.append("Remember that all people on this system get their info verified. ");
		s.append("Have you ever wondered how we achieve it? We use Facebook as a verification method for your bio.<br/>");
		s.append("<br/>");
		s.append("Don’t panic! ");
		s.append("<strong>We never post on Facebook! </strong>"); 
		s.append("Remember that this app is all about privacy and verified info. ");
		s.append("We will take this step gracefully so that your privacy is not compromised. ");
		s.append("We need this permission only once during this setup and then we are done with Facebook forever!");
		
        return s.toString();
	}
	
	/*
	 *  DO NOT DELETE
	// We added such a method on Session.java file which is in Facebook SDK.
	// It enables to pass an extra parameter which is permissions...
	// The implementation has some extra property set like ".setPermissions(permissions)"
	public static Session openActiveSession(Activity activity, boolean allowLoginUI, 
    		List<String> permissions, StatusCallback callback) {
    	return openActiveSession(activity, allowLoginUI, 
    			new OpenRequest(activity).setPermissions(permissions).setCallback(callback));
    }	
	*/
	
	private void startFacebook(){
		// start Facebook Login
		List<String> requiredPermissions = this.getRequiredFacebookPermissions(true);
		Session.openActiveSession(this.getActivity(), true, requiredPermissions, new Session.StatusCallback() {
			// callback when session changes state
			@Override
			public void call(Session session, SessionState state, Exception exception) {
				// session is never null indeed...
				if (session != null && session.isOpened()) {
					if(!FacebookInfoFragment.this.hasRequiredPermissions(session, false)){
						String errorMessage = "We cannot continue because you have opt'ed out some Facebook permissions.";							
						GingerHelpers.showErrorMessage(FacebookInfoFragment.this.getActivity(), errorMessage);
						session.closeAndClearTokenInformation();
					}
					else{
						Log.v("FacebookInfoFragment", "Facebook session with required permissions are ready.");
						FacebookInfoFragment.this.onFacebookLoginWithPermissions(session);
					}					
				}
			}
		});
	}
	
	private List<String> getRequiredFacebookPermissions(boolean includeOptionals){
		List<String> permissions = new ArrayList<String>();
        permissions.add("email");
        /*
        permissions.add("user_hometown");				// field = hometown
        permissions.add("user_birthday");				// field = birthday
        permissions.add("user_work_history");			// field = work
        permissions.add("user_education_history");		// field = education
        permissions.add("user_relationships");			// field = relationship_status
        */
        // permissions.add("user_religion_politics");		// field = religion,political
        return permissions;
	}
	
	/*
	private void requestMorePermissions(Session session){
		List<String> permissions = this.getRequiredFacebookPermissions(true);
        session.requestNewReadPermissions(new Session.NewPermissionsRequest(this.getActivity(), permissions));
    }
	*/
	
	private boolean hasRequiredPermissions(Session session, boolean checkOptionals) {		
		List<String> requiredPermissions = this.getRequiredFacebookPermissions(checkOptionals);		
		List<String> existingPermissions = session.getPermissions();
		for(String reqPerm : requiredPermissions){
			if(!existingPermissions.contains(reqPerm)){
				Log.v("FacebookInfoFragment", reqPerm + " permission doesn't exist");
	        	return false;
	        }
		}
        
        return true;
    }
	
	private void onFacebookLoginWithPermissions(Session session){
		Log.v("FacebookInfoFragment", "Facebook token: " + session.getAccessToken());
		// GingerHelpers.toast(FacebookInfoFragment.this.getActivity(), "Logged in to Facebook!");
		this.getFacebookUserAsTest(session);
	}
	
	private void getFacebookUserAsTest(Session session){	
		//make request to the /me API
		Log.v("FacebookInfoFragment", "Retrieving Facebook user info...");
		Request request = Request.newMeRequest(session, new Request.GraphUserCallback() {
			// callback after Graph API response with user object
			@Override
			public void onCompleted(GraphUser user, Response response) {				
				if(response != null && response.getError() != null){
					GingerHelpers.showErrorMessage(FacebookInfoFragment.this.getActivity(), response.getError().getErrorMessage());
				}
				else if (user == null) {
					Log.e("FacebookInfoFragment", "Facebook user retrieved from 'Me' request is null");
					GingerHelpers.showErrorMessage(FacebookInfoFragment.this.getActivity(), "We couldn't retrieve your data from Facebook");					
				}
				else {
					Log.v("FacebookInfoFragment", "Facebook user: " + user.getName());
					//GingerHelpers.toast(FacebookInfoFragment.this.getActivity(), "You are " + user.getName());
					FacebookInfoFragment.this.onFacebookUserRetrieval(user);
				}
			}
		});
		
		Request.executeBatchAsync(request);
	}
	
	private String getGenderAsString(Object o){
		if(o == null){
			return "Unspecified";
		}
		
		if(o.toString().compareToIgnoreCase("Male") == 0){
			return "Male";
		}
		
		if(o.toString().compareToIgnoreCase("Female") == 0){
			return "Female";
		}
		
		return "Unspecified";
	} 
	
	private void onFacebookUserRetrieval(GraphUser user){
		
		String gender = this.getGenderAsString(user.getProperty("gender"));
		
		SharedPreferences prefs = this.getActivity().getSharedPreferences(
			getString(R.string.pref_filename_key), Context.MODE_PRIVATE);
		
		SharedPreferences.Editor editor = prefs.edit();		
		editor.putString(this.getString(R.string.pref_user_gender_key), gender);
		editor.commit();
		
		((PageAdvancer)this.getActivity()).moveToNextPage(0);
	}
}
