package net.pic4pic.ginger.entities;

public class ImageUploadRequest extends BaseRequest {
	
	protected String fullLocalPath;	
	protected boolean isProfileImage;
	
	public String getFullLocalPath(){
		return this.fullLocalPath;
	}
	
	public void setFullLocalPath(String path){
		this.fullLocalPath = path;
	}

	/**
	 * @return the isProfileImage
	 */
	public boolean isProfileImage() {
		return isProfileImage;
	}

	/**
	 * @param isProfileImage the isProfileImage to set
	 */
	public void setProfileImage(boolean isProfileImage) {
		this.isProfileImage = isProfileImage;
	}	
}
