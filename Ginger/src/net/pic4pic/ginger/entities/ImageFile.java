package net.pic4pic.ginger.entities;

import java.io.Serializable;
import java.util.Date;
import java.util.UUID;

import com.google.gson.annotations.SerializedName;

public class ImageFile implements Serializable {
	
	private static final long serialVersionUID = 1;
	
	@SerializedName("Id")
	protected UUID id;
	
	@SerializedName("GroupingId")
    protected UUID groupingId;
	
	@SerializedName("UserId")
    protected UUID userId;

    @SerializedName("Status")
	protected AssetState status;

	@SerializedName("ContentType")
	protected String contentType;
    
	@SerializedName("ContentLength")
	protected int contentLength;

	@SerializedName("Width")
	protected int width;

	@SerializedName("Height")
	protected int height;

	@SerializedName("CloudUrl")
	protected String cloudUrl;

	@SerializedName("IsBlurred")
	protected boolean blurred;
	
	@SerializedName("IsThumbnail")
	protected boolean thumbnailed;

	@SerializedName("IsProfilePicture")
	protected boolean forProfile;

	@SerializedName("CreateTimeUTC")
	protected Date createTimeUTC;

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
	 * @return the groupingId
	 */
	public UUID getGroupingId() {
		return groupingId;
	}

	/**
	 * @param groupingId the groupingId to set
	 */
	public void setGroupingId(UUID groupingId) {
		this.groupingId = groupingId;
	}

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

	/**
	 * @return the status
	 */
	public AssetState getStatus() {
		return status;
	}

	/**
	 * @param status the status to set
	 */
	public void setStatus(AssetState status) {
		this.status = status;
	}

	/**
	 * @return the contentType
	 */
	public String getContentType() {
		return contentType;
	}

	/**
	 * @param contentType the contentType to set
	 */
	public void setContentType(String contentType) {
		this.contentType = contentType;
	}

	/**
	 * @return the contentLength
	 */
	public int getContentLength() {
		return contentLength;
	}

	/**
	 * @param contentLength the contentLength to set
	 */
	public void setContentLength(int contentLength) {
		this.contentLength = contentLength;
	}

	/**
	 * @return the width
	 */
	public int getWidth() {
		return width;
	}

	/**
	 * @param width the width to set
	 */
	public void setWidth(int width) {
		this.width = width;
	}

	/**
	 * @return the height
	 */
	public int getHeight() {
		return height;
	}

	/**
	 * @param height the height to set
	 */
	public void setHeight(int height) {
		this.height = height;
	}

	/**
	 * @return the cloudUrl
	 */
	public String getCloudUrl() {
		return cloudUrl;
	}

	/**
	 * @param cloudUrl the cloudUrl to set
	 */
	public void setCloudUrl(String cloudUrl) {
		this.cloudUrl = cloudUrl;
	}

	/**
	 * @return the blurred
	 */
	public boolean isBlurred() {
		return blurred;
	}

	/**
	 * @param blurred the blurred to set
	 */
	public void setBlurred(boolean blurred) {
		this.blurred = blurred;
	}

	/**
	 * @return the flag indicating whether the image is a thumb nail image
	 */
	public boolean isThumbnailed() {
		return thumbnailed;
	}

	/**
	 * @param thumbnailed the flag to set
	 */
	public void setThumbnailed(boolean thumbnailed) {
		this.thumbnailed = thumbnailed;
	}

	/**
	 * @return the forProfile
	 */
	public boolean isForProfile() {
		return forProfile;
	}

	/**
	 * @param forProfile the forProfile to set
	 */
	public void setForProfile(boolean forProfile) {
		this.forProfile = forProfile;
	}

	/**
	 * @return the createTimeUTC
	 */
	public Date getCreateTimeUTC() {
		return createTimeUTC;
	}

	/**
	 * @param createTimeUTC the createTimeUTC to set
	 */
	public void setCreateTimeUTC(Date createTimeUTC) {
		this.createTimeUTC = createTimeUTC;
	}
}
