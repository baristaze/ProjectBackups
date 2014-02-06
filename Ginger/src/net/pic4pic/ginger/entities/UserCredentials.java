package net.pic4pic.ginger.entities;

import com.google.gson.annotations.SerializedName;

public class UserCredentials extends BaseRequest {
	
	@SerializedName("Username")
    protected String Username;

	@SerializedName("Password")
    protected String Password;

	/**
	 * @return the user name
	 */
	public String getUsername() {
		return Username;
	}

	/**
	 * @param username the user name to set
	 */
	public void setUsername(String username) {
		Username = username;
	}

	/**
	 * @return the password
	 */
	public String getPassword() {
		return Password;
	}

	/**
	 * @param password the password to set
	 */
	public void setPassword(String password) {
		Password = password;
	}
}
