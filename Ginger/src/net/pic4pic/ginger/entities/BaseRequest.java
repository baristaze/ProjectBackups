package net.pic4pic.ginger.entities;

import com.google.gson.annotations.SerializedName;

import java.util.UUID;

public class BaseRequest {
	
	@SerializedName("ClientId")
    protected UUID clientId;

	/**
	 * @return the clientId
	 */
	public UUID getClientId() {
		return clientId;
	}

	/**
	 * @param clientId the clientId to set
	 */
	public void setClientId(UUID clientId) {
		this.clientId = clientId;
	}    
}
