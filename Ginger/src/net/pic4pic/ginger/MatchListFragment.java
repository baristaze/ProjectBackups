package net.pic4pic.ginger;

import java.util.ArrayList;

import android.content.Intent;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.AbsListView.LayoutParams;
import android.widget.AdapterView;
import android.widget.Button;
import android.widget.FrameLayout;
import android.widget.ListView;
import android.widget.Toast;

import net.pic4pic.ginger.entities.MatchedCandidate;

public class MatchListFragment extends Fragment {
	
	private LayoutInflater inflater;
	
	// a public empty constructor is a must in fragment. 
	// Do not add any parameter to this constructor.
	public MatchListFragment(/*no parameter here please*/) {
	}

	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
		
		this.inflater = inflater;
		View rootView = inflater.inflate(R.layout.match_listview, container, false);
		
		MainActivity activity = (MainActivity)this.getActivity();
		if(activity.isNeedOfRequestingMatches()){		
			
			// log
			Log.i("MatchListFragment", "Starting to retrieve matches");
			
			// below asynchronous process will call our 'onMatchComplete' method
			activity.startRetrievingMatches();
		}
		else {
			
			// log
			Log.i("MatchListFragment", "Caches matches is being used");
			
			// get matches
			ArrayList<MatchedCandidate> candidates = activity.getMatchedCandidates();
						
			// update UI
			this.updateUI(rootView, candidates);
		}
		
		return rootView;
	}
	
	public void onMatchComplete(ArrayList<MatchedCandidate> matches){
		
		Log.i("MatchListFragment", "onMatchComplete signal retrieved");
		
		// update UI
		View rootView = this.getView();
		if(rootView != null){
			this.updateUI(this.getView(), matches);
		}
		else{
			Log.e("MatchListFragment", "Something is too fast: Retrieved matches before rendering the root view");
		}
	}
	
	private void updateUI(View rootView, ArrayList<MatchedCandidate> candidates){
		
		Log.i("MatchListFragment", "Updating UI based on matched candidates...");
		
		// remove spinner block
		this.removeTheFrontestView(rootView);
		
		// update UI
		if(candidates != null && candidates.size() > 0){
		
			// fill up list view
			this.populateListView(rootView, candidates);
			
			// add 'show more' button
			this.bindShowMoreSectionToListView(rootView);
						
			// remove info-for-empty-content block 
			this.removeTheFrontestView(rootView);
		}
		else{
			// Sorry, no match at this time.
			// Tap and pull down to refresh!
			// Nothing to so since the current view is already the one that we want
		}	
	}
	
	private boolean removeTheFrontestView(View rootView){
		
		// get the frame view
		FrameLayout frameLayout = (FrameLayout)rootView.findViewById(R.id.matchListParentFrame);
		
		if(frameLayout.getChildCount() > 1){
			// remove last view
			frameLayout.removeViewAt(frameLayout.getChildCount()-1);
			return true;
		}
		else{
			Log.e("MatchListFragment", "We cannot remove the latest view since we have only 1");
			return false;
		}
	}
	
	private void populateListView(View rootView, ArrayList<MatchedCandidate> matches){
		
		// fill up list view
		ListView listview = (ListView) rootView.findViewById(R.id.matchList);
		MatchListItemAdapter adapter = new MatchListItemAdapter(this.getActivity(), matches);
		listview.setAdapter(adapter);
		
		// bind click actions
		listview.setOnItemClickListener(new AdapterView.OnItemClickListener() {
			@Override
			public void onItemClick(AdapterView<?> parent, final View view, int position, long id) {
				MatchedCandidate item = (MatchedCandidate) parent.getItemAtPosition(position);
				onShowPersonDetails(item, view);
			}
		});		
	}
	
	private void bindShowMoreSectionToListView(View rootView){
		
		ListView listview = (ListView) rootView.findViewById(R.id.matchList);		
		View footer = this.inflater.inflate(R.layout.show_more_btn, null);		
		float height = this.getActivity().getResources().getDimension(R.dimen.person_li_footer_height);
		LayoutParams lp = new LayoutParams(LayoutParams.MATCH_PARENT, (int) height);		
		footer.setLayoutParams(lp);
		listview.addFooterView(footer);
		//listview.addFooterView(footer, null, false);
		//listview.setDivider(null);
		//listview.setDividerHeight(0);
	    
		Button showMoreButton = (Button)footer.findViewById(R.id.showMoreButton);
		showMoreButton.setOnClickListener(new OnClickListener() {
			@Override
		    public void onClick(View v) {
		    	onShowMoreMatches(v);
		    }
		 });	
	}
	
	public void onShowMoreMatches(View v){
		Toast.makeText(this.getActivity(), "Please purchase some credit", Toast.LENGTH_LONG).show();
	}
	
	public void onShowPersonDetails(MatchedCandidate person, View v){
		// Toast.makeText(this.getActivity(), "Showing " + person, Toast.LENGTH_LONG).show();
		Intent intent = new Intent(this.getActivity(), PersonActivity.class);
		intent.putExtra(PersonActivity.PersonType, person);

		// calling a child activity for a result keeps the parent activity alive.
		// by that way, we don't have to keep track of active tab when child activity is closed.
		startActivityForResult(intent, PersonActivity.PersonActivityCode);
	}
}
