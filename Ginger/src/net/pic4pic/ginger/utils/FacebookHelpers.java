package net.pic4pic.ginger.utils;

import java.util.ArrayList;
import java.util.List;

import net.pic4pic.ginger.R;

import android.app.Activity;
import android.content.Context;
import android.content.SharedPreferences;

import com.facebook.Request;
import com.facebook.Response;
import com.facebook.Session;
import com.facebook.SessionState;
import com.facebook.model.GraphUser;

public class FacebookHelpers {
	
	public interface FacebookLoginListener{
		public void onFacebookUserRetrieval(String facebookSessionToken, long facebookUserId, Object state);
	}
	
	public void startFacebook(final Activity activity, final FacebookLoginListener listener, final Object callersState){
		// start Facebook Login
		List<String> requiredPermissions = getRequiredFacebookPermissions(true);
		Session.openActiveSession(activity, true, requiredPermissions, new Session.StatusCallback() {
			// callback when session changes state
			@Override
			public void call(Session session, SessionState state, Exception exception) {
				// session is never null indeed...
				if (session != null && session.isOpened()) {
					if (!hasRequiredPermissions(session, false)){
						String errorMessage = "We cannot continue because you have opt'ed out some Facebook permissions.";
						GingerHelpers.showErrorMessage(activity, errorMessage);
						session.closeAndClearTokenInformation();
					} 
					else{
						MyLog.bag().v("Facebook session with required permissions are ready.");
						onFacebookLoginWithPermissions(activity, session, listener, callersState);
					}
				}
			}
		});
	}
	
	private void onFacebookLoginWithPermissions(final Activity activity, final Session session, final FacebookLoginListener listener, final Object state) {

		MyLog.bag().add("FacebookSessionToken", session.getAccessToken()).v();
		// GingerHelpers.toast(FacebookInfoFragment.this.getActivity(),
		// "Logged in to Facebook!");

		// make request to the /me API
		MyLog.bag().v("Retrieving Facebook user info...");
		Request request = Request.newMeRequest(session,
			new Request.GraphUserCallback() {
				// callback after Graph API response with user object
				@Override
				public void onCompleted(GraphUser user, Response response) {
					if (response != null && response.getError() != null) {
						GingerHelpers.showErrorMessage(activity, response.getError().getErrorMessage());
					} 
					else if (user == null) {
						MyLog.bag().e("Facebook user retrieved from 'Me' request is null");
						GingerHelpers.showErrorMessage(activity, "We couldn't retrieve your data from Facebook");
					} 
					else {
						MyLog.bag().add("FacebookUserName", user.getName()).v("");
						// GingerHelpers.toast(FacebookInfoFragment.this.getActivity(),
						// "You are " + user.getName());
						long facebookUserId = Long.parseLong(user.getId());
						
						SharedPreferences prefs = activity.getSharedPreferences(activity.getString(R.string.pref_filename_key), Context.MODE_PRIVATE);
						
						SharedPreferences.Editor editor = prefs.edit();
						editor.putString(activity.getString(R.string.pref_user_facebookid_key), session.getAccessToken());
						editor.putLong(activity.getString(R.string.pref_user_facebooktoken_key), facebookUserId);
						editor.commit();
						
						listener.onFacebookUserRetrieval(session.getAccessToken(), facebookUserId, state);
					}
				}
			});

		Request.executeBatchAsync(request);
	}
	
	protected List<String> getRequiredFacebookPermissions(boolean includeOptionals) {
		List<String> permissions = new ArrayList<String>();
		permissions.add("email");
		/*
		// gender doesn't require any permission
		permissions.add("user_hometown"); // field = hometown
		permissions.add("user_birthday"); // field = birthday
		permissions.add("user_work_history"); // field = work
		permissions.add("user_education_history"); // field = education
		permissions.add("user_relationships"); // field = relationship_status
		*/
		// permissions.add("user_religion_politics"); // field =
		// religion,political
		return permissions;
	}
	
	/*
	 * private void requestMorePermissions(Session session){ List<String>
	 * permissions = this.getRequiredFacebookPermissions(true);
	 * session.requestNewReadPermissions(new
	 * Session.NewPermissionsRequest(this.getActivity(), permissions)); }
	 */

	protected boolean hasRequiredPermissions(Session session, boolean checkOptionals) {
		List<String> requiredPermissions = getRequiredFacebookPermissions(checkOptionals);
		List<String> existingPermissions = session.getPermissions();
		for (String reqPerm : requiredPermissions) {
			if (!existingPermissions.contains(reqPerm)) {
				MyLog.bag().add("UnauthorizedFacebookPermissoon", reqPerm).v("Permission doesn't exist: " + reqPerm);
				return false;
			}
		}

		return true;
	}
}
