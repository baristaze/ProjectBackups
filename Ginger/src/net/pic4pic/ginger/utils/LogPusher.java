package net.pic4pic.ginger.utils;

import java.util.ArrayList;
import java.util.Map;
import java.util.Timer;
import java.util.TimerTask;
import java.util.UUID;

import net.pic4pic.ginger.tasks.LogPusherTask;

public class LogPusher {
	
	private final int MaxCachedLogCount = 3000;
	private final int TimerSpinTimeInMilliSeconds = 2 * 1000;
	
	private final int LogThreshold = 100;
	private final int TimeThresholdInSeconds = 60;
	
	private Timer timer;
	private Map<UUID, LogBag> logs;
	private long lastPushTimeInNano;
	
	private LogPusher(){
		
		this.lastPushTimeInNano = System.nanoTime();
		this.logs = new LRUCache<UUID, LogBag>(MaxCachedLogCount, false); // not access ordered 
		this.timer = new Timer();
		this.timer.schedule(new TimerTask() {			
			  @Override
			  public void run() {
				  doTimerWork();
			  }
			}, 0, TimerSpinTimeInMilliSeconds);
	}
	
	private static LogPusher instance;	
	public synchronized static LogPusher instance(){		
		if (instance == null) {
        	instance = new LogPusher();
        }		
        return instance;
	}
	
	public synchronized void add(UUID logKey, LogBag log){
		this.logs.put(logKey, log);
	}
	
	private synchronized boolean shouldTriggerAPush(){
		
		if(this.logs.size() >= LogThreshold){
			return true;
		}
		else if(this.logs.size() <= 0){
			return false;
		}
		else{
			long timeDiff = System.nanoTime() - this.lastPushTimeInNano;
			timeDiff /= 1000000;
			if(timeDiff >= TimeThresholdInSeconds * 1000){
				return true; 
			}
			else{
				return false;
			}
		}
	}
	
	private synchronized void doTimerWork(){
		
		if(this.shouldTriggerAPush()){
			
		  ArrayList<LogBag> temp = new ArrayList<LogBag>();
		  for(LogBag bag : this.logs.values()){
			  temp.add(bag);
		  }
		  
		  this.logs.clear();
		  LogPusherTask task = new LogPusherTask(temp);
		  task.execute();
	  }
	}
}
