package net.pic4pic.ginger.tasks;

import java.util.ArrayList;

import android.app.Activity;
import android.os.AsyncTask;

import net.pic4pic.ginger.entities.BaseRequest;
import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.entities.PurchaseOffer;
import net.pic4pic.ginger.entities.PurchaseOfferListResponse;
import net.pic4pic.ginger.service.Service;
import net.pic4pic.ginger.utils.MyLog;

public class OfferRetrieverTask extends AsyncTask<String, Void, PurchaseOfferListResponse> {

	private Activity activity;
	private BaseRequest request;
	private OfferListener listener; 
	
	public OfferRetrieverTask(Activity activity, OfferListener listener, BaseRequest request){
		this.activity = activity;
		this.listener = listener;
		this.request = request;
	}
	
	@Override
    protected PurchaseOfferListResponse doInBackground(String... executeArgs) {
		
		try {
			final PurchaseOfferListResponse response = Service.getInstance().getOffers(this.activity, this.request);
			if(response.getErrorCode() == 0) {
				return response;				
			}
			else{
				MyLog.bag().e("Offers could not be retrieved. Error: " + response.getErrorMessage());
			}
		}
		catch(GingerException e){
			MyLog.bag().add(e).e("Offers could not be retrieved.");
		}
		catch(Exception e){
			e.printStackTrace();
			MyLog.bag().add(e).e( "Unexpected error received while retrieving offers.");
		}
		
		return null;
	}
	
	@Override
	protected void onPostExecute(PurchaseOfferListResponse response) {
		
		if(response == null){
			return;
		}
		
		ArrayList<PurchaseOffer> offers = response.getItems();
		if(offers.size() > 0){
			this.listener.onOffersRetrieved(offers);
		}
		else{
			MyLog.bag().w("There is not any available offer.");
		}
    }
	
	public interface OfferListener{
		void onOffersRetrieved(ArrayList<PurchaseOffer> offers);
	}
}
