package net.pic4pic.ginger.entities;

import java.io.Serializable;
import java.util.Date;
import java.util.UUID;

import com.google.gson.annotations.SerializedName;

public class PicForPic implements Serializable {
	
	private static final long serialVersionUID = 1;
	
	@SerializedName("Id")
    protected UUID id;

	@SerializedName("UserId1")
	protected UUID userId1;

	@SerializedName("UserId2")
	protected UUID userId2;

	@SerializedName("PicId1")
	protected UUID picId1;

	@SerializedName("PicId2")
	protected UUID picId2;

	@SerializedName("RequestTimeUTC")
	protected Date requestTimeUTC;

	@SerializedName("AcceptTimeUTC")
	protected Date acceptTimeUTC;
	
	/**
	 * @return the id
	 */
	public UUID getId() {
		return id;
	}

	/**
	 * @param id the id to set
	 */
	public void setId(UUID id) {
		this.id = id;
	}

	/**
	 * @return the userId1
	 */
	public UUID getUserId1() {
		return userId1;
	}

	/**
	 * @param userId1 the userId1 to set
	 */
	public void setUserId1(UUID userId1) {
		this.userId1 = userId1;
	}

	/**
	 * @return the userId2
	 */
	public UUID getUserId2() {
		return userId2;
	}

	/**
	 * @param userId2 the userId2 to set
	 */
	public void setUserId2(UUID userId2) {
		this.userId2 = userId2;
	}

	/**
	 * @return the picId1
	 */
	public UUID getPicId1() {
		return picId1;
	}

	/**
	 * @param picId1 the picId1 to set
	 */
	public void setPicId1(UUID picId1) {
		this.picId1 = picId1;
	}

	/**
	 * @return the picId2
	 */
	public UUID getPicId2() {
		return picId2;
	}

	/**
	 * @param picId2 the picId2 to set
	 */
	public void setPicId2(UUID picId2) {
		this.picId2 = picId2;
	}

	/**
	 * @return the requestTimeUTC
	 */
	public Date getRequestTimeUTC() {
		return requestTimeUTC;
	}

	/**
	 * @param requestTimeUTC the requestTimeUTC to set
	 */
	public void setRequestTimeUTC(Date requestTimeUTC) {
		this.requestTimeUTC = requestTimeUTC;
	}

	/**
	 * @return the acceptTimeUTC
	 */
	public Date getAcceptTimeUTC() {
		return acceptTimeUTC;
	}

	/**
	 * @param acceptTimeUTC the acceptTimeUTC to set
	 */
	public void setAcceptTimeUTC(Date acceptTimeUTC) {
		this.acceptTimeUTC = acceptTimeUTC;
	}
	
	/**
	 * @return whether this pic4pic is accepted or not
	 */
    public boolean isAccepted() {
        return this.getAcceptTimeUTC() != null && !this.getAcceptTimeUTC().equals(new Date(0));
    }
}
