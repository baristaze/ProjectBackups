package net.pic4pic.ginger.entities;


import com.google.gson.annotations.*;

public class ImageUploadResponse extends BaseResponse {
	
	@SerializedName("FullImage")
	protected ImageFile fullImage;
	
	@SerializedName("Thumbnail")
	protected ImageFile thumbnail;
	
	public ImageFile getFullImage(){
		return this.fullImage;
	}
	
	public ImageFile getThumbnail(){
		return this.thumbnail;
	}
	
	public void setFullImage(ImageFile fullImage){
		this.fullImage = fullImage;
	}
	
	public void setThumbnail(ImageFile thumbnail){
		this.thumbnail = thumbnail;
	}
}
