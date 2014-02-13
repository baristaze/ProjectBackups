package net.pic4pic.ginger.entities;

import com.google.gson.annotations.SerializedName;

public class FacebookRequest extends BaseRequest {
	
	@SerializedName("FacebookUserId")
    protected long facebookUserId;

	@SerializedName("FacebookAccessToken")
	protected String facebookAccessToken;
	
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
}
