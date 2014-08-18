package net.pic4pic.ginger.utils;

import java.util.ArrayList;
import java.util.UUID;

import android.util.Log;

public class LogBag {
	
	protected static final int Debug = 1;
	protected static final int Verbose = 2;
	protected static final int Info = 3;
	protected static final int Warning = 4;
	protected static final int Error = 5;
	
	public static final String TagLevel = "Level";
	public static final String TagMessage = "Message";
	public static final String TagException = "Exception";
	
	public static final String TagFileName = "File";
	public static final String TagClassName = "Class";
	public static final String TagMethodName = "Method";
	public static final String TagLineNumber = "Line";
	
	public static final String TagAppType = "AppType";
	public static final String TagAppVersion = "AppVersion";
	
	public static final String TagClientId = "ClientId";
	public static final String TagUserId = "UserId";
	public static final String TagUsername = "Username";
	
	public static final String TagObjectType = "ObjectType";
	public static final String TagObjectId = "ObjectId";
	
	public static final String TagParam = "Param";
	
	private ArrayList<LogBagItem> items = new ArrayList<LogBagItem>();
	
	public LogBag add(String tag, String value){
		
		if(tag != null && tag.length() > 0){
			this.items.add(new LogBagItem(tag, value));
		}
		
		return this;
	}
	
	public LogBag add(Exception ex){		
		
		if(ex != null){
			return this.add(TagException, ex.toString());
		}
		
		return this;
	}
	
	public LogBag addClientId(UUID clientId){
		
		if(clientId == null){
			clientId = new UUID(0, 0); 
		}
		
		return this.add(TagClientId, clientId.toString());
	}
	
	public LogBag addUser(UUID userId){
		
		if(userId == null){
			userId = new UUID(0, 0); 
		}
		
		return this.add(TagUserId, userId.toString());
	}
	
	public LogBag addUser(String username){		
		return this.add(TagUsername, username);
	}
	
	public LogBag addObject(String objectType, UUID objectId){
		
		if(objectId == null){
			objectId = new UUID(0, 0); 
		}
		
		this.add(TagObjectId, objectId.toString());
		return this.add(TagObjectType, objectType);
	}
	
	public LogBag addParam(Object object){		
		return this.add(TagParam, (object == null ? null : object.toString()));
	}
	
	public void e(){
		this.e(null);
	}
	
	public void e(String message){
		this.log(Error, message);
	}
	
	public void w(){
		this.w(null);
	}
	
	public void w(String message){		
		this.log(Warning, message);
	}
	
	public void i(){
		this.i(null);
	}
	
	public void i(String message){
		this.log(Info, message);
	}
	
	public void v(){
		this.v(null);
	}
	
	public void v(String message){
		this.log(Verbose, message);
	}
	
	public void d (){
		this.d(null);
	}
	
	public void d (String message){
		this.log(Verbose, message);
	}
	
	/*
	public void e(String tagNotInUse, String message){
		this.log(Error, message);
	}
	
	public void w(String tagNotInUse, String message){
		this.log(Warning, message);
	}
	
	public void i(String tagNotInUse, String message){
		this.log(Info, message);
	}
	
	public void v(String tagNotInUse, String message){
		this.log(Verbose, message);
	}
	
	public void d (String tagNotInUse, String message){
		this.log(Verbose, message);
	}
	*/
	
	protected String getLevelAsText(int level){
		
		switch(level){
		
			case Error : 
				return "Error";
				
			case Warning : 
				return "Warning";
				
			case Info : 
				return "Info";
				
			case Debug : 
				return "Debug";
				
			default:
				return "Verbose";
		}
	}
	
	@Override
	public String toString(){
		
		if(this.items.size() == 0){
			return "";
		}
		
		boolean appended = false;
		StringBuilder builder = new StringBuilder();		
		LogBagItem exceptionLogBagItem = null;		
		for(int x=this.items.size()-1; x>=0; x--){
			
			LogBagItem logBagItem = this.items.get(x);
			if(logBagItem.getName().equalsIgnoreCase(TagException)){
				// this will be inserted at the end
				exceptionLogBagItem = logBagItem;
			}
			else{
				
				if(appended){
					builder.append("[;]");
				}
				
				builder.append("[" + logBagItem.toString() + "]");
				appended = true;
			}
		}
		
		if(exceptionLogBagItem != null){
			
			if(appended){
				builder.append("[;]");
			}
			
			builder.append("[" + exceptionLogBagItem.toString() + "]");
			appended = true;
		}
		
		return builder.toString();
	}	
	
	protected String getValueOf(String tag){
		for(LogBagItem item : this.items){
			if(item.getName().equalsIgnoreCase(tag)){
				return item.getValue();
			}
		}
		
		return "";
	}
	
	protected void log(int level, String message){		
		this.add(TagLevel, this.getLevelAsText(level));
		if(message != null && message.length() > 0){
			this.add(TagMessage, message);
		}
		
		this.logToConsole(level);
	}
	
	protected void logToConsole(int level){
	
		String tag = "Ginger";
		String cls = this.getValueOf(TagClassName);
		if(cls != null && cls.length() > 0){
			tag += "-" + cls;
		}		
		
		String log = this.toString();
		
		switch(level){
		
			case Error : 
				Log.e(tag, log);
				break;
				
			case Warning : 
				Log.w(tag, log);
				break;
				
			case Info : 
				Log.i(tag, log);
				break;			
				
			case Debug : 
				Log.d(tag, log);
				break;
				
			default:
				Log.v(tag, log);
				break;
		}
	}
}
