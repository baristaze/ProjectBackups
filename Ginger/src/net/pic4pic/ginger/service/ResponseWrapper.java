package net.pic4pic.ginger.service;

import net.pic4pic.ginger.entities.BaseResponse;

class ResponseWrapper<R extends BaseResponse>	{
	
	private R data;
	
	public ResponseWrapper(){
		
	}
	
	public ResponseWrapper(R response){
		this.data = response;
	}
	
	public R getData(){
		return this.data;
	}
	
	public void setData(R response){
		this.data = response;
	}
}
