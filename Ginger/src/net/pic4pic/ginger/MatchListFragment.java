package net.pic4pic.ginger;

import java.util.ArrayList;
import java.util.UUID;

import android.app.AlertDialog;
import android.content.DialogInterface;
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

import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.entities.MatchedCandidate;
import net.pic4pic.ginger.entities.PurchaseOffer;
import net.pic4pic.ginger.service.InAppPurchasingService;
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
			this.updateUI(view, candidates, false);
		}	
		
		// start retrieving available offers for this user
		if(activity.isNeedOfRetrievingOffers()){
			activity.startRetrievingAvailableOffers();
		}
	}
	
	public void onMatchComplete(ArrayList<MatchedCandidate> matches, boolean isAfterPurchase){
		
		int matchCount = (matches == null ? 0 : matches.size());
		MyLog.i("MatchListFragment", "onMatchComplete signal retrieved. Match count: " + matchCount);
		
		// update UI
		View rootView = this.getView();
		if(rootView != null){
			this.updateUI(this.getView(), matches, isAfterPurchase);
		}
		else{			
			MyLog.e("MatchListFragment", "Retrieved matches before rendering the root.");
		}
	}
	
	private void updateUI(View rootView, ArrayList<MatchedCandidate> candidates, boolean isAfterPurchase){
		
		MyLog.i("MatchListFragment", "Updating UI based on matched candidates...");
		
		// remove spinner block
		this.removeTheFrontestView(rootView, isAfterPurchase);
		
		// update UI
		if(candidates != null && candidates.size() > 0){
		
			// get list view
			ListView listview = (ListView) rootView.findViewById(R.id.matchList);
			
			// bind 'show more' button
			// NOTE: This needs to be called before populateListView();
			// Main reason is that .setFooter() needs to be called before .setAdapter();
			if(!isAfterPurchase){
				this.bindShowMoreSectionToListView(listview);
			}
			
			// fill up list view
			this.populateListView(listview, candidates);
						
			// remove info-for-empty-content block 
			this.removeTheFrontestView(rootView, isAfterPurchase);
		}
		else{
			// Sorry, no match at this time.
			// Tap and pull down to refresh!
			// Nothing to so since the current view is already the one that we want
		}	
	}
	
	private boolean removeTheFrontestView(View rootView, boolean isAfterPurchase){
		
		// get the frame view
		FrameLayout frameLayout = (FrameLayout)rootView.findViewById(R.id.matchListParentFrame);
		
		if(frameLayout.getChildCount() > 1){
			// remove last view
			frameLayout.removeViewAt(frameLayout.getChildCount()-1);
			return true;
		}
		else{
			if(!isAfterPurchase){
				MyLog.e("MatchListFragment", "We cannot remove the latest view since we have only 1");
			}			
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
		
		MyLog.v("MatchListFragment", "Show More button is clicked");
		final MainActivity mainActivity = (MainActivity)this.getActivity();
		if(mainActivity.getCurrentUser().getCurrentCredit() >= 10){
			mainActivity.startBuyingNewCandidates();
			return;
		}
		
		// if we are here, that means the user doesn't have enough credit to buy new matches.
		AlertDialog.Builder builder = new AlertDialog.Builder(mainActivity);
        builder.setTitle(mainActivity.getString(R.string.credit_insufficient_title));
        builder.setMessage(mainActivity.getString(R.string.credit_insufficient_descr));
        builder.setNeutralButton(R.string.general_OK, new DialogInterface.OnClickListener() {
			@Override
			public void onClick(DialogInterface dialog, int which) {
				dialog.dismiss();
				MatchListFragment.this.offerBuyingCredit();
			}
		});                 
        
        AlertDialog creditInfoDialog = builder.create();
        creditInfoDialog.show();
	}
	
    public void offerBuyingCredit(){
    	 
    	final MainActivity mainActivity = (MainActivity)this.getActivity();
		final InAppPurchasingService purchasingSvc = mainActivity.getPurchasingService();
		if(purchasingSvc == null || !purchasingSvc.isConnected()){
			GingerHelpers.showErrorMessage(this.getActivity(), "Couldn't be connected to the Google Play Billing Services. Please try again later.");
			return;
		}
		
		final ArrayList<PurchaseOffer> offers = mainActivity.getAvailableOffers();
		if(offers == null || offers.size() == 0){
			GingerHelpers.showErrorMessage(this.getActivity(), "Couldn't retrieve available offers. Please try again later.");
			mainActivity.startRetrievingAvailableOffers(); // try again...
			return;	
		}
		
		int i=0;
		String[] choices = new String[offers.size()]; 
		for(PurchaseOffer offer : offers){
			String choice = String.format("%s ($%s)", offer.getInternalItemName(), offer.getItemPrice());
			if(offer.getInternalItemDescription() != null && offer.getInternalItemDescription().length() > 0){
				choice += "\n" + offer.getInternalItemDescription() + "";
			}
			choices[i++] = choice;
		}
		
		int selected = 0;
		if(choices.length > 1){
			selected = 1;
		}
		if(choices.length > 2){
			selected = 2;
		}
		
		AlertDialog.Builder builder = new AlertDialog.Builder(mainActivity);
        builder.setTitle("Select Purchase Item");
        builder.setSingleChoiceItems(choices, selected, null);
        builder.setNegativeButton(R.string.general_Cancel, new DialogInterface.OnClickListener() {
			@Override
			public void onClick(DialogInterface dialog, int which) {
				MyLog.v("MatchListFragment", "Purchasing credit is cancelled");
				dialog.dismiss();
			}
		});        
        builder.setPositiveButton(R.string.general_OK, new DialogInterface.OnClickListener() {
			@Override
			public void onClick(DialogInterface dialog, int which) {
				
				ListView listView = ((AlertDialog)dialog).getListView();
				int position = listView.getCheckedItemPosition();
				if(position >= 0 && position < offers.size()){
					PurchaseOffer offer = offers.get(position);
					final String sku = offer.getAppStoreItemId();
					MyLog.i("MatchListFragment", "Purchasing credit is selected. OfferID = " + offer.getInternalItemId());
					NonBlockedTask.SafeRun(new ITask(){
						@Override
						public void perform() {
							try {
								// below process will return its result to the MainActivity.onActivityResult
								MyLog.i("MatchListFragment", "Starting InApp Purchase. SKU = " + sku);
								purchasingSvc.startBuyingItem(sku, UUID.randomUUID().toString());
							} 
							catch (final GingerException e) {
								MyLog.e("MatchListFragment", "InApp Purchase couldn't be started for SKU '" + sku + "'. Error: " + e.getMessage());
								mainActivity.runOnUiThread(new Runnable(){
									@Override
									public void run() {
										GingerHelpers.showErrorMessage(mainActivity, e.getMessage());
									}
								});
							}	
						}
					});
					
					dialog.dismiss();
				}
				else{
					// MyLog.e("MatchListFragment", "Invalid position is returned from AlertDialog: " + position);
					GingerHelpers.showErrorMessage(mainActivity, "Please select an item");
				}
			}
		});        
        
        AlertDialog purchasePlanDialog = builder.create();
        purchasePlanDialog.show();
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
