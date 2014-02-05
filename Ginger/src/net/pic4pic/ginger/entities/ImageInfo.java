package net.pic4pic.ginger.entities;

import java.io.Serializable;

public class ImageInfo implements Serializable {	
	
	private static final long serialVersionUID = 1;
	
	private boolean blurred;
	private String thumbnail;
	private String original;
	
	public boolean isBlurred(){
		return this.blurred;
	}
	
	public void setBlurred(boolean blurred){
		this.blurred = blurred;
	}
	
	public String getThumbnail(){
		return this.thumbnail;
	}
	
	public void setThumbnail(String thumbnail){
		this.thumbnail = thumbnail;
	}
	
	public String getOriginal(){
		return this.original;
	}
	
	public void setOriginal(String original){
		this.original = original;
	}
}
