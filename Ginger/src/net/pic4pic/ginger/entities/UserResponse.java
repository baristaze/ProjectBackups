package net.pic4pic.ginger.entities;

import java.io.Serializable;
import java.util.ArrayList;
import java.util.UUID;

import com.google.gson.annotations.SerializedName;

public class UserResponse extends BaseResponse implements Serializable {
	
	private static final long serialVersionUID = 1;
	
	private static final String FacebookPermissionsMin = "email";
	private static final String SettingKey_InitialFacebookPermissions = "InitialFacebookPermissions";
	
	@SerializedName("AuthToken")
    protected String authToken;

	@SerializedName("UserProfile")
    protected UserProfile userProfile;

	@SerializedName("ProfilePictures")
    protected UserProfilePics profilePictures;

	@SerializedName("OtherPictures")
    protected ArrayList<PicturePair> otherPictures = new ArrayList<PicturePair>();
	
	@SerializedName("Settings")
	protected ArrayList<NameValuePair> settings = new ArrayList<NameValuePair>();
	
	/**
	 * Shortcut getter
	 * @return userId of the profile
	 */
	public UUID getUserId(){
		return this.getUserProfile().getUserId();
	}
	
	/**
	 * @return the authToken
	 */
	public String getAuthToken() {
		return authToken;
	}

	/**
	 * @param authToken the authToken to set
	 */
	public void setAuthToken(String authToken) {
		this.authToken = authToken;
	}

	/**
	 * @return the userProfile
	 */
	public UserProfile getUserProfile() {
		return userProfile;
	}

	/**
	 * @param userProfile the userProfile to set
	 */
	public void setUserProfile(UserProfile userProfile) {
		this.userProfile = userProfile;
	}

	/**
	 * @return the profilePictures
	 */
	public UserProfilePics getProfilePictures() {
		return profilePictures;
	}

	/**
	 * @param profilePictures the profilePictures to set
	 */
	public void setProfilePictures(UserProfilePics profilePictures) {
		this.profilePictures = profilePictures;
	}
	
	/**
	 * @return the otherPictures
	 */
	public ArrayList<PicturePair> getOtherPictures() {
		return otherPictures;
	}
	
	/**
	 * @return the settings
	 */
	public ArrayList<NameValuePair> getSettings() {
		return settings;
	}
	
	public ArrayList<String> getInitialFacebookPermissions(){
		
		ArrayList<String> list = new ArrayList<String>();		
		
		if(this.settings != null && this.settings.size() > 0){			
			for(NameValuePair pair : this.settings){
				if(pair.name != null && SettingKey_InitialFacebookPermissions.equals(pair.name.trim())){
					list = splitAsFacebookPermissionsOrDefault(pair.getValue());
				}
			}
		}
		
		if(list.size() == 0){
			list.add(FacebookPermissionsMin);
		}	
		
		return list;
	}
	
	public String getInitialFacebookPermissionsAndConcat(){
		
		ArrayList<String> list = this.getInitialFacebookPermissions();
		StringBuilder builder = new StringBuilder();
		for(int x=0; x<list.size(); x++){
			builder.append(list.get(x));
			if(x != (list.size()-1)){
				builder.append(",");
			}
		}
		
		return builder.toString();
	}
	
	public static ArrayList<String> splitAsFacebookPermissionsOrDefault(String concatenated){
		
		ArrayList<String> list = new ArrayList<String>();		
		
		if(concatenated != null && concatenated.trim().length() > 0){
			concatenated = concatenated.trim();
			String[] tokens = concatenated.split(",");
			for(String token : tokens){
				if(token != null && token.trim().length() > 0 && !token.trim().equals(",")){
					list.add(token.trim());
				}
			}
		}
		
		if(list.size() == 0){
			list.add(FacebookPermissionsMin);
		}	
		
		return list;
	}
	
}
