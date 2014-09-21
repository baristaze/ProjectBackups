package net.pic4pic.ginger.entities;

import java.io.Serializable;
import java.util.UUID;

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
	
	/**
	 * @return
	 */
	public boolean hasAnyClearPicture(){
		
		if(this.fullSize != null && !this.fullSize.isBlurred()){
			return true;
		}
		
		if(this.thumbnail != null && !this.thumbnail.isBlurred()){
			return true;
		}
		
		return false;
	}
	
	public UUID getGroupingImageId(){
		
		UUID emptyGuid = new UUID(0, 0);
		
		if(this.fullSize != null && !this.fullSize.getGroupingId().equals(emptyGuid)){
			return this.fullSize.getGroupingId();
		}
		
		if(this.thumbnail != null && !this.thumbnail.getGroupingId().equals(emptyGuid)){
			return this.thumbnail.getGroupingId();
		}
		
		return emptyGuid;
	}
}
