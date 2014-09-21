package net.pic4pic.ginger;

import java.util.ArrayList;
import java.util.Date;

import net.pic4pic.ginger.entities.GeoLocation;
import net.pic4pic.ginger.entities.Locality;
import net.pic4pic.ginger.entities.Location;
import net.pic4pic.ginger.entities.MatchedCandidate;
import net.pic4pic.ginger.entities.MatchedCandidateListResponse;
import net.pic4pic.ginger.tasks.PreviewCandidatesTask;
import net.pic4pic.ginger.tasks.PreviewCandidatesTask.PreviewCandidatesListener;
import net.pic4pic.ginger.utils.GingerHelpers;
import net.pic4pic.ginger.utils.LocationManagerUtil;
import net.pic4pic.ginger.utils.LocationManagerUtil.LocationListener;
import net.pic4pic.ginger.utils.MyLog;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.AdapterView;
import android.widget.Button;
import android.widget.FrameLayout;
import android.widget.ListView;
import android.widget.AbsListView.LayoutParams;

public class PreviewActivity extends Activity implements PreviewCandidatesListener, LocationListener {

	public static final String JumpToSignupCode = "net.pic4pic.ginger.PreviewActivity.JumpToSignup";
	
	private int preSelectedTabIndexOnMainActivity = 0;
	
	private Location lastLocationInfo = null;
	private Date lastMatchRetrieveTime = null;
	private ArrayList<MatchedCandidate> matches = new ArrayList<MatchedCandidate>();	
	
	public ArrayList<MatchedCandidate> getMatchedCandidates(){
		return this.matches;
	}
	
	public Location getLastLocation(){
		return this.lastLocationInfo;
	}
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		
		super.onCreate(savedInstanceState);
		
		// other activities might have hidden the bar
		this.getActionBar().show();

		this.setTitle(" pic4pic - preview");
		
		// set content view
		setContentView(R.layout.activity_preview);
				
		// get data from other activities		
		Intent intent = getIntent();
		this.preSelectedTabIndexOnMainActivity = intent.getIntExtra("PreSelectedTabIndexOnMainActivity", this.preSelectedTabIndexOnMainActivity);
		
		// start location manager
		LocationManagerUtil locManager = new LocationManagerUtil(this, this);
		if(!locManager.init()){
			MyLog.bag().e("Location Manager couldn't be initialized.");
		}
		
		if(this.isNeedOfRequestingMatches()){		
			
			// log
			MyLog.bag().i("Starting to retrieve matches");
			
			// below asynchronous process will call our 'onMatchComplete' method
			this.startRetrievingMatches();
		}
		else {
			
			// log
			MyLog.bag().i("Cached matches are being used");
			
			// get matches
			ArrayList<MatchedCandidate> candidates = this.getMatchedCandidates();
						
			// update UI
			this.updateUI(candidates);
		}
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {

		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.preview, menu);
		return true;
	}

	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
		// Handle action bar item clicks here. The action bar will
		// automatically handle clicks on the Home/Up button, so long
		// as you specify a parent activity in AndroidManifest.xml.
		int id = item.getItemId();
		if (id == R.id.action_settings) {
			return true;
		}
		return super.onOptionsItemSelected(item);
	}

	@Override
	public void onSaveInstanceState(Bundle outState) {
		
		super.onSaveInstanceState(outState);

		if(outState == null){
			return;
		}
		
		outState.putInt("PreSelectedTabIndexOnMainActivity", this.preSelectedTabIndexOnMainActivity);
		
		if(this.matches != null){
			outState.putSerializable("matches", this.matches);
		}
		
		if(this.lastMatchRetrieveTime != null){
			outState.putSerializable("lastMatchRetrieveTime", this.lastMatchRetrieveTime);
		}
		
		if(this.lastLocationInfo != null){
			outState.putSerializable("lastLocationInfo", this.lastLocationInfo);
		}
	}
	
	@Override
	public void onRestoreInstanceState(Bundle savedInstanceState) {
		
		super.onRestoreInstanceState(savedInstanceState);		
		this.recreatePropertiesFromSavedBundle(savedInstanceState);
	}
	
	@SuppressWarnings("unchecked")
	private boolean recreatePropertiesFromSavedBundle(Bundle state){
		
		if(state == null){
			return false;
		}
		
		boolean restored = false;
		if(state.containsKey("PreSelectedTabIndexOnMainActivity")){
			this.preSelectedTabIndexOnMainActivity = state.getInt("PreSelectedTabIndexOnMainActivity", 0);
			restored = true;
		}
		
		if(state.containsKey("matches")){
			this.matches = (ArrayList<MatchedCandidate>)state.getSerializable("matches");
			restored = true;
		}
		
		if(state.containsKey("lastMatchRetrieveTime")){
			this.lastMatchRetrieveTime = (Date)state.getSerializable("lastMatchRetrieveTime");
			restored = true;
		}
		
		if(state.containsKey("lastLocationInfo")){
			this.lastLocationInfo = (Location)state.getSerializable("lastLocationInfo");
			restored = true;
		}
		
		return restored;
	}
	
	@Override
	public void onLocationChanged(GeoLocation geoLocation, Locality locality) {
		
		// has locality changed dramatically?
		boolean isCityChanged = false;
		if(this.lastLocationInfo == null){
			isCityChanged = true;
		}
		else{
			if(!this.lastLocationInfo.getLocality().getCity().equalsIgnoreCase(locality.getCity())){
				isCityChanged = true;
			}
		}
		
		// save current location
		Location loc = new Location();
		loc.setGeoLocation(geoLocation);
		loc.setLocality(locality);
		this.lastLocationInfo = loc;
		
		MyLog.bag()
		.add("GeoLocation", geoLocation.toString())
		.add("Locality",locality.toString())
		.v("Location Changed");
		
		if(isCityChanged){
			this.lastMatchRetrieveTime = null;
			MyLog.bag().v("Current location (city) has changed dramatically. Resetting flag to retrieve matches again.");
		}
	}
	
	public boolean isNeedOfRequestingMatches(){
		
		if(this.lastMatchRetrieveTime == null){
			
			MyLog.bag().v("Last match retrieve time is null. We need to request matches again");
			return true;
		}
		
		Date now = new Date();
		long diffAsMilliSeconds = now.getTime() - this.lastMatchRetrieveTime.getTime();
		long diffAsSeconds = diffAsMilliSeconds / 1000;
				
		if(this.matches.size() == 0 && diffAsSeconds > 3){
			MyLog.bag().v("View doesn't have any match on it. We need to request matches again");
			return true;
		}
		
		return false;
	}
	
	protected void startRetrievingMatches(){
		
		if(this.lastLocationInfo == null){
			MyLog.bag().w("Current location is null. Match response might not be meaningful.");
		}
		
		PreviewCandidatesTask task = new PreviewCandidatesTask(this, this, this.lastLocationInfo);
		task.execute();
	}
	
	@Override
	public void onMatchComplete(MatchedCandidateListResponse response, Location location){
		
		if(response.getErrorCode() == 0){
			
			this.lastMatchRetrieveTime = new Date();
			
			// update cache
			ArrayList<MatchedCandidate> candidates = response.getItems();
			if(candidates != null && candidates.size() > 0){
				this.matches.clear();
				this.matches.addAll(candidates);
			}
			
			// update UI
			this.updateViewOnMatchComplete(this.matches);			
		}
		else{
			// show error
			GingerHelpers.showErrorMessage(this, response.getErrorMessage());
			
			// update UI
			this.updateViewOnMatchComplete(this.matches);
		}	
	}
	
	protected void updateViewOnMatchComplete(ArrayList<MatchedCandidate> candidates){
		
		int matchCount = (candidates == null ? 0 : candidates.size());
		MyLog.bag().i("onMatchComplete signal retrieved. Match count: " + matchCount);
		
		this.updateUI(candidates);
	}
	
	private void updateUI(ArrayList<MatchedCandidate> candidates){
		
		MyLog.bag().i("Updating UI based on matched candidates...");
		
		// remove spinner block
		this.removeTheFrontestView();
		
		// update UI
		if(candidates != null && candidates.size() > 0){
		
			// get list view
			ListView listview = (ListView) this.findViewById(R.id.matchList);
			
			// bind 'register' button
			// NOTE: This needs to be called before populateListView();
			// Main reason is that .setFooter() needs to be called before .setAdapter();
			this.bindRegisterSectionToListView(listview);
			
			// fill up list view
			this.populateListView(listview, candidates);
						
			// remove info-for-empty-content block 
			this.removeTheFrontestView();
		}
		else{
			// Sorry, no match at this time.
			// Tap and pull down to refresh!
			// Nothing to so since the current view is already the one that we want
		}	
	}
	
	private boolean removeTheFrontestView(){
		
		// get the frame view
		FrameLayout frameLayout = (FrameLayout)this.findViewById(R.id.matchListParentFrame);
		
		if(frameLayout.getChildCount() > 1){
			// remove last view
			frameLayout.removeViewAt(frameLayout.getChildCount()-1);
			return true;
		}
		else{						
			return false;
		}
	}
	
	private ListView populateListView(ListView listview, ArrayList<MatchedCandidate> matches){
		
		// fill up list view		
		MatchListItemAdapter adapter = new MatchListItemAdapter(this, matches);
		listview.setAdapter(adapter);
		
		// bind click actions
		listview.setOnItemClickListener(new AdapterView.OnItemClickListener() {
			@Override
			public void onItemClick(AdapterView<?> parent, final View view, int position, long id) {
				MatchedCandidate item = (MatchedCandidate) parent.getItemAtPosition(position);
				onShowPersonDetails(item);
			}
		});	
		
		return listview;
	}
	
	private Button bindRegisterSectionToListView(ListView listview){
		
		MyLog.bag().i("Adding ShowMore button");
		
		LayoutInflater inflater = LayoutInflater.from(this);
		View footer = inflater.inflate(R.layout.register_btn, null);	
		float height = this.getResources().getDimension(R.dimen.person_li_footer_height);
		LayoutParams lp = new LayoutParams(LayoutParams.MATCH_PARENT, (int) height);		
		footer.setLayoutParams(lp);
		
		// NOTE: Footer needs to be set before calling listView.setAdapter(...); // !!!
		listview.addFooterView(footer);		
		//listview.addFooterView(footer, null, false);
		//listview.setDivider(null);
		//listview.setDividerHeight(0);
	    
		Button registerButton = (Button)footer.findViewById(R.id.registerButton);
		registerButton.setOnClickListener(new OnClickListener() {
			@Override
		    public void onClick(View v) {
				PreviewActivity.this.startSignUp();
		    }
		 });
		
		return registerButton;
	}
	
	public void onShowPersonDetails(MatchedCandidate person){
		
		// Toast.makeText(this.getActivity(), "Showing " + person, Toast.LENGTH_LONG).show();
		Intent intent = new Intent(this, PreviewPersonActivity.class);
		intent.putExtra(PreviewPersonActivity.PreviewPersonType, person);

		// calling a child activity for a result keeps the parent activity alive.
		// by that way, we don't have to keep track of active tab when child activity is closed.
		this.startActivityForResult(intent, PreviewPersonActivity.PreviewPersonActivityCode);
	}
	
	private void startSignUp(){
		
		// start sign-up		
		Intent intent = new Intent(this, SignupActivity.class);
		intent.putExtra("PreSelectedTabIndexOnMainActivity", this.preSelectedTabIndexOnMainActivity); // pass through
		this.startActivity(intent);
		this.finish();
	}
	
	@Override
	public void onActivityResult(int requestCode, int resultCode, Intent data) {
		
		if (requestCode == PreviewPersonActivity.PreviewPersonActivityCode) {
			MyLog.bag().v("PreviewPersonActivity has returned");			
			if(resultCode == Activity.RESULT_OK && data != null){				
				Bundle bundle = data.getExtras();
				boolean jumpToSignup = bundle.getBoolean(PreviewActivity.JumpToSignupCode, false);
				if(jumpToSignup){
					this.startSignUp();
				}				
			}
	    }		
		else{
			MyLog.bag().v("Unknown Activity  has been received by MainActivity: " + requestCode);
		}
	}
}
