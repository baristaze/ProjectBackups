package net.pic4pic.ginger.utils;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.TimeZone;
import java.util.UUID;

import net.pic4pic.ginger.entities.LogReportingLevel;
import net.pic4pic.ginger.service.Service;

import com.google.gson.annotations.SerializedName;

import android.util.Log;

public class LogBag {
	
	protected static final int Debug = 1;
	protected static final int Verbose = 2;
	protected static final int Info = 3;
	protected static final int Metric = 4;
	protected static final int Warning = 5;
	protected static final int Error = 6;
	
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
	
	public static final String TagObjectType = "ObjectType";
	public static final String TagObjectId = "ObjectId";
	
	public static final String TagParam = "Param";
	
	@SerializedName("Pairs")
	protected ArrayList<LogBagItem> items = new ArrayList<LogBagItem>();
	
	public LogBag add(String tag, String value){
		
		if(tag != null && tag.length() > 0){
			this.items.add(new LogBagItem(tag, value));
		}
		
		return this;
	}
	
	public LogBag add(Throwable ex){		
		
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
	
	public void m(){		
		this.m(null);
	}
	
	public void m(String message){		
		this.log(Metric, message);
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
	
	protected String getLevelAsText(int level){
		
		switch(level){
		
			case Error : 
				return "Error";
				
			case Warning : 
				return "Warning";
			
			case Metric : 
				return "Metric";
				
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
		
		// add level
		this.add(TagLevel, this.getLevelAsText(level));
		
		// add message
		if(message != null && message.length() > 0){
			this.add(TagMessage, message);
		}
		
		// log to console
		this.logToConsole(level, false);
		
		// add extra since this needs to be sent to server
		if(this.shouldBeSent(level)){
			
			UUID logKey = UUID.randomUUID();
			this.add("LogId", logKey.toString());		
			SimpleDateFormat dateFormatter = new SimpleDateFormat("MM/dd/yyyy HH:mm:ss");
			dateFormatter.setTimeZone(TimeZone.getTimeZone("GMT"));
			this.add("LogTimeUTC", dateFormatter.format(new Date()));
			
			// send
			LogPusher.instance().add(logKey, this);	
		}
		
		// do not clear! we don't know what happens.
	}
	
	protected boolean shouldBeSent(int level)
	{
		int reportingLevel = Service.getInstance().getLogReportingLevel();
		if(level == LogBag.Error && reportingLevel >= LogReportingLevel.OnlyErrors){
			return true;
		}
		if(level == LogBag.Warning && reportingLevel >= LogReportingLevel.WarningsAndAbove){
			return true;
		}
		if(level == LogBag.Metric && reportingLevel >= LogReportingLevel.MetricsAndAbove){
			return true;
		}
		if(level == LogBag.Info && reportingLevel >= LogReportingLevel.InfoAndAbove){
			return true;
		}
		if(level == LogBag.Verbose && reportingLevel >= LogReportingLevel.VerboseAndAbove){
			return true;
		}
		if(level == LogBag.Debug && reportingLevel >= LogReportingLevel.All){
			return true;
		}
		
		return false;
	}
	
	protected void logToConsole(int level, boolean summarize){
	
		String tag = "Ginger";
		String cls = this.getValueOf(TagClassName);
		if(cls != null && cls.length() > 0){
			tag += "-" + cls;
		}		
		
		String log = "";
		if(summarize){
			String msg = this.getValueOf(TagMessage);
			String exc = this.getValueOf(TagException);
			log = msg == null ? "" : msg;
			if(exc != null && exc.length() > 0){
				if(log.length() > 0){
					log += " : ";
				}
				log += exc;
			}
		}

		if(log.length() <=0 ){
			log = this.toString();
		}
		
		switch(level){
		
			case Error : 
				Log.e(tag, log);
				break;
				
			case Warning : 
				Log.w(tag, log);
				break;
				
			case Metric : 
				Log.i(tag, log); // it is still info
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
