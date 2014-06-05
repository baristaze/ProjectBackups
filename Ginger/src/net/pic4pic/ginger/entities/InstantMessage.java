package net.pic4pic.ginger.entities;

import java.io.Serializable;
import java.util.Date;
import java.util.UUID;

import com.google.gson.annotations.SerializedName;

public class InstantMessage implements Serializable {
	
	private static final long serialVersionUID = 1;
	
	@SerializedName("Id")
    protected UUID id;

	@SerializedName("UserId1")
	protected UUID userId1;

	@SerializedName("UserId2")
	protected UUID userId2;

	@SerializedName("Content")
	protected String content;

	@SerializedName("SentTimeUTC")
	protected Date sentTimeUTC;

	@SerializedName("ReadTimeUTC")
	protected Date readTimeUTC;
	
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
	 * @return the content
	 */
	public String getContent() {
		return content;
	}

	/**
	 * @param content the content to set
	 */
	public void setContent(String content) {
		this.content = content;
	}

	/**
	 * @return the sentTimeUTC
	 */
	public Date getSentTimeUTC() {
		return sentTimeUTC;
	}

	/**
	 * @param sentTimeUTC the sentTimeUTC to set
	 */
	public void setSentTimeUTC(Date sentTimeUTC) {
		this.sentTimeUTC = sentTimeUTC;
	}

	/**
	 * @return the readTimeUTC
	 */
	public Date getReadTimeUTC() {
		return readTimeUTC;
	}

	/**
	 * @param readTimeUTC the readTimeUTC to set
	 */
	public void setReadTimeUTC(Date readTimeUTC) {
		this.readTimeUTC = readTimeUTC;
	}
}
