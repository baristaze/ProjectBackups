package net.pic4pic.ginger;

import java.util.ArrayList;
import java.util.Date;

import android.content.Intent;
import android.graphics.drawable.Drawable;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.AbsListView.LayoutParams;
import android.widget.AdapterView;
import android.widget.Button;
import android.widget.FrameLayout;
import android.widget.HeaderViewListAdapter;
import android.widget.ListView;
import android.widget.Toast;

import net.pic4pic.ginger.entities.AppStoreType;
import net.pic4pic.ginger.entities.BaseResponse;
import net.pic4pic.ginger.entities.BuyingNewMatchRequest;
import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.entities.MatchedCandidate;
import net.pic4pic.ginger.entities.MatchedCandidateListResponse;
import net.pic4pic.ginger.entities.PurchaseOfferListResponse;
import net.pic4pic.ginger.entities.PurchaseRecord;
import net.pic4pic.ginger.entities.SimpleRequest;
import net.pic4pic.ginger.service.Service;
import net.pic4pic.ginger.tasks.ITask;
import net.pic4pic.ginger.tasks.NonBlockedTask;
import net.pic4pic.ginger.utils.GingerHelpers;
import net.pic4pic.ginger.utils.MyLog;

public class MatchListFragment extends Fragment {
	
	// a public empty constructor is a must in fragment. 
	// Do not add any parameter to this constructor.
	public MatchListFragment(/*no parameter here please*/) {
	}

	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
		
		View rootView = inflater.inflate(R.layout.match_listview, container, false);
		return rootView;
	}
	
	@Override
	public void onViewCreated(View view, Bundle savedInstanceState){
		
		super.onViewCreated(view, savedInstanceState);
		
		MainActivity activity = (MainActivity)this.getActivity();
		if(activity.isNeedOfRequestingMatches()){		
			
			// log
			MyLog.i("MatchListFragment", "Starting to retrieve matches");
			
			// below asynchronous process will call our 'onMatchComplete' method
			activity.startRetrievingMatches();
		}
		else {
			
			// log
			MyLog.i("MatchListFragment", "Cached matches are being used");
			
			// get matches
			ArrayList<MatchedCandidate> candidates = activity.getMatchedCandidates();
						
			// update UI
			this.updateUI(view, candidates);
		}	
	}
	
	public void onMatchComplete(ArrayList<MatchedCandidate> matches){
		
		int matchCount = (matches == null ? 0 : matches.size());
		MyLog.i("MatchListFragment", "onMatchComplete signal retrieved. Match count: " + matchCount);
		
		// update UI
		View rootView = this.getView();
		if(rootView != null){
			this.updateUI(this.getView(), matches);
		}
		else{			
			MyLog.e("MatchListFragment", "Retrieved matches before rendering the root.");
		}
	}
	
	private void updateUI(View rootView, ArrayList<MatchedCandidate> candidates){
		
		MyLog.i("MatchListFragment", "Updating UI based on matched candidates...");
		
		// remove spinner block
		this.removeTheFrontestView(rootView);
		
		// update UI
		if(candidates != null && candidates.size() > 0){
		
			// get list view
			ListView listview = (ListView) rootView.findViewById(R.id.matchList);
			
			// bind 'show more' button
			// NOTE: This needs to be called before populateListView();
			// Main reason is that .setFooter() needs to be called before .setAdapter();
			this.bindShowMoreSectionToListView(listview);
			
			// fill up list view
			this.populateListView(listview, candidates);
						
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
			MyLog.e("MatchListFragment", "We cannot remove the latest view since we have only 1");
			return false;
		}
	}
	
	private ListView populateListView(ListView listview, ArrayList<MatchedCandidate> matches){
		
		// fill up list view		
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
		
		return listview;
	}
	
	private Button bindShowMoreSectionToListView(ListView listview){
		
		MyLog.i("MatchListFragment", "Adding ShowMore button");
		
		LayoutInflater inflater = LayoutInflater.from(this.getActivity());
		View footer = inflater.inflate(R.layout.show_more_btn, null);	
		float height = this.getActivity().getResources().getDimension(R.dimen.person_li_footer_height);
		LayoutParams lp = new LayoutParams(LayoutParams.MATCH_PARENT, (int) height);		
		footer.setLayoutParams(lp);
		
		// NOTE: Footer needs to be set before calling listView.setAdapter(...); // !!!
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
		
		return showMoreButton;
	}
	
	public void onShowMoreMatches(View v){
		
		PurchaseRecord purchaseRecord = new PurchaseRecord();
		purchaseRecord.setAppStoreId(AppStoreType.GooglePlay);
		purchaseRecord.setInternalOfferId(3);
		purchaseRecord.setInternalPurchasePayLoad("test fake 2");
		purchaseRecord.setPurchaseInstanceId("fake test 23");
		purchaseRecord.setPurchaseReferenceToken("xyzw");
		purchaseRecord.setPurchaseTimeUTC(new Date());
		
		final SimpleRequest<PurchaseRecord> request = new SimpleRequest<PurchaseRecord>();
		request.setData(purchaseRecord);
		
		final BuyingNewMatchRequest buyNewRequest = new BuyingNewMatchRequest();
		buyNewRequest.setMaxCount(5);
		
		NonBlockedTask.SafeRun(new ITask(){
			@Override
			public void perform() {
				try {
					final BaseResponse response = Service.getInstance().processPurchase(MatchListFragment.this.getActivity(), request);
					//final BaseResponse response = Service.getInstance().getCurrentCredit(MatchListFragment.this.getActivity(), request);
					//final PurchaseOfferListResponse response = Service.getInstance().getOffers(MatchListFragment.this.getActivity(), request);
					//final MatchedCandidateListResponse response = Service.getInstance().buyNewMatches(MatchListFragment.this.getActivity(), buyNewRequest);
					MatchListFragment.this.getActivity().runOnUiThread(new Runnable(){
						@Override
						public void run() {
							Toast.makeText(MatchListFragment.this.getActivity(), "New credit: " + response.getCurrentCredit(), Toast.LENGTH_LONG).show();							
							//Toast.makeText(MatchListFragment.this.getActivity(), "Offer Count: " + response.getItems().size(), Toast.LENGTH_LONG).show();
							//Toast.makeText(MatchListFragment.this.getActivity(), "New Match Count: " + response.getItems().size(), Toast.LENGTH_LONG).show();
						}						
					});
				} 
				catch (final GingerException e) {				
					MatchListFragment.this.getActivity().runOnUiThread(new Runnable(){
						@Override
						public void run() {
							GingerHelpers.showErrorMessage(MatchListFragment.this.getActivity(), e.getMessage());							
						}						
					});	
				}
			}			
		});		
	}
	
	public void onShowPersonDetails(MatchedCandidate person, View listViewItem){
		
		// Toast.makeText(this.getActivity(), "Showing " + person, Toast.LENGTH_LONG).show();
		Intent intent = new Intent(this.getActivity(), PersonActivity.class);
		intent.putExtra(MainActivity.AuthenticatedUserBundleType, ((MainActivity)this.getActivity()).getCurrentUser());
		intent.putExtra(PersonActivity.PersonType, person);

		// calling a child activity for a result keeps the parent activity alive.
		// by that way, we don't have to keep track of active tab when child activity is closed.
		this.getActivity().startActivityForResult(intent, PersonActivity.PersonActivityCode);
		
		// update the core data
		// person.setLastViewTimeUTC(new Date());
	}
	
	public void updateCandidateView(final MatchedCandidate person, final boolean hasFamiliarityChanged){
		
		View rootView = this.getView(); 
		if(rootView == null){
			MyLog.w("MatchListFragment", "Root view is null? Hah!");
			return;
		}
		
		final ListView listView = (ListView)rootView.findViewById(R.id.matchList);
		
		// we have this wrapper adapter since we are using a footer.
		final HeaderViewListAdapter wrapperAdapter = (HeaderViewListAdapter)listView.getAdapter();
		final MatchListItemAdapter adapter = (MatchListItemAdapter)wrapperAdapter.getWrappedAdapter();

		// below iteration goes through only VISIBLE elements
		int found = 0;
		int start = listView.getFirstVisiblePosition();
		for (int i = start; i < listView.getLastVisiblePosition(); i++) {
			MatchedCandidate temp = (MatchedCandidate)listView.getItemAtPosition(i);
			if(person.getUserId().equals(temp.getUserId())) {				
				final View listItemView = listView.getChildAt(i-start);				
				final Drawable background = GingerHelpers.getListItemBackgroundDrawable(this.getActivity(), person.isViewed());
		    	final int position = i;
		    	NonBlockedTask.SafeSleepAndRunOnUI(400, new ITask(){
					@Override
					public void perform() {						
						
						// change thumb-nail if necessary
						if(hasFamiliarityChanged){
							// refresh whole row (list item view)
							// ...
							// getting view for the target child refreshes it automatically
							adapter.getView(position, listItemView, listView);
							
							// log
							MyLog.i("MatchListFragment", "Refreshing avatar image for user: " + person.getUserId());
						}
						
						// change background...
						listItemView.setBackground(background);
					    //listItemView.refreshDrawableState(); // useless
					}
				});
		    	
		    	MyLog.v("MatchListFragment", "ListItemView is found(" + (found+1) + "). Background will be changed shortly for: " + person.getUserId());
		    	
		    	found++;
		    	break;
			}
		}
		
		if(found <= 0){
			MyLog.w("MatchListFragment", "ListItemView seems invisible?");
		}
	}
}
