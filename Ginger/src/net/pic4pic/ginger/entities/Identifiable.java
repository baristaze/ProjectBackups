package net.pic4pic.ginger.entities;

import com.google.gson.annotations.SerializedName;

public class Identifiable {
    
	@SerializedName("Id")
	protected long id;
	
	public long getId(){
		return this.id;
	}
	
	public void setId(long id){
		this.id = id;
	}
}
