package net.pic4pic.ginger.entities;

import java.io.Serializable;

import com.google.gson.annotations.SerializedName;

public class UserResponse extends BaseResponse implements Serializable {
	
	private static final long serialVersionUID = 1;
	
	@SerializedName("AuthToken")
    protected String authToken;

	@SerializedName("UserProfile")
    protected UserProfile userProfile;

	@SerializedName("ProfilePictures")
    protected UserProfilePics profilePictures;

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
}
