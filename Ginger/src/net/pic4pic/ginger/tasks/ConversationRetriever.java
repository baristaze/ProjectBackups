package net.pic4pic.ginger.tasks;

import java.util.ArrayList;
import java.util.Date;

import net.pic4pic.ginger.utils.MyLog;

import android.app.Activity;

public class ConversationRetriever implements Runnable {

	private Activity activity;
	private ConversationListener listener;
	
	public ConversationRetriever(Activity activity, ConversationListener listener){
		this.activity = activity;
		this.listener = listener;
	}
	
	@Override
	public void run() {
		
		try {
			final ArrayList<String> result = this.retrieveConversation();
			this.activity.runOnUiThread(new Runnable(){
				@Override
				public void run() {
					ConversationRetriever.this.listener.onConversationReceived(result);
				}				
			});
		}
		catch(Exception e){
			MyLog.e("ConversationRetriever", "Conversation could not be retrieved. Error: " + e.getMessage());
		}
	}
	
	protected ArrayList<String> retrieveConversation(){
		ArrayList<String> result = new ArrayList<String>();
		result.add("Hello there! " + (new Date()).toString());
		result.add("Hello baby!" + (new Date()).toString());
		return result;
	}
	
	public interface ConversationListener{
		public void onConversationReceived(ArrayList<String> messageThread);
	}
}
