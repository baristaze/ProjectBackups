package net.pic4pic.ginger.entities;

import java.io.Serializable;

import com.google.gson.annotations.SerializedName;

public class UserProfilePics implements Serializable {
	
	private static final long serialVersionUID = 1;
	
	@SerializedName("FullSizeClear")
    protected ImageFile fullSizeClear;

	@SerializedName("FullSizeBlurred")
    protected ImageFile fullSizeBlurred;

	@SerializedName("ThumbnailClear")
    protected ImageFile thumbnailClear;

	@SerializedName("ThumbnailBlurred")
    protected ImageFile thumbnailBlurred;

	/**
	 * @return the fullSizeClear
	 */
	public ImageFile getFullSizeClear() {
		return fullSizeClear;
	}

	/**
	 * @param fullSizeClear the fullSizeClear to set
	 */
	public void setFullSizeClear(ImageFile fullSizeClear) {
		this.fullSizeClear = fullSizeClear;
	}

	/**
	 * @return the fullSizeBlurred
	 */
	public ImageFile getFullSizeBlurred() {
		return fullSizeBlurred;
	}

	/**
	 * @param fullSizeBlurred the fullSizeBlurred to set
	 */
	public void setFullSizeBlurred(ImageFile fullSizeBlurred) {
		this.fullSizeBlurred = fullSizeBlurred;
	}

	/**
	 * @return the thumbnailClear
	 */
	public ImageFile getThumbnailClear() {
		return thumbnailClear;
	}

	/**
	 * @param thumbnailClear the thumbnailClear to set
	 */
	public void setThumbnailClear(ImageFile thumbnailClear) {
		this.thumbnailClear = thumbnailClear;
	}

	/**
	 * @return the thumbnailBlurred
	 */
	public ImageFile getThumbnailBlurred() {
		return thumbnailBlurred;
	}

	/**
	 * @param thumbnailBlurred the thumbnailBlurred to set
	 */
	public void setThumbnailBlurred(ImageFile thumbnailBlurred) {
		this.thumbnailBlurred = thumbnailBlurred;
	}
}
