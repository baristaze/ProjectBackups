package net.pic4pic.ginger.entities;

import java.io.Serializable;

import com.google.gson.annotations.SerializedName;

public class ImageUploadResponse extends BaseResponse implements Serializable {
	
	private static final long serialVersionUID = 1;
	
	@SerializedName("Images")
	protected UserProfilePics images;

	@SerializedName("UploadReference")
	protected String uploadReference;

	/**
	 * @return the images
	 */
	public UserProfilePics getImages() {
		return images;
	}

	/**
	 * @param images the images to set
	 */
	public void setImages(UserProfilePics images) {
		this.images = images;
	}

	/**
	 * @return the uploadReference
	 */
	public String getUploadReference() {
		return uploadReference;
	}

	/**
	 * @param uploadReference the uploadReference to set
	 */
	public void setUploadReference(String uploadReference) {
		this.uploadReference = uploadReference;
	}
}
