package net.pic4pic.ginger;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.Menu;

import net.pic4pic.ginger.entities.MatchedCandidate;
import net.pic4pic.ginger.entities.UserResponse;
import net.pic4pic.ginger.utils.MyLog;

public class ConversationActivity extends Activity {

	public static final int ConversationActivityCode = 401;
	
	private UserResponse me;
	private MatchedCandidate person;
	
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
	}
	
	@Override
	public void onRestoreInstanceState(Bundle savedInstanceState) {
		
		super.onRestoreInstanceState(savedInstanceState);
		
		MyLog.v("ConversationActivity", "onRestoreInstanceState");
		if(this.recreatePropertiesFromSavedBundle(savedInstanceState)){
			MyLog.i("ConversationActivity", "At least one property is restored successfully");
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
			
		return restored;
	}
	
	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.conversation, menu);
		return true;
	}
}
