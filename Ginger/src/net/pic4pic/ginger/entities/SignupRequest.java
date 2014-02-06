package net.pic4pic.ginger.entities;

import com.google.gson.annotations.SerializedName;

public class SignupRequest extends UserCredentials {
	
	@SerializedName("FacebookUserId")
    protected long facebookUserId;

	@SerializedName("FacebookAccessToken")
	protected String facebookAccessToken;

	@SerializedName("PhotoUploadReference")
	protected String photoUploadReference;

	/**
	 * @return the facebookUserId
	 */
	public long getFacebookUserId() {
		return facebookUserId;
	}

	/**
	 * @param facebookUserId the facebookUserId to set
	 */
	public void setFacebookUserId(long facebookUserId) {
		this.facebookUserId = facebookUserId;
	}

	/**
	 * @return the facebookAccessToken
	 */
	public String getFacebookAccessToken() {
		return facebookAccessToken;
	}

	/**
	 * @param facebookAccessToken the facebookAccessToken to set
	 */
	public void setFacebookAccessToken(String facebookAccessToken) {
		this.facebookAccessToken = facebookAccessToken;
	}

	/**
	 * @return the photoUploadReference
	 */
	public String getPhotoUploadReference() {
		return photoUploadReference;
	}

	/**
	 * @param photoUploadReference the photoUploadReference to set
	 */
	public void setPhotoUploadReference(String photoUploadReference) {
		this.photoUploadReference = photoUploadReference;
	}
}
