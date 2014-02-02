package net.pic4pic.ginger.entities;

public class ImageUploadRequest {
	
	protected String fullLocalPath;
	protected int width;
	protected int height;
	
	public String getFullLocalPath(){
		return this.fullLocalPath;
	}
	
	public int getWidth(){
		return this.width;
	}
	
	public int getHeight(){
		return this.height;
	}
	
	public void setFullLocalPath(String path){
		this.fullLocalPath = path;
	}
	
	public void setWidth(int width){
		this.width = width;
	}
	
	public void setHeight(int height){
		this.height = height;
	}
}
