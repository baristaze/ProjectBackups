package net.pic4pic.ginger.entities;

import java.util.ArrayList;
import java.util.List;

import com.google.gson.annotations.SerializedName;

import net.pic4pic.ginger.utils.LogBag;

public class ClientLogRequest extends BaseRequest {
	
	@SerializedName("Logs")
	protected ArrayList<LogBag> logs = new ArrayList<LogBag>();
	
	public void addRange(List<LogBag> logs){
		this.logs.addAll(logs);
	}
	
	public ArrayList<LogBag> getLogs(){
		return this.logs;
	} 
}
