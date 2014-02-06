package net.pic4pic.ginger.entities;

public class ImageUploadRequest extends BaseRequest {
	
	protected String fullLocalPath;
	
	public String getFullLocalPath(){
		return this.fullLocalPath;
	}
	
	public void setFullLocalPath(String path){
		this.fullLocalPath = path;
	}	
}
