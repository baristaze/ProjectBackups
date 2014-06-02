package net.pic4pic.ginger;

import java.util.ArrayList;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.TimeUnit;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.Menu;
import android.widget.LinearLayout;
import android.widget.ScrollView;
import android.widget.TextView;

import net.pic4pic.ginger.entities.MatchedCandidate;
import net.pic4pic.ginger.entities.UserResponse;
import net.pic4pic.ginger.tasks.ConversationRetriever;
import net.pic4pic.ginger.tasks.ConversationRetriever.ConversationListener;
import net.pic4pic.ginger.utils.MyLog;

public class ConversationActivity extends Activity implements ConversationListener {

	public static final int ConversationActivityCode = 401;
	
	private UserResponse me;
	private MatchedCandidate person;
	private ArrayList<String> messageThread = new ArrayList<String>();
	private ConversationRetriever conversationRetriever;
	private ScheduledExecutorService conversationPoller;
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		
		MyLog.v("ConversationActivity", "onCreate");
		
		super.onCreate(savedInstanceState);
		if(this.recreatePropertiesFromSavedBundle(savedInstanceState)){
			MyLog.i("ConversationActivity", "At least one property is restored successfully");
		}
		
		this.setContentView(R.layout.activity_conversation);
		
		Intent intent = getIntent();
		this.me = (UserResponse)intent.getSerializableExtra(MainActivity.AuthenticatedUserBundleType);
		MyLog.v("ConversationActivity", "Current user is: " + this.me.getUserProfile().getUsername());
		
		this.person = (MatchedCandidate)intent.getSerializableExtra(PersonActivity.PersonType);
		MyLog.v("ConversationActivity", "Candidate is: " + this.person.getCandidateProfile().getUsername());
		
		this.onConversationReceived(this.messageThread);
	}
	
	@Override
	public void onSaveInstanceState(Bundle outState) {
		
		super.onSaveInstanceState(outState);

		if(outState == null){
			return;
		}
		
		MyLog.v("ConversationActivity", "onSaveInstanceState");
		
		if(this.me != null){
			outState.putSerializable("me", this.me);
		}
		
		if(this.person != null){
			outState.putSerializable("person", this.person);
		}
		
		if(this.messageThread != null){
			outState.putSerializable("messageThread", this.messageThread);
		}
	}
	
	@Override
	public void onRestoreInstanceState(Bundle savedInstanceState) {
		
		super.onRestoreInstanceState(savedInstanceState);
		
		MyLog.v("ConversationActivity", "onRestoreInstanceState");
		if(this.recreatePropertiesFromSavedBundle(savedInstanceState)){
			MyLog.i("ConversationActivity", "At least one property is restored successfully");
		}
	}
	
	@SuppressWarnings("unchecked")
	private boolean recreatePropertiesFromSavedBundle(Bundle state){
		
		if(state == null){
			return false;
		}
		
		boolean restored = false;
		if(state.containsKey("me")){
			this.me = (UserResponse)state.getSerializable("me");
			restored = true;
		}
		
		if(state.containsKey("person")){
			this.person = (MatchedCandidate)state.getSerializable("person");
			restored = true;
		}
		
		if(state.containsKey("messageThread")){
			this.messageThread = (ArrayList<String>)state.getSerializable("messageThread");
			restored = true;
		}
			
		return restored;
	}
	
	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.conversation, menu);
		return true;
	}

	/**
	 * onCreate -> onStart -> onResume
	 * Stopped -> onRestart -> onResume
	 */
	@Override
	protected void onResume() {
		MyLog.v("ConversationActivity", "onResume...");
		super.onResume();
		
		// always create from scratch otherwise it won't restart.
		this.conversationPoller = Executors.newScheduledThreadPool(1);
		this.conversationRetriever = new ConversationRetriever(this, this);
		this.conversationPoller.scheduleWithFixedDelay(this.conversationRetriever, 0, 1000, TimeUnit.MILLISECONDS);
	}
	
	@Override
	protected void onPause() {
		
		MyLog.v("ConversationActivity", "onPause...");
		
		// stop
		this.conversationPoller.shutdown();
		
		// wait for stop
		try {
			if(this.conversationPoller.awaitTermination(500, TimeUnit.MILLISECONDS)){
				MyLog.v("ConversationActivity", "Terminated successfully.");
			}
			else{
				MyLog.w("ConversationActivity", "Couldn't terminate in 500 msec.");
			}
		} 
		catch (InterruptedException e) {
			MyLog.e("ConversationActivity", "awaitTermination threw exception: " + e.getMessage());
		}
		
		// if not stopped yet; enforce to shut down
		if(!this.conversationPoller.isTerminated()){
			MyLog.i("ConversationActivity", "Conversation poller hasn't terminated yet. Forcing to stop...");
			this.conversationPoller.shutdownNow();
		}
		
		// parent onPause
		super.onPause();
	}

	@Override
	public void onConversationReceived(ArrayList<String> messages) {
		
		if(messages == null || messages.size() == 0){
			return;
		}
		
		MyLog.v("ConversationActivity", "Message thread is received. Updating view...");
		
		LayoutInflater inflater = this.getLayoutInflater();
		final ScrollView messageThreadScroll = (ScrollView)this.findViewById(R.id.messageThreadScroll);
		final LinearLayout messageThreadView = (LinearLayout)this.findViewById(R.id.messageThread);
		
		this.messageThread.addAll(messages);
		
		int x=0;
		for(String message : messages){
			TextView messageView = null;
			if(((x++) % 2) == 0){
				messageView = (TextView)inflater.inflate(R.layout.message_incoming, messageThreadView, false);
			}
			else{
				messageView = (TextView)inflater.inflate(R.layout.message_outgoing, messageThreadView, false);
			}
			messageView.setText(message);			
			messageThreadView.addView(messageView);			
			messageThreadScroll.post(new Runnable() {
		        @Override
		        public void run() {
		        	messageThreadScroll.fullScroll(ScrollView.FOCUS_DOWN);
		        }
		    });
		}
	}
}
