package net.pic4pic.ginger;

import java.util.UUID;

import android.content.Context;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.text.Html;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.TextView;

import net.pic4pic.ginger.entities.BaseResponse;
import net.pic4pic.ginger.entities.FacebookRequest;
import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.entities.SignupRequest;
import net.pic4pic.ginger.entities.UserProfile;
import net.pic4pic.ginger.entities.UserResponse;
import net.pic4pic.ginger.service.Service;
import net.pic4pic.ginger.tasks.ITask;
import net.pic4pic.ginger.tasks.ImageDownloadTask;
import net.pic4pic.ginger.tasks.NonBlockedTask;
import net.pic4pic.ginger.tasks.SignupTask;
import net.pic4pic.ginger.utils.FacebookHelpers;
import net.pic4pic.ginger.utils.GingerHelpers;
import net.pic4pic.ginger.utils.MyLog;
import net.pic4pic.ginger.utils.PageAdvancer;
import net.pic4pic.ginger.utils.UserHelpers;

public class FacebookInfoFragment extends Fragment implements SignupTask.SignupListener, FacebookHelpers.FacebookLoginListener {

	private Button continueButton;

	// constructor cannot have any parameters!!!
	public FacebookInfoFragment(/*no parameter here please*/){
	}

	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {

		View rootView = inflater.inflate(R.layout.fragment_facebook_info, container, false);
		this._applyData(rootView);

		this.continueButton = (Button)(rootView.findViewById(R.id.continueButton));
		this.continueButton.setOnClickListener(new OnClickListener(){
			@Override
			public void onClick(View v) {
				// ((PageAdvancer)FacebookInfoFragment.this.getActivity()).moveToNextPage(0);
				FacebookHelpers helpers = new FacebookHelpers();
				helpers.startFacebook(getActivity(), FacebookInfoFragment.this, null);
			}});

		return rootView;
	}

	private void _applyData(View rootView){

		SharedPreferences prefs = this.getActivity().getSharedPreferences(
				getString(R.string.pref_filename_key), Context.MODE_PRIVATE);

		String username = prefs.getString(this.getString(R.string.pref_username_key), "");
		String thumbnailUrl = prefs.getString(this.getString(R.string.pref_user_thumbnail_plain_key), "");
		String thumbnailIdAsText = prefs.getString(this.getString(R.string.pref_user_thumbnail_plain_id_key), "");
		String header = String.format(this.getHeader(), username);
		String info = String.format(this.getInfo(), username);

		TextView headerTextView = (TextView)(rootView.findViewById(R.id.headerTextView));
		headerTextView.setText(Html.fromHtml(header));

		TextView infoTextView = (TextView)(rootView.findViewById(R.id.infoTextView));
		infoTextView.setText(Html.fromHtml(info));

		MyLog.bag().v("Thumbnail Url = " + thumbnailUrl);
		if (thumbnailUrl != null && !thumbnailUrl.isEmpty() && thumbnailIdAsText != null && !thumbnailIdAsText.isEmpty()){
			UUID thumbnailId = UUID.fromString(thumbnailIdAsText);
			ImageView imageView = (ImageView)(rootView.findViewById(R.id.thumbnailImage));
			ImageDownloadTask asyncTask = new ImageDownloadTask(thumbnailId, imageView);
			asyncTask.execute(thumbnailUrl);
		}
	}

	public void applyData(){

		View rootView = this.getView();
		if (rootView != null){
			_applyData(rootView);
		} 
		else{
			MyLog.bag().e("applyData() has been called before onCreateView() of FacebookInfoFragment");
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
	 * DO NOT DELETE
	 // We added such a method on Session.java file which is in Facebook SDK. 
	 // It enables to pass an extra parameter which is permissions... 
	 // The implementation has some extra property set like ".setPermissions(permissions)" 
	 public static Session openActiveSession(Activity activity, boolean allowLoginUI, 
	 	List<String> permissions, StatusCallback callback) { 
	 	return openActiveSession(activity, allowLoginUI, 
	 		new OpenRequest(activity).setPermissions(permissions).setCallback(callback));
	 }
	 */

	@Override
	public void onFacebookUserRetrieval(String facebookAccessToken, long facebookUserId, Object state) {

		SharedPreferences prefs = this.getActivity().getSharedPreferences(getString(R.string.pref_filename_key), Context.MODE_PRIVATE);
		String username = prefs.getString(this.getString(R.string.pref_username_key), "");
		String password = prefs.getString(this.getString(R.string.pref_password_key), "");
		String photoUploadReference = prefs.getString(this.getString(R.string.pref_user_uploadreference_key), "");

		// prepare sign up request
		SignupRequest request = new SignupRequest();
		request.setUsername(username);
		request.setPassword(password);
		request.setFacebookUserId(facebookUserId);
		request.setFacebookAccessToken(facebookAccessToken);
		request.setPhotoUploadReference(photoUploadReference);

		// start sign up
		SignupTask task = new SignupTask(this, this.getActivity(), this.continueButton, request);
		task.execute();
	}

	@Override
	public void onSignup(UserResponse response, SignupRequest request) {

		if (response.getErrorCode() != 0) {
			// "We couldn't verify your data with Facebook"
			MyLog.bag().e(response.getErrorMessage());
			GingerHelpers.showErrorMessage(this.getActivity(), response.getErrorMessage());
		} 
		else {

			UserProfile user = response.getUserProfile();
			UserHelpers.saveUserPropertiesToFile(this.getActivity(), user);
			
			// get facebook friends	in background	
			final FacebookRequest friendsRequest = new FacebookRequest();
			friendsRequest.setClientId(request.getClientId());
			friendsRequest.setFacebookAccessToken(request.getFacebookAccessToken());
			friendsRequest.setFacebookUserId(request.getFacebookUserId());
			NonBlockedTask.SafeRun(new ITask(){
				@Override
				public void perform() {
					try
					{
						BaseResponse response = Service.getInstance().downloadFriends(FacebookInfoFragment.this.getActivity(), friendsRequest);
						if(response.getErrorCode() == 0){
							MyLog.bag().i("Friends retrieved");
						}
						else {
							MyLog.bag().e("Friend request failed: " + response.getErrorMessage());
						}
					}
					catch(GingerException e) {
						
						MyLog.bag().add(e).e("Friend request failed");
					}
				}
			});

			SignupActivity activity = ((SignupActivity) this.getActivity());
			PersonalDetailsFragment nextFragment = (PersonalDetailsFragment) activity.getFragment(SignupActivity.FRAG_INDEX_PERSONAL_DETAILS);
			nextFragment.setUserResponse(response);
			((PageAdvancer) this.getActivity()).moveToNextPage(0);
		}
	}
}
