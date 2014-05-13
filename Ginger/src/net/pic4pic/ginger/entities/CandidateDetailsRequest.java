package net.pic4pic.ginger.entities;

import java.util.UUID;

import com.google.gson.annotations.SerializedName;

public class CandidateDetailsRequest extends BaseRequest {
	
	@SerializedName("UserId")
    protected UUID userId;
	
	/**
	 * @return the userId
	 */
	public UUID getUserId() {
		return userId;
	}

	/**
	 * @param userId the userId to set
	 */
	public void setUserId(UUID userId) {
		this.userId = userId;
	}
}
