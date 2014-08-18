package net.pic4pic.ginger;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Iterator;
import java.util.LinkedHashMap;
import java.util.UUID;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.TimeUnit;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.WindowManager;
import android.widget.Button;
import android.widget.EditText;
import android.widget.LinearLayout;
import android.widget.ScrollView;
import android.widget.TextView;

import net.pic4pic.ginger.entities.ConversationRequest;
import net.pic4pic.ginger.entities.ConversationResponse;
import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.entities.InstantMessage;
import net.pic4pic.ginger.entities.InstantMessageRequest;
import net.pic4pic.ginger.entities.IntegerEnum;
import net.pic4pic.ginger.entities.MatchedCandidate;
import net.pic4pic.ginger.entities.UserResponse;
import net.pic4pic.ginger.service.Service;
import net.pic4pic.ginger.tasks.ConversationRetriever;
import net.pic4pic.ginger.tasks.ITask;
import net.pic4pic.ginger.tasks.NonBlockedTask;
import net.pic4pic.ginger.tasks.ConversationRetriever.ConversationListener;
import net.pic4pic.ginger.utils.GingerHelpers;
import net.pic4pic.ginger.utils.MyLog;

public class ConversationActivity extends Activity implements ConversationListener {
	
	public enum ConversationMode implements IntegerEnum {
		StartTyping(0),
		ReadFirst(1);
		
		private final int value;
		
		private ConversationMode(int value) {
			this.value = value;
		}

		public int getIntValue() {
			return this.value;
		}
	}
	
	public static final int ConversationActivityCode = 401;
	public static final String ConversationModeType = "net.pic4pic.ginger.ConversationMode";
	
	private UserResponse me;
	private MatchedCandidate person;
	private ConversationRetriever conversationRetriever;
	private ScheduledExecutorService conversationPoller;
	private UUID lastExchangedMessageId = new UUID(0, 0);
	
	private LinkedHashMap<UUID, InstantMessage> messageThread = new LinkedHashMap<UUID, InstantMessage>();
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		
		MyLog.bag().v("onCreate");
		super.onCreate(savedInstanceState);
		if(this.recreatePropertiesFromSavedBundle(savedInstanceState)){
			MyLog.bag().i("At least one property is restored successfully");
		}
		
		this.setContentView(R.layout.activity_conversation);
		
		Intent intent = getIntent();
		this.me = (UserResponse)intent.getSerializableExtra(MainActivity.AuthenticatedUserBundleType);
		MyLog.bag().v("Current user is: " + this.me.getUserProfile().getUsername());
		
		this.person = (MatchedCandidate)intent.getSerializableExtra(PersonActivity.PersonType);
		MyLog.bag().v("Candidate is: " + this.person.getCandidateProfile().getUsername());
		
		final Button sendButton = (Button)this.findViewById(R.id.sendButton);
		sendButton.setOnClickListener(new OnClickListener(){
			@Override
			public void onClick(View v) {				
				ConversationActivity.this.sendInstantMessage(sendButton);
			}});
				
		this.renderMessages(this.messageThread.values().iterator(), false);
		
		int mode = intent.getIntExtra(ConversationActivity.ConversationModeType, 0);
		if(mode != ConversationMode.ReadFirst.getIntValue()){
			getWindow().setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_STATE_VISIBLE);
			EditText messageEditText = (EditText)this.findViewById(R.id.messageEditText);			
			messageEditText.requestFocus();
			
		}
		else{
			getWindow().setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_STATE_HIDDEN);
		}
	}
	
	@Override
	public void onSaveInstanceState(Bundle outState) {
		
		super.onSaveInstanceState(outState);

		if(outState == null){
			return;
		}
		
		MyLog.bag().v("onSaveInstanceState");
		
		if(this.me != null){
			outState.putSerializable("me", this.me);
		}
		
		if(this.person != null){
			outState.putSerializable("person", this.person);
		}
		
		if(this.lastExchangedMessageId != null){
			outState.putSerializable("lastExchangedMessageId", this.lastExchangedMessageId);
		}
		
		if(this.messageThread != null && this.messageThread.size() > 0){
			outState.putSerializable("messageThread", this.messageThread.values().toArray());
		}
	}
	
	@Override
	public void onRestoreInstanceState(Bundle savedInstanceState) {
		
		super.onRestoreInstanceState(savedInstanceState);
		
		MyLog.bag().v("onRestoreInstanceState");
		if(this.recreatePropertiesFromSavedBundle(savedInstanceState)){
			MyLog.bag().i("At least one property is restored successfully");
		}
	}
	
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
		
		if(state.containsKey("lastExchangedMessageId")){
			this.lastExchangedMessageId = (UUID)state.getSerializable("lastExchangedMessageId");
			restored = true;
		}
		
		if(state.containsKey("messageThread")){
			Object[] items = (Object[]) state.getSerializable("messageThread");
			for(Object o : items){
				InstantMessage im = (InstantMessage)o;
				this.messageThread.put(im.getId(), im);
			}
			restored = true;
		}
			
		return restored;
	}
	
	/**
	 * onCreate -> onStart -> onResume
	 * Stopped -> onRestart -> onResume
	 */
	@Override
	protected void onResume() {
		MyLog.bag().v("onResume...");
		super.onResume();
		
		// always create poller from scratch otherwise it won't restart.
		ConversationRequest request = new ConversationRequest();
		request.setUserIdToInteract(this.person.getUserId());
		request.setLastExchangedMessageId(this.lastExchangedMessageId);
		
		this.conversationPoller = Executors.newScheduledThreadPool(1);
		this.conversationRetriever = new ConversationRetriever(this, this, request);
		this.conversationPoller.scheduleWithFixedDelay(this.conversationRetriever, 0, 1500, TimeUnit.MILLISECONDS);
	}
	
	@Override
	protected void onPause() {
		
		MyLog.bag().v("onPause...");
		
		// stop
		this.conversationPoller.shutdown();
		
		// wait for stop
		try {
			if(this.conversationPoller.awaitTermination(500, TimeUnit.MILLISECONDS)){
				MyLog.bag().v("Terminated successfully.");
			}
			else{
				MyLog.bag().w("Couldn't terminate in 500 msec.");
			}
		} 
		catch (InterruptedException ex) {
			MyLog.bag().add(ex).e();
		}
		
		// if not stopped yet; enforce to shut down
		if(!this.conversationPoller.isTerminated()){
			MyLog.bag().i("Conversation poller hasn't terminated yet. Forcing to stop...");
			this.conversationPoller.shutdownNow();
		}
		
		// parent onPause
		super.onPause();
	}
	
	@Override
	public void onConversationReceived(ArrayList<InstantMessage> messages) {
		
		if(messages == null || messages.size() == 0){
			return;
		}
		
		MyLog.bag().v("Message thread is received. Updating view...");
		
		// update the last IM's id
		this.lastExchangedMessageId = messages.get(0).getId();
		
		// reverse order
		ArrayList<InstantMessage> temp = new ArrayList<InstantMessage>(); 	
		Collections.reverse(messages);
		for(InstantMessage m : messages){
			if(!messageThread.containsKey(m.getId())){
				messageThread.put(m.getId(), m);
				temp.add(m);
			}
		}		
		
		this.renderMessages(temp.iterator(), false);
	}
	
	private void renderMessages(Iterator<InstantMessage> messages, boolean shouldClearFirst){
		
		LayoutInflater inflater = this.getLayoutInflater();
		final ScrollView messageThreadScroll = (ScrollView)this.findViewById(R.id.messageThreadScroll);
		final LinearLayout messageThreadView = (LinearLayout)this.findViewById(R.id.messageThread);
		
		if(shouldClearFirst){
			messageThreadView.removeAllViews();
		}
		
		while(messages.hasNext()){
			InstantMessage im = messages.next();
			TextView messageView = null;
			if(im.getUserId1().equals(this.me.getUserId())){
				messageView = (TextView)inflater.inflate(R.layout.message_outgoing, messageThreadView, false);				
			}
			else{
				messageView = (TextView)inflater.inflate(R.layout.message_incoming, messageThreadView, false);
			}
			messageView.setText(im.getContent());			
			messageThreadView.addView(messageView);			
			messageThreadScroll.post(new Runnable() {
		        @Override
		        public void run() {
		        	messageThreadScroll.fullScroll(ScrollView.FOCUS_DOWN);
		        }
		    });
		}
	}
	
	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.conversation, menu);
		return true;
	}
	
	public boolean onOptionsItemSelected(MenuItem item) {
		switch (item.getItemId()) {
		case android.R.id.home:
			//NavUtils.navigateUpFromSameTask(this);	
			this.setResultIntent();
			finish();
			return true;
		}
		return super.onOptionsItemSelected(item);
	}
	
	@Override
	public void onBackPressed() {
		this.setResultIntent();
		super.onBackPressed();
	}

	private void setResultIntent(){
		
		MyLog.bag().v("Setting result intent");
		
		Intent resultIntent = new Intent();
		resultIntent.putExtra(MainActivity.AuthenticatedUserBundleType, this.me);
		resultIntent.putExtra(PersonActivity.PersonType, this.person);		
		this.setResult(Activity.RESULT_OK, resultIntent);
	}
	
	private void sendInstantMessage(final Button sendButton){
		
		// get input
		final EditText contentView = (EditText)this.findViewById(R.id.messageEditText);
		String content = contentView.getText().toString().trim();
		if(content.length() <= 0){
			GingerHelpers.showErrorMessage(this, "Please type a message");
			return;
		}
		
		// prepare request
		final InstantMessageRequest imRequest = new InstantMessageRequest();
		imRequest.setUserIdToInteract(this.person.getUserId());
		imRequest.setContent(content);
		
		// disable button at the beginning
		contentView.setEnabled(false);
		sendButton.setEnabled(false);
		
		// send IM
		NonBlockedTask.SafeRun(new ITask(){
			
			@Override
			public void perform() {
				
				boolean success = false;
				
				try{
					// mark as liked
					final ConversationResponse response = Service.getInstance().sendInstantMessage(ConversationActivity.this, imRequest);
					
					// check result
					if(response.getErrorCode() == 0){
						
						success = true;
						
						// log
						MyLog.bag().v("Message Sent: " + imRequest.getContent());
					
						ConversationActivity.this.runOnUiThread(new Runnable(){
							@Override
							public void run() {
								
								contentView.setText("");
								contentView.setEnabled(true);
								sendButton.setEnabled(true);
								ConversationActivity.this.onConversationReceived(response.getItems());
							}
						});
					}
					else{
						// log error
						MyLog.bag().e("Sending message failed with error: " + response.getErrorMessage());
						GingerHelpers.toastShort(ConversationActivity.this, response.getErrorMessage());
					}
				}
				catch(GingerException ge){
					
					// log error
					MyLog.bag().add(ge).e("Sending message failed");
					GingerHelpers.toastShort(ConversationActivity.this, "Sending message failed. Please try again.");
				}
				catch(Exception e){
					
					// log error
					MyLog.bag().add(e).e("Sending message failed");
					GingerHelpers.toastShort(ConversationActivity.this, "Sending message failed. Please try again.");
				}
				
				if(!success){
					ConversationActivity.this.runOnUiThread(new Runnable(){
						@Override
						public void run() {
							// enable back since we failed
							contentView.setEnabled(true);
							sendButton.setEnabled(true);
						}
					});
				}
			}
		});
	}
}
