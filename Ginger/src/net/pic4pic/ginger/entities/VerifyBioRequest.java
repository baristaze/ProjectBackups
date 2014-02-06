package net.pic4pic.ginger.entities;

import com.google.gson.annotations.SerializedName;

public class VerifyBioRequest extends BaseRequest {

	@SerializedName("FacebookUserId")
    protected long facebookUserId;

	@SerializedName("FacebookAccessToken")
	protected String facebookAccessToken;

	@SerializedName("UserFields")
	protected String userFields;

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
	 * @return the userFields
	 */
	public String getUserFields() {
		return userFields;
	}

	/**
	 * @param userFields the userFields to set
	 */
	public void setUserFields(String userFields) {
		this.userFields = userFields;
	}
	
	public void addToUserFields(String field){
		
		if(field == null){
			return;
		}
		
		field = field.trim();
		if(this.userFields == null){
			this.userFields = field;
		}
		else{
			this.userFields += "," + field;
		}
	}	
}
