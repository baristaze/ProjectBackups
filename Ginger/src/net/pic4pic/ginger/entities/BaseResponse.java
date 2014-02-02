package net.pic4pic.ginger.entities;

import com.google.gson.annotations.SerializedName;

public class BaseResponse {
	
	@SerializedName("ErrorCode")
	protected int errorCode;
	
	@SerializedName("ErrorMessage")
	protected String errorMessage;
	
	@SerializedName("NeedsRelogin")
	protected boolean needsRelogin;
	
	public int getErrorCode(){
		return this.errorCode;
	}
	
	public String getErrorMessage(){
		return this.errorMessage;
	}
	
	public boolean isNeedRelogin(){
		return this.needsRelogin;
	}
	
	public void setErrorCode(int errorCode){
		this.errorCode = errorCode;
	}
	
	public void setErrorMessage(String errorMessage){
		this.errorMessage = errorMessage;
	}
	
	public void setNeedRelogin(boolean needsRelogin){
		this.needsRelogin = needsRelogin;
	}
}
