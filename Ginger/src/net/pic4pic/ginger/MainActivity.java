package net.pic4pic.ginger;

import java.util.ArrayList;
import java.util.Date;
import java.util.UUID;

import com.google.android.gms.gcm.GoogleCloudMessaging;

import android.app.ActionBar;
import android.app.Activity;
import android.app.AlertDialog;
import android.app.FragmentTransaction;
import android.content.ComponentCallbacks2;
import android.content.DialogInterface;
import android.content.Intent;
import android.graphics.Bitmap;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentActivity;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentPagerAdapter;
import android.support.v4.view.ViewPager;
import android.view.ContextThemeWrapper;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import net.pic4pic.ginger.entities.BaseRequest;
import net.pic4pic.ginger.entities.BaseResponse;
import net.pic4pic.ginger.entities.BuyingNewMatchRequest;
import net.pic4pic.ginger.entities.Familiarity;
import net.pic4pic.ginger.entities.GeoLocation;
import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.entities.ImageUploadRequest;
import net.pic4pic.ginger.entities.ImageUploadResponse;
import net.pic4pic.ginger.entities.InAppPurchaseResult;
import net.pic4pic.ginger.entities.Locality;
import net.pic4pic.ginger.entities.Location;
import net.pic4pic.ginger.entities.MatchedCandidate;
import net.pic4pic.ginger.entities.MatchedCandidateListResponse;
import net.pic4pic.ginger.entities.Notification;
import net.pic4pic.ginger.entities.NotificationListResponse;
import net.pic4pic.ginger.entities.NotificationRequest;
import net.pic4pic.ginger.entities.PicturePair;
import net.pic4pic.ginger.entities.PurchaseOffer;
import net.pic4pic.ginger.entities.PurchaseRecord;
import net.pic4pic.ginger.entities.SimpleRequest;
import net.pic4pic.ginger.entities.UserResponse;
import net.pic4pic.ginger.service.InAppPurchasingService;
import net.pic4pic.ginger.service.InAppPurchasingService.PurchasingServiceListener;
import net.pic4pic.ginger.tasks.BuyNewMatchTask.BuyNewMatchListener;
import net.pic4pic.ginger.tasks.BuyNewMatchTask;
import net.pic4pic.ginger.tasks.ImageUploadTask;
import net.pic4pic.ginger.tasks.ImageUploadTask.ImageUploadListener;
import net.pic4pic.ginger.tasks.MatchedCandidatesTask;
import net.pic4pic.ginger.tasks.ProcessPurchaseTask;
import net.pic4pic.ginger.tasks.MatchedCandidatesTask.MatchedCandidatesListener;
import net.pic4pic.ginger.tasks.NotificationsTask;
import net.pic4pic.ginger.tasks.NotificationsTask.NotificationsListener;
import net.pic4pic.ginger.tasks.OfferRetrieverTask;
import net.pic4pic.ginger.tasks.OfferRetrieverTask.OfferListener;
import net.pic4pic.ginger.tasks.ProcessPurchaseTask.PurchaseProcessListener;
import net.pic4pic.ginger.tasks.PurchaseBackLogTask;
import net.pic4pic.ginger.tasks.PurchaseBackLogTask.PurchaseBackLogListener;
import net.pic4pic.ginger.tasks.PushNotificationRegisterTask;
import net.pic4pic.ginger.utils.GingerHelpers;
import net.pic4pic.ginger.utils.ImageCacher;
import net.pic4pic.ginger.utils.LocationManagerUtil;
import net.pic4pic.ginger.utils.LocationManagerUtil.LocationListener;
import net.pic4pic.ginger.utils.ImageActivity;
import net.pic4pic.ginger.utils.ImageStorageHelper;
import net.pic4pic.ginger.utils.MyLog;
import net.pic4pic.ginger.utils.PurchaseUtils;
import net.pic4pic.ginger.utils.PushNotificationHelpers;

public class MainActivity extends FragmentActivity 
implements ActionBar.TabListener, MatchedCandidatesListener, NotificationsListener, OfferListener, 
/*implements*/ PurchaseProcessListener, BuyNewMatchListener, PurchaseBackLogListener, PurchasingServiceListener, LocationListener,
/*implements*/ ImageUploadListener, ComponentCallbacks2{
	
	public static final String AuthenticatedUserBundleType = "net.pic4pic.ginger.AuthenticatedUser";
	public static final String UpdatedMatchCandidate = "net.pic4pic.ginger.UpdatedMatchCandidate";
	public static final int CaptureCameraCode = 102;
	public static final int PickFromGalleryCode = 103;
	
	private int preSelectedTabIndexOnMainActivity = 0;
	
	private UserResponse me;
	public UserResponse getCurrentUser(){
		return this.me;
	}
	
	private Date lastMatchRetrieveTime = null;
	private Date lastNotificationRetrieveTime = null;
	private ArrayList<MatchedCandidate> matches = new ArrayList<MatchedCandidate>();
	private ArrayList<Notification> notifications = new ArrayList<Notification>();
	private ArrayList<PurchaseOffer> availableOffers = new ArrayList<PurchaseOffer>();
	private Location lastLocationInfo = null;
	
	private SectionsPagerAdapter mSectionsPagerAdapter;	
	private ViewPager mViewPager;
	
	private InAppPurchasingService inappPurchasingSvc = null;
	private GoogleCloudMessaging pushNotificationSvc = null;
	
	@Override
	public void onTrimMemory(int level){
		
		String memoryWarning = this.convertMemoryWarningLevelToString(level);
		
		if(level == TRIM_MEMORY_RUNNING_CRITICAL || level == TRIM_MEMORY_RUNNING_LOW){
			MyLog.bag().add("MemoryWarningLevel", memoryWarning).w("onTrimMemory warning is fired");		
			ImageCacher.Instance().clear();
		}
		else if(level == TRIM_MEMORY_RUNNING_MODERATE){
			MyLog.bag().add("MemoryWarningLevel", memoryWarning).i("onTrimMemory warning is fired");
			// no need to clear the cache yet.
		}
		else{
			// ignore the rest
		}
		
	}
	
	private String convertMemoryWarningLevelToString(int level){
		
		switch(level){
		
			case TRIM_MEMORY_BACKGROUND:
				return "TRIM_MEMORY_BACKGROUND";
				
			case TRIM_MEMORY_COMPLETE:
				return "TRIM_MEMORY_COMPLETE";
				
			case TRIM_MEMORY_MODERATE:
				return "TRIM_MEMORY_MODERATE";
				
			case TRIM_MEMORY_RUNNING_CRITICAL:
				return "TRIM_MEMORY_RUNNING_CRITICAL";
				
			case TRIM_MEMORY_RUNNING_LOW:
				return "TRIM_MEMORY_RUNNING_LOW";
				
			case TRIM_MEMORY_RUNNING_MODERATE:
				return "TRIM_MEMORY_RUNNING_MODERATE";
				
			case TRIM_MEMORY_UI_HIDDEN:
				return "TRIM_MEMORY_UI_HIDDEN";
				
			default:
				return Integer.toString(level);
		}
	}
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		
		MyLog.bag().v("onCreate");
		
		super.onCreate(savedInstanceState);
		if(this.recreatePropertiesFromSavedBundle(savedInstanceState)){
			MyLog.bag().i("At least one property is restored successfully");
		}
		
		setContentView(R.layout.activity_main);
		
		Intent intent = getIntent();
		
		this.preSelectedTabIndexOnMainActivity = intent.getIntExtra("PreSelectedTabIndexOnMainActivity", this.preSelectedTabIndexOnMainActivity);
		if(this.preSelectedTabIndexOnMainActivity != 0)
		{
			MyLog.bag().v("Launched by a push notification");
		}
		
		this.me = (UserResponse)intent.getSerializableExtra(AuthenticatedUserBundleType);
		MyLog.bag().v("Current user is: " + this.me.getUserProfile().getUsername());
		
		// Set up the action bar.
		final ActionBar actionBar = getActionBar();
		actionBar.setNavigationMode(ActionBar.NAVIGATION_MODE_TABS);
		
		// start location manager
		LocationManagerUtil locManager = new LocationManagerUtil(this, this);
		if(!locManager.init()){
			MyLog.bag().e("Location Manager couldn't be initialized.");
		}

		// Create the adapter that will return a fragment for each of the three
		// primary sections of the application.
		mSectionsPagerAdapter = new SectionsPagerAdapter(getSupportFragmentManager());

		// Set up the ViewPager with the sections adapter.
		mViewPager = (ViewPager) findViewById(R.id.pager);
		mViewPager.setAdapter(mSectionsPagerAdapter);
		
		// When swiping between different sections, select the corresponding
		// tab. We can also use ActionBar.Tab#select() to do this if we have
		// a reference to the Tab.
		mViewPager.setOnPageChangeListener(new ViewPager.SimpleOnPageChangeListener() {
					@Override
					public void onPageSelected(int position) {
						actionBar.setSelectedNavigationItem(position);
					}
				});

		// For each of the sections in the app, add a tab to the action bar.
		for (int i = 0; i < mSectionsPagerAdapter.getCount(); i++) {
			// Create a tab with text corresponding to the page title defined by
			// the adapter. Also specify this Activity object, which implements
			// the TabListener interface, as the callback (listener) for when
			// this tab is selected.
			actionBar.addTab(actionBar.newTab()
					.setText(mSectionsPagerAdapter.getPageTitle(i))
					.setTabListener(this));
		}
		
		if(this.preSelectedTabIndexOnMainActivity != 0){
			mViewPager.setCurrentItem(this.preSelectedTabIndexOnMainActivity);
		}		
		
		// creating purchase service
		if(this.inappPurchasingSvc == null){
			this.inappPurchasingSvc = new InAppPurchasingService(this, this);
			this.inappPurchasingSvc.createConnection();
		}
		
		// connect to the purchasing service
		try {
			this.inappPurchasingSvc.connect();
			MyLog.bag().v("Activity has been bound to InApp Purchasing Service");
		} 
		catch (GingerException e) {
			MyLog.bag().add(e).e("Couldn't bind to InApp Purchasing Service");
		}
		
		// handle push notification registrations if it is not done yet
		this.checkAndRegisterForPushNotifications();
	}
	
	@Override
	protected void onResume() {
	    super.onResume();

	    // handle push notification registrations if it is not done yet
	 	this.checkAndRegisterForPushNotifications();
	}
	
	private void checkAndRegisterForPushNotifications(){
		if(PushNotificationHelpers.checkPlayServices(this)){
			this.pushNotificationSvc = GoogleCloudMessaging.getInstance(this);
			String registrationId = PushNotificationHelpers.getRegistrationId(this);
            if (registrationId == null || registrationId.isEmpty()) {
            	PushNotificationRegisterTask task = new PushNotificationRegisterTask(
            			this, this.pushNotificationSvc, PushNotificationHelpers.SENDER_ID);
            	task.execute();
            }
		}	
	}
	
	@Override
	protected void onDestroy(){
		
		// stopping purchase service
		if(this.inappPurchasingSvc != null){
			
			try {
				this.inappPurchasingSvc.disconnect();
				MyLog.bag().v("Disconnected from InApp Purchasing Service");
			} 
			catch (GingerException e) {
				MyLog.bag().add(e).e("Couldn't disconnect from InApp Purchasing Service");
			}
			
			this.inappPurchasingSvc = null;
		}
		
		super.onDestroy();
	}
	
	@Override
	public void onSaveInstanceState(Bundle outState) {
		
		super.onSaveInstanceState(outState);

		if(outState == null){
			return;
		}
		
		MyLog.bag().v("onSaveInstanceState");
		
		outState.putInt("PreSelectedTabIndexOnMainActivity", this.preSelectedTabIndexOnMainActivity);
		
		if(this.me != null){
			outState.putSerializable("me", this.me);
		}

		if(this.lastMatchRetrieveTime != null){
			outState.putSerializable("lastMatchRetrieveTime", this.lastMatchRetrieveTime);
		}
		
		if(this.lastNotificationRetrieveTime != null){
			outState.putSerializable("lastNotificationRetrieveTime", this.lastNotificationRetrieveTime);
		}
	   
		if(this.matches != null){
			outState.putSerializable("matches", this.matches);
		}
		
		if(this.notifications != null){
			outState.putSerializable("notifications", this.notifications);
		}
		
		if(this.availableOffers != null){
			outState.putSerializable("availableOffers", this.availableOffers);
		}
		
		if(this.lastLocationInfo != null){
			outState.putSerializable("lastLocationInfo", this.lastLocationInfo);
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
		
		if(state.containsKey("me")){
			this.me = (UserResponse)state.getSerializable("me");
			restored = true;
		}
		
		if(state.containsKey("lastMatchRetrieveTime")){
			this.lastMatchRetrieveTime = (Date)state.getSerializable("lastMatchRetrieveTime");
			restored = true;
		}
		
		if(state.containsKey("lastNotificationRetrieveTime")){
			this.lastNotificationRetrieveTime = (Date)state.getSerializable("lastNotificationRetrieveTime");
			restored = true;
		}
		
		if(state.containsKey("matches")){
			this.matches = (ArrayList<MatchedCandidate>)state.getSerializable("matches");
			restored = true;
		}
		
		if(state.containsKey("notifications")){
			this.notifications = (ArrayList<Notification>)state.getSerializable("notifications");
			restored = true;
		}
		
		if(state.containsKey("availableOffers")){
			this.availableOffers = (ArrayList<PurchaseOffer>)state.getSerializable("availableOffers");
			restored = true;
		}
		
		if(state.containsKey("lastLocationInfo")){
			this.lastLocationInfo = (Location)state.getSerializable("lastLocationInfo");
			restored = true;
		}
		
		return restored;
	}
	
	public ArrayList<MatchedCandidate> getMatchedCandidates(){
		return this.matches;
	}
	
	public ArrayList<Notification> getNotifications(){
		return this.notifications;
	}
	
	public Location getLastLocation(){
		return this.lastLocationInfo;
	}
	
	public boolean isNeedOfRequestingMatches(){
		
		if(this.lastMatchRetrieveTime == null){
			
			MyLog.bag().v("Last match retrieve time is null. We need to request matches again");
			return true;
		}
		
		Date now = new Date();
		long diffAsMilliSeconds = now.getTime() - this.lastMatchRetrieveTime.getTime();
		long diffAsSeconds = diffAsMilliSeconds / 1000;
		long diffAsMinutes = diffAsSeconds / 60;
		if(diffAsMinutes > 30){
			MyLog.bag().v("Last match retrieve time was 30 minutes ago. We need to request matches again");
			return true;
		}
		
		if(this.matches.size() == 0 && diffAsSeconds > 30){
			MyLog.bag().v("View doesn't have any match on it. We need to request matches again");
			return true;
		}
		
		return false;
	}
	
	public boolean isNeedOfRequestingNotifications(){
		
		if(this.lastNotificationRetrieveTime == null){
			
			MyLog.bag().v("Last notification retrieve time is null. We need to request notifications again");
			return true;
		}
		
		Date now = new Date();
		long diffAsMilliSeconds = now.getTime() - this.lastNotificationRetrieveTime.getTime();
		long diffAsSeconds = diffAsMilliSeconds / 1000;
		long diffAsMinutes = diffAsSeconds / 60;
		if(diffAsMinutes > 30){
			MyLog.bag().v("Last notification retrieve time was 30 minutes ago. We need to request notifications again");
			return true;
		}
		
		if(this.notifications.size() == 0 && diffAsSeconds > 30){
			MyLog.bag().v("View doesn't have any notification on it. We need to request notifications again");
			return true;
		}
		
		return false;
	}
	
	public boolean isNeedOfRetrievingOffers(){

		if(this.availableOffers == null || this.availableOffers.size() == 0){
			return true;
		}
		
		return false;
	}
	
	public void startRetrievingMatches(){
		
		if(this.lastLocationInfo == null){
			MyLog.bag().w("Current location is null. Match response might not be meaningful.");
		}
		
		MatchedCandidatesTask task = new MatchedCandidatesTask(this, this, this.lastLocationInfo);
		task.execute();
	}
	
	public void startRetrievingNotifications(){		
		NotificationRequest request = new NotificationRequest();
		request.setLastNotificationId(new UUID(0,0));
		/*
		if(this.notifications.size() > 0){
			request.setLastNotificationId(this.notifications.get(0).getId());
		}
		*/
		NotificationsTask task = new NotificationsTask(this, this, request);
		task.execute();
	}
	
	public InAppPurchasingService getPurchasingService(){
		return this.inappPurchasingSvc;
	}
		
	public ArrayList<PurchaseOffer> getAvailableOffers(){
		return this.availableOffers;
	}
	
	public void startRetrievingAvailableOffers(){
		
		MyLog.bag().v("Retrieving available offers...");
		BaseRequest request = new BaseRequest();
		OfferRetrieverTask task = new OfferRetrieverTask(this, this, request);
		task.execute();
	}
	
	@Override
	public void onOffersRetrieved(ArrayList<PurchaseOffer> offers){
		MyLog.bag().v("Available offers retrieved: " + offers.size());
		this.availableOffers = offers;
	}
	
	@Override
	public void onPurchasingServiceConnected(){
		PurchaseBackLogTask task = new PurchaseBackLogTask(this, this, this.inappPurchasingSvc);
		task.execute();
	}

	public void onBackLogProcessingComplete(int currentCredit, ArrayList<InAppPurchaseResult> ghostPurchases){
		
		// update credit
		if(currentCredit >= 0){
			MyLog.bag().i("onBackLogProcessingComplete. New Credit: " + currentCredit);
			this.me.setCurrentCredit(currentCredit);
		}
		
		if(ghostPurchases != null && ghostPurchases.size() > 0){
			ArrayList<PurchaseOffer> offers = this.getAvailableOffers();
			if(offers != null && offers.size() > 0){
				for(InAppPurchaseResult ghostPurchase : ghostPurchases){
					// saving purchase record to the file (unprocessed + non-consumed) will help it get re-processed at a later time.
					PurchaseRecord ghost = this.findOfferAndSavePurchaseRecord(ghostPurchase, offers);
					if(ghost != null){
						MyLog.bag().i("onBackLogProcessingComplete. A ghost record is saved to a local file to be processed later: " + ghost.getPurchaseReferenceToken());
					}
				}
			}
		}
	}
	
	// this is called after an item is purchased on Google Play and our onActivityResult is invoked
	public void onPurchaseCompleteOnAppStore(int requestCode, int resultCode, Intent data){
		
		if(resultCode != Activity.RESULT_OK){
			MyLog.bag().v("onPurchaseCompletedOnAppStore cancelled or failed. resultCode = " + resultCode);
			return;
		}
		
		MyLog.bag().v("Purchase is completed on AppStore.");
		
		InAppPurchaseResult result = null;
		try {
			result = this.inappPurchasingSvc.processActivityResult(requestCode, resultCode, data);
			MyLog.bag().i("InAppPurchaseResult has been retrieved properly.");
		} 
		catch (GingerException e) {
			MyLog.bag().add(e).e();
			GingerHelpers.showErrorMessage(this, e.getMessage());
			return;
		}
				
		PurchaseRecord purchaseRecord = this.findOfferAndSavePurchaseRecord(result, this.getAvailableOffers());
		if(purchaseRecord == null){
			return;
		}
		
		SimpleRequest<PurchaseRecord> request = new SimpleRequest<PurchaseRecord>();
		request.setData(purchaseRecord);
		
		// below task calls 'onPurchaseProcessed' method below when done
		MyLog.bag().v("Sending the purchase record to the server...");
		ProcessPurchaseTask task = new ProcessPurchaseTask(this, this, request, null);
		task.execute();
	}
	
	private PurchaseRecord findOfferAndSavePurchaseRecord(InAppPurchaseResult result, ArrayList<PurchaseOffer> offers){
		
		PurchaseOffer selectedOffer = null;
		for(PurchaseOffer offer : offers){
			if(offer.getAppStoreItemId().equals(result.getProductItemSku())){
				selectedOffer = offer;
				break;
			}
		}
		
		if(selectedOffer == null){
			String errorMessage = "The purchased item '" + result.getProductItemSku() + "' is not one of the avaialable offers.";
			MyLog.bag().e(errorMessage);
			GingerHelpers.showErrorMessage(this, errorMessage);
			return null;	
		}
		
		PurchaseRecord purchaseRecord = new PurchaseRecord();
		purchaseRecord.setAppStoreId(selectedOffer.getAppStoreId());
		purchaseRecord.setInternalOfferId(selectedOffer.getInternalItemId());
		purchaseRecord.setInternalPurchasePayLoad(result.getDeveloperPayload());
		purchaseRecord.setPurchaseInstanceId(result.getOrderId());
		purchaseRecord.setPurchaseReferenceToken(result.getPurchaseToken());
		purchaseRecord.setPurchaseTimeUTC(result.getPurchaseTimeUTC());
		purchaseRecord.setOriginalData(result.getOriginalData());
		purchaseRecord.setDataSignature(result.getDataSignature());
		
		// save to the file
		try {
			MyLog.bag().v("Saving the unprocessed purchase record to the file: " + purchaseRecord.getPurchaseReferenceToken());
			PurchaseUtils.saveUnprocessedPurchaseToFile(this, purchaseRecord);
		} 
		catch (GingerException e) {
			MyLog.bag().add(e).e("Saving the unprocessed purchase record to the file failed");
		}
		
		// save to the file 2
		try {
			MyLog.bag().v("Saving the unconsumed purchase record to the file: " + purchaseRecord.getPurchaseReferenceToken());
			PurchaseUtils.saveUnconsumedPurchaseToFile(this, purchaseRecord);
		} 
		catch (GingerException e) {
			MyLog.bag().add(e).e("Saving the unconsumed purchase record to the file failed");
		}
		
		return purchaseRecord;
	}
	
	@Override
	public void onPurchaseProcessed(BaseResponse response, SimpleRequest<PurchaseRecord> request){
		
		if(response.getErrorCode() == 0){
			
			// log
			MyLog.bag().v("Service call 'processPurchase' has returned successfully.");
			
			// update credit
			MyLog.bag().i("New Credit after purchase: " + response.getCurrentCredit());
			this.me.setCurrentCredit(response.getCurrentCredit());
			
			// update the file
			try {
				PurchaseRecord purchaseRecord = request.getData();
				MyLog.bag().v("Removing the unpurchase purchase record from file: " + purchaseRecord.getPurchaseReferenceToken());
				PurchaseUtils.removeUnprocessedPurchaseFromFile(this, purchaseRecord);
			} 
			catch (GingerException e) {
				MyLog.bag().add(e).e("Removing the unpurchase purchase record from file failed");
			}
			
			// consume
			MyLog.bag().v("Consuming the last purchase on Google Play to enable future purchases.");
			PurchaseUtils.startConsumingPurchaseOnAppStoreAndClearLocal(this, this.inappPurchasingSvc, request.getData());
			
			// buy new MATCHES since we have more credits now... 
			MyLog.bag().v("Requesting more PAID matches after credit purchase.");
			this.startBuyingNewCandidates();
		}
		else {
			GingerHelpers.showErrorMessage(this, response.getErrorMessage());
		}
	}
	
	public void startBuyingNewCandidates(){
		
		if(this.lastLocationInfo == null){
			MyLog.bag().w("Current location is null. Match response after purchase might not be meaningful.");
		}
		
		BuyingNewMatchRequest request = new BuyingNewMatchRequest();
		request.setMaxCount(5);
		request.setLocation(this.lastLocationInfo);
		
		BuyNewMatchTask task = new BuyNewMatchTask(this, this, request, null);
		task.execute();
	}
	
	@Override
	public void onBuyNewMatchComplete(MatchedCandidateListResponse response, BuyingNewMatchRequest request){
		
		if(response.getErrorCode() == 0){
			
			// update credit
			this.me.setCurrentCredit(response.getCurrentCredit());
			
			// update cache
			ArrayList<MatchedCandidate> candidates = response.getItems();
			if(candidates != null && candidates.size() > 0){
				this.matches.addAll(0, candidates);
			}
			
			// update UI
			this.mSectionsPagerAdapter.getMatchListFragment().onMatchComplete(this.matches, true);			
		}
		else{
			// show error
			GingerHelpers.showErrorMessage(this, response.getErrorMessage());
		}		
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
			this.mSectionsPagerAdapter.getMatchListFragment().onMatchComplete(this.matches, false);			
		}
		else{
			// show error
			GingerHelpers.showErrorMessage(this, response.getErrorMessage());
			
			// update UI
			this.mSectionsPagerAdapter.getMatchListFragment().onMatchComplete(this.matches, false);
		}	
	}
	
	@Override
	public void onNotificationsComplete(NotificationListResponse response, NotificationRequest request){
		
		if(response.getErrorCode() == 0){
			
			this.lastNotificationRetrieveTime = new Date();
			
			// update cache
			ArrayList<Notification> notifs = response.getItems();
			if(notifs != null && notifs.size() > 0){
				this.notifications.clear();
				this.notifications.addAll(notifs);
			}
			
			// update UI
			this.mSectionsPagerAdapter.getNotificationListFragment().onNotificationsComplete(this.notifications);			
		}
		else{
			// show error
			GingerHelpers.showErrorMessage(this, response.getErrorMessage());
			
			// update UI
			this.mSectionsPagerAdapter.getNotificationListFragment().onNotificationsComplete(this.notifications);
		}	
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
	
	@Override
	public void onUpload(final ImageUploadRequest request, final ImageUploadResponse response){
		
		if(response.getErrorCode() != 0){			
			// failure
			String error = response.getErrorMessage();
			// show error message
			new AlertDialog.Builder(new ContextThemeWrapper(this, android.R.style.Theme_Holo_Dialog))
		    .setTitle(this.getString(R.string.general_error_title))
		    .setMessage(error)		    
		    .setIcon(android.R.drawable.ic_dialog_alert)
		    .setCancelable(false)
		    .setNegativeButton(this.getString(R.string.general_Cancel), new DialogInterface.OnClickListener() {
		        public void onClick(DialogInterface dialog, int which) {
		        	// do nothing
		        }})
		    .setPositiveButton(this.getString(R.string.general_retry), new DialogInterface.OnClickListener() {
		        public void onClick(DialogInterface dialog, int which) {
		        	ImageUploadTask task = new ImageUploadTask(MainActivity.this, MainActivity.this, request);
		    		task.execute();
		        }})
		    .show();
		}
		else{
			// success... add to the gallery view
			PicturePair picturePair = new PicturePair();
			picturePair.setFullSize(response.getImages().getFullSizeClear());
			picturePair.setThumbnail(response.getImages().getThumbnailClear());
			this.me.getOtherPictures().add(picturePair);
			this.mSectionsPagerAdapter.getProfileFragment().onNewImageAdded(picturePair);
		}
	}
	
	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.main, menu);
		return true;
	}

	@Override
	public void onTabSelected(ActionBar.Tab tab, FragmentTransaction fragmentTransaction) {
		// When the given tab is selected, switch to the corresponding page in
		// the ViewPager.
		mViewPager.setCurrentItem(tab.getPosition());
	}

	@Override
	public void onTabUnselected(ActionBar.Tab tab, FragmentTransaction fragmentTransaction) {
	}

	@Override
	public void onTabReselected(ActionBar.Tab tab, FragmentTransaction fragmentTransaction) {
	}
	
	@Override
	public void onActivityResult(int requestCode, int resultCode, Intent data) {
		
		if (requestCode == MainActivity.CaptureCameraCode) {
			MyLog.bag().v("CaptureCameraActivity has returned");
			this.processCameraActivityResult(resultCode, data);			
	    }
		else if (requestCode == MainActivity.PickFromGalleryCode) {
			MyLog.bag().v("PickFromGalleryActivity has returned");
			this.processGalleryActivityResult(resultCode, data);
		}
		else if (requestCode == TextInputActivity.TextInputCode) {
			MyLog.bag().v("TextInputActivity has returned");
			this.mSectionsPagerAdapter.getProfileFragment().processTextInputActivityResult(resultCode, data);
		}
		else if(requestCode == PersonActivity.PersonActivityCode){
			MyLog.bag().v("PersonActivity has returned");			
			if(resultCode == Activity.RESULT_OK && data != null){				
				Bundle bundle = data.getExtras();
				MatchedCandidate candidate = (MatchedCandidate)bundle.getSerializable(MainActivity.UpdatedMatchCandidate);
				MyLog.bag().i("Candidate: " + candidate.getUserId() + " viewed: " + candidate.isViewed());				
				this.updateCandidate(candidate);				
				this.updateNotification(candidate);
			}
		}
		else if(requestCode == InAppPurchasingService.INAPP_PURCHASE_REQUEST_CODE){
			MyLog.bag().v("InAppPurchasingService has returned");
			if(resultCode != Activity.RESULT_OK){
				MyLog.bag().v("onPurchaseCompletedOnAppStore cancelled or failed. resultCode = " + resultCode);
			}
			else if (data == null){
				MyLog.bag().e("onPurchaseCompletedOnAppStore returned null intent although resultCode is OK.");
			}
			else{
				this.onPurchaseCompleteOnAppStore(requestCode, resultCode, data);
			}
		}
		else{
			MyLog.bag().v("Unknown Activity  has been received by MainActivity: " + requestCode);
		}
	}
	
	protected void processCameraActivityResult(int resultCode, Intent data){
		this.processNewImageActivity(resultCode, data, ImageActivity.Source.Camera);
	}
	
	protected void processGalleryActivityResult(int resultCode, Intent data){
		this.processNewImageActivity(resultCode, data, ImageActivity.Source.Gallery);
	}
	
	protected void processNewImageActivity(int resultCode, Intent data, ImageActivity.Source source)
	{
		ImageActivity.Result result = ImageActivity.getProcessedResult(this, resultCode, data);
		if(result.getBitmap() == null){
			String errorMessage = result.getErrorMessage();
			if(errorMessage == null || errorMessage.length() == 0){
				errorMessage = "Unexpected error occurred";
			}
			GingerHelpers.toast(this, errorMessage);
			return;
		}
		
		Bitmap photo = result.getBitmap();
		photo = ImageActivity.trimSize(photo);
		
		String fileName = this.getString(R.string.secondary_photos_last_filename_key);
		if(!ImageStorageHelper.saveToInternalStorage(this, photo, fileName, true)){
			GingerHelpers.toast(this, "A private copy of the selected photo couldn't be saved locally.");
			return;
		}
		
		MyLog.bag().v("Photo has been saved to the internal storage");
		
		String absoluteFilePath = ImageStorageHelper.getAbsolutePath(this, fileName);
		ImageUploadRequest request = new ImageUploadRequest();
		request.setFullLocalPath(absoluteFilePath);
		request.setProfileImage(false);
		ImageUploadTask task = new ImageUploadTask(this, this, request);
		task.execute();
	}
	
	public void updateCandidate(MatchedCandidate candidate){
		
		if(candidate == null){
			return;
		}
		
		for(int x=0; x<this.matches.size(); x++){
			MatchedCandidate initial = this.matches.get(x);
			UUID userId1 = initial.getUserId();
			UUID userId2 = candidate.getUserId();
			if(userId1.equals(userId2)){
				
				// prepare flag
				Familiarity f1 = initial.getCandidateProfile().getFamiliarity();
				Familiarity f2 = candidate.getCandidateProfile().getFamiliarity();
				boolean hasFamiliarityChanged = (f1.getIntValue() != f2.getIntValue());
				
				// update person
				this.matches.set(x, candidate);
				
				// update view... having this within the for loop is OK.
				this.mSectionsPagerAdapter.getMatchListFragment().updateCandidateView(candidate, hasFamiliarityChanged);
				
				// break is fine here.
				break;
			}
		}
	}
	
	public void updateNotification(MatchedCandidate candidate){
		
		if(candidate == null){
			return;
		}
		
		boolean hasFamiliarityChanged = false;
		for(int x=0; x<this.notifications.size(); x++){
			MatchedCandidate initial = this.notifications.get(x).getSender();
			UUID userId1 = initial.getUserId();
			UUID userId2 = candidate.getUserId();
			if(userId1.equals(userId2)){
				
				// prepare flag
				Familiarity f1 = initial.getCandidateProfile().getFamiliarity();
				Familiarity f2 = candidate.getCandidateProfile().getFamiliarity();
				if(f1.getIntValue() != f2.getIntValue()){
					hasFamiliarityChanged = true;
				}
				
				// set
				this.notifications.get(x).setSender(candidate);
				
				// don't break since we might have multiple notifications from a candidate
				// break;
			}
		}
		
		// keep this outside of for loop
		// below method updates the avatar only
		this.mSectionsPagerAdapter.getNotificationListFragment().updateCandidateView(candidate, hasFamiliarityChanged);
	}
	
	/**
	 * A {@link FragmentPagerAdapter} that returns a fragment corresponding to
	 * one of the sections/tabs/pages.
	 */
	public class SectionsPagerAdapter extends FragmentPagerAdapter {
		
		private MatchListFragment matchesFragment;
		private NotificationListFragment notificationListFragment;
		private ProfileFragment profileFragment;
		
		public synchronized MatchListFragment getMatchListFragment(){
			if(this.matchesFragment == null){
				this.matchesFragment = new MatchListFragment();
			}
			
			return this.matchesFragment;
		}
		
		public synchronized NotificationListFragment getNotificationListFragment(){
			if(this.notificationListFragment == null){
				this.notificationListFragment = new NotificationListFragment();
			}
			
			return this.notificationListFragment;
		}
		
		public synchronized ProfileFragment getProfileFragment(){
			if(this.profileFragment == null){
				this.profileFragment = new ProfileFragment();
			}
			
			return this.profileFragment;
		}
		
		public SectionsPagerAdapter(FragmentManager fm) {
			super(fm);
		}

		@Override
		public Fragment getItem(int position) {			
			// getItem is called to instantiate the fragment for the given page.
			// Return a DummySectionFragment (defined as a static inner class
			// below) with the page number as its lone argument.
			if(position == 0){
				return this.getMatchListFragment();
			}
			else if(position == 1){
				return this.getNotificationListFragment();
			}
			else if (position == 2){
				return this.getProfileFragment();
			}
			else{
				Fragment fragment = new DummySectionFragment();
				Bundle args = new Bundle();
				args.putInt(DummySectionFragment.ARG_SECTION_NUMBER, position + 1);
				fragment.setArguments(args);
				return fragment;
			}
		}

		@Override
		public int getCount() {
			// Show 3 total pages.
			return 3;
		}

		@Override
		public CharSequence getPageTitle(int position) {			
			switch (position) {			
				case 0:
					return getString(R.string.title_section_matches);
				case 1:
					return getString(R.string.title_section_notif);
				case 2:
					return getString(R.string.title_section_me);
			}
			
			return null;
		}
	}

	/**
	 * A dummy fragment representing a section of the app, but that simply
	 * displays dummy text.
	 */
	public static class DummySectionFragment extends Fragment {
		/**
		 * The fragment argument representing the section number for this
		 * fragment.
		 */
		public static final String ARG_SECTION_NUMBER = "section_number";

		public DummySectionFragment() {
		}

		@Override
		public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			View rootView = inflater.inflate(R.layout.fragment_main_dummy, container, false);
			TextView dummyTextView = (TextView) rootView.findViewById(R.id.section_label);
			dummyTextView.setText(Integer.toString(getArguments().getInt(ARG_SECTION_NUMBER)));
			return rootView;
		}
	}
}
