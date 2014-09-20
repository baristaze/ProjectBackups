package net.pic4pic.ginger.tasks;

import java.util.ArrayList;

import net.pic4pic.ginger.entities.ConversationRequest;
import net.pic4pic.ginger.entities.ConversationResponse;
import net.pic4pic.ginger.entities.InstantMessage;
import net.pic4pic.ginger.service.Service;
import net.pic4pic.ginger.utils.MyLog;

import android.app.Activity;

public class ConversationRetriever implements Runnable {

	private Activity activity;
	private ConversationListener listener;
	private ConversationRequest request;
	
	public ConversationRetriever(Activity activity, ConversationListener listener, ConversationRequest request){
		this.activity = activity;
		this.listener = listener;
		this.request = request;
	}
	
	@Override
	public void run() {
		
		try {
			ConversationResponse result = Service.getInstance().getConversation(this.activity, this.request);
			if(result.getErrorCode() == 0) {
				// consume messages
				final ArrayList<InstantMessage> messageThread = result.getItems();
				this.activity.runOnUiThread(new Runnable(){
					@Override
					public void run() {
						ConversationRetriever.this.listener.onConversationReceived(messageThread);
					}				
				});
				// update request for re-use
				if(messageThread.size() > 0){
					// first one is the latest message
					this.request.setLastExchangedMessageId(messageThread.get(0).getId());
				}				
			}
			else{
				MyLog.bag().e("Conversation could not be retrieved. Error: " + result.getErrorMessage());
			}
		}
		catch(Throwable e){
			
		}
	}
	
	public interface ConversationListener{
		public void onConversationReceived(ArrayList<InstantMessage> messageThread);
	}
}
