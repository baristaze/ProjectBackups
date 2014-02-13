package net.pic4pic.ginger.entities;

import com.google.gson.annotations.SerializedName;

public class VerifyBioRequest extends FacebookRequest {

	@SerializedName("UserFields")
	protected String userFields;

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
