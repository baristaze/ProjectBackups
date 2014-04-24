package net.pic4pic.ginger.entities;

import java.io.Serializable;
import com.google.gson.annotations.SerializedName;

public class PicturePair  implements Serializable {
	
	private static final long serialVersionUID = 1;
	
	@SerializedName("FullSize")
    protected ImageFile fullSize;
	
	@SerializedName("Thumbnail")
    protected ImageFile thumbnail;

	/**
	 * @return the fullSize
	 */
	public ImageFile getFullSize() {
		return fullSize;
	}

	/**
	 * @param fullSize the fullSize to set
	 */
	public void setFullSize(ImageFile fullSize) {
		this.fullSize = fullSize;
	}

	/**
	 * @return the thumb-nail
	 */
	public ImageFile getThumbnail() {
		return thumbnail;
	}

	/**
	 * @param thumbnail the thumb-nail to set
	 */
	public void setThumbnail(ImageFile thumbnail) {
		this.thumbnail = thumbnail;
	}
}
