package net.pic4pic.ginger.entities;

import java.util.UUID;

import com.google.gson.annotations.SerializedName;

public class MarkingRequest extends BaseRequest {

	@SerializedName("MarkingType")
    protected MarkingType markingType;
	
	@SerializedName("ObjectType")
    protected ObjectType objectType;
	
	@SerializedName("ObjectId")
    protected UUID objectId;

	/**
	 * @return the markingType
	 */
	public MarkingType getMarkingType() {
		return markingType;
	}

	/**
	 * @param markingType the markingType to set
	 */
	public void setMarkingType(MarkingType markingType) {
		this.markingType = markingType;
	}

	/**
	 * @return the objectType
	 */
	public ObjectType getObjectType() {
		return objectType;
	}

	/**
	 * @param objectType the objectType to set
	 */
	public void setObjectType(ObjectType objectType) {
		this.objectType = objectType;
	}

	/**
	 * @return the objectId
	 */
	public UUID getObjectId() {
		return objectId;
	}

	/**
	 * @param objectId the objectId to set
	 */
	public void setObjectId(UUID objectId) {
		this.objectId = objectId;
	}
}
