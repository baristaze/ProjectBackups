package net.pic4pic.ginger.entities;

import com.google.gson.annotations.SerializedName;

public class ImageFile extends MetaFile{
    
	@SerializedName("Width")
	protected int width;

	@SerializedName("Height")
	protected int height;
		
	@SerializedName("IsBlurred")
	protected boolean blurred;
	
	public int getWidth(){
		return this.width;
	}
	
	public int getHeight(){
		return this.height;
	}
	
	public boolean isBlurred(){
		return this.blurred;
	}
	
	public void setWidth(int width){
		this.width = width;
	}
	
	public void setHeight(int height){
		this.height = height;
	}
	
	public void setBlurred(boolean blurred){
		this.blurred = blurred;
	}
}
