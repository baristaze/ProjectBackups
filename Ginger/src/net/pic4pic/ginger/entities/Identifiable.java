package net.pic4pic.ginger.entities;

import java.util.UUID;
import com.google.gson.annotations.SerializedName;

public class Identifiable {
    
	@SerializedName("Id")
	protected UUID id;
	
	public UUID getId(){
		return this.id;
	}
	
	public void setId(UUID id){
		this.id = id;
	}
}
