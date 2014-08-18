package net.pic4pic.ginger.service;

import java.util.UUID;

import android.content.Context;

import net.pic4pic.ginger.entities.AcceptingPic4PicRequest;
import net.pic4pic.ginger.entities.BaseRequest;
import net.pic4pic.ginger.entities.BaseResponse;
import net.pic4pic.ginger.entities.BuyingNewMatchRequest;
import net.pic4pic.ginger.entities.CandidateDetailsRequest;
import net.pic4pic.ginger.entities.CandidateDetailsResponse;
import net.pic4pic.ginger.entities.ClientLogRequest;
import net.pic4pic.ginger.entities.ConversationRequest;
import net.pic4pic.ginger.entities.ConversationResponse;
import net.pic4pic.ginger.entities.ConversationsSummaryResponse;
import net.pic4pic.ginger.entities.FacebookRequest;
import net.pic4pic.ginger.entities.GingerException;
import net.pic4pic.ginger.entities.ImageUploadRequest;
import net.pic4pic.ginger.entities.ImageUploadResponse;
import net.pic4pic.ginger.entities.InstantMessageRequest;
import net.pic4pic.ginger.entities.Location;
import net.pic4pic.ginger.entities.MarkingRequest;
import net.pic4pic.ginger.entities.MatchedCandidateListResponse;
import net.pic4pic.ginger.entities.MatchedCandidateResponse;
import net.pic4pic.ginger.entities.MobileDevice;
import net.pic4pic.ginger.entities.NotificationListResponse;
import net.pic4pic.ginger.entities.NotificationRequest;
import net.pic4pic.ginger.entities.PurchaseOfferListResponse;
import net.pic4pic.ginger.entities.PurchaseRecord;
import net.pic4pic.ginger.entities.SignupRequest;
import net.pic4pic.ginger.entities.SimpleRequest;
import net.pic4pic.ginger.entities.StartingPic4PicRequest;
import net.pic4pic.ginger.entities.UserCredentials;
import net.pic4pic.ginger.entities.UserResponse;
import net.pic4pic.ginger.entities.VerifyBioRequest;

public class ServiceProxy extends ServiceBase implements IService {

	@Override
	public UUID init(Context context){
		
		UUID clientId = super.getClientId(context);
		if(clientId == null){
			clientId = UUID.randomUUID();
			if(super.saveClientId(context, clientId)){
				return clientId;
			}
			else{
				return null;
			}
		}
		else{
			return clientId;
		}		
	}
	
	public UUID getCachedClientId(){
		return this.clientIdCached;
	}
	
	@Override
	public UserResponse checkUsername(Context context, UserCredentials request) throws GingerException {
		
		if(request.getClientId() == null){
			request.setClientId(super.getClientId(context));
		}
		
		UserResponse response = super.post(
				request, 
				UserResponse.class, 
				ServiceEndpoint.MainService, 
				null, 
				"/svc/rest/checkusername").getData();
		
		if(response.getAuthToken() != null && response.getAuthToken().trim().length() > 0){
			super.saveAuthToken(context, response.getAuthToken().trim());
		}
		
		return response;
	}

	@Override
	public UserResponse signin(Context context, UserCredentials request) throws GingerException {
		
		if(request.getClientId() == null){
			request.setClientId(super.getClientId(context));
		}
		
		UserResponse response = super.post(
				request, 
				UserResponse.class, 
				ServiceEndpoint.MainService, 
				null, 
				"/svc/rest/signin").getData();
		
		if(response.getAuthToken() != null && response.getAuthToken().trim().length() > 0){
			super.saveAuthToken(context, response.getAuthToken().trim());
		}
		
		return response;
	}

	@Override
	public UserResponse verifyBio(Context context, VerifyBioRequest request) throws GingerException {

		if(request.getClientId() == null){
			request.setClientId(super.getClientId(context));
		}
		
		return super.post(
				request, 
				UserResponse.class, 
				ServiceEndpoint.MainService, 
				super.getAuthToken(context), 
				"/svc/rest/verifybio").getData();
	}
	
	@Override
	public BaseResponse downloadFriends(Context context, FacebookRequest request) throws GingerException{
		
		if(request.getClientId() == null){
			request.setClientId(super.getClientId(context));
		}		
		
		return super.post(
				request, 
				BaseResponse.class, 
				ServiceEndpoint.MainService, 
				super.getAuthToken(context), 
				"/svc/rest/friends").getData();	
	}

	@Override
	public UserResponse signup(Context context, SignupRequest request) throws GingerException {
		
		if(request.getClientId() == null){
			request.setClientId(super.getClientId(context));
		}
		
		UserResponse response = super.post(
				request, 
				UserResponse.class, 
				ServiceEndpoint.MainService, 
				null, 
				"/svc/rest/signup").getData();
		
		if(response.getAuthToken() != null && response.getAuthToken().trim().length() > 0){
			super.saveAuthToken(context, response.getAuthToken().trim());
		}
		
		return response;
	}
	
	@Override
	public BaseResponse activateUser(Context context, BaseRequest request) throws GingerException{
		
		if(request.getClientId() == null){
			request.setClientId(super.getClientId(context));
		}
		
		return super.post(
				request, 
				BaseResponse.class, 
				ServiceEndpoint.MainService, 
				super.getAuthToken(context), 
				"/svc/rest/activate").getData();
	}
	
	@Override
	public ImageUploadResponse uploadProfileImage(Context context, ImageUploadRequest request) throws GingerException {
		
		if(request.getClientId() == null){
			request.setClientId(super.getClientId(context));
		}
		
		return super.postLocalImage(
				request.getFullLocalPath(), 
				request.getClientId(),
				ImageUploadResponse.class, 
				ServiceEndpoint.ImageService, 
				super.getAuthToken(context, false), 
				"/file/upload?blurSize=20&profile=1&thumbx=200&thumby=200").getData();
	}
	
	@Override
	public MatchedCandidateListResponse getTodaysMatches(Context context, SimpleRequest<Location> request) throws GingerException{
		
		if(request.getClientId() == null){
			request.setClientId(super.getClientId(context));
		}
		
		return super.post(
				request, 
				MatchedCandidateListResponse.class, 
				ServiceEndpoint.MainService, 
				super.getAuthToken(context), 
				"/svc/rest/matches").getData();
	}
	
	@Override
	public MatchedCandidateListResponse buyNewMatches(Context context, BuyingNewMatchRequest request) throws GingerException{
		
		if(request.getClientId() == null){
			request.setClientId(super.getClientId(context));
		}
		
		return super.post(
				request, 
				MatchedCandidateListResponse.class, 
				ServiceEndpoint.MainService, 
				super.getAuthToken(context), 
				"/svc/rest/matches/buy").getData();	
	}
	
	@Override
	public MatchedCandidateResponse requestPic4Pic(Context context, StartingPic4PicRequest request) throws GingerException{
		
		if(request.getClientId() == null){
			request.setClientId(super.getClientId(context));
		}
		
		return super.post(
				request, 
				MatchedCandidateResponse.class, 
				ServiceEndpoint.MainService, 
				super.getAuthToken(context), 
				"/svc/rest/p4p/request").getData();
	}
    
	@Override
    public MatchedCandidateResponse acceptPic4Pic(Context context, AcceptingPic4PicRequest request) throws GingerException{
		
		if(request.getClientId() == null){
			request.setClientId(super.getClientId(context));
		}
		
		return super.post(
				request, 
				MatchedCandidateResponse.class, 
				ServiceEndpoint.MainService, 
				super.getAuthToken(context), 
				"/svc/rest/p4p/accept").getData();	
    }
	
	@Override
	public CandidateDetailsResponse getCandidateDetails(Context context, CandidateDetailsRequest request) throws GingerException{
		
		if(request.getClientId() == null){
			request.setClientId(super.getClientId(context));
		}
		
		return super.post(
				request, 
				CandidateDetailsResponse.class, 
				ServiceEndpoint.MainService, 
				super.getAuthToken(context), 
				"/svc/rest/candidate/details").getData();	
	}
	
	@Override
	public NotificationListResponse getNotifications(Context context, NotificationRequest request) throws GingerException{
		
		if(request.getClientId() == null){
			request.setClientId(super.getClientId(context));
		}
		
		return super.post(
				request, 
				NotificationListResponse.class, 
				ServiceEndpoint.MainService, 
				super.getAuthToken(context), 
				"/svc/rest/notifications").getData();
	}
	
	@Override
	public BaseResponse mark(Context context, MarkingRequest request) throws GingerException{
		
		if(request.getClientId() == null){
			request.setClientId(super.getClientId(context));
		}
		
		return super.post(
				request, 
				BaseResponse.class, 
				ServiceEndpoint.MainService, 
				super.getAuthToken(context), 
				"/svc/rest/mark").getData();	
	}
	
	@Override
    public ConversationResponse sendInstantMessage(Context context, InstantMessageRequest request)  throws GingerException{
	
		if(request.getClientId() == null){
			request.setClientId(super.getClientId(context));
		}
		
		return super.post(
				request, 
				ConversationResponse.class, 
				ServiceEndpoint.InstantMessageService, 
				super.getAuthToken(context), 
				"/svc/rest/im/send").getData();
	}
    
	@Override
    public ConversationResponse getConversation(Context context, ConversationRequest request) throws GingerException{
		
		if(request.getClientId() == null){
			request.setClientId(super.getClientId(context));
		}
		
		return super.post(
				request, 
				ConversationResponse.class, 
				ServiceEndpoint.InstantMessageService, 
				super.getAuthToken(context), 
				"/svc/rest/im/conversation").getData();	
	}
    
	@Override
    public ConversationsSummaryResponse getConversationSummary(Context context, BaseRequest request) throws GingerException{
		
		if(request.getClientId() == null){
			request.setClientId(super.getClientId(context));
		}
		
		return super.post(
				request, 
				ConversationsSummaryResponse.class, 
				ServiceEndpoint.InstantMessageService, 
				super.getAuthToken(context), 
				"/svc/rest/im/conversation/summary").getData();		
	}
	
	@Override
	public BaseResponse processPurchase(Context context, SimpleRequest<PurchaseRecord> request) throws GingerException{
		
		if(request.getClientId() == null){
			request.setClientId(super.getClientId(context));
		}
		
		return super.post(
				request, 
				BaseResponse.class, 
				ServiceEndpoint.MainService, 
				super.getAuthToken(context), 
				"/svc/rest/purchase").getData();	
	}
	
	@Override
	public BaseResponse getCurrentCredit(Context context, BaseRequest request) throws GingerException{
		
		if(request.getClientId() == null){
			request.setClientId(super.getClientId(context));
		}
		
		return super.post(
				request, 
				BaseResponse.class, 
				ServiceEndpoint.MainService, 
				super.getAuthToken(context), 
				"/svc/rest/credit").getData();	
	}
	
	@Override
	public PurchaseOfferListResponse getOffers(Context context, BaseRequest request) throws GingerException{
		
		if(request.getClientId() == null){
			request.setClientId(super.getClientId(context));
		}
		
		return super.post(
				request, 
				PurchaseOfferListResponse.class, 
				ServiceEndpoint.MainService, 
				super.getAuthToken(context), 
				"/svc/rest/offers").getData();		
	}	
	
	@Override
	public BaseResponse trackDevice(Context context, MobileDevice request) throws GingerException{
		
		if(request.getClientId() == null){
			request.setClientId(super.getClientId(context));
		}
		
		return super.post(
				request, 
				BaseResponse.class, 
				ServiceEndpoint.MainService, 
				super.getAuthToken(context, false), // authentication is not required
				"/svc/rest/device").getData();
	}
	
	public BaseResponse assureSupportAtLocation(Context context, SimpleRequest<Location> request) throws GingerException{
		
		if(request.getClientId() == null){
			request.setClientId(super.getClientId(context));
		}
		
		return super.post(
				request, 
				BaseResponse.class, 
				ServiceEndpoint.MainService, 
				super.getAuthToken(context, false), // authentication is not required
				"/svc/rest/location/new").getData();
	}
    
    public BaseResponse setCurrentLocation(Context context, SimpleRequest<Location> request) throws GingerException{
    	
    	if(request.getClientId() == null){
			request.setClientId(super.getClientId(context));
		}
		
		return super.post(
				request, 
				BaseResponse.class, 
				ServiceEndpoint.MainService, 
				super.getAuthToken(context, true),
				"/svc/rest/location/current").getData();
    }
    
    public BaseResponse sendClientLogs(ClientLogRequest request) throws GingerException{
    	
    	if(request.getClientId() == null){
    		UUID temp = this.getCachedClientId();
    		if(temp == null){
    			temp = new UUID(0, 0);
    		}
    		request.setClientId(temp);
		}		
    	
		return super.post(
				request, 
				BaseResponse.class, 
				ServiceEndpoint.LogService, 
				null,
				"/svc/rest/log").getData();
    }
}
