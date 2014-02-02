package net.pic4pic.ginger.entities;

import java.util.Date;
import java.util.UUID;

import com.google.gson.annotations.SerializedName;

public class MetaFile extends Identifiable {
	
	@SerializedName("CloudId")
    protected UUID cloudId;

	@SerializedName("ContentType")
	protected String contentType;
    
	@SerializedName("ContentLength")
	protected int contentLength;

	@SerializedName("OriginalUrl")
	protected String originalUrl;

	@SerializedName("CloudUrl")
	protected String cloudUrl;

	@SerializedName("CreateTimeUTC")
	protected Date createTimeUTC;   
	
	// getters 
    public UUID getCloudId(){
    	return this.cloudId;
    }
    
    public String getContentType(){
		return this.contentType;
	}
	
    public int getContentLength(){
		return this.contentLength;
	}
	
    public String getOriginalUrl(){
		return this.originalUrl;
	}
	
    public String getCloudUrl(){
		return this.cloudUrl;
	}
	
    public Date getCreateTimeUTC(){
		return this.createTimeUTC;
	}
    
    // setters 
    public void setCloudId(UUID cloudId){
    	this.cloudId = cloudId;
    }
    
    public void setContentType(String contentType){
		this.contentType = contentType;
	}
	
    public void setContentLength(int contentLength){
		this.contentLength = contentLength;
	}
	
    public void setOriginalUrl(String originalUrl){
		this.originalUrl = originalUrl;
	}
	
    public void setCloudUrl(String cloudUrl){
		this.cloudUrl = cloudUrl;
	}
	
    public void setCreateTimeUTC(Date createTimeUTC){
		this.createTimeUTC = createTimeUTC;
	}
}